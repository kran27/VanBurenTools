using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AltUI.Config;
using AltUI.Forms;
using ImGuiColorTextEditNet;
using ImGuiNET;
using VBLauncher.My.Resources;
using VBLauncher.Properties;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace VBLauncher;

public partial class EditorWindow
{
    private static readonly uint[] _vegPalette =
    [
        //AABBGGRR
        0xff7f7f7f, // Default
        0xffeb956c, // Keyword (Done)
        0xffc094ed, // Number (Done)
        0xff6da2c9, // String (Done)
        0xff6da2c9, // Char literal (Done)
        0xffbdbdbd, // Punctuation (Done)
        0xff408080, // Preprocessor
        0xffaaaaaa, // Identifier
        0xffffbfe1, // Known identifier (Done)
        0xffc040a0, // Preproc identifier
        0xff6cc485, // Comment (single line) (Done)
        0xff6cc485, // Comment (multi line) (Done)
        0xff101010, // Background
        0xffe0e0e0, // Cursor
        0x80a06020, // Selection
        0x800020ff, // ErrorMarker
        0x40f08000, // Breakpoint
        0xff707000, // Line number
        0x40000000, // Current line fill
        0x40808080, // Current line fill (inactive)
        0x40a0a0a0, // Current line edge
        0xa0a0a0a0, // Executing Line
        0xffccc366, // Custom (enum value)
        0xffff91c1, // Custom 2 (header)
        0xff9bcc39 // Custom 3 (func)
    ];

    // not named specifically for GIAM as it's used in GIWP
    private readonly string[] _ammoTypes =
    [
        "None", "15mm EC", ".223", ".22/.22 Injector", "Empty", "2mm EC", ".30", "40mm", ".44", ".45", ".50", "7.62mm",
        "9mm", "BB", "Microfusion Cell", "Naphate", "Heavy Rivet", "Lil' Rivet", "Rocket", "Small Energy Cell"
    ];

    private readonly string[] _gcreSkills =
    [
        "Firearms", "Melee", "Unarmed", "Barter", "Persuasion", "Deception", "Mechanics", "Medic", "Outdoorsman",
        "Science", "Security", "Sneak", "Steal"
    ];

    private readonly string[] _gcreSockets =
    [
        "Head", "Hair", "Ponytail", "Moustache", "Beard", "Eye", "Body", "Hand", "Feet", "Back", "Shoulder", "Vanity"
    ];

    private readonly string[] _gcreSpecials =
    [
        "Strength", "Perception", "Endurance", "Charisma", "Intelligence", "Agility", "Luck"
    ];

    private readonly string[] _gcreTraits =
    [
        "Bruiser", "Chem Reliant", "Clean Living", "Fast Shot", "Feral Kid", "Finesse", "Gifted", "Good Natured",
        "Increased Metabolism", "Kamikaze", "Night Person", "One Hander", "One In a Million", "Red Scare", "Skilled",
        "Small Frame"
    ];

    private readonly string[] _gitmSlots = ["Body", "Head", "Hands"];
    private readonly string[] _gitmSockets = ["Back", "Body", "Eye", "Feet", "Hands", "Head", "In-Hand", "Shoes", "Vanity"];
    private readonly string[] _gitmTypes = ["ITM", "AMO", "ARM", "???", "WEA"];

    private readonly string[] _gobjTypes = [".use", ".dor", ".con"];

    private readonly string[] _gwamDamageTypes = ["Ballistic", "Bio", "Electric", "EMP", "General", "Heat"];
    private readonly string[] _triggerTypes = ["Building", "Script", "Transition"];

    // ReSharper disable once InconsistentNaming - 2 is part of the abbreviation
    private int _2mwtChunk;
    private CommandList _commandList = null!;
    private ImFontPtr? _consoleFont;
    private dynamic _currentFile = null!;

    private int _eeovSelected = -1;
    private string _eeovTemp = "";

    private Vector3 _emapColor = new(1f, 1f, 1f);

    private int _eme2Index;
    private string[] _eme2Names = [];

    private int _emefIndex;
    private string[] _emefNames = [];

    private int _emepIndex;
    private string[] _emepNames = [];

    private Vector3 _emfgColor = new(1f, 1f, 1f);

    private int _emnoIndex;
    private string[] _emnoNames = [];

    // EMNP is most similar to 2MWT, single EMNP section with multiple "chunks"
    private int _emnpChunk;

    private int _emsdIndex;
    private string[] _emsdNames = [];

    private int _epthIndex;
    private string[] _epthNames = [];
    private int _epthPoint;
    private string _extension = null!;

    private string _filename = null!;
    private ImFontPtr? _font;
    private int _gcreEquipped;
    private int _gcreSkill;
    private int _gcreSocket;

    private int _gcreSpecial;
    private int _gcreTag;
    private string _gcreTemp = "";
    private int _gcreTrait;

    private int _gitmSocket;
    private GraphicsDevice _graphicsDevice = null!;
    private int _gwamIndex;
    private string[] _gwamNames = [];
    private ImGuiRenderer _imguiRenderer = null!;
    private string[] _stf = null!;

    private int _triggerIndex;
    private string[] _triggerNames = [];
    private int _triggerPoint;

    private TextEditor _vegTextEditor = null!;
    private Sdl2Window _window = null!;
    
    private SceneRenderer _sceneRenderer = null!;
    private GrpBrowserImGui _grpBrowserImGui = null!;
    private AssetLibraryImGui _assetLibrary = null!;

    // Gizmo input state. Lives across frames while the user is dragging.
    //   _gizmoDragAxis  -1 = idle, 0/1/2 = translate X/Y/Z, 3 = yaw
    //                   rotate (around world Y), 4/5/6 = planar
    //                   translate XY/XZ/YZ.
    //   _gizmoDragStartCenter   entity pivot at drag start (world space).
    //   _gizmoDragAppliedDelta  total translation already pushed via
    //                           TranslateGroup, so we can write the
    //                           final delta into EME2.l/EMEP on release
    //                           without touching the entity until then.
    //   _gizmoDragAppliedAngle  same but for the rotation ring — total
    //                           yaw applied via RotateGroup since drag
    //                           start, in radians.
    //   _gizmoDragStartAngle    initial pivot→cursor angle in the XZ
    //                           plane, captured at click. Δangle is
    //                           computed against this each frame.
    private int _gizmoDragAxis = -1;
    private Vector3 _gizmoDragStartCenter;
    private Vector2 _gizmoDragStartMouse;
    private Vector3 _gizmoDragAppliedDelta;
    private float _gizmoDragAppliedAngle;
    private float _gizmoDragStartAngle;

    // FNV-1a 32-bit hash of the current file's serialized form, captured
    // on every successful scene rebuild. Each DrawX-that-loads-the-scene
    // re-computes this every frame and rebuilds the scene if it differs,
    // so editor-side edits to skl/inventory/EME2/etc. are reflected in
    // the 3D viewport immediately. ResetTempValues() resets it to 0 so a
    // fresh file load always triggers the first-rebuild path.
    private uint _lastRenderHash;

    // Per-frame routing for viewport picking + translate-gizmo input.
    // `imageHovered` reflects whether the mouse is over the scene image
    // this frame; `viewportPx` is its pixel size. Behaviour:
    //   - Hover any pickable AABB → orange outline (HoveredGroupId).
    //   - Left click on a pickable → SelectedGroupId = hit (or null on
    //     click into empty space).
    //   - With something selected and not yet dragging, hovering the
    //     selection's gizmo axis line (within ~6 px in screen space)
    //     paints that axis yellow.
    //   - Left mouse press while hovering an axis enters drag mode for
    //     that axis. Cursor delta is projected onto the axis in world
    //     space and applied via TranslateGroup, leaving EME2 alone
    //     until release.
    //   - Drag release commits the accumulated delta to the source
    //     EME2.l record; the existing hash-rebuild path picks up the
    //     change next frame.
    private void HandleViewportPickAndGizmo(bool imageHovered, Vector2 viewportPx)
    {
        // Cursor relative to the image. Used by both pick and axis test.
        var imgMin = ImGui.GetItemRectMin();
        var mp = ImGui.GetMousePos() - imgMin;

        // ---- HoveredGroupId (always-on AABB feedback) ----
        if (imageHovered && _gizmoDragAxis < 0)
        {
            _sceneRenderer.HoveredGroupId = _sceneRenderer.TryPickGroup(mp, viewportPx);
        }
        else
        {
            _sceneRenderer.HoveredGroupId = null;
        }

        // ---- Gizmo axis hover (yellow when over an axis line) ----
        var hoveredAxis = -1;
        if (imageHovered && _gizmoDragAxis < 0
            && _sceneRenderer.SelectedGroupId.HasValue
            && _sceneRenderer.TryGetGroupCenter(_sceneRenderer.SelectedGroupId.Value, out var pivot))
        {
            hoveredAxis = PickGizmoAxis(pivot, mp, viewportPx);
            _sceneRenderer.GizmoActiveAxis = hoveredAxis;
        }
        else if (_gizmoDragAxis < 0)
        {
            _sceneRenderer.GizmoActiveAxis = -1;
        }

        // ---- Click handling ----
        if (imageHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left) && _gizmoDragAxis < 0)
        {
            if (hoveredAxis >= 0)
            {
                _gizmoDragAxis = hoveredAxis;
                _gizmoDragStartMouse = mp;
                _sceneRenderer.GizmoActiveAxis = hoveredAxis;
                if (_sceneRenderer.SelectedGroupId.HasValue
                    && _sceneRenderer.TryGetGroupCenter(_sceneRenderer.SelectedGroupId.Value, out var startC))
                {
                    _gizmoDragStartCenter = startC;
                }
                _gizmoDragAppliedDelta = Vector3.Zero;
                _gizmoDragAppliedAngle = 0f;
                if (hoveredAxis == 3
                    && TryRayPlanePivot(mp, viewportPx, _gizmoDragStartCenter, Vector3.UnitY, out var startHit))
                {
                    _gizmoDragStartAngle = MathF.Atan2(startHit.Z - _gizmoDragStartCenter.Z,
                                                       startHit.X - _gizmoDragStartCenter.X);
                }
            }
            else
            {
                // Plain pick — set or clear selection.
                _sceneRenderer.SelectedGroupId = _sceneRenderer.TryPickGroup(mp, viewportPx);
            }
        }

        // ---- Drag in progress ----
        if (_gizmoDragAxis >= 0)
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                if (!_sceneRenderer.SelectedGroupId.HasValue)
                {
                    _gizmoDragAxis = -1;
                    return;
                }

