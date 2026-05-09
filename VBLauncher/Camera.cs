// This file is a modified version of the Camera.cs file from https://github.com/mellinoe/veldrid-samples
// It is my understanding that it falls under the MIT license, as per Veldrid's license, as the samples themselves are not explicitly licensed.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace VBLauncher;

// Free vs Orbit are two completely different control schemes that share the
// same projection/view plumbing. Free-fly is the WASD+rmb-look used for
// inspecting maps. Orbit revolves around a fixed target (good for prop
// previews where you want the model centered and the camera's distance to
// stay sensible). Switching modes preserves yaw/pitch so the rotation feels
// continuous.
public enum CameraMode { Free, Orbit }

public class Camera
{
    private float _fov = 1f;
    private float _near = 0.1f;
    private float _far = 500f;

    private Matrix4x4 _viewMatrix;
    private Matrix4x4 _projectionMatrix;

    private Vector3 _position = new(0, 3, 0);
    private Vector3 _lookDirection = new(0, -.3f, -1f);
    private float _moveSpeed = 10.0f;

    private float _yaw;
    private float _pitch;

    private Vector2 _previousMousePos;
    private float _windowWidth;
    private float _windowHeight;

    // ----- Orbit-mode state -----
    // The target is the world-space point the camera revolves around.
    // Distance is how far back along the camera's forward axis the eye
    // sits; scroll-wheel adjusts it geometrically (×0.9 / ÷0.9 per tick)
    // so the same input feels right at vastly different scene scales.
    private CameraMode _mode = CameraMode.Free;
    private Vector3 _target = Vector3.Zero;
    private float _distance = 10f;

    public event Action<Matrix4x4> ProjectionChanged;
    public event Action<Matrix4x4> ViewChanged;

    public Camera(float width, float height)
    {
        _windowWidth = width;
        _windowHeight = height;
        UpdatePerspectiveMatrix();
        UpdateViewMatrix();
    }

    public Matrix4x4 ViewMatrix => _viewMatrix;
    public Matrix4x4 ProjectionMatrix => _projectionMatrix;

    public Vector3 Position { get => _position; set { _position = value; UpdateViewMatrix(); } }

    public float FarDistance { get => _far; set { _far = value; UpdatePerspectiveMatrix(); } }
    public float FieldOfView { get => _fov; set { _fov = value; UpdatePerspectiveMatrix(); } }
    public float NearDistance { get => _near; set { _near = value; UpdatePerspectiveMatrix(); } }

    public float AspectRatio => _windowWidth / _windowHeight;

    public float Yaw { get => _yaw; set { _yaw = value; UpdateViewMatrix(); } }
    public float Pitch { get => _pitch; set { _pitch = value; UpdateViewMatrix(); } }

    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public Vector3 Forward => GetLookDir();

    public CameraMode Mode
    {
        get => _mode;
        set
        {
            if (_mode == value) return;
            _mode = value;
            // When switching INTO orbit, snap the orbit target to whatever the
            // camera is currently looking at so the view doesn't jump.
            // Distance is reset to a moderate value; FocusOnBounds will tighten
            // it if the caller actually has geometry to frame.
            if (_mode == CameraMode.Orbit)
            {
                _target = _position + GetLookDir() * _distance;
                RecomputeOrbitPosition();
            }
        }
    }

    public Vector3 Target { get => _target; set { _target = value; if (_mode == CameraMode.Orbit) RecomputeOrbitPosition(); } }
    public float Distance { get => _distance; set { _distance = MathF.Max(0.01f, value); if (_mode == CameraMode.Orbit) RecomputeOrbitPosition(); } }

    // Frame an axis-aligned box: aim the camera at its center with a
    // distance proportional to the box's largest extent. Works in either
    // mode — Free leaves yaw/pitch where they were and just repositions;
    // Orbit recenters the orbit target.
    public void FocusOnBounds(Vector3 min, Vector3 max)
    {
        var center = (min + max) * 0.5f;
        var extent = max - min;
        var maxDim = MathF.Max(extent.X, MathF.Max(extent.Y, extent.Z));
        if (maxDim < 1f) maxDim = 10f;
        var dist = maxDim * 1.2f;

        if (_mode == CameraMode.Orbit)
        {
            _target = center;
            _distance = dist;
            // Use a slight downward pitch by default so the model isn't seen
            // edge-on; yaw stays at 0 (looking along -Z toward origin).
            _yaw = 0f;
            _pitch = -0.35f;
            RecomputeOrbitPosition();
        }
        else
        {
            _yaw = 0f;
            _pitch = -0.5f;
            _position = center + new Vector3(0, dist * 0.8f, dist);
            UpdateViewMatrix();
        }
    }

