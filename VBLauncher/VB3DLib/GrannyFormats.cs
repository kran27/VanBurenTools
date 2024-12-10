using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VB3DLib
{
    public class GrannyFormats
    {
        // Van Buren uses v2.1.0.5, which doesn't exist in x64, this is v2.8.0.49.
        [DllImport("granny2_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GrannyGetFileInfo")]
        private static extern IntPtr GrannyGetFileInfo(IntPtr file);

        [DllImport("granny2_x64.dll", CallingConvention = CallingConvention.StdCall,
            EntryPoint = "GrannyReadEntireFileFromMemory")]
        private static extern IntPtr GrannyReadEntireFileFromMemory(int memorySize, IntPtr memory);

        [DllImport("granny2_x64.dll", CallingConvention = CallingConvention.StdCall,
            EntryPoint = "GrannyCurveGetDimension")]
        private static extern int GrannyCurveGetDimension(IntPtr Curve);

        private static T ReadFromPointerArray<T>(IntPtr ptrArray, int index) where T : struct
        {
            var ptr = Marshal.ReadIntPtr(ptrArray, index * IntPtr.Size);
            return (T)(Marshal.PtrToStructure(ptr, typeof(T)) ?? throw new ExternalException());
        }

        private static T ReadFromArray<T>(IntPtr array, int index) where T : struct
        {
            var elementPtr = new IntPtr(array.ToInt64() + Marshal.SizeOf(typeof(T)) * index);
            return (T)(Marshal.PtrToStructure(elementPtr, typeof(T)) ?? throw new ExternalException());
        }

        private static Matrix4x4 ConvertToMatrix4x4(float[] array)
        {
            return new Matrix4x4(
                array[0], array[1], array[2], array[3],
                array[4], array[5], array[6], array[7],
                array[8], array[9], array[10], array[11],
                array[12], array[13], array[14], array[15]
            );
        }

        private static transform CreateTransform(unm_transform t)
        {
            var otpt = new transform
            {
                Position = new Vector3(t.Position[0], t.Position[1], t.Position[2]),
                Orientation = new Quaternion(t.Orientation[0], t.Orientation[1], t.Orientation[2], t.Orientation[3]),
                ScaleShear = new float[3][]
            };
            otpt.ScaleShear[0] = new[] { t.ScaleShear[0], t.ScaleShear[1], t.ScaleShear[2] };
            otpt.ScaleShear[1] = new[] { t.ScaleShear[3], t.ScaleShear[4], t.ScaleShear[5] };
            otpt.ScaleShear[2] = new[] { t.ScaleShear[6], t.ScaleShear[7], t.ScaleShear[8] };
            return otpt;
        }

        private static void ParseArtToolInfo(ref file_info parsedFile, unm_file_info fileInfo)
        {
            var artToolInfo =
                (unm_art_tool_info)(Marshal.PtrToStructure(fileInfo.ArtToolInfo, typeof(unm_art_tool_info)) ?? throw new ExternalException());
            parsedFile.ArtToolInfo.FromArtToolName = Marshal.PtrToStringAnsi(artToolInfo.FromArtToolName) ?? throw new ExternalException();
            parsedFile.ArtToolInfo.ArtToolMajorRevision = artToolInfo.ArtToolMajorRevision;
            parsedFile.ArtToolInfo.ArtToolMinorRevision = artToolInfo.ArtToolMinorRevision;
            parsedFile.ArtToolInfo.UnitsPerMeter = artToolInfo.UnitsPerMeter;
            parsedFile.ArtToolInfo.Origin =
                new Vector3(artToolInfo.Origin[0], artToolInfo.Origin[1], artToolInfo.Origin[2]);
            parsedFile.ArtToolInfo.RightVector = new Vector3(artToolInfo.RightVector[0], artToolInfo.RightVector[1],
                artToolInfo.RightVector[2]);
            parsedFile.ArtToolInfo.UpVector = new Vector3(artToolInfo.UpVector[0], artToolInfo.UpVector[1],
                artToolInfo.UpVector[2]);
            parsedFile.ArtToolInfo.BackVector = new Vector3(artToolInfo.BackVector[0], artToolInfo.BackVector[1],
                artToolInfo.BackVector[2]);
        }

        private static void ParseExporterInfo(ref file_info parsedFile, unm_file_info fileInfo)
        {
            var exporterInfo =
                (unm_exporter_info)(Marshal.PtrToStructure(fileInfo.ExporterInfo, typeof(unm_exporter_info)) ?? throw new ExternalException());
            parsedFile.ExporterInfo.ExporterName = Marshal.PtrToStringAnsi(exporterInfo.ExporterName) ?? throw new ExternalException();
            parsedFile.ExporterInfo.ExporterMajorRevision = exporterInfo.ExporterMajorRevision;
            parsedFile.ExporterInfo.ExporterMinorRevision = exporterInfo.ExporterMinorRevision;
            parsedFile.ExporterInfo.ExporterCustomization = exporterInfo.ExporterCustomization;
            parsedFile.ExporterInfo.ExporterBuildNumber = exporterInfo.ExporterBuildNumber;
        }

        private static void ParseModels(ref file_info parsedFile, unm_file_info fileInfo)
        {
            parsedFile.Models = new List<model>();
            for (var i = 0; i < fileInfo.ModelCount; i++) 
            {
                var model = ReadFromPointerArray<unm_model>(fileInfo.Models, i);
                var parsedModel = new model
                {
                    Name = Marshal.PtrToStringAnsi(model.Name) ?? throw new ExternalException()
                };
                var skeleton = (unm_skeleton)(Marshal.PtrToStructure(model.Skeleton, typeof(unm_skeleton)) ??
                                              throw new ExternalException());
                parsedModel.Skeleton.Name = Marshal.PtrToStringAnsi(skeleton.Name) ?? throw new ExternalException();
                parsedModel.Skeleton.Bones = new List<bone>();
                for (var j = 0; j < skeleton.BoneCount; j++)
                {
                    var bone = ReadFromArray<unm_bone>(skeleton.Bones, j);
                    var parsedBone = new bone
                    {
                        Name = Marshal.PtrToStringAnsi(bone.Name) ?? throw new ExternalException(),
                        ParentIndex = bone.ParentIndex,
                        LocalTransform = CreateTransform(bone.LocalTransform),
                        InverseWorld4x4 = ConvertToMatrix4x4(bone.InverseWorld4x4)
                    };
                    Matrix4x4.Invert(parsedBone.InverseWorld4x4, out var World4x4);
                    parsedBone.ActualPosition =
                        Vector3.Transform(Vector3.Zero,
                            World4x4); // transform data is parent-relative, so i use world transform for easy previewing
                    parsedModel.Skeleton.Bones.Add(parsedBone);
                }

                parsedModel.InitialPlacement = CreateTransform(model.InitialPlacement);
                parsedFile.Models.Add(parsedModel);
            }
        }

        private static List<float[]> ParseCurve(unm_curve2 curve)
        {
            var unmCurve = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(unm_curve2)));
            Marshal.StructureToPtr(curve, unmCurve, false);
            var curvePtr = curve.CurveData.Object;
            var curveData = (unm_curve_data_da_k32f_c32f)(Marshal.PtrToStructure(curvePtr,
                typeof(unm_curve_data_da_k32f_c32f)) ?? throw new ExternalException());
            var dimension = GrannyCurveGetDimension(unmCurve);
            Marshal.FreeHGlobal(unmCurve);
            Console.WriteLine($"Parsing keyframed curve with dimension {dimension}");
            var keyframes = new List<float[]>();
            // since data is keyframed, no spline interpolation is needed, knots are just the time of each keyframe
            // control count is knot count * dimension
            for (var i = 0; i < curveData.KnotCount; i++)
            {
                var knot = ReadFromArray<float>(curveData.Knots, i);
                var control = new float[dimension+1];
                control[0] = knot;
                for (var j = 1; j <= dimension; j++)
                {
                    control[j] = ReadFromArray<float>(curveData.Controls, i * dimension + j);
                }
                //Console.WriteLine($"Keyframe at {knot}: {string.Join(", ", control)}");
                keyframes.Add(control);
            }

            Console.WriteLine();

            return keyframes;
        }
        
        private static void ComputeBoneWorldTransforms(model parsedModel)
        {
            var bones = parsedModel.Skeleton.Bones;

            // Compute world transforms for all root bones (ParentIndex == -1)
            for (var i = 0; i < bones.Count; i++)
            {
                if (bones[i].ParentIndex == -1)
                {
                    ComputeWorldTransformRecursive(bones, i, Matrix4x4.Identity);
                }
            }
        }

        private static void ComputeWorldTransformRecursive(List<bone> bones, int boneIndex, Matrix4x4 parentWorld)
        {
            var bone = bones[boneIndex];

            // Convert the LocalTransform to a Matrix4x4
            var localMatrix = TransformToMatrix(bone.LocalTransform);
            var worldMatrix = localMatrix * parentWorld;

            // Store the actual position of the bone by transforming (0,0,0)
            var position = Vector3.Transform(Vector3.Zero, worldMatrix);
            bone.ActualPosition = position;

            // If needed, you can also store worldMatrix in the bone if you want easy future reference:
            // bone.WorldTransform = worldMatrix; // (Add this field if you like)

            // Update the bone in the list
            bones[boneIndex] = bone;

            // Recurse for children
            for (var i = 0; i < bones.Count; i++)
            {
                if (bones[i].ParentIndex == boneIndex)
                {
                    ComputeWorldTransformRecursive(bones, i, worldMatrix);
                }
            }
        }

        private static Matrix4x4 TransformToMatrix(transform t)
        {
            // Convert the ScaleShear[3x3], Orientation (Quaternion), and Position into a single world matrix
            var scaleShearMat = new Matrix4x4(
                t.ScaleShear[0][0], t.ScaleShear[0][1], t.ScaleShear[0][2], 0,
                t.ScaleShear[1][0], t.ScaleShear[1][1], t.ScaleShear[1][2], 0,
                t.ScaleShear[2][0], t.ScaleShear[2][1], t.ScaleShear[2][2], 0,
                0,                 0,                 0,                 1
            );

            var rotationMat = Matrix4x4.CreateFromQuaternion(t.Orientation);

            // The order can vary depending on how Granny data is defined, but typically:
            // World = ScaleShear * Rotation, then set Position
            var worldMat = rotationMat;
            worldMat = scaleShearMat * worldMat; // apply scale/shear

            worldMat.Translation = t.Position; // apply translation

            return worldMat;
        }
        
        private static void AdjustTransformTrackCurves(transform_track tt)
        {
            // Orientation adjustments:
            // C++ snippet does:
            // quat2.x = -ctrl[0], quat2.y = ctrl[2], quat2.z = ctrl[1], quat2.w = -ctrl[3]
            // and then inverse the quaternion.
            // In C# indexing: OrientationCurve[i] = [time, x, y, z, w]
            // original ctrl[] mapping: ctrl[1]=x, ctrl[2]=y, ctrl[3]=z, ctrl[4]=w
            // after reorder:
            // x = -originalX
            // y = originalZ
            // z = originalY
            // w = -originalW
            // then q = inverse(q)
            foreach (var t in tt.OrientationCurve)
            {
                var o = t;
                var ox = o[1];
                var oy = o[2];
                var oz = o[3];
                var ow = o[4];

                var q = new Quaternion(-ox, oz, oy, -ow);
                q = Quaternion.Inverse(q);

                t[1] = q.X;
                t[2] = q.Y;
                t[3] = q.Z;
                t[4] = q.W;
            }

            // Position adjustments:
            // C++ snippet does:
            // x = -ctrl[0]*3.0f, y = ctrl[2]*3.0f, z = ctrl[1]*3.0f
            // In our indexing: PositionCurve[i] = [time, x, y, z]
            // original: ctrl[1]=x, ctrl[2]=y, ctrl[3]=z
            // newx = -x * 3.0f
            // newy = z * 3.0f
            // newz = y * 3.0f
            foreach (var t in tt.PositionCurve)
            {
                var p = t;
                var px = p[1];
                var py = p[2];
                var pz = p[3];

                var newx = -px * 3.0f;
                var newy = pz * 3.0f;
                var newz = py * 3.0f;

                t[1] = newx;
                t[2] = newy;
                t[3] = newz;
            }

            // Scale adjustments:
            // The C++ code doesn't reorder or negate scale. It simply uses the values as-is.
            // dimension=3: ScaleShearCurve[i] = [time, sx, sy, sz]
            // No reordering needed, so we leave it as-is.
        }


        private static void ParseAnimations(ref file_info parsedFile, unm_file_info fileInfo)
        {
            parsedFile.Animations = new List<animation>();
            for (var i = 0; i < fileInfo.AnimationCount; i++)
            {
                var animation = ReadFromPointerArray<unm_animation>(fileInfo.Animations, i);
                var parsedAnimation = new animation
                {
                    Name = Marshal.PtrToStringAnsi(animation.Name) ?? throw new ExternalException(),
                    Duration = animation.Duration,
                    TimeStep = animation.TimeStep,
                    Oversampling = animation.Oversampling,
                    TrackGroups = new List<track_group>()
                };
                for (var j = 0; j < animation.TrackGroupCount; j++)
                {
                    var trackGroup = ReadFromPointerArray<unm_track_group>(animation.TrackGroups, j);
                    var parsedTrackGroup = new track_group
                    {
                        Name = Marshal.PtrToStringAnsi(trackGroup.Name) ?? throw new ExternalException(),
                        TransformTracks = new List<transform_track>(),
                        InitialPlacement = CreateTransform(trackGroup.InitialPlacement)
                    };

                    for (var k = 0; k < trackGroup.TransformTrackCount; k++)
                    {
                        var transformTrack = ReadFromArray<unm_transform_track>(trackGroup.TransformTracks, k);
                        var parsedTransformTrack = new transform_track
                        {
                            Name = Marshal.PtrToStringAnsi(transformTrack.Name) ?? throw new ExternalException(),
                            Flags = transformTrack.Flags,
                            OrientationCurve = ParseCurve(transformTrack.OrientationCurve).ToArray(),
                            PositionCurve = ParseCurve(transformTrack.PositionCurve).ToArray(),
                            ScaleShearCurve = ParseCurve(transformTrack.ScaleShearCurve).ToArray() // Parse scale/shear
                        };

                        // Apply the same logic as the C++ snippet to reorder orientation and position data
                        AdjustTransformTrackCurves(parsedTransformTrack);

                        parsedTrackGroup.TransformTracks.Add(parsedTransformTrack);
                    }

                    parsedAnimation.TrackGroups.Add(parsedTrackGroup);
                }

                parsedFile.Animations.Add(parsedAnimation);
            }
        }


        public static file_info ReadFileFromMemory(byte[] rawData)
        {
            var unmanagedPointer = Marshal.AllocHGlobal(rawData.Length);
            Marshal.Copy(rawData, 0, unmanagedPointer, rawData.Length);
            var file = GrannyReadEntireFileFromMemory(rawData.Length, unmanagedPointer);
            Marshal.FreeHGlobal(unmanagedPointer);
            var fileInfo = (unm_file_info)(Marshal.PtrToStructure(GrannyGetFileInfo(file), typeof(unm_file_info)) ?? throw new ExternalException());

            var parsedFile = new file_info();

            if (fileInfo.ArtToolInfo != IntPtr.Zero)
            {
                ParseArtToolInfo(ref parsedFile, fileInfo);
            }

            if (fileInfo.ExporterInfo != IntPtr.Zero)
            {
                ParseExporterInfo(ref parsedFile, fileInfo);
            }

            if (fileInfo.FromFileName != IntPtr.Zero)
            {
                parsedFile.FromFileName = Marshal.PtrToStringAnsi(fileInfo.FromFileName) ?? throw new ExternalException();
            }

            if (fileInfo.SkeletonCount > 0)
            {
                ParseModels(ref parsedFile, fileInfo);
            }

            if (fileInfo.AnimationCount > 0)
            {
                ParseAnimations(ref parsedFile, fileInfo);
            }

            return parsedFile;
        }

        internal enum member_type
        {
            EndMember = 0x0,
            InlineMember = 0x1,
            ReferenceMember = 0x2,
            ReferenceToArrayMember = 0x3,
            ArrayOfReferencesMember = 0x4,
            VariantReferenceMember = 0x5,
            UnsupportedMemberType_Remove = 0x6,
            ReferenceToVariantArrayMember = 0x7,
            StringMember = 0x8,
            TransformMember = 0x9,
            Real32Member = 0xA,
            Int8Member = 0xB,
            UInt8Member = 0xC,
            BinormalInt8Member = 0xD,
            NormalUInt8Member = 0xE,
            Int16Member = 0xF,
            UInt16Member = 0x10,
            BinormalInt16Member = 0x11,
            NormalUInt16Member = 0x12,
            Int32Member = 0x13,
            UInt32Member = 0x14,
            Real16Member = 0x15,
            EmptyReferenceMember = 0x16,
            OnePastLastMemberType = 0x17,
            Bool32Member = 0x13,
        };

        // structs, mostly containing pointers, used while converting to the actually used structs

        #region "unmanaged structs"

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_data_type_definition
        {
            public member_type Type;
            public IntPtr Name;
            public IntPtr ReferenceType;
            public int ArrayWidth;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] Extra;

            public uint Ignored__Ignored;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_variant
        {
            public IntPtr Type;
            public IntPtr Object;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_art_tool_info
        {
            public IntPtr FromArtToolName;
            public int ArtToolMajorRevision;
            public int ArtToolMinorRevision;
            public float UnitsPerMeter;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] Origin;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] RightVector;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] UpVector;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] BackVector;

            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_exporter_info
        {
            public IntPtr ExporterName;
            public int ExporterMajorRevision;
            public int ExporterMinorRevision;
            public int ExporterCustomization;
            public int ExporterBuildNumber;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_pixel_layout
        {
            public int BytesPerPixel;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] ShiftForComponent;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] BitsForComponent;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_texture_mip_level
        {
            public int Stride;
            public int PixelByteCount;
            public IntPtr PixelBytes;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_texture_image
        {
            public int MIPLevelCount;
            public IntPtr MIPLevels;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_texture
        {
            public IntPtr FromFileName;
            public int TextureType;
            public int Width;
            public int Height;
            public int Encoding;
            public int SubFormat;
            public unm_pixel_layout Layout;
            public int ImageCount;
            public IntPtr Images;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_material_map
        {
            public IntPtr Usage;
            public IntPtr Material;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_material
        {
            public IntPtr Name;
            public int MapCount;
            public IntPtr Maps;
            public IntPtr Texture;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_transform
        {
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] Position;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] Orientation;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public float[] ScaleShear;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_bone
        {
            public IntPtr Name;
            public int ParentIndex;
            public unm_transform LocalTransform;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] InverseWorld4x4;

            public float LODError;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_skeleton
        {
            public IntPtr Name;
            public int BoneCount;
            public IntPtr Bones;
            public int LODType;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_vertex_annotation_set
        {
            public IntPtr Name;
            public IntPtr VertexAnnotationType;
            public int VertexAnnotationCount;
            public IntPtr VertexAnnotations;
            public int IndicesMapFromVertexToAnnotation;
            public int VertexAnnotationIndexCount;
            public IntPtr VertexAnnotationIndices;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_vertex_data
        {
            public IntPtr VertexType;
            public int VertexCount;
            public IntPtr Vertices;
            public int VertexComponentNameCount;
            public IntPtr VertexComponentNames;
            public int VertexAnnotationSetCount;
            public IntPtr VertexAnnotationSets;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_tri_material_group
        {
            public int MaterialIndex;
            public int TriFirst;
            public int TriCount;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_tri_annotation_set
        {
            public IntPtr Name;
            public IntPtr TriAnnotationType;
            public int TriAnnotationCount;
            public IntPtr TriAnnotations;
            public int IndicesMapFromTriToAnnotation;
            public int TriAnnotationIndexCount;
            public IntPtr TriAnnotationIndices;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_tri_topology
        {
            public int GroupCount;
            public IntPtr Groups;
            public int IndexCount;
            public IntPtr Indices;
            public int Index16Count;
            public IntPtr Indices16;
            public int VertexToVertexCount;
            public IntPtr VertexToVertexMap;
            public int VertexToTriangleCount;
            public IntPtr VertexToTriangleMap;
            public int SideToNeighborCount;
            public IntPtr SideToNeighborMap;
            public int BonesForTriangleCount;
            public IntPtr BonesForTriangle;
            public int TriangleToBoneCount;
            public IntPtr TriangleToBoneIndices;
            public int TriAnnotationSetCount;
            public IntPtr TriAnnotationSets;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_morph_target
        {
            public IntPtr ScalarName;
            public IntPtr VertexData;
            public int DataIsDeltas;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_material_binding
        {
            public IntPtr Material;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_bone_binding
        {
            public IntPtr BoneName;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] OBBMin;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] OBBMax;

            public int TriangleCount;
            public IntPtr TriangleIndices;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_mesh
        {
            public IntPtr Name;
            public IntPtr PrimaryVertexData;
            public int MorphTargetCount;
            public IntPtr MorphTargets;
            public IntPtr PrimaryTopology;
            public int MaterialBindingCount;
            public IntPtr MaterialBindings;
            public int BoneBindingCount;
            public IntPtr BoneBindings;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_model_mesh_binding
        {
            public IntPtr Mesh;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_model
        {
            public IntPtr Name;
            public IntPtr Skeleton;
            public unm_transform InitialPlacement;
            public int MeshBindingCount;
            public IntPtr MeshBindings;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_curve_data_header
        {
            public byte Format;
            public byte Degree;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_curve_data_da_k32f_c32f
        {
            public unm_curve_data_header CurveDataHeader;
            public short Padding;
            public int KnotCount;
            public IntPtr Knots;
            public int ControlCount;
            public IntPtr Controls;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_curve2
        {
            public unm_variant CurveData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_vector_track
        {
            public IntPtr Name;
            public uint TrackKey;
            public int Dimension;
            public unm_curve2 ValueCurve;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_transform_track
        {
            public IntPtr Name;
            public int Flags;
            public unm_curve2 OrientationCurve;
            public unm_curve2 PositionCurve;
            public unm_curve2 ScaleShearCurve;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_text_track_entry
        {
            public float TimeStamp;
            public IntPtr Text;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_text_track
        {
            public IntPtr Name;
            public int EntryCount;
            public IntPtr Entries;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_periodic_loop
        {
            public float Radius;
            public float dAngle;
            public float dZ;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] BasisX;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] BasisY;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] Axis;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_track_group
        {
            public IntPtr Name;
            public int VectorTrackCount;
            public IntPtr VectorTracks;
            public int TransformTrackCount;
            public IntPtr TransformTracks;
            public int TransformLODErrorCount;
            public IntPtr TransformLODErrors;
            public int TextTrackCount;
            public IntPtr TextTracks;
            public unm_transform InitialPlacement;
            public int AccumulationFlags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] LoopTranslation;

            public IntPtr PeriodicLoop;
            public IntPtr RootMotion;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_animation
        {
            public IntPtr Name;
            public float Duration;
            public float TimeStep;
            public float Oversampling;
            public int TrackGroupCount;
            public IntPtr TrackGroups;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_file_info
        {
            public IntPtr ArtToolInfo;
            public IntPtr ExporterInfo;
            public IntPtr FromFileName;
            public int TextureCount;
            public IntPtr Textures;
            public int MaterialCount;
            public IntPtr Materials;
            public int SkeletonCount;
            public IntPtr Skeletons;
            public int VertexDataCount;
            public IntPtr VertexDatas;
            public int TriTopologyCount;
            public IntPtr TriTopologies;
            public int MeshCount;
            public IntPtr Meshes;
            public int ModelCount;
            public IntPtr Models;
            public int TrackGroupCount;
            public IntPtr TrackGroups;
            public int AnimationCount;
            public IntPtr Animations;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_grn_reference
        {
            public uint SectionIndex;
            public uint Offset;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_grn_file_header
        {
            public uint Version;
            public uint TotalSize;
            public uint CRC;
            public uint SectionArrayOffset;
            public uint SectionArrayCount;
            public unm_grn_reference RootObjectTypeDefinition;
            public unm_grn_reference RootObject;
            public uint TypeTag;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] ExtraTags;

            public uint StringDatabaseCRC;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public uint[] ReservedUnused;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct unm_grn_file_magic_value
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] MagicValue;

            public uint HeaderSize;
            public uint HeaderFormat;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] Reserved;
        };

        #endregion "unmanaged structs"

        // cleaned up structs with only relevant data (for this exact use case, general usage requires further implementation)
        // e.g. no files have textures or materials, so that data is ignored.
        // TODO: add mesh support, not needed for animations/skinning, but meshes are used for collisions.
        public struct file_info
        {
            public art_tool_info ArtToolInfo;
            public exporter_info ExporterInfo;

            public string FromFileName;

            // model and animation are the highest level of data, I use them exclusively to make parsing easier
            // granny viewer gives lists of all skeletons, track groups, etc. unneeded for this use case.
            public List<model> Models;
            public List<animation> Animations;
        }

        public struct art_tool_info
        {
            public string FromArtToolName;
            public int ArtToolMajorRevision;
            public int ArtToolMinorRevision;
            public float UnitsPerMeter;
            public Vector3 Origin;
            public Vector3 RightVector;
            public Vector3 UpVector;
            public Vector3 BackVector;
        }

        public struct exporter_info
        {
            public string ExporterName;
            public int ExporterMajorRevision;
            public int ExporterMinorRevision;
            public int ExporterCustomization;
            public int ExporterBuildNumber;
        }

        public struct model
        {
            public string Name;
            public skeleton Skeleton;

            public transform InitialPlacement;
            //public List<mesh> Meshes;
        }

        public struct skeleton
        {
            public string Name;
            public List<bone> Bones;
        }

        public struct bone
        {
            public string Name;
            public int ParentIndex;
            public Vector3 ActualPosition; // horrible naming, this is the bone's position used for previewing
            public transform LocalTransform;
            public Matrix4x4 InverseWorld4x4;
        }

        public struct transform
        {
            public uint Flags;
            public Vector3 Position;
            public Quaternion Orientation;
            public float[][] ScaleShear;
        }

        public struct animation
        {
            public string Name;
            public float Duration;
            public float TimeStep;
            public float Oversampling;
            public List<track_group> TrackGroups;
        }

        public struct track_group
        {
            public string Name;
            public List<transform_track> TransformTracks;

            public transform InitialPlacement;
            //TODO: is more needed?
        }
        
        public struct transform_track
        {
            public string Name;
            public int Flags;
            public float[][] OrientationCurve;
            public float[][] PositionCurve;
            public float[][] ScaleShearCurve;
        }
    }
}