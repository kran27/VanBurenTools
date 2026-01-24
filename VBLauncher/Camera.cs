// This file is a modified version of the Camera.cs file from https://github.com/mellinoe/veldrid-samples
// It is my understanding that it falls under the MIT license, as per Veldrid's license, as the samples themselves are not explicitly licensed.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace VBLauncher;

public class Camera
{
    private float _fov = 1f;
    private float _near = 0.01f;
    private float _far = 1000f;

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
    public float FieldOfView => _fov;
    public float NearDistance { get => _near; set { _near = value; UpdatePerspectiveMatrix(); } }

    public float AspectRatio => _windowWidth / _windowHeight;

    public float Yaw { get => _yaw; set { _yaw = value; UpdateViewMatrix(); } }
    public float Pitch { get => _pitch; set { _pitch = value; UpdateViewMatrix(); } }

    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public Vector3 Forward => GetLookDir();

    public void Update(float deltaSeconds)
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