                if (_gizmoDragAxis <= 2)
                {
                    // Single-axis translate: project cursor onto the axis
                    // line in world space; delta along the axis = drag.
                    var axis = _gizmoDragAxis switch
                    {
                        0 => Vector3.UnitX,
                        1 => Vector3.UnitY,
                        _ => Vector3.UnitZ,
                    };
                    var startWorld = ScreenToAxisPoint(_gizmoDragStartMouse, viewportPx, _gizmoDragStartCenter, axis);
                    var nowWorld   = ScreenToAxisPoint(mp,                    viewportPx, _gizmoDragStartCenter, axis);
                    var totalDelta = (nowWorld - startWorld) * axis;

                    var incremental = totalDelta - _gizmoDragAppliedDelta;
                    if (incremental.LengthSquared() > 1e-12f)
                    {
                        _sceneRenderer.TranslateGroup(_sceneRenderer.SelectedGroupId.Value, incremental);
                        _gizmoDragAppliedDelta = totalDelta;
                    }
                }
                else if (_gizmoDragAxis == 3)
                {
                    // Yaw rotate: project cursor onto the XZ plane
                    // through pivot, take the angle from pivot to hit.
                    if (TryRayPlanePivot(mp, viewportPx, _gizmoDragStartCenter, Vector3.UnitY, out var hit))
                    {
                        var nowAngle = MathF.Atan2(hit.Z - _gizmoDragStartCenter.Z,
                                                   hit.X - _gizmoDragStartCenter.X);
                        // Negated so a clockwise drag (in the viewer's
                        // sense) rotates the entity clockwise. atan2 in
                        // XZ runs the opposite direction once you fold
                        // in CreateRotationY's sign convention.
                        var totalAngle = -WrapAngle(nowAngle - _gizmoDragStartAngle);
                        var incremental = totalAngle - _gizmoDragAppliedAngle;
                        if (MathF.Abs(incremental) > 1e-6f)
                        {
                            _sceneRenderer.RotateGroup(_sceneRenderer.SelectedGroupId.Value, incremental, _gizmoDragStartCenter);
                            _gizmoDragAppliedAngle = totalAngle;
                        }
                    }
                }
                else
                {
                    // Planar translate: project both cursor positions
                    // onto the plane through pivot whose normal is the
                    // out-of-plane axis. Delta = current - start.
                    var (a, b) = _sceneRenderer.PlaneAxes(_gizmoDragAxis);
                    var planeNormal = Vector3.Cross(a, b); // unit
                    if (TryRayPlanePivot(_gizmoDragStartMouse, viewportPx, _gizmoDragStartCenter, planeNormal, out var pStart)
                     && TryRayPlanePivot(mp,                    viewportPx, _gizmoDragStartCenter, planeNormal, out var pNow))
                    {
                        var totalDelta = pNow - pStart;
                        // Constrain to the plane (eliminate any normal-axis numerics).
                        totalDelta -= Vector3.Dot(totalDelta, planeNormal) * planeNormal;

                        var incremental = totalDelta - _gizmoDragAppliedDelta;
                        if (incremental.LengthSquared() > 1e-12f)
                        {
                            _sceneRenderer.TranslateGroup(_sceneRenderer.SelectedGroupId.Value, incremental);
                            _gizmoDragAppliedDelta = totalDelta;
                        }
                    }
                }
            }
            else
            {
                CommitGizmoDragToRecord();
                _gizmoDragAxis = -1;
                _gizmoDragAppliedDelta = Vector3.Zero;
                _gizmoDragAppliedAngle = 0f;
                _sceneRenderer.GizmoActiveAxis = -1;
            }
        }
    }

    // Write the accumulated drag delta(s) back to whichever record
    // owns the selected group. Translation X is negated because AddMap
    // builds the placement transform with -ent.l.X / -ep.p.X. Rotation
    // sign matches the negation in CreateRotationY(-ent.l.W).
    private void CommitGizmoDragToRecord()
    {
        if (!_sceneRenderer.SelectedGroupId.HasValue) return;
        var gid = _sceneRenderer.SelectedGroupId.Value;
        if (_sceneRenderer.GroupToMapEntity.TryGetValue(gid, out var ent))
        {
            ent.l = new Vector4(
                ent.l.X - _gizmoDragAppliedDelta.X,
                ent.l.Y + _gizmoDragAppliedDelta.Y,
                ent.l.Z + _gizmoDragAppliedDelta.Z,
                ent.l.W - _gizmoDragAppliedAngle);
        }
        else if (_sceneRenderer.GroupToEntryPoint.TryGetValue(gid, out var ep))
        {
            ep.p = new Vector3(
                ep.p.X - _gizmoDragAppliedDelta.X,
                ep.p.Y + _gizmoDragAppliedDelta.Y,
                ep.p.Z + _gizmoDragAppliedDelta.Z);
            ep.r -= _gizmoDragAppliedAngle;
        }
    }

    // Cast the cursor ray and intersect it with the plane through
    // `planePoint` whose normal is `planeNormal`. Used for the rotation
    // ring (Y-up plane) and the planar-translate handles. Returns false
    // when the ray is parallel to the plane.
    private bool TryRayPlanePivot(Vector2 mp, Vector2 viewportPx, Vector3 planePoint, Vector3 planeNormal, out Vector3 hit)
    {
        hit = default;
        var ndcX = mp.X / viewportPx.X * 2f - 1f;
        var ndcY = 1f - mp.Y / viewportPx.Y * 2f;
        if (!Matrix4x4.Invert(_sceneRenderer.GetViewProjection(), out var invVp)) return false;
        var nearH = Vector4.Transform(new Vector4(ndcX, ndcY, 0f, 1f), invVp);
        var farH  = Vector4.Transform(new Vector4(ndcX, ndcY, 1f, 1f), invVp);
        if (nearH.W == 0f || farH.W == 0f) return false;
        var nearW = new Vector3(nearH.X, nearH.Y, nearH.Z) / nearH.W;
        var farW  = new Vector3(farH.X,  farH.Y,  farH.Z)  / farH.W;
        var rayDir = Vector3.Normalize(farW - nearW);
        var denom = Vector3.Dot(rayDir, planeNormal);
        if (MathF.Abs(denom) < 1e-6f) return false;
        var t = Vector3.Dot(planePoint - nearW, planeNormal) / denom;
        if (t < 0f) return false;
        hit = nearW + rayDir * t;
        return true;
    }

    // Delete-key removal for the currently-selected map entity. Walks
    // the same dictionaries CommitGizmoDragToRecord does (group → EME2c
    // or EMEPc) and removes the matching record from the loaded Map's
    // list. The hash-driven rebuild picks up the change next frame.
    // Skipped while ImGui is consuming keyboard input (so Delete still
    // works in inspector text fields), and during an active gizmo drag.
    private void HandleDeleteSelected()
    {
        if (_gizmoDragAxis >= 0) return;
        if (!_sceneRenderer.SelectedGroupId.HasValue) return;
        if (_currentFile is not Map map) return;
        if (ImGui.GetIO().WantTextInput) return;
        if (!ImGui.IsKeyPressed(ImGuiKey.Delete)) return;

        var gid = _sceneRenderer.SelectedGroupId.Value;
        if (_sceneRenderer.GroupToMapEntity.TryGetValue(gid, out var ent))
        {
            map.EME2.Remove(ent);
        }
        else if (_sceneRenderer.GroupToEntryPoint.TryGetValue(gid, out var ep))
        {
            map.EMEP.Remove(ep);
        }
        else return;

        _sceneRenderer.SelectedGroupId = null;
        _sceneRenderer.HoveredGroupId = null;
    }

    private static float WrapAngle(float a)
    {
        a = (a + MathF.PI) % MathF.Tau;
        if (a < 0f) a += MathF.Tau;
        return a - MathF.PI;
    }

    // Distance in screen pixels at which an axis "counts" as hovered.
    private const float GizmoPickRadiusPx = 8f;

    // Returns the axis/handle the mouse is over (closest match within
    // the per-handle hit radius), or -1 for none. Numbering matches
    // _gizmoDragAxis: 0/1/2 = X/Y/Z, 3 = yaw ring, 4/5/6 = XY/XZ/YZ
    // planar squares. Order checks so planes win over the axes that
    // border them (planes are inside the axis end-points), and the
    // rotation ring is only picked when the mouse isn't over an axis.
    private int PickGizmoAxis(Vector3 pivot, Vector2 mp, Vector2 viewportPx)
    {
        // 1. Planar-translate squares — point-in-quad test on the
        //    screen-projected corners. Tested first because they sit
        //    inside the axis lines and would lose otherwise.
        for (var p = 4; p <= 6; p++)
        {
            Span<Vector3> corners3 = stackalloc Vector3[4];
            _sceneRenderer.GetPlaneHandleCorners(p, pivot, corners3);
            Span<Vector2> corners2 = stackalloc Vector2[4];
            var allOnScreen = true;
            for (var i = 0; i < 4; i++)
            {
                if (!_sceneRenderer.WorldToScreen(corners3[i], viewportPx, out corners2[i]))
                {
                    allOnScreen = false; break;
                }
            }
            if (allOnScreen && PointInQuad(mp, corners2)) return p;
        }

        // 2. Translate axes
        var len = _sceneRenderer.GizmoAxisLength(pivot);
        Span<Vector3> ends = stackalloc Vector3[]
        {
            pivot + Vector3.UnitX * len,
            pivot + Vector3.UnitY * len,
            pivot + Vector3.UnitZ * len,
        };
        var bestAxis = -1;
        var bestDist = GizmoPickRadiusPx;
        if (_sceneRenderer.WorldToScreen(pivot, viewportPx, out var p0))
        {
            for (var i = 0; i < 3; i++)
            {
                if (!_sceneRenderer.WorldToScreen(ends[i], viewportPx, out var p1)) continue;
                var d = PointSegmentDistance(mp, p0, p1);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestAxis = i;
                }
            }
        }
        if (bestAxis >= 0) return bestAxis;

        // 3. Rotation ring — closest screen-space distance to any
        //    segment of the projected ring polygon.
        Span<Vector3> ring3 = stackalloc Vector3[48];
        _sceneRenderer.GetRotationRingPoints(pivot, ring3);
        Span<Vector2> ring2 = stackalloc Vector2[48];
        Span<bool> ringOk = stackalloc bool[48];
        for (var i = 0; i < ring3.Length; i++)
            ringOk[i] = _sceneRenderer.WorldToScreen(ring3[i], viewportPx, out ring2[i]);
        var ringBest = GizmoPickRadiusPx;
        for (var i = 0; i < ring3.Length; i++)
        {
            var j = (i + 1) % ring3.Length;
            if (!ringOk[i] || !ringOk[j]) continue;
            var d = PointSegmentDistance(mp, ring2[i], ring2[j]);
            if (d < ringBest) ringBest = d;
        }
        return ringBest < GizmoPickRadiusPx ? 3 : -1;
    }

    private static bool PointInQuad(Vector2 p, Span<Vector2> q)
    {
        // Treat as two triangles (0,1,2) + (0,2,3); convex quads only,
        // which the planar handles always are.
        return PointInTri(p, q[0], q[1], q[2]) || PointInTri(p, q[0], q[2], q[3]);
    }

    private static bool PointInTri(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float Sign(Vector2 p1, Vector2 p2, Vector2 p3) =>
            (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        var d1 = Sign(p, a, b);
        var d2 = Sign(p, b, c);
        var d3 = Sign(p, c, a);
        var hasNeg = d1 < 0 || d2 < 0 || d3 < 0;
        var hasPos = d1 > 0 || d2 > 0 || d3 > 0;
        return !(hasNeg && hasPos);
    }

    private static float PointSegmentDistance(Vector2 p, Vector2 a, Vector2 b)
    {
        var ab = b - a;
        var ap = p - a;
        var lenSq = ab.LengthSquared();
        if (lenSq < 1e-6f) return Vector2.Distance(p, a);
        var t = Math.Clamp(Vector2.Dot(ap, ab) / lenSq, 0f, 1f);
        var proj = a + ab * t;
        return Vector2.Distance(p, proj);
    }

    // Project the screen-space mouse position into world space and
    // intersect that ray with the line { axisPoint + t*axisDir | t∈ℝ },
    // returning the closest point on the axis line. Used during drag to
    // map cursor motion to a world-space translation.
    private Vector3 ScreenToAxisPoint(Vector2 mp, Vector2 viewportPx, Vector3 axisPoint, Vector3 axisDir)
    {
        // Build a world-space ray from the camera through `mp`.
        var ndcX = mp.X / viewportPx.X * 2f - 1f;
        var ndcY = 1f - mp.Y / viewportPx.Y * 2f;
        if (!Matrix4x4.Invert(_sceneRenderer.GetViewProjection(), out var invVp))
            return axisPoint;
        var nearH = Vector4.Transform(new Vector4(ndcX, ndcY, 0f, 1f), invVp);
        var farH  = Vector4.Transform(new Vector4(ndcX, ndcY, 1f, 1f), invVp);
        if (nearH.W == 0f || farH.W == 0f) return axisPoint;
        var nearW = new Vector3(nearH.X, nearH.Y, nearH.Z) / nearH.W;
        var farW  = new Vector3(farH.X,  farH.Y,  farH.Z)  / farH.W;
        var rayOrigin = nearW;
        var rayDir = Vector3.Normalize(farW - nearW);

        // Closest-point on two lines (the camera ray and the gizmo axis
        // line). Standard formula: solve for s on the ray that minimises
        // distance to the axis line.
        var a = axisDir;          // axis dir (unit)
        var b = rayDir;           // ray dir (unit)
        var w0 = axisPoint - rayOrigin;
        var aDotB = Vector3.Dot(a, b);
        var denom = 1f - aDotB * aDotB;
        if (MathF.Abs(denom) < 1e-6f) return axisPoint; // parallel
        // Closest point on line a from line b:
        //   s = ((u·v)(v·w0) − u·w0) / (1 − (u·v)²)
        // The previous version divided by -denom, returning -s, which made
        // the entity move in the opposite direction from the cursor.
        var sc = (aDotB * Vector3.Dot(b, w0) - Vector3.Dot(a, w0)) / denom;
        return axisPoint + a * sc;
    }

    // Hashes the resource via its existing ViewMatrix * ProjectionMatrix
    // composition so HandleViewportPickAndGizmo can do its own
    // projections without re-implementing the camera. (Kept inside the
    // renderer because the camera is private there.)

    // Hashes the resource via its existing ToByte() serializer. Any
    // change to a field that affects ToByte's output will produce a
    // different hash; fields that don't are inert (cheap insurance
    // against missing edge cases per type). Returns 0 on null/error so
    // callers don't blow up while a partially-edited file is invalid.
    private static uint ComputeRenderHash(dynamic? file)
    {
        if (file == null) return 0;
        try
        {
            uint h = 2166136261u;
            foreach (byte b in (IEnumerable<byte>)file.ToByte())
            {
                h ^= b;
                h *= 16777619u;
            }
            return h;
        }
        catch { return 0; }
    }

    public void Run()
    {
        ResourceSystem.Initialize();
        // Initialize the window
        var windowCreateInfo = new WindowCreateInfo
        {
            X = 100,
            Y = 100,
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = "Van Buren Editor"
        };
        _window = VeldridStartup.CreateWindow(ref windowCreateInfo);
        _window.Resized += OnWindowResized;
        _window.KeyDown += OnKeyDown;
        // reuse DWM code from AltUI
        ThemeProvider.SetupWindow(_window.Handle, 2);

        // Initialize the graphics device
        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window);
        _commandList = _graphicsDevice.ResourceFactory.CreateCommandList();

        // Initialize ImGui
        _imguiRenderer = new ImGuiRenderer(
            _graphicsDevice,
            _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            _window.Width,
            _window.Height);
        ImGui.StyleColorsDark();
        ApplyAltUIColours();

        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        // Veldrid's ImGuiRenderer constructor calls NewFrame() before we get here, so the
        // first ini load happens with DockingEnable off — which causes ImGui to undock every
        // window. Reload the ini now that the flag is set so dock state actually restores.
        if (File.Exists("imgui.ini"))
            ImGui.LoadIniSettingsFromDisk("imgui.ini");
        if (File.Exists(@"C:\Windows\Fonts\segoeui.ttf"))
        {
            _font = io.Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\segoeui.ttf", 17f);
            _imguiRenderer.RecreateFontDeviceTexture();
        }

        if (File.Exists(@"C:\Windows\Fonts\CascadiaCode.ttf"))
        {
            _consoleFont = io.Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\CascadiaCode.ttf", 16f);
            _imguiRenderer.RecreateFontDeviceTexture();
        }
        
        var desktopResolution = Screen.PrimaryScreen.Bounds.Size;
        _sceneRenderer = new SceneRenderer(_graphicsDevice, _imguiRenderer, _window, _commandList, desktopResolution.Width, desktopResolution.Height);
        // The browser is a fully separate top-level OS window — owns its
        // own Sdl2Window / swapchain / ImGui context / renderer / scene,
        // and doesn't touch the editor's _currentFile or main viewport
        // unless the user explicitly clicks "Load in Editor" on a
        // browsed resource.
        _grpBrowserImGui = new GrpBrowserImGui(_graphicsDevice,
            loadInEditor: (name, ext, resource) =>
            {
                // Mirror LoadFile()'s prologue: STF is required by all
                // drawer methods (DrawGENT calls STFBox which indexes into
                // _stf). Without this we'd null-deref the moment the user
                // clicks Load in Editor on a CRT/etc.
                if (!CheckAndLoadStf()) return;
                ResetTempValues();
                _filename = name;
                _extension = "." + ext;
                _currentFile = resource!;
            });

        // Asset library: standalone window, used to drop new entities
        // into the currently-loaded map. The placement callback below
        // appends to the loaded Map's EME2 list and selects the new
        // entry so the gizmo appears immediately and the user can move
        // it. Bails silently when no map is loaded — the menu item is
        // available regardless, so the user finds out by clicking.
        _assetLibrary = new AssetLibraryImGui(_graphicsDevice,
            placeInMap: (name, ext) =>
            {
                if (_currentFile is not Map map) return;
                var spawn = _sceneRenderer.GetCameraGroundTarget();
                // World→record: AddMap places entities at (-l.X, l.Y, l.Z)
                // so we negate X back when storing.
                var ent = new EME2c
                {
                    name = $"{name}.{ext}",
                    l = new Vector4(-spawn.X, spawn.Y, spawn.Z, 0f),
                };
                map.EME2.Add(ent);
                // The next DrawMap() will see the hash mismatch, rebuild
                // the scene with the new entity, and the user's gizmo
                // can grab it. Pre-select by group id: the new entity
                // gets the highest id since it's appended last.
                _sceneRenderer.SelectedGroupId = map.EME2.Count;
            });

        MainLoop();
    }

    private void MainLoop()
    {
        var stopwatch = Stopwatch.StartNew();
        const double targetFrameTime = 1.0 / 60.0;

        // set window icon
        var stream = new MemoryStream();
        var bmp = Resources.VBEditor.ToBitmap();
        bmp.Save(stream, ImageFormat.Bmp);
        Sdl2WindowExtensions.SetWindowIcon(_window, stream.ToArray());

        while (_window.Exists)
        {
            var frameStartTime = stopwatch.Elapsed.TotalSeconds;

            // Handle input
            var inputSnapshot = _window.PumpEvents();
            _imguiRenderer.Update((float)targetFrameTime, inputSnapshot);
            InputTracker.UpdateFrameInput(inputSnapshot);
            if (!_window.Exists) break;

            // Render 3D scene
            if (_sceneRenderer.Initialized) _sceneRenderer.DrawScene((float)targetFrameTime);

            // Update ImGui
            if (_font is { } font) ImGui.PushFont(font);

            DrawImGui();

            // Render frame
            _commandList.Begin();
            _commandList.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.CornflowerBlue);
            _imguiRenderer.Render(_graphicsDevice, _commandList);
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
            _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);

            // Pump + render the standalone GRP browser window if it's open.
            // No-op when the user hasn't toggled it visible. Has its own
            // ImGui context so we save/restore the editor context around it.
            _grpBrowserImGui.Frame((float)targetFrameTime);
            _assetLibrary.Frame((float)targetFrameTime);

            Sdl2WindowExtensions.UpdateMouseCursor();

            // Frame timing control
            var frameEndTime = stopwatch.Elapsed.TotalSeconds;
            var frameDuration = frameEndTime - frameStartTime;
            if (!(frameDuration < targetFrameTime)) continue;
            var sleepTime = (int)((targetFrameTime - frameDuration) * 1000);
            Thread.Sleep(sleepTime);
        }

        DisposeResources();
    }

    private void OnWindowResized()
    {
        _graphicsDevice.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
        _imguiRenderer.WindowResized(_window.Width, _window.Height);
    }

    private void OnKeyDown(KeyEvent keyEvent)
    {
        if (_sceneRenderer.AllowInput)
            return; // Allow scene renderer to handle input first
        switch (keyEvent)
        {
            case { Key: Key.O, Modifiers: ModifierKeys.Control }:
                OpenFile();
                break;
            case { Key: Key.S, Modifiers: ModifierKeys.Control }:
                SaveFile();
                break;
        }
    }

    private void DisposeResources()
    {
        _commandList.Dispose();
        _imguiRenderer.Dispose();
        _graphicsDevice.Dispose();
    }

    private Vector4 ColToVec(Color col)
    {
        return new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, col.A / 255f);
    }

    public static Vector3 ColToVec3(Color col)
    {
        return new Vector3(col.R / 255f, col.G / 255f, col.B / 255f);
    }

    // TODO: ensure all used colors are set from ThemeProvider
    private void ApplyAltUIColours()
    {
        var colors = ImGui.GetStyle().Colors;
        colors[0] = ColToVec(ThemeProvider.Theme.Colors.LightText);
        colors[1] = ColToVec(ThemeProvider.Theme.Colors.DisabledText);
        colors[2] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[3] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground); // ChildBg — was a placeholder red until something used BeginChild

        colors[4] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[5] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[6] = new Vector4(0f, 0f, 0f, 0f);
        colors[7] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[8] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[9] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
        colors[10] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[11] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[12] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[13] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[14] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[15] = new Vector4(0.34f, 0.34f, 0.34f, 1f);
        colors[16] = new Vector4(0.4f, 0.4f, 0.4f, 1f);
        colors[17] = new Vector4(0.56f, 0.56f, 0.56f, 1f);
        colors[18] = ColToVec(ThemeProvider.Theme.Colors.LightText);
        colors[19] = new Vector4(0.34f, 0.34f, 0.34f, 1f);
        colors[20] = new Vector4(0.56f, 0.56f, 0.56f, 1f);
        colors[21] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[22] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[23] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
        colors[24] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[25] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[26] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[27] = new Vector4(0f, 0f, 0f, 0f);
        colors[28] = new Vector4(0f, 0f, 0f, 0f);
        colors[29] = new Vector4(0f, 0f, 0f, 0f);
        colors[30] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[31] = new Vector4(0.44f, 0.44f, 0.44f, 1f);
        colors[32] = new Vector4(0.4f, 0.44f, 0.47f, 1f);
        colors[33] = ColToVec(ThemeProvider.Theme.Colors.LightBackground);
        colors[34] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[35] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[36] = ColToVec(ThemeProvider.Theme.Colors.HeaderBackground);
        colors[37] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[38] = ColToVec(ThemeProvider.Theme.Colors.DefaultBackground);
        colors[39] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[40] = new Vector4(0.15f, 0.15f, 0.15f, 1f);
        colors[41] = ColToVec(ThemeProvider.Theme.Colors.TransparentBackground);
        colors[42] = new Vector4(1f, 0f, 1f, 1f);
        colors[43] = new Vector4(0f, 1f, 1f, 1f);
        colors[44] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);
        colors[45] = new Vector4(0f, 0f, 0f, 1f);
        colors[46] = new Vector4(1f, 0f, 1f, 1f);
        colors[47] = new Vector4(0.2f, 0.22f, 0.23f, 1f);
        colors[48] = new Vector4(0.33f, 0.67f, 0.86f, 1f);
        colors[49] = new Vector4(0.24f, 0.24f, 0.24f, 1f);
        colors[50] = new Vector4(1f, 0f, 1f, 1f);
        colors[51] = new Vector4(0f, 1f, 1f, 1f);
        colors[52] = ColToVec(ThemeProvider.Theme.Colors.GreySelection);

        var style = ImGui.GetStyle();
        style.ScrollbarSize = 15;
        style.WindowRounding = 7;
        style.ChildRounding = 4;
        style.FrameRounding = 3;
        style.PopupRounding = 4;
        style.ScrollbarRounding = 9;
        style.GrabRounding = 3;
        style.TabRounding = 4;
        style.WindowBorderSize = 1f;
        style.FrameBorderSize = 1f;
    }

    // outputs the current ImGui colour settings to a C++ function (for use in VBExtender's ingame menu)
    private static string GetColourCode()
    {
        var sb = new StringBuilder();
        sb.AppendLine("void embraceTheDarkness()");
        sb.AppendLine("{");
        sb.AppendLine("\tImVec4* colors = ImGui::GetStyle().Colors;");
        var colors = ImGui.GetStyle().Colors;
        for (var i = 0; i < (int)ImGuiCol.COUNT; i++)
        {
            var col = colors[i];
            sb.AppendLine($"\tcolors[{i}] = ImVec4({col.X}f, {col.Y}f, {col.Z}f, {col.W}f);");
        }

        sb.AppendLine();
        // style settings
        sb.AppendLine("\tImGuiStyle& style = ImGui::GetStyle();");
        // get style settings
        var style = ImGui.GetStyle();
        sb.AppendLine($"\tstyle.ScrollbarSize = {style.ScrollbarSize};");
        sb.AppendLine($"\tstyle.WindowRounding = {style.WindowRounding};");
        sb.AppendLine($"\tstyle.ChildRounding = {style.ChildRounding};");
        sb.AppendLine($"\tstyle.FrameRounding = {style.FrameRounding};");
        sb.AppendLine($"\tstyle.PopupRounding = {style.PopupRounding};");
        sb.AppendLine($"\tstyle.ScrollbarRounding = {style.ScrollbarRounding};");
        sb.AppendLine($"\tstyle.GrabRounding = {style.GrabRounding};");
        sb.AppendLine($"\tstyle.TabRounding = {style.TabRounding};");
        sb.AppendLine($"\tstyle.WindowBorderSize = {style.WindowBorderSize}f;");
        sb.AppendLine($"\tstyle.FrameBorderSize = {style.FrameBorderSize}f;");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private void DrawImGui()
    {
        // Create menu bar
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.BeginMenu("New"))
                {
                    var t = new[]
                        { ".amo", ".arm", ".con", ".crt", ".dor", ".itm", ".map", ".use", ".veg", ".wea" };
                    foreach (var s in t)
                    {
                        if (!ImGui.MenuItem(s)) continue;
                        ResetTempValues();
                        _currentFile = s switch
                        {
                            ".amo" => new AMO(),
                            ".arm" => new ARM(),
                            ".con" => new USE(USEType.CON),
                            ".crt" => new CRT(),
                            ".dor" => new USE(USEType.DOR),
                            //".int" => new INT(), // TODO: how to handle new INT?
                            ".itm" => new ITM(),
                            ".map" => new Map(),
                            ".use" => new USE(),
                            ".veg" => new VEG([]),
                            ".wea" => new WEA(),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        if (s == ".veg") InitVEG();
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Open", "Ctrl+O"))
                    OpenFile();
                if (ImGui.MenuItem("Open From .grp"))
                    OpenFromGRP();
                if (ImGui.MenuItem("Save", "Ctrl+S"))
                    SaveFile();
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Options"))
            {
                if (ImGui.MenuItem("Set English.stf Location"))
                    SetEngStfLocation();
                if (ImGui.MenuItem("Enable STF Editing", "", Settings.Default.STFEditEnabled))
                {
                    Settings.Default.STFEditEnabled = !Settings.Default.STFEditEnabled;
                    Settings.Default.Save();
                }
                
                if (ImGui.MenuItem("Show Skeleton", "", _sceneRenderer.DrawSkeletonOverlay))
                    _sceneRenderer.DrawSkeletonOverlay = !_sceneRenderer.DrawSkeletonOverlay;
                if (ImGui.MenuItem("Animate", "", _sceneRenderer.AnimateSkinned))
                    _sceneRenderer.AnimateSkinned = !_sceneRenderer.AnimateSkinned;
                
                if (ImGui.MenuItem("Print Camera Location/Rotation"))
                    _sceneRenderer.PrintCameraDiagnostics();

                if (ImGui.MenuItem("Show Map Overlays", "", _sceneRenderer.ShowMapOverlays))
                    _sceneRenderer.ShowMapOverlays = !_sceneRenderer.ShowMapOverlays;

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools"))
            {
                if (ImGui.MenuItem(".stf to .txt"))
                    FullSTFToText();
                if (ImGui.MenuItem(".txt to .stf"))
                    FullTextToSTF();
                if (ImGui.MenuItem("Extract All .grp Files"))
                    ExtractAllGRPFiles();
                if (ImGui.MenuItem("Extract + Convert All .grp Files"))
                    ExtractAllGRPFiles(true);
                if (ImGui.MenuItem("Show .grp Browser", "", _grpBrowserImGui.IsOpen))
                    _grpBrowserImGui.Toggle();
                if (ImGui.MenuItem("Asset Library", "", _assetLibrary.IsOpen))
                    _assetLibrary.Toggle();

                if (ImGui.MenuItem(".png to .rle"))
                {
                    var ofd = new OpenFileDialog
                    {
                        Filter = "PNG Files|*.png",
                        Title = "Select a PNG file to convert to RLE"
                    };
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        var bmp = new Bitmap(ofd.FileName);
                        var rle = GrpBrowser.EncodeRLE(bmp);
                        var sfd = new SaveFileDialog
                        {
                            Filter = "RLE Files|*.rle",
                            Title = "Save the RLE file"
                        };
                        if (sfd.ShowDialog() == DialogResult.OK)
                            File.WriteAllBytes(sfd.FileName, rle);
                    }
                }

                //if (ImGui.MenuItem("Get ImGui Colours"))
                //    File.WriteAllText("ImGuiColours.txt", GetColourCode());
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        // Create a fullscreen window for docking
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());

        // Browser is its own OS window now — nothing to draw inline here.
        // GrpBrowserImGui.Frame() is called from MainLoop() after the
        // editor's swapbuffers.

        if (_sceneRenderer.Initialized && !_sceneRenderer.IsCleared()) {
            bool hovered = false;
            bool focused = false;

            ImGui.Begin("Viewport");
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, new Vector2(0, 0));
            // Zero out the 1px frame border that AltUI's style applies to
            // every frame widget. ImageButton honours that border by
            // shrinking the displayed image by 2px and offsetting it
            // (+1, +1), which destroys 1:1 texture sampling — every
            // 1px line in the scene ends up averaged across two
            // display pixels and develops gaps.
            ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 0f);
            ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero); // Remove button background
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero); // Remove hover background
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero); // Remove active background
            var availableSize = ImGui.GetContentRegionAvail();
            // Logical (ImGui) display size is integer-floored so the
            // ImageButton lays out on whole-pixel boundaries.
            uint pixelW = (uint)MathF.Max(1, MathF.Floor(availableSize.X));
            uint pixelH = (uint)MathF.Max(1, MathF.Floor(availableSize.Y));
            // Clamp to the underlying framebuffer texture's actual
            // dimensions. The framebuffer is fixed at desktop size at
            // SceneRenderer construction; we render to a sub-rect inside
            // it and crop the display via UVs so each texel maps 1:1 to
            // a viewport pixel — never resampled.
            pixelW = Math.Min(pixelW, _sceneRenderer.FbWidth);
            pixelH = Math.Min(pixelH, _sceneRenderer.FbHeight);
            var drawSize = new Vector2(pixelW, pixelH);
            if (_sceneRenderer.Width != pixelW || _sceneRenderer.Height != pixelH)
            {
                _sceneRenderer.HandleWindowResize(pixelW, pixelH);
            }
            // UV crop: top-left (pixelW × pixelH) of the framebuffer
            // texture. Anything outside this rect in the framebuffer is
            // stale / unrendered and intentionally never sampled.
            var uv0 = new Vector2(0f, 0f);
            var uv1 = new Vector2(
                pixelW / (float)_sceneRenderer.FbWidth,
                pixelH / (float)_sceneRenderer.FbHeight);
            ImGui.ImageButton("SceneImage", _sceneRenderer.ImGuiBinding, drawSize, uv0, uv1);
            var imageHovered = ImGui.IsItemHovered();
            // Disable WASD/orbit while a gizmo drag is in flight so the
            // camera doesn't fly around when the user holds left mouse.
            _sceneRenderer.AllowInput = imageHovered && _gizmoDragAxis < 0;

            HandleViewportPickAndGizmo(imageHovered, drawSize);
            HandleDeleteSelected();

            ImGui.End();

            ImGui.PopStyleColor(3); // Restore button styles
            ImGui.PopStyleVar(4);   // FramePadding, ItemSpacing, ItemInnerSpacing, FrameBorderSize
        }
        
        switch (_currentFile)
        {
            case CRT:
                DrawCRT();
                break;
            case Map:
                DrawMap();
                break;
            case USE:
                DrawUSE();
                break;
            case ARM:
                DrawARM();
                break;
            case AMO:
                DrawAMO();
                break;
            case WEA:
                DrawWEA();
                break;
            case ITM:
                DrawITM();
                break;
            case VEG:
                DrawVEG();
                break;
            case INT:
                DrawINT();
                break;
        }
    }