    public void Update(float deltaSeconds)
    {
        if (_mode == CameraMode.Orbit)
        {
            UpdateOrbit(deltaSeconds);
            return;
        }
        UpdateFree(deltaSeconds);
    }

    private void UpdateFree(float deltaSeconds)
    {
        float sprintFactor = InputTracker.GetKey(Key.ControlLeft)
            ? 0.1f
            : InputTracker.GetKey(Key.ShiftLeft)
                ? 2.5f
                : 1f;
        Vector3 motionDir = Vector3.Zero;
        if (InputTracker.GetKey(Key.A))
        {
            motionDir += -Vector3.UnitX;
        }
        if (InputTracker.GetKey(Key.D))
        {
            motionDir += Vector3.UnitX;
        }
        if (InputTracker.GetKey(Key.W))
        {
            motionDir += -Vector3.UnitZ;
        }
        if (InputTracker.GetKey(Key.S))
        {
            motionDir += Vector3.UnitZ;
        }
        if (InputTracker.GetKey(Key.Q))
        {
            motionDir += -Vector3.UnitY;
        }
        if (InputTracker.GetKey(Key.E))
        {
            motionDir += Vector3.UnitY;
        }

        if (motionDir != Vector3.Zero)
        {
            Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            motionDir = Vector3.Transform(motionDir, lookRotation);
            _position += motionDir * MoveSpeed * sprintFactor * deltaSeconds;
            UpdateViewMatrix();
        }

        Vector2 mouseDelta = InputTracker.MousePosition - _previousMousePos;
        _previousMousePos = InputTracker.MousePosition;

        if (InputTracker.GetMouseButton(MouseButton.Right)) // left click removed so it can be used to select items
        {
            Yaw += -mouseDelta.X * 0.01f;
            Pitch += -mouseDelta.Y * 0.01f;
            Pitch = Clamp(Pitch, -1.55f, 1.55f);

            UpdateViewMatrix();
        }

        if (InputTracker.GetMouseButton(MouseButton.Middle))
        {
            // Get camera's right and up vectors
            Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            Vector3 right = Vector3.Transform(Vector3.UnitX, lookRotation);
            Vector3 up = Vector3.Transform(Vector3.UnitY, lookRotation);

            // Sensitivity factor
            float panSpeed = 0.003f * sprintFactor * MoveSpeed;

            // Apply panning (invert Y for natural drag feel)
            _position += -right * mouseDelta.X * panSpeed;
            _position += up * mouseDelta.Y * panSpeed;

            UpdateViewMatrix();
        }

        float scrollDelta = InputTracker.ScrollDelta;

        if (scrollDelta != 0)
        {
            Vector3 forward = GetLookDir();
            _position += forward * scrollDelta * MoveSpeed * sprintFactor * 0.1f;
            UpdateViewMatrix();
        }
        
        InputTracker.ScrollDelta = 0; // Reset scroll delta after processing
    }

    // Orbit controls:
    //   right-mouse drag → rotate around target (yaw/pitch)
    //   middle-mouse drag → pan target along camera's right/up axes
    //   scroll wheel → multiplicative zoom (×0.9 per tick) so the feel is
    //     scale-invariant; ctrl/shift speed it down/up like the free path
    //   WASD intentionally does nothing in orbit mode — the whole point of
    //     orbit is that the camera is anchored to the target.
    private void UpdateOrbit(float deltaSeconds)
    {
        Vector2 mouseDelta = InputTracker.MousePosition - _previousMousePos;
        _previousMousePos = InputTracker.MousePosition;

        bool changed = false;

        if (InputTracker.GetMouseButton(MouseButton.Right))
        {
            _yaw += -mouseDelta.X * 0.01f;
            _pitch += -mouseDelta.Y * 0.01f;
            _pitch = Clamp(_pitch, -1.55f, 1.55f);
            changed = true;
        }

        if (InputTracker.GetMouseButton(MouseButton.Middle))
        {
            // Pan the orbit target so the camera follows it. Pan speed
            // scales with distance so the gesture feels right whether
            // we're looking at a chair or a whole map.
            Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, 0f);
            Vector3 right = Vector3.Transform(Vector3.UnitX, lookRotation);
            Vector3 up = Vector3.Transform(Vector3.UnitY, lookRotation);
            float panSpeed = 0.0015f * _distance;
            _target += -right * mouseDelta.X * panSpeed;
            _target += up * mouseDelta.Y * panSpeed;
            changed = true;
        }