// reset all temporary values used in the editor (indexes, lists, etc.)
    private void ResetTempValues()
    {
        _sceneRenderer.Reset();
        _lastRenderHash = 0;
        _intSelectedObj = 0;
        ClearIntTextureCache();
        _eeovSelected = -1;
        _eeovTemp = "";
        _gcreSpecial = 0;
        _gcreSkill = 0;
        _gcreTrait = 0;
        _gcreTag = 0;
        _gcreSocket = 0;
        _gcreEquipped = -1;
        _gcreTemp = "";
        _gwamIndex = 0;
        _gwamNames = [];
        _eme2Index = 0;
        _eme2Names = [];
        _emepIndex = 0;
        _emepNames = [];
        _emefIndex = 0;
        _emefNames = [];
        _emsdIndex = 0;
        _emsdNames = [];
        _epthIndex = 0;
        _epthNames = [];
        _epthPoint = 0;
        _triggerIndex = 0;
        _triggerNames = [];
        _triggerPoint = 0;
        _2mwtChunk = 0;
    }

    private void STFBox(string text, ref int sr)
    {
        ImGui.Text(text);
        ImGui.InputInt("##" + text, ref sr);
        if (sr == 0) return;
        ImGui.SameLine();
        ImGui.PushItemWidth(-1);
        if (!Settings.Default.STFEditEnabled)
            ImGui.BeginDisabled();
        if (sr == _stf.Length)
        {
            Array.Resize(ref _stf, _stf.Length + 1);
            _stf[sr] = "";
        }

        ImGui.InputText("##" + text + "in", ref _stf[sr - 1], 100u);
        if (!Settings.Default.STFEditEnabled)
            ImGui.EndDisabled();
        ImGui.PopItemWidth();
    }

    private static void HelpMarker(string desc)
    {
        ImGui.TextDisabled("(?)");
        if (!ImGui.IsItemHovered(ImGuiHoveredFlags.DelayShort)) return;
        ImGui.BeginTooltip();
        ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
        ImGui.TextUnformatted(desc);
        ImGui.PopTextWrapPos();
        ImGui.EndTooltip();
    }

    private void DrawEntity()
    {
        // Live preview: rebuild the scene whenever the resource's
        // serialized form changes (=user edited a relevant field). On
        // first load, also focus the camera. Subsequent edit-driven
        // rebuilds skip FocusCamera so the user doesn't lose their
        // current viewing angle on every keystroke.
        var hash = ComputeRenderHash(_currentFile);
        if (_sceneRenderer.IsCleared())
        {
            _sceneRenderer.CameraMode = CameraMode.Orbit;
            _sceneRenderer.AddEntity(_currentFile);
            _sceneRenderer.FocusCamera();
            _lastRenderHash = hash;
        }
        else if (hash != _lastRenderHash)
        {
            _sceneRenderer.Reset(preserveSelection: true);
            _sceneRenderer.AddEntity(_currentFile);
            _lastRenderHash = hash;
        }
        DrawEEN2();
        DrawGENT();
    }

    private void DrawCRT()
    {
        DrawEntity();
        DrawGCHR();
        DrawGCRE();
        DrawGWAM();
    }

    private void DrawITM()
    {
        DrawEntity();
        DrawGITM();
    }

    private void DrawMap()
    {
        var hash = ComputeRenderHash(_currentFile);
        if (_sceneRenderer.IsCleared())
        {
            // Free-fly is what you want for maps and full octree scenes
            // where you need to navigate inside the geometry.
            _sceneRenderer.CameraMode = CameraMode.Free;
            _sceneRenderer.AddMap((Map)_currentFile);
            _sceneRenderer.FocusCamera();
            _lastRenderHash = hash;
        }
        else if (hash != _lastRenderHash)
        {
            // Rebuild on edits (EME2 entity placement, EMAP.s1 octree
            // ref, etc.) without re-framing the camera so the user keeps
            // their current vantage point. Selection survives because
            // group ids are allocated in deterministic EME2 order — the
            // user's selected entity gets the same id post-rebuild.
            _sceneRenderer.Reset(preserveSelection: true);
            _sceneRenderer.AddMap((Map)_currentFile);
            _lastRenderHash = hash;
        }
        DrawEMAP();
        DrawEME2();
        DrawECAM();
        DrawEMEP();
        DrawEMEF();
        DrawEMSD();
        DrawEPTH();
        DrawTrigger();
        Draw2MWT();
        DrawEMFG();
        DrawEMNO();
        DrawEMNP();
    }

    private void DrawUSE()
    {
        DrawEntity();
        DrawGOBJ();
    }

    private void DrawARM()
    {
        var hash = ComputeRenderHash(_currentFile);
        if (_sceneRenderer.IsCleared())
        {
            _sceneRenderer.CameraMode = CameraMode.Orbit;
            _sceneRenderer.AddARM(_currentFile);
            _sceneRenderer.FocusCamera();
            _lastRenderHash = hash;
        }
        else if (hash != _lastRenderHash)
        {
            _sceneRenderer.Reset(preserveSelection: true);
            _sceneRenderer.AddARM(_currentFile);
            _lastRenderHash = hash;
        }
        DrawITM();
        DrawGIAR();
    }

    private void DrawAMO()
    {
        DrawITM();
        DrawGIAM();
    }

    private void DrawWEA()
    {
        DrawITM();
        DrawGIWP();
    }

    private void InitVEG()
    {
        var pushed = false;
        if (_consoleFont is { } font)
        {
            ImGui.PushFont(font);
            pushed = true;
        }

        _vegTextEditor = new TextEditor
        {
            Options =
            {
                IsColorizerEnabled = true
            },
            Renderer =
            {
                IsShowingWhitespace = false,
                Palette = _vegPalette
            },
            SyntaxHighlighter = new VEGSyntaxHighlighter()
        };
        if (pushed) ImGui.PopFont();
    }

    private void DrawVEG()
    {
        ImGui.Begin("VEG Editor");
        // VEG is stored as binary but edited in a custom textual language (based on .G3D)
        var pushed = false;
        if (_consoleFont is { } font)
        {
            ImGui.PushFont(font);
            pushed = true;
        }

        _vegTextEditor.Render("VEG Editor", new Vector2(0, 0));
        if (pushed) ImGui.PopFont();
    }

    // Read-only .int viewer. Cache is keyed by texture filename and rebuilt
    // lazily on first use; cleared in ResetTempValues when a new file loads.
    private int _intSelectedObj;
    private float _intCanvasZoom = 1.0f;
    private bool _intShowDebugFields;
    private bool _intShowFullScene = true;
    private bool _intHighlightSelected = true;
    // Render mode: 0 = all 9 fragments raw; 1 = center slot only; 2 = use
    // postName3[1] as a per-object border-render flag (nonzero ⇒ draw all 9,
    // zero ⇒ draw only the center). Default to the border-flag mode since it
    // matches the engine's actual gating; switch to mode 0 to debug.
    private int _intFragmentMode = 2;
    private static readonly string[] _intFragmentModeNames =
        ["All 9 (raw)", "Center only (slot 4)", "Border-flag (postName3[1])"];
    private readonly Dictionary<string, IntPtr> _intTexBindings = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Vector2> _intTexSizes = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Texture> _intTexOwners = [];

    private void ClearIntTextureCache()
    {
        foreach (var t in _intTexOwners)
        {
            try { _imguiRenderer?.RemoveImGuiBinding(t); } catch { /* tearing down */ }
            try { t.Dispose(); } catch { /* already gone */ }
        }
        _intTexOwners.Clear();
        _intTexBindings.Clear();
        _intTexSizes.Clear();
    }

    // Look up a texture by filename via ResourceSystem and bind it to the
    // ImGui renderer. Falls back from .tga → .bmp (and vice versa) since
    // .int files reference both. Returns IntPtr.Zero if not found.
    private IntPtr GetIntTextureBinding(string texName, out Vector2 size)
    {
        size = Vector2.Zero;
        if (string.IsNullOrEmpty(texName)) return IntPtr.Zero;
        if (_intTexBindings.TryGetValue(texName, out var cached))
        {
            size = _intTexSizes[texName];
            return cached;
        }

        ImageSharpTexture? img = ResourceSystem.FindResource(texName) as ImageSharpTexture;
        if (img == null)
        {
            // .tga ↔ .bmp swap fallback — .int files reference both extensions.
            var swapped = texName.EndsWith(".tga", StringComparison.OrdinalIgnoreCase)
                ? texName[..^4] + ".bmp"
                : texName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)
                    ? texName[..^4] + ".tga"
                    : null;
            if (swapped != null)
                img = ResourceSystem.FindResource(swapped) as ImageSharpTexture;
        }
        if (img == null)
        {
            _intTexBindings[texName] = IntPtr.Zero;
            _intTexSizes[texName] = Vector2.Zero;
            return IntPtr.Zero;
        }

        var factory = _graphicsDevice.ResourceFactory;
        var tex = img.CreateDeviceTexture(_graphicsDevice, factory);
        var binding = _imguiRenderer.GetOrCreateImGuiBinding(factory, tex);
        _intTexOwners.Add(tex);
        _intTexBindings[texName] = binding;
        size = new Vector2(tex.Width, tex.Height);
        _intTexSizes[texName] = size;
        return binding;
    }

    private void DrawINT()
    {
        ImGui.Begin("INT");
        if (_currentFile is not INT gui) { ImGui.End(); return; }

        ImGui.Text($"{gui.name}  rev={gui.revision}  objects={gui.objects.Count}");
        ImGui.SameLine();
        ImGui.SetNextItemWidth(220);
        ImGui.Combo("Fragments", ref _intFragmentMode, _intFragmentModeNames, _intFragmentModeNames.Length);
        ImGui.SameLine();
        ImGui.Checkbox("Full scene", ref _intShowFullScene);
        ImGui.SameLine();
        ImGui.Checkbox("Highlight selected", ref _intHighlightSelected);
        ImGui.SameLine();
        ImGui.Checkbox("Debug fields", ref _intShowDebugFields);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(120);
        ImGui.SliderFloat("Zoom", ref _intCanvasZoom, 0.25f, 2.0f, "%.2fx");

        if (gui.objects.Count == 0) { ImGui.End(); return; }
        if (_intSelectedObj < 0 || _intSelectedObj >= gui.objects.Count) _intSelectedObj = 0;

        var avail = ImGui.GetContentRegionAvail();
        ImGui.BeginChild("##intObjs", new Vector2(260, avail.Y), true);
        for (var i = 0; i < gui.objects.Count; i++)
        {
            var o = gui.objects[i];
            var kindName = o.magic.ToString();
            var label = $"[{(kindName.Length > 0 ? char.ToUpper(kindName[0]) : '?')}] {o.name}##{i}";
            if (ImGui.Selectable(label, i == _intSelectedObj))
                _intSelectedObj = i;
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip($"magic={(int)o.magic} ({kindName})\nrect1 {o.rect1.x1},{o.rect1.y1} → {o.rect1.x2},{o.rect1.y2}\nini={o.ini}");
        }
        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("##intRight", new Vector2(0, avail.Y), false);
        DrawIntObjectInspector(gui, gui.objects[_intSelectedObj]);
        ImGui.EndChild();

        ImGui.End();
    }

    private void DrawIntObjectInspector(INT gui, INT.obj o)
    {
        if (ImGui.BeginTabBar("##intTabs"))
        {
            if (ImGui.BeginTabItem("Properties"))
            {
                DrawIntPropertiesTab(o);
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Fragments (9-slice)"))
            {
                DrawIntFragmentsTab(o);
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Canvas"))
            {
                DrawIntCanvasTab(gui, o);
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
    }

    private void DrawIntPropertiesTab(INT.obj o)
    {
        ImGui.Text($"Kind: {o.magic} (magic={(int)o.magic})");
        ImGui.Text($"Name: {o.name}");
        ImGui.Text($"INI:  {o.ini}");
        ImGui.Text($"rect1: ({o.rect1.x1}, {o.rect1.y1}) → ({o.rect1.x2}, {o.rect1.y2})  [{o.rect1.Width}×{o.rect1.Height}]");
        ImGui.Text($"rect2: ({o.rect2.x1}, {o.rect2.y1}) → ({o.rect2.x2}, {o.rect2.y2})  [{o.rect2.Width}×{o.rect2.Height}]");
        ImGui.Text($"colorTint: 0x{o.colorTint:X8}  BGRA (0x80=full)  → display 0x{INT.ExpandBGRA(o.colorTint):X8}");
        if (!string.IsNullOrEmpty(o.label))
            ImGui.Text($"Label: {o.label}");

        if (o.buttonSounds is INT.ButtonSounds bs)
        {
            ImGui.Separator(); ImGui.TextDisabled("Button sounds");
            ImGui.Text($"down:  {bs.soundDown}");
            ImGui.Text($"up:    {bs.soundUp}");
            ImGui.Text($"hover: {bs.soundHover}");
        }

        if (o.trailerBlocks is { Count: > 0 } blocks)
        {
            ImGui.Separator(); ImGui.TextDisabled($"Font / style ({blocks.Count} block{(blocks.Count == 1 ? "" : "s")})");
            for (var i = 0; i < blocks.Count; i++)
            {
                var fb = blocks[i];
                var resolved = ResolveStringRef(fb.stringRef);
                var justifyName = (fb.pad is { Length: > 2 }) ? fb.pad[2] switch
                {
                    0 => "left", 1 => "right", 2 => "center", var x => $"?({x})"
                } : "?";
                ImGui.Text($"#{i}: stringRef={fb.stringRef}  font=\"{fb.font}\"  size={fb.fontSize}  color=0x{fb.color:X8}  justify={justifyName}");
                if (!string.IsNullOrEmpty(resolved))
                {
                    ImGui.SameLine();
                    ImGui.TextDisabled($"→ \"{resolved}\"");
                }
            }
        }

        if (_intShowDebugFields)
        {
            ImGui.Separator(); ImGui.TextDisabled("Raw / undecoded");
            ImGui.Text($"postName3:  {BitConverter.ToString(o.postName3 ?? [])}");
            ImGui.Text($"postRect2Byte: 0x{o.postRect2Byte:X2}");
            ImGui.Text($"lblW={o.lblW}  lblH={o.lblH}  lblA={o.lblA}  lblB={o.lblB}");
            ImGui.Text($"strRef=0x{o.strRef:X8}  lblFloat={o.lblFloat}");
            ImGui.Text($"last={o.last}");
            if (o.buttonRev12Int.HasValue)
                ImGui.Text($"buttonRev12Int={o.buttonRev12Int.Value}");
            if (o.trailerBlocks is { Count: > 0 } blocks2)
            {
                ImGui.Separator(); ImGui.TextDisabled("FontBlock pad bytes");
                for (var i = 0; i < blocks2.Count; i++)
                    ImGui.Text($"#{i}: {BitConverter.ToString(blocks2[i].pad ?? [])}");
            }
        }
    }

    private static readonly string[] _intSlotNames =
        ["UL", "UM", "UR", "L", "M", "R", "LL", "LM", "LR"];

    private void DrawIntFragmentsTab(INT.obj o)
    {
        if (ImGui.BeginTable("##intFrags", 8,
                ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("Slot");
            ImGui.TableSetupColumn("Use");
            ImGui.TableSetupColumn("Texture");
            ImGui.TableSetupColumn("Src");
            ImGui.TableSetupColumn("Dst");
            ImGui.TableSetupColumn("Tile");
            ImGui.TableSetupColumn("Tint");
            ImGui.TableSetupColumn("fEnd");
            ImGui.TableHeadersRow();
            for (var i = 0; i < o.fragments.Count; i++)
            {
                var f = o.fragments[i];
                var willDraw = ShouldDrawFragment(o, i);
                ImGui.TableNextRow();
                ImGui.TableNextColumn(); ImGui.Text(_intSlotNames[i]);
                ImGui.TableNextColumn(); ImGui.TextDisabled(willDraw ? "draw" : "skip");
                ImGui.TableNextColumn(); ImGui.Text(string.IsNullOrEmpty(f.texture) ? "(none)" : f.texture);
                ImGui.TableNextColumn(); ImGui.Text($"{f.srcW}×{f.srcH}");
                ImGui.TableNextColumn(); ImGui.Text($"{f.rect.Width}×{f.rect.Height}");
                ImGui.TableNextColumn(); ImGui.Text(DescribeTileMode(f));
                ImGui.TableNextColumn(); ImGui.Text($"0x{f.tint:X8}");
                ImGui.TableNextColumn(); ImGui.Text(f.fEnd.ToString());
            }
            ImGui.EndTable();
        }
    }

    private static string DescribeTileMode(INT.fragment f)
    {
        // We render with the texture's intrinsic dimensions as the tile unit, so
        // tiling happens whenever dst > tex_intrinsic. The src field appears to
        // be a hint, not the actual tile flag — derive mode from real geometry.
        var tilesX = f.srcW > 1 && f.rect.Width > f.srcW;
        var tilesY = f.srcH > 1 && f.rect.Height > f.srcH;
        if (tilesX && tilesY) return "TileBoth";
        if (tilesX) return "TileX";
        if (tilesY) return "TileY";
        return "Stretch";
    }

    private bool ShouldDrawFragment(INT.obj o, int slot)
    {
        return _intFragmentMode switch
        {
            1 => slot == 4,
            // postName3[1] gates the border (non-center) slots: nonzero = render
            // the 8 border slots of the 9-slice; zero = render only the center.
            2 => slot == 4 || (o.postName3 is { Length: > 1 } p && p[1] != 0),
            _ => true,
        };
    }

    // The engine draws every .int into a fixed 1024×768 backbuffer; anything
    // outside that area is clipped. Mirror that here so the editor preview
    // matches what's visible in-game.
    private static INT.rect SceneBounds() =>
        new() { x1 = 0, y1 = 0, x2 = 1024, y2 = 768 };

    private void DrawIntCanvasTab(INT gui, INT.obj selected)
    {
        // Game backbuffer is fixed 1024×768; everything outside is clipped. In
        // single-object mode we still place the object at its absolute coords
        // inside that 1024×768 canvas (its rect1 is in screen-space).
        INT.rect bounds = SceneBounds();
        var w = bounds.Width * _intCanvasZoom;
        var h = bounds.Height * _intCanvasZoom;
        ImGui.Text($"Canvas {(_intShowFullScene ? "(full scene)" : "(object only)")} 1024×768 @ {_intCanvasZoom:F2}x");
        ImGui.BeginChild("##intCanvas", ImGui.GetContentRegionAvail(), true, ImGuiWindowFlags.HorizontalScrollbar);
        var origin = ImGui.GetCursorScreenPos();
        var dl = ImGui.GetWindowDrawList();
        // Dark backdrop sized to the engine's 1024×768 game viewport.
        dl.AddRectFilled(origin, origin + new Vector2(w, h), 0xff181818);
        // Clip every draw to the viewport so off-canvas geometry doesn't leak.
        ImGui.PushClipRect(origin, origin + new Vector2(w, h), true);

        if (_intShowFullScene)
        {
            for (var oi = 0; oi < gui.objects.Count; oi++)
                DrawIntObject(dl, origin, gui.objects[oi], oi == _intSelectedObj && _intHighlightSelected);
        }
        else
        {
            DrawIntObject(dl, origin, selected, _intHighlightSelected);
        }

        ImGui.PopClipRect();
        ImGui.Dummy(new Vector2(w, h));
        ImGui.EndChild();
    }

    private void DrawIntObject(ImDrawListPtr dl, Vector2 origin, INT.obj o, bool highlighted)
    {
        for (var i = 0; i < o.fragments.Count; i++)
        {
            if (!ShouldDrawFragment(o, i)) continue;
            DrawFragment(dl, origin, o.fragments[i]);
        }
        DrawObjectText(dl, origin, o);
        if (highlighted)
        {
            var p1 = origin + new Vector2(o.rect1.x1, o.rect1.y1) * _intCanvasZoom;
            var p2 = origin + new Vector2(o.rect1.x2, o.rect1.y2) * _intCanvasZoom;
            dl.AddRect(p1, p2, 0xff00ffff, 0, 0, 2.0f);
        }
    }

    private string ResolveStringRef(int sr)
    {
        if (sr <= 0 || _stf is null) return "";
        var idx = sr - 1; // string table is 1-indexed (matches STFBox)
        if (idx < 0 || idx >= _stf.Length) return $"#{sr}";
        return _stf[idx] ?? "";
    }

    // Render visible text for Label/Edit objects via the global string table
    // (_stf) using FontBlock.stringRef as a 1-based index — same lookup the
    // STFBox editor widget uses for entity name refs.
    private void DrawObjectText(ImDrawListPtr dl, Vector2 origin, INT.obj o)
    {
        if (o.magic != INT.magic.label && o.magic != INT.magic.edit) return;
        if (o.trailerBlocks is not { Count: > 0 } blocks) return;
        var fb = blocks[0];
        var text = ResolveStringRef(fb.stringRef);
        if (string.IsNullOrEmpty(text)) text = !string.IsNullOrEmpty(o.label) ? o.label : $"#{fb.stringRef}";
        var color = INT.ToImGuiColor(fb.color);

        var rectP1 = origin + new Vector2(o.rect1.x1, o.rect1.y1) * _intCanvasZoom;
        var rectP2 = origin + new Vector2(o.rect1.x2, o.rect1.y2) * _intCanvasZoom;
        var rectW = rectP2.X - rectP1.X;
        var rectH = rectP2.Y - rectP1.Y;

        var textSize = ImGui.CalcTextSize(text);
        // pad[2]: 0=left, 1=right, 2=center horizontal alignment.
        var justify = (fb.pad is { Length: > 2 } pad) ? pad[2] : (byte)0;
        var x = justify switch
        {
            1 => rectP2.X - textSize.X,
            2 => rectP1.X + (rectW - textSize.X) * 0.5f,
            _ => rectP1.X,
        };
        // Vertically center within the rect — ImGui's default font has no
        // baseline metric we can rely on, so this is the cleanest default.
        var y = rectP1.Y + (rectH - textSize.Y) * 0.5f;
        dl.AddText(new Vector2(x, y), color, text);
    }

    private void DrawFragment(ImDrawListPtr dl, Vector2 origin, INT.fragment f)
    {
        if (string.IsNullOrEmpty(f.texture)) return;
        var binding = GetIntTextureBinding(f.texture, out _);
        if (binding == IntPtr.Zero) return;

        var z = _intCanvasZoom;
        var dstX = origin.X + f.rect.x1 * z;
        var dstY = origin.Y + f.rect.y1 * z;
        var dstW = f.rect.Width * z;
        var dstH = f.rect.Height * z;
        if (dstW <= 0 || dstH <= 0) return;

        // Stretch the entire texture to fill the destination rect — single
        // AddImage call. Avoids the miscalibrated tile loop that scaled
        // buttons to ~1.5x.
        var tint = INT.ToImGuiColor(f.tint);
        if (tint == 0) tint = 0xffffffff;
        dl.AddImage(binding, new Vector2(dstX, dstY), new Vector2(dstX + dstW, dstY + dstH),
            Vector2.Zero, Vector2.One, tint);
    }

    private void DrawEEN2()
    {
        if (_currentFile.EEN2 is not EEN2c een2) return;
        
        ImGui.Begin("EEN2");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Model Prefix");
        ImGui.InputText("##ModelPrefix", ref een2.skl, 100u);
        ImGui.Text("Inventory Icon");
        //ImGui.InputText("##InventoryIcon", ref een2.invtex, 100u);
        InputWithTypeAheadSearch.Draw("##InventoryIcon", ResourceSystem.Resources["Interface"]["tga"].Keys.Where(x => x.EndsWith("_INV")), ref een2.invtex, out var selected);
        if (!string.IsNullOrEmpty(selected))
            een2.invtex = selected + ".tga";
        ImGui.Text("Action Bar Icon");
        //ImGui.InputText("##ActionBarIcon", ref een2.acttex, 100u);
        InputWithTypeAheadSearch.Draw("##ActionBarIcon", ResourceSystem.Resources["Interface"]["tga"].Keys.Where(x => x.EndsWith("_ACT")), ref een2.acttex, out selected);
        if (!string.IsNullOrEmpty(selected))
            een2.acttex = selected + ".tga";
        ImGui.Checkbox("Selectable", ref een2.sel);
        ImGui.PopItemWidth();
        DrawEEOV(een2.EEOV);
        ImGui.End();
    }

    private void DrawEEOV(EEOVc eeov)
    {
        ImGui.PushItemWidth(-1);
        ImGui.Text("Entity Name");
        ImGui.InputText("##EntityName", ref eeov.s1, 100u);
        ImGui.Text("Dialog File");
        //ImGui.InputText("##DialogFile", ref eeov.s2, 100u);
        InputWithTypeAheadSearch.Draw("##DialogFile", ResourceSystem.Resources["Scripts"]["dlg"].Keys, ref eeov.s2, out var selected);
        if (!string.IsNullOrEmpty(selected))
            eeov.s2 = selected;
        ImGui.Text("Script File");
        //ImGui.InputText("##ScriptFile", ref eeov.s3, 100u);
        InputWithTypeAheadSearch.Draw("##ScriptFile", ResourceSystem.Resources["Scripts"]["amx"].Keys, ref eeov.s3, out selected);
        if (!string.IsNullOrEmpty(selected))
            eeov.s3 = selected + ".amx";
        ImGui.Text("Skin Texture");
        //ImGui.InputText("##SkinTexture", ref eeov.s4, 100u);
        InputWithTypeAheadSearch.Draw("##SkinTexture", ResourceSystem.Resources["Critters"]["tga"].Keys.Where(x => x.StartsWith("Skin_")), ref eeov.s4, out selected);
        if (!string.IsNullOrEmpty(selected))
            eeov.s4 = selected + ".tga";
        ImGui.Text("Effect File");
        //ImGui.InputText("##EffectFile", ref eeov.s5, 100u);
        InputWithTypeAheadSearch.Draw("##EffectFile", ResourceSystem.Resources["Effects"]["veg"].Keys, ref eeov.s5, out selected);
        if (!string.IsNullOrEmpty(selected))
            eeov.s5 = selected + ".veg";
        ImGui.Text("Inventory");
        ImGui.ListBox("##Inventory", ref _eeovSelected, eeov.inv, eeov.inv.Length);
        ImGui.PopItemWidth();
        ImGui.PushItemWidth(ImGui.GetWindowWidth() - 78);
        ImGui.InputText("##InventoryItem", ref _eeovTemp, 100u);
        ImGui.PopItemWidth();
        ImGui.SameLine();
        if (ImGui.Button(" + ##Inventory"))
        {
            if (string.IsNullOrWhiteSpace(_eeovTemp)) return;
            eeov.inv = eeov.inv.Append(_eeovTemp).ToArray();
            _eeovTemp = "";
        }

        ImGui.SameLine();
        if (!ImGui.Button(" - ##Inventory")) return;
        if (_eeovSelected == -1) return;
        var tmp = eeov.inv.ToList();
        tmp.RemoveAt(_eeovSelected);
        eeov.inv = tmp.ToArray();
        _eeovSelected = -1;
    }

    private void DrawGENT()
    {
        if (_currentFile.GENT is not GENTc gent) return;
        ImGui.Begin("GENT");
        ImGui.PushItemWidth(200f);
        STFBox("Hover String", ref gent.HoverSR);
        STFBox("Look String", ref gent.LookSR);
        STFBox("Name String", ref gent.NameSR);
        STFBox("Unknown String", ref gent.UnkwnSR);
        ImGui.Columns(2, "health");
        ImGui.SetColumnWidth(0, 215);
        ImGui.SetColumnWidth(1, 215);
        ImGui.PushItemWidth(200);
        ImGui.Text("Max Health");
        ImGui.InputInt("##Health", ref gent.MaxHealth);
        ImGui.NextColumn();
        ImGui.PushItemWidth(200);
        ImGui.Text("Initial Health");
        ImGui.InputInt("##InitHealth", ref gent.StartHealth);
        ImGui.Columns();
        ImGui.End();
    }

    private void DrawGCHR()
    {
        if (_currentFile.GCHR is not GCHRc gchr) return;
        ImGui.Begin("GCHR");
        ImGui.Text("Name");
        ImGui.InputText("##Name", ref gchr.name, 100u);
        ImGui.End();
    }

    private void DrawGCRE()
    {
        if (_currentFile.GCRE is not GCREc gcre) return;
        ImGui.Begin("GCRE");
        ImGui.Columns(2, "gcreColumns");
        ImGui.Text("Special");
        ImGui.Combo("##Special", ref _gcreSpecial, _gcreSpecials, _gcreSpecials.Length);
        ImGui.SameLine();
        ImGui.PushItemWidth(-1);
        ImGui.InputInt("##SpecialVal", ref gcre.Special[_gcreSpecial]);
        ImGui.PopItemWidth();
        ImGui.Text("Skills");
        ImGui.Combo("##Skills", ref _gcreSkill, _gcreSkills, _gcreSkills.Length);
        var skill = gcre.Skills.FirstOrDefault(s => s.Index == _gcreSkill);
        ImGui.PushItemWidth(-1);
        ImGui.SameLine();
        if (skill is not null)
        {
            ImGui.InputInt("##SkillVal", ref skill.Value);
        }
        else
        {
            var tmpVal = 0;
            if (ImGui.InputInt("##SkillVal", ref tmpVal) && tmpVal != 0) gcre.Skills.Add(new Skill(_gcreSkill, tmpVal));
        }

        ImGui.PopItemWidth();
        ImGui.Text("Traits");
        ImGui.Combo("##Traits", ref _gcreTrait, _gcreTraits, _gcreTraits.Length);
        var traitSel = gcre.Traits.Contains(_gcreTrait);
        ImGui.SameLine();
        if (ImGui.Checkbox("Trait Active", ref traitSel))
        {
            if (traitSel)
                gcre.Traits.Add(_gcreTrait);
            else
                gcre.Traits.Remove(_gcreTrait);
        }

        ImGui.Text("Tag Skills");
        ImGui.Combo("##TagSkills", ref _gcreTag, _gcreSkills, _gcreSkills.Length);
        var tagSel = gcre.TagSkills.Contains(_gcreTag);
        ImGui.SameLine();
        if (ImGui.Checkbox("Tag Active", ref tagSel))
        {
            if (tagSel)
                gcre.TagSkills.Add(_gcreTag);
            else
                gcre.TagSkills.Remove(_gcreTag);
        }

        ImGui.Text("Portrait");
        ImGui.InputText("##Portrait", ref gcre.PortStr, 100u);
        ImGui.Text("Age");
        ImGui.InputInt("##Age", ref gcre.Age);
        ImGui.NextColumn();
        ImGui.Text("Socket");
        ImGui.Combo("##Socket", ref _gcreSocket, _gcreSockets, _gcreSockets.Length);
        var sock = _gcreSocket switch
        {
            0 => gcre.Hea,
            1 => gcre.Hai,
            2 => gcre.Pon,
            3 => gcre.Mus,
            4 => gcre.Bea,
            5 => gcre.Eye,
            6 => gcre.Bod,
            7 => gcre.Han,
            8 => gcre.Fee,
            9 => gcre.Bac,
            10 => gcre.Sho,
            11 => gcre.Van,
            _ => throw new ArgumentOutOfRangeException()
        };
        ImGui.Text("Socket Model");
        ImGui.InputText("##Model", ref sock.Model, 100u);
        ImGui.Text("Socket Texture");
        ImGui.InputText("##Texture", ref sock.Tex, 100u);
        ImGui.Text("Equipped Items");
        ImGui.ListBox("##Equipped", ref _gcreEquipped, gcre.Inventory, gcre.Inventory.Length);
        ImGui.InputText("##EquippedItem", ref _gcreTemp, 100u);
        ImGui.SameLine();
        if (ImGui.Button(" + ##Equipped"))
        {
            if (string.IsNullOrWhiteSpace(_gcreTemp)) return;
            gcre.Inventory = gcre.Inventory.Append(_gcreTemp).ToArray();
            _gcreTemp = "";
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ##Equipped"))
        {
            if (_gcreEquipped == -1) return;
            var tmp = gcre.Inventory.ToList();
            tmp.RemoveAt(_gcreEquipped);
            gcre.Inventory = tmp.ToArray();
            _gcreEquipped = -1;
        }

        _gwamNames = gcre.GWAM.Select(i => i.NameSR == 0 ? "" : _stf[i.NameSR - 1]).ToArray();
        ImGui.End();
    }

    private void DrawGWAM()
    {
        if (_currentFile.GCRE is not GCREc gcre) return;
        ImGui.Begin("GWAM");
        // i believe every isntance of GWAM can have more than one.
        ImGui.Combo("##gwams", ref _gwamIndex, _gwamNames, _gwamNames.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            gcre.GWAM.Add(new GWAMc());
            _gwamIndex = gcre.GWAM.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && gcre.GWAM.Count > 0)
            gcre.GWAM.RemoveAt(_gwamIndex);
        if (_gwamIndex >= gcre.GWAM.Count)
            _gwamIndex = gcre.GWAM.Count - 1;
        if (gcre.GWAM.Count == 0)
        {
            ImGui.End();
            return;
        }

        var gwam = gcre.GWAM[_gwamIndex];
        // .wea also has GWAM but isn't implemented yet. get later.
        ImGui.Columns(3, "gwamColumns");
        ImGui.Text("Animation");
        ImGui.InputInt("##Animation", ref gwam.Anim);
        ImGui.Text("Range");
        ImGui.InputInt("##Range", ref gwam.Range);
        ImGui.Text("Action Points");
        ImGui.InputInt("##AP", ref gwam.AP);
        ImGui.NextColumn();
        ImGui.Text("Damage Type");
        ImGui.Combo("##DamageType", ref gwam.DmgType, _gwamDamageTypes, _gwamDamageTypes.Length);
        ImGui.Text("Min Dmg");
        ImGui.InputInt("##MinDmg", ref gwam.MinDmg);
        ImGui.NextColumn();
        ImGui.Text("Shots Fired");
        ImGui.InputInt("##ShotsFired", ref gwam.ShotsFired);
        ImGui.Text("Max Dmg");
        ImGui.InputInt("##MaxDmg", ref gwam.MaxDmg);
        ImGui.Columns();
        STFBox("Attack Name", ref gwam.NameSR);
        ImGui.Text("Effect");
        ImGui.InputText("##Effect", ref gwam.VegName, 100u);
        ImGui.End();
    }

    private void DrawGITM()
    {
        if (_currentFile.GITM is not GITMc gitm) return;
        ImGui.Begin("GITM");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Type");
        ImGui.Combo("##Type", ref gitm.type, _gitmTypes, _gitmTypes.Length);
        ImGui.Text("Slot");
        ImGui.Combo("##Slot", ref gitm.eqslot, _gitmSlots, _gitmSlots.Length);
        ImGui.Checkbox("Equippable", ref gitm.equip);
        ImGui.Text("Reload");
        ImGui.InputInt("##Reload", ref gitm.reload);
        // TODO: figure out how melee works
        var tempBool = false;
        ImGui.Checkbox("Melee", ref tempBool);

        ImGui.Text("Hide Socket");
        ImGui.Checkbox("Hair", ref gitm.hHai);
        ImGui.SameLine();
        ImGui.Checkbox("Beard", ref gitm.hBea);
        ImGui.SameLine();
        ImGui.Checkbox("Moustache", ref gitm.hMus);
        ImGui.SameLine();
        ImGui.Checkbox("Eye", ref gitm.hEye);
        ImGui.SameLine();
        ImGui.Checkbox("Ponytail", ref gitm.hPon);
        ImGui.SameLine();
        ImGui.Checkbox("Vanity", ref gitm.hVan);

        ImGui.Text("Socket");
        ImGui.Combo("##Socket", ref _gitmSocket, _gitmSockets, _gitmSockets.Length);
        var sock = _gitmSocket switch
        {
            0 => gitm.Bac,
            1 => gitm.Bod,
            2 => gitm.Eye,
            3 => gitm.Fee,
            4 => gitm.Han,
            5 => gitm.Hea,
            6 => gitm.IHS,
            7 => gitm.Sho,
            8 => gitm.Van,
            _ => throw new ArgumentOutOfRangeException()
        };
        ImGui.Text("Socket Model");
        ImGui.InputText("##Model", ref sock.Model, 100u);
        ImGui.Text("Socket Texture");
        ImGui.InputText("##Texture", ref sock.Tex, 100u);
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawEMAP()
    {
        if (_currentFile.EMAP is not EMAPc emap) return;
        _emapColor = ColToVec3(emap.col);
        ImGui.Begin("EMAP");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Map Mesh");
        ImGui.InputText("##MapMesh", ref emap.s1, 100u);
        ImGui.Text("Height Map");
        ImGui.InputText("##HeightMap", ref emap.s2, 100u);
        ImGui.Text("Minimap Texture");
        ImGui.InputText("##MinimapTexture", ref emap.s3, 100u);
        ImGui.Text("Lighting Colour");
        ImGui.ColorEdit3("Lighting Colour", ref _emapColor);
        emap.col = Color.FromArgb(255, (int)(_emapColor.X * 255), (int)(_emapColor.Y * 255), (int)(_emapColor.Z * 255));
        ImGui.Checkbox("Ignore Lighting", ref emap.il);
        ImGui.End();
        if (_currentFile is not Map map) return;

        if (_currentFile.EME2.Count > 0)
            _eme2Names = map.EME2.Select(e => e.name).ToArray();
        if (_currentFile.EMEP.Count > 0)
            _emepNames = map.EMEP.Select(e => e.index.ToString()).ToArray();
        if (_currentFile.EMEF.Count > 0)
            _emefNames = map.EMEF.Select(e => e.s1).ToArray();
        if (_currentFile.EMSD.Count > 0)
            _emsdNames = map.EMSD.Select(e => e.s1).ToArray();
        if (_currentFile.EPTH.Count > 0)
            _epthNames = map.EPTH.Select(e => e.name).ToArray();
        if (_currentFile.Triggers.Count <= 0) return;
        _triggerNames = new string[_currentFile.Triggers.Count];
        for (var i = 0; i < _currentFile.Triggers.Count; i++)
            _triggerNames[i] = i.ToString();
    }

    private void DrawEME2()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EME2");
        ImGui.Combo("##EME2", ref _eme2Index, _eme2Names, _eme2Names.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.EME2.Add(new EME2c());
            _eme2Index = map.EME2.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.EME2.Count > 0)
            map.EME2.RemoveAt(_eme2Index);
        if (_eme2Index >= map.EME2.Count)
            _eme2Index = map.EME2.Count - 1;
        if (map.EME2.Count == 0)
        {
            ImGui.End();
            return;
        }

        var eme2 = map.EME2[_eme2Index];
        ImGui.Text("Creature File");
        ImGui.InputText("##CreatureFile", ref eme2.name, 100u);
        DrawEEOV(eme2.EEOV);
        ImGui.InputFloat4("XYZR", ref eme2.l);
        ImGui.End();
    }

    private void DrawECAM()
    {
        if (_currentFile.ECAM is not ECAMc ecam) return;
        ImGui.Begin("ECAM");
        ImGui.Text("Camera Position");
        ImGui.InputFloat4("##XYZR", ref ecam.p);
        ImGui.End();
    }

    private void DrawEMEP()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EMEP");
        ImGui.Combo("##EMEP", ref _emepIndex, _emepNames, _emepNames.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.EMEP.Add(new EMEPc());
            _emepIndex = map.EMEP.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.EMEP.Count > 0)
            map.EMEP.RemoveAt(_emepIndex);
        if (_emepIndex >= map.EMEP.Count)
            _emepIndex = map.EMEP.Count - 1;
        if (map.EMEP.Count == 0)
        {
            ImGui.End();
            return;
        }

        var emep = map.EMEP[_emepIndex];
        ImGui.Text("Entry Point ID");
        ImGui.InputInt("##EntryPointID", ref emep.index);
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZ", ref emep.p);
        ImGui.SameLine();
        ImGui.InputFloat("##R", ref emep.r);
        ImGui.End();
    }

    private void DrawEMEF()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EMEF");
        ImGui.Combo("##EMEF", ref _emefIndex, _emefNames, _emefNames.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.EMEF.Add(new EMEFc());
            _emefIndex = map.EMEF.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.EMEF.Count > 0)
            map.EMEF.RemoveAt(_emefIndex);
        if (_emefIndex >= map.EMEF.Count)
            _emefIndex = map.EMEF.Count - 1;
        if (map.EMEF.Count == 0)
        {
            ImGui.End();
            return;
        }

        var emef = map.EMEF[_emefIndex];
        ImGui.Text("Effect ID");
        ImGui.InputText("##EffectID", ref emef.s1, 100u);
        ImGui.Text("Effect File");
        ImGui.InputText("##EffectFile", ref emef.s2, 100u);
        ImGui.Text("Position");
        ImGui.InputFloat4("##XYZR", ref emef.l);
        ImGui.End();
    }

    private void DrawEMSD()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EMSD");
        ImGui.Combo("##EMSD", ref _emsdIndex, _emsdNames, _emsdNames.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.EMSD.Add(new EMSDc());
            _emsdIndex = map.EMSD.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.EMSD.Count > 0)
            map.EMSD.RemoveAt(_emsdIndex);
        if (_emsdIndex >= map.EMSD.Count)
            _emsdIndex = map.EMSD.Count - 1;
        if (map.EMSD.Count == 0)
        {
            ImGui.End();
            return;
        }

        var emsd = map.EMSD[_emsdIndex];
        ImGui.Text("Sound ID");
        ImGui.InputText("##SoundID", ref emsd.s1, 100u);
        ImGui.Text("Sound File");
        ImGui.InputText("##SoundFile", ref emsd.s2, 100u);
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZ", ref emsd.l);
        ImGui.End();
    }

    private void DrawEMNO()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EMNO");
        // get all the names of the EMNOs (use string references)
        _emnoNames = map.EMNO.Count == 0 ? [] : map.EMNO.Select(e => e.sr == 0 ? map.EMNO.IndexOf(e).ToString() : _stf[e.sr - 1]).ToArray();
        ImGui.Combo("##EMNO", ref _emnoIndex, _emnoNames, _emnoNames.Length);
        ImGui.PushItemWidth(-1);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.EMNO.Add(new EMNOc());
            _emnoIndex = map.EMNO.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.EMNO.Count > 0)
            map.EMNO.RemoveAt(_emnoIndex);
        if (_emnoIndex >= map.EMNO.Count)
            _emnoIndex = map.EMNO.Count - 1;
        if (_emnoIndex == -1)
            _emnoIndex = 0;
        if (map.EMNO.Count == 0)
        {
            ImGui.End();
            return;
        }

        var emno = map.EMNO[_emnoIndex];
        ImGui.Text("Position");
        ImGui.InputFloat2("##XY", ref emno.l);
        ImGui.Text("Icon Texture");
        ImGui.InputText("##IconTexture", ref emno.tex, 100u);
        ImGui.PopItemWidth();
        STFBox("Name", ref emno.sr);
        ImGui.End();
    }

    private void DrawEPTH()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EPTH");
        ImGui.Combo("##EPTH", ref _epthIndex, _epthNames, _epthNames.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.EPTH.Add(new EPTHc());
            _epthIndex = map.EPTH.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.EPTH.Count > 0)
        {
            map.EPTH.RemoveAt(_epthIndex);
            _epthPoint = 0;
        }

        if (_epthIndex >= map.EPTH.Count)
            _epthIndex = map.EPTH.Count - 1;
        if (map.EPTH.Count == 0)
        {
            ImGui.End();
            return;
        }

        var epth = map.EPTH[_epthIndex];
        ImGui.Text("Path ID");
        ImGui.InputText("##PathID", ref epth.name, 100u);
        ImGui.Text("Point");
        // create string array of point indices
        var points = new string[epth.p.Count];
        for (var i = 0; i < epth.p.Count; i++)
            points[i] = i.ToString();
        ImGui.Combo("##Point", ref _epthPoint, points, points.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + ##Point"))
        {
            epth.p.Add(new Vector4());
            _epthPoint = epth.p.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ##Point") && epth.p.Count > 0)
            epth.p.RemoveAt(_epthPoint);
        if (_epthPoint >= epth.p.Count)
            _epthPoint = epth.p.Count - 1;
        if (epth.p.Count == 0)
        {
            ImGui.End();
            return;
        }

        var point = epth.p[_epthPoint];
        ImGui.Text("Position");
        ImGui.InputFloat4("##XYZR", ref point);
        epth.p[_epthPoint] = point; // can't pass by ref so we must reassign
        ImGui.End();
    }

    private void DrawTrigger()
    {
        if (_currentFile is not Map map) return;
        ImGui.Begin("EMTR");
        ImGui.Combo("##Trigger", ref _triggerIndex, _triggerNames, _triggerNames.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            map.Triggers.Add(new Trigger());
            _triggerIndex = map.Triggers.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && map.Triggers.Count > 0)
        {
            map.Triggers.RemoveAt(_triggerIndex);
            _triggerPoint = 0;
        }

        if (_triggerIndex >= map.Triggers.Count)
            _triggerIndex = map.Triggers.Count - 1;
        if (map.Triggers.Count == 0)
        {
            ImGui.End();
            return;
        }

        var trigger = map.Triggers[_triggerIndex];
        ImGui.Text("Trigger Type");
        var triggerIndex = trigger.ExTR.type switch
        {
            "B" => 0,
            "S" => 1,
            "T" => 2,
            _ => -1
        };
        if (ImGui.Combo("##TriggerType", ref triggerIndex, _triggerTypes, _triggerTypes.Length))
            trigger.ExTR.type = triggerIndex switch
            {
                0 => "B",
                1 => "S",
                2 => "T",
                _ => throw new ArgumentOutOfRangeException()
            };
        ImGui.Text("Trigger Value");
        ImGui.InputText("##TriggerValue", ref trigger.ExTR.s, 100u);
        // TODO: what is EMTR.n?
        // create string array of point indices
        var points = new string[trigger.EMTR.r.Count];
        for (var i = 0; i < trigger.EMTR.r.Count; i++)
            points[i] = i.ToString();
        ImGui.Text("Point");
        ImGui.Combo("##Point", ref _triggerPoint, points, points.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + ##Point"))
        {
            trigger.EMTR.r.Add(new Vector3());
            _triggerPoint = trigger.EMTR.r.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ##Point") && trigger.EMTR.r.Count > 0)
            trigger.EMTR.r.RemoveAt(_triggerPoint);
        if (_triggerPoint >= trigger.EMTR.r.Count)
            _triggerPoint = trigger.EMTR.r.Count - 1;
        if (trigger.EMTR.r.Count == 0)
        {
            ImGui.End();
            return;
        }

        var point = trigger.EMTR.r[_triggerPoint];
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZR", ref point);
        trigger.EMTR.r[_triggerPoint] = point; // can't pass by ref so we must reassign
        ImGui.End();
    }

    private void Draw2MWT()
    {
        if (_currentFile._2MWT is not _2MWTc)
        {
            _currentFile._2MWT = new _2MWTc();
            return;
        }

        _2MWTc _2mwt = _currentFile._2MWT;
        ImGui.Begin("2MWT");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Water Mesh"); // i think?
        ImGui.InputText("##WaterMesh", ref _2mwt.mpf, 100u);
        ImGui.Checkbox("Static Water", ref _2mwt.frozen);
        ImGui.SameLine();
        ImGui.Checkbox("Dark Water", ref _2mwt.dark);
        // create string array of location indices
        var chunks = new string[_2mwt.chunks.Count];
        for (var i = 0; i < _2mwt.chunks.Count; i++)
            chunks[i] = i.ToString();
        ImGui.Text("Water Chunk");
        ImGui.PushItemWidth(250);
        ImGui.Combo("##Chunk", ref _2mwtChunk, chunks, chunks.Length);
        ImGui.PopItemWidth();
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            _2mwt.chunks.Add(new _2MWTChunk());
            _2mwtChunk = _2mwt.chunks.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && _2mwt.chunks.Count > 0)
            _2mwt.chunks.RemoveAt(_2mwtChunk);
        if (_2mwtChunk >= _2mwt.chunks.Count)
            _2mwtChunk = _2mwt.chunks.Count - 1;
        if (_2mwt.chunks.Count == 0)
        {
            ImGui.End();
            return;
        }

        var chunk = _2mwt.chunks[_2mwtChunk];
        ImGui.Text("Water Lightmap");
        ImGui.InputText("##WaterTexture", ref chunk.tex, 100u);
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZ", ref chunk.loc);
        ImGui.Text("Lightmap UV");
        ImGui.InputFloat2("##UV", ref chunk.texloc);
        ImGui.End();
    }

    private void DrawEMNP()
    {
        if (_currentFile.EMNP is not EMNPc)
        {
            _currentFile.EMNP = new EMNPc();
            return;
        }

        EMNPc emnp = _currentFile.EMNP;
        ImGui.Begin("EMNP");
        // make list of indices
        var chunks = new string[emnp.chunks.Count];
        for (var i = 0; i < emnp.chunks.Count; i++)
            chunks[i] = i.ToString();
        ImGui.Text("EMNP Chunk");
        ImGui.Combo("##Chunk", ref _emnpChunk, chunks, chunks.Length);
        ImGui.SameLine();
        if (ImGui.Button(" + "))
        {
            emnp.chunks.Add(new EMNPChunk());
            _emnpChunk = emnp.chunks.Count - 1;
        }

        ImGui.SameLine();
        if (ImGui.Button(" - ") && emnp.chunks.Count > 0)
            emnp.chunks.RemoveAt(_emnpChunk);
        if (_emnpChunk >= emnp.chunks.Count)
            _emnpChunk = emnp.chunks.Count - 1;
        if (_emnpChunk == -1)
            _emnpChunk = 0;
        if (emnp.chunks.Count == 0)
        {
            ImGui.End();
            return;
        }

        ImGui.PushItemWidth(-1);
        var chunk = emnp.chunks[_emnpChunk];
        ImGui.Text("Value 1");
        var temp1 = (int)chunk.@bool;
        ImGui.InputInt("##Value1", ref temp1);
        ImGui.Text("Position");
        ImGui.InputFloat3("##XYZ", ref chunk.l);
        ImGui.Text("Value 2");
        var temp2 = (int)chunk.b1;
        ImGui.InputInt("##Value2", ref temp2);
        ImGui.Text("Value 3");
        var temp3 = (int)chunk.b2;
        ImGui.InputInt("##Value3", ref temp3);
        ImGui.Text("Value 4");
        var temp4 = (int)chunk.b3;
        ImGui.InputInt("##Value4", ref temp4);
        ImGui.Text("Value 5");
        var temp5 = (int)chunk.b4;
        ImGui.InputInt("##Value5", ref temp5);
        ImGui.Text("Value 6");
        var temp6 = (int)chunk.b5;
        ImGui.InputInt("##Value6", ref temp6);
        chunk.@bool = (byte)temp1;
        chunk.b1 = (byte)temp2;
        chunk.b2 = (byte)temp3;
        chunk.b3 = (byte)temp4;
        chunk.b4 = (byte)temp5;
        chunk.b5 = (byte)temp6;
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawGOBJ()
    {
        if (_currentFile.GOBJ is not GOBJc gobj) return;
        ImGui.Begin("GOBJ");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Type");
        if (ImGui.Combo("##Type", ref gobj.Type, _gobjTypes, _gobjTypes.Length))
            _extension = _gobjTypes[gobj.Type]; // update the extension (used when saving)
        ImGui.End();
    }

    private void DrawGIAR()
    {
        if (_currentFile.GIAR is not GIARc giar) return;
        ImGui.Begin("GIAR");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Ballistic Resistance");
        ImGui.InputInt("##BallisticResistance", ref giar.BallR);
        ImGui.Text("Bio Resistance");
        ImGui.InputInt("##BioResistance", ref giar.BioR);
        ImGui.Text("Electric Resistance");
        ImGui.InputInt("##ElectricResistance", ref giar.ElecR);
        ImGui.Text("EMP Resistance");
        ImGui.InputInt("##EMPResistance", ref giar.EMPR);
        ImGui.Text("Normal Resistance");
        ImGui.InputInt("##NormalResistance", ref giar.NormR);
        ImGui.Text("Heat Resistance");
        ImGui.InputInt("##HeatResistance", ref giar.HeatR);
        ImGui.End();
    }

    private void DrawEMFG()
    {
        if (_currentFile.EMFG is not EMFGc)
        {
            _currentFile.EMFG = new EMFGc();
            return;
        }

        EMFGc emfg = _currentFile.EMFG;
        _emfgColor = ColToVec3(emfg.colour);
        ImGui.Begin("EMFG");
        ImGui.Checkbox("Enabled", ref emfg.enabled);
        ImGui.PushItemWidth(-1);
        ImGui.Text("Colour");
        ImGui.ColorEdit3("##Colour", ref _emfgColor);
        emfg.colour = Color.FromArgb(255, (int)(_emfgColor.X * 255), (int)(_emfgColor.Y * 255),
            (int)(_emfgColor.Z * 255));
        ImGui.Text("Base Height");
        ImGui.InputFloat("##BaseHeight", ref emfg.base_height);
        ImGui.Text("Anim1 Speed");
        ImGui.InputFloat("##Anim1Speed", ref emfg.anim1Speed);
        ImGui.Text("Anim1 Height");
        ImGui.InputFloat("##Anim1Height", ref emfg.anim1Height);
        ImGui.Text("Total Height");
        ImGui.InputFloat("##TotalHeight", ref emfg.total_height);
        ImGui.Text("Anim2 Speed");
        ImGui.InputFloat("##Anim2Speed", ref emfg.anim2Speed);
        ImGui.Text("Anim2 Height");
        ImGui.InputFloat("##Anim2Height", ref emfg.anim2Height);
        ImGui.Text("Vertical Offset");
        ImGui.InputFloat("##VerticalOffset", ref emfg.verticalOffset);
        ImGui.Text("Max Fog Density");
        ImGui.InputFloat("##MaxFogDensity", ref emfg.max_fog_density);
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawGIAM()
    {
        if (_currentFile.GIAM is not GIAMc giam) return;
        ImGui.Begin("GIAM");
        ImGui.PushItemWidth(-1);
        ImGui.Text("Caliber");
        ImGui.Combo("##Caliber", ref giam.Caliber, _ammoTypes, _ammoTypes.Length);
        ImGui.Text("Ammo Extra (+238)");
        ImGui.InputInt("##AmmoExtra238", ref giam.AmmoExtra238);
        ImGui.Text("Damage Bonus");
        ImGui.InputInt("##DamageBonus", ref giam.DamageBonus);
        ImGui.Text("DR Penalty (armor pen)");
        ImGui.InputInt("##DRPenalty", ref giam.DRPenalty);
        ImGui.Text("Skill Bonus (to-hit)");
        ImGui.InputInt("##SkillBonus", ref giam.SkillBonus);
        if (giam.Version >= 1)
        {
            ImGui.Text("Damage Type");
            ImGui.InputInt("##DamageType", ref giam.DamageType);
            ImGui.Text("Ammo Category");
            ImGui.InputInt("##AmmoCategory", ref giam.AmmoCategory);
        }
        if (giam.Version >= 2)
        {
            ImGui.Text("Ammo Modifier (variant id)");
            ImGui.InputInt("##AmmoModifier", ref giam.AmmoModifier);
            ImGui.Text("Range Bonus");
            ImGui.InputInt("##RangeBonus", ref giam.RangeBonus);
        }
        ImGui.PopItemWidth();
        ImGui.End();
    }

    private void DrawGIWP()
    {
        if (_currentFile is not WEA wea || wea.GIWP is not GIWPc giwp) return;
        ImGui.Begin("GIWP");
        ImGui.PushItemWidth(-1);

        ImGui.Text("Weapon Class (1-5 small arms, 6-8 melee, 9-10 big guns)");
        ImGui.InputInt("##WeaponClass", ref giwp.WeaponClass);

        ImGui.Separator();
        ImGui.Text("Primary Ammo");
        ImGui.Text("Caliber");
        ImGui.InputInt("##PrimaryCaliber", ref giwp.PrimaryCaliber);
        ImGui.Text("Reload Duration (ticks)");
        ImGui.InputInt("##PrimaryReloadDuration", ref giwp.PrimaryReloadDuration);
        ImGui.Text("Loaded / Capacity");
        ImGui.InputInt("##PrimaryAmmoLoaded", ref giwp.PrimaryAmmoLoaded);
        ImGui.InputInt("##PrimaryAmmoCapacity", ref giwp.PrimaryAmmoCapacity);
        if (giwp.Version != 0)
        {
            ImGui.Text("Modifier (set at runtime by reload)");
            ImGui.InputInt("##PrimaryModifier", ref giwp.PrimaryModifier);
        }

        ImGui.Separator();
        ImGui.Text("Secondary Ammo");
        ImGui.Text("Caliber");
        ImGui.InputInt("##SecondaryCaliber", ref giwp.SecondaryCaliber);
        ImGui.Text("Reload Duration (ticks)");
        ImGui.InputInt("##SecondaryReloadDuration", ref giwp.SecondaryReloadDuration);
        ImGui.Text("Loaded / Capacity");
        ImGui.InputInt("##SecondaryAmmoLoaded", ref giwp.SecondaryAmmoLoaded);
        ImGui.InputInt("##SecondaryAmmoCapacity", ref giwp.SecondaryAmmoCapacity);
        if (giwp.Version != 0)
        {
            ImGui.Text("Modifier (set at runtime by reload)");
            ImGui.InputInt("##SecondaryModifier", ref giwp.SecondaryModifier);
        }

        ImGui.Separator();
        ImGui.Text("Resource name (always empty in shipped data)");
        ImGui.InputText("##ResourceName", ref giwp.ResourceName, 64u);

        ImGui.Separator();
        ImGui.Text("Engine-reserved (no observed gameplay use)");
        ImGui.InputInt("##Unknown250", ref giwp.Unknown250);
        ImGui.InputInt("##Unknown255", ref giwp.Unknown255);
        ImGui.InputInt("##Unknown256", ref giwp.Unknown256);

        ImGui.Separator();
        ImGui.Text($"Attack Modes ({giwp.AttackModes.Count})");
        for (var i = 0; i < giwp.AttackModes.Count; i++)
        {
            var am = giwp.AttackModes[i];
            if (!ImGui.CollapsingHeader($"Attack Mode {i} (Anim={am.Anim}, AP={am.AP})##am{i}"))
                continue;
            ImGui.Indent();
            ImGui.InputInt($"Anim##am{i}", ref am.Anim);
            ImGui.InputInt($"DmgType (0=ballistic 2=elec 4=normal 5=energy)##am{i}", ref am.DmgType);
            ImGui.InputInt($"ShotsFired##am{i}", ref am.ShotsFired);
            ImGui.InputInt($"Range##am{i}", ref am.Range);
            ImGui.InputInt($"MinDmg##am{i}", ref am.MinDmg);
            ImGui.InputInt($"MaxDmg##am{i}", ref am.MaxDmg);
            if (am.Version != 0)
            {
                ImGui.InputInt($"AP##am{i}", ref am.AP);
                var consumed = am.Consumed != 0;
                if (ImGui.Checkbox($"Consumed##am{i}", ref consumed)) am.Consumed = (byte)(consumed ? 1 : 0);
                var thrown = am.Thrown != 0;
                if (ImGui.Checkbox($"Thrown##am{i}", ref thrown)) am.Thrown = (byte)(thrown ? 1 : 0);
            }
            if (am.Version >= 2) ImGui.InputInt($"PoseOverride##am{i}", ref am.PoseOverride);
            if (am.Version >= 3) ImGui.InputInt($"NameSR (string id)##am{i}", ref am.NameSR);
            if (am.Version >= 4) ImGui.InputText($"VegName (.veg fx)##am{i}", ref am.VegName, 128u);
            ImGui.Unindent();
        }

        ImGui.PopItemWidth();
        ImGui.End();
    }
}