        float scrollDelta = InputTracker.ScrollDelta;
        if (scrollDelta != 0)
        {
            float zoomStep = MathF.Pow(0.9f, scrollDelta);
            // Modifier keys mirror UpdateFree: shift = faster, ctrl = slower.
            if (InputTracker.GetKey(Key.ShiftLeft)) zoomStep = MathF.Pow(zoomStep, 2.5f);
            else if (InputTracker.GetKey(Key.ControlLeft)) zoomStep = MathF.Pow(zoomStep, 0.1f);
            _distance = MathF.Max(0.01f, _distance * zoomStep);
            changed = true;
        }

        InputTracker.ScrollDelta = 0;
        if (changed) RecomputeOrbitPosition();
    }

    private void RecomputeOrbitPosition()
    {
        // Position the eye on a sphere of radius _distance around _target,
        // facing inward. UpdateViewMatrix uses _position + lookDir, so we
        // back the eye off along the *negative* look direction.
        var lookDir = GetLookDir();
        _lookDirection = lookDir;
        _position = _target - lookDir * _distance;
        _viewMatrix = Matrix4x4.CreateLookAt(_position, _target, Vector3.UnitY);
        ViewChanged?.Invoke(_viewMatrix);
    }

    private float Clamp(float value, float min, float max)
    {
        return value > max
            ? max
            : value < min
                ? min
                : value;
    }

    public void WindowResized(float width, float height)
    {
        _windowWidth = width;
        _windowHeight = height;
        UpdatePerspectiveMatrix();
    }

    private void UpdatePerspectiveMatrix()
    {
        _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_fov, _windowWidth / _windowHeight, _near, _far);
        ProjectionChanged?.Invoke(_projectionMatrix);
    }

    private void UpdateViewMatrix()
    {
        Vector3 lookDir = GetLookDir();
        _lookDirection = lookDir;
        _viewMatrix = Matrix4x4.CreateLookAt(_position, _position + _lookDirection, Vector3.UnitY);
        ViewChanged?.Invoke(_viewMatrix);
    }

    private Vector3 GetLookDir()
    {
        Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
        Vector3 lookDir = Vector3.Transform(-Vector3.UnitZ, lookRotation);
        return lookDir;
    }

    // Direct view-matrix override. Bypasses the yaw/pitch state entirely
    // by computing a LookAt and stashing the resulting direction back into
    // _yaw/_pitch (so subsequent free/orbit input picks up where this
    // override left off). Used by the thumbnail renderer to position the
    // camera at a fixed isometric angle without going through input.
    public void SetLookAt(Vector3 from, Vector3 to)
    {
        _position = from;
        var dir = to - from;
        if (dir.LengthSquared() < 1e-8f) dir = -Vector3.UnitZ;
        dir = Vector3.Normalize(dir);
        _lookDirection = dir;
        _pitch = MathF.Asin(MathF.Max(-1f, MathF.Min(1f, dir.Y)));
        _yaw = MathF.Atan2(dir.X, -dir.Z);
        _viewMatrix = Matrix4x4.CreateLookAt(_position, to, Vector3.UnitY);
        ViewChanged?.Invoke(_viewMatrix);
    }

    public CameraInfo GetCameraInfo() => new()
    {
        CameraPosition_WorldSpace = _position,
        CameraLookDirection = _lookDirection
    };
}

[StructLayout(LayoutKind.Sequential)]
public struct CameraInfo
{
    public Vector3 CameraPosition_WorldSpace;
    private float _padding1;
    public Vector3 CameraLookDirection;
    private float _padding2;
}