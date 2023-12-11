﻿using System.Numerics;
using System.Runtime.InteropServices;

namespace VB3DLib
{
    public class GrannyFormats
    {
        // original file name is granny2.dll but i had to use a different version than the game does.
        // theoretically it's possible to use the same version but struct info is sourced from a .pdb of this version.
        [DllImport("granny2.7.0.30.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_GrannyGetFileInfo@4")]
        private static extern IntPtr GrannyGetFileInfo(IntPtr file);

        [DllImport("granny2.7.0.30.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "_GrannyReadEntireFileFromMemory@8")]
        private static extern IntPtr GrannyReadEntireFileFromMemory(int memorySize, IntPtr memory);

        private static T ReadFromPointerArray<T>(IntPtr ptrArray, int index) where T : struct
        {
            var ptr = Marshal.ReadIntPtr(ptrArray, index * IntPtr.Size);
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        private static T ReadFromArray<T>(IntPtr array, int index) where T : struct
        {
            var elementPtr = new IntPtr(array.ToInt64() + Marshal.SizeOf(typeof(T)) * index);
            return (T)Marshal.PtrToStructure(elementPtr, typeof(T));
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
            otpt.ScaleShear[0] = new[] {t.ScaleShear[0], t.ScaleShear[1], t.ScaleShear[2]};
            otpt.ScaleShear[1] = new[] {t.ScaleShear[3], t.ScaleShear[4], t.ScaleShear[5]};
            otpt.ScaleShear[2] = new[] {t.ScaleShear[6], t.ScaleShear[7], t.ScaleShear[8]};
            return otpt;
        }

        public static file_info ReadFileFromMemory(byte[] rawData)
        {
            var unmanagedPointer = Marshal.AllocHGlobal(rawData.Length);
            Marshal.Copy(rawData, 0, unmanagedPointer, rawData.Length);
            var file = GrannyReadEntireFileFromMemory(rawData.Length, unmanagedPointer);
            Marshal.FreeHGlobal(unmanagedPointer);
            var fileInfo = (unm_file_info)Marshal.PtrToStructure(GrannyGetFileInfo(file), typeof(unm_file_info));

            var parsedFile = new file_info();

            if (fileInfo.ArtToolInfo != IntPtr.Zero)
            {
                var artToolInfo = (unm_art_tool_info)Marshal.PtrToStructure(fileInfo.ArtToolInfo, typeof(unm_art_tool_info));
                parsedFile.ArtToolInfo.FromArtToolName = Marshal.PtrToStringAnsi(artToolInfo.FromArtToolName);
                parsedFile.ArtToolInfo.ArtToolMajorRevision = artToolInfo.ArtToolMajorRevision;
                parsedFile.ArtToolInfo.ArtToolMinorRevision = artToolInfo.ArtToolMinorRevision;
                parsedFile.ArtToolInfo.UnitsPerMeter = artToolInfo.UnitsPerMeter;
                parsedFile.ArtToolInfo.Origin = new Vector3(artToolInfo.Origin[0], artToolInfo.Origin[1], artToolInfo.Origin[2]);
                parsedFile.ArtToolInfo.RightVector = new Vector3(artToolInfo.RightVector[0], artToolInfo.RightVector[1], artToolInfo.RightVector[2]);
                parsedFile.ArtToolInfo.UpVector = new Vector3(artToolInfo.UpVector[0], artToolInfo.UpVector[1], artToolInfo.UpVector[2]);
                parsedFile.ArtToolInfo.BackVector = new Vector3(artToolInfo.BackVector[0], artToolInfo.BackVector[1], artToolInfo.BackVector[2]);
            }
            if (fileInfo.ExporterInfo != IntPtr.Zero)
            {
                var exporterInfo = (unm_exporter_info)Marshal.PtrToStructure(fileInfo.ExporterInfo, typeof(unm_exporter_info));
                parsedFile.ExporterInfo.ExporterName = Marshal.PtrToStringAnsi(exporterInfo.ExporterName);
                parsedFile.ExporterInfo.ExporterMajorRevision = exporterInfo.ExporterMajorRevision;
                parsedFile.ExporterInfo.ExporterMinorRevision = exporterInfo.ExporterMinorRevision;
                parsedFile.ExporterInfo.ExporterCustomization = exporterInfo.ExporterCustomization;
                parsedFile.ExporterInfo.ExporterBuildNumber = exporterInfo.ExporterBuildNumber;
            }
            if (fileInfo.FromFileName != IntPtr.Zero)
            {
                parsedFile.FromFileName = Marshal.PtrToStringAnsi(fileInfo.FromFileName);
            }

            parsedFile.Models = new List<model>();
            for (var i = 0; i < fileInfo.ModelCount; i++)
            {
                var model = ReadFromPointerArray<unm_model>(fileInfo.Models, i);
                var parsedModel = new model();
                parsedModel.Name = Marshal.PtrToStringAnsi(model.Name);
                var skeleton = (unm_skeleton)Marshal.PtrToStructure(model.Skeleton, typeof(unm_skeleton));
                parsedModel.Skeleton.Name = Marshal.PtrToStringAnsi(skeleton.Name);
                parsedModel.Skeleton.Bones = new List<bone>();
                for (var j = 0; j < skeleton.BoneCount; j++)
                {
                    var bone = ReadFromArray<unm_bone>(skeleton.Bones, j);
                    var parsedBone = new bone();
                    parsedBone.Name = Marshal.PtrToStringAnsi(bone.Name);
                    parsedBone.ParentIndex = bone.ParentIndex;
                    parsedBone.LocalTransform = CreateTransform(bone.LocalTransform);
                    parsedBone.InverseWorld4x4 = ConvertToMatrix4x4(bone.InverseWorld4x4);
                    parsedModel.Skeleton.Bones.Add(parsedBone);
                }
                parsedModel.InitialPlacement = CreateTransform(model.InitialPlacement);
                parsedFile.Models.Add(parsedModel);
            }

            parsedFile.Animations = new List<animation>();
            for (var i = 0; i < fileInfo.AnimationCount; i++)
            {
                var animation = ReadFromPointerArray<unm_animation>(fileInfo.Animations, i);
                var parsedAnimation = new animation();
                parsedAnimation.Name = Marshal.PtrToStringAnsi(animation.Name);
                parsedAnimation.Duration = animation.Duration;
                parsedAnimation.TimeStep = animation.TimeStep;
                parsedAnimation.Oversampling = animation.Oversampling;
                parsedAnimation.TrackGroups = new List<track_group>();
                for (var j = 0; j < animation.TrackGroupCount; j++)
                {
                    var trackGroup = ReadFromPointerArray<unm_track_group>(animation.TrackGroups, j);
                    var parsedTrackGroup = new track_group();
                    parsedTrackGroup.Name = Marshal.PtrToStringAnsi(trackGroup.Name);
                    parsedTrackGroup.TransformTracks = new List<transform_track>();
                    for (var k = 0; k < trackGroup.TransformTrackCount; k++)
                    {
                        var transformTrack = ReadFromArray<unm_transform_track>(trackGroup.TransformTracks, k);
                        var parsedTransformTrack = new transform_track();
                        parsedTransformTrack.Name = Marshal.PtrToStringAnsi(transformTrack.Name);
                        parsedTransformTrack.Flags = transformTrack.Flags;
                        parsedTransformTrack.OrientationCurve = new curve_data();
                        var orientationCurve = (unm_curve_data_da_k32f_c32f)Marshal.PtrToStructure(transformTrack.OrientationCurve.CurveData.Object, typeof(unm_curve_data_da_k32f_c32f)); // assuming this curve type is always correct
                        parsedTransformTrack.OrientationCurve.Knots = new List<float>();
                        for (var l = 0; l < orientationCurve.KnotCount; l++)
                        {
                            parsedTransformTrack.OrientationCurve.Knots.Add(ReadFromArray<float>(orientationCurve.Knots, l));
                        }
                        parsedTransformTrack.OrientationCurve.Controls = new List<float>();
                        for (var l = 0; l < orientationCurve.ControlCount; l++)
                        {
                            parsedTransformTrack.OrientationCurve.Controls.Add(ReadFromArray<float>(orientationCurve.Controls, l));
                        }
                        parsedTransformTrack.PositionCurve = new curve_data();
                        var positionCurve = (unm_curve_data_da_k32f_c32f)Marshal.PtrToStructure(transformTrack.PositionCurve.CurveData.Object, typeof(unm_curve_data_da_k32f_c32f)); // assuming this curve type is always correct
                        parsedTransformTrack.PositionCurve.Knots = new List<float>();
                        for (var l = 0; l < positionCurve.KnotCount; l++)
                        {
                            parsedTransformTrack.PositionCurve.Knots.Add(ReadFromArray<float>(positionCurve.Knots, l));
                        }
                        parsedTransformTrack.PositionCurve.Controls = new List<float>();
                        for (var l = 0; l < positionCurve.ControlCount; l++)
                        {
                            parsedTransformTrack.PositionCurve.Controls.Add(ReadFromArray<float>(positionCurve.Controls, l));
                        }
                        parsedTransformTrack.ScaleShearCurve = new curve_data();
                        var scaleShearCurve = (unm_curve_data_da_k32f_c32f)Marshal.PtrToStructure(transformTrack.ScaleShearCurve.CurveData.Object, typeof(unm_curve_data_da_k32f_c32f)); // assuming this curve type is always correct
                        parsedTransformTrack.ScaleShearCurve.Knots = new List<float>();
                        for (var l = 0; l < scaleShearCurve.KnotCount; l++)
                        {
                            parsedTransformTrack.ScaleShearCurve.Knots.Add(ReadFromArray<float>(scaleShearCurve.Knots, l));
                        }
                        parsedTransformTrack.ScaleShearCurve.Controls = new List<float>();
                        for (var l = 0; l < scaleShearCurve.ControlCount; l++)
                        {
                            parsedTransformTrack.ScaleShearCurve.Controls.Add(ReadFromArray<float>(scaleShearCurve.Controls, l));
                        }
                        parsedTrackGroup.TransformTracks.Add(parsedTransformTrack);
                        parsedTrackGroup.InitialPlacement = CreateTransform(trackGroup.InitialPlacement);
                    }
                    parsedAnimation.TrackGroups.Add(parsedTrackGroup);
                }
                parsedFile.Animations.Add(parsedAnimation);
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

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_variant
        {
            public IntPtr Type;
            public IntPtr Object;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_exporter_info
        {
            public IntPtr ExporterName;
            public int ExporterMajorRevision;
            public int ExporterMinorRevision;
            public int ExporterCustomization;
            public int ExporterBuildNumber;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_pixel_layout
        {
            public int BytesPerPixel;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] ShiftForComponent;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] BitsForComponent;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_texture_mip_level
        {
            public int Stride;
            public int PixelByteCount;
            public IntPtr PixelBytes;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_texture_image
        {
            public int MIPLevelCount;
            public IntPtr MIPLevels;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_material_map
        {
            public IntPtr Usage;
            public IntPtr Material;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_material
        {
            public IntPtr Name;
            public int MapCount;
            public IntPtr Maps;
            public IntPtr Texture;
            public unm_variant ExtendedData;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_skeleton
        {
            public IntPtr Name;
            public int BoneCount;
            public IntPtr Bones;
            public int LODType;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_tri_material_group
        {
            public int MaterialIndex;
            public int TriFirst;
            public int TriCount;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_morph_target
        {
            public IntPtr ScalarName;
            public IntPtr VertexData;
            public int DataIsDeltas;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_material_binding
        {
            public IntPtr Material;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_model_mesh_binding
        {
            public IntPtr Mesh;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_model
        {
            public IntPtr Name;
            public IntPtr Skeleton;
            public unm_transform InitialPlacement;
            public int MeshBindingCount;
            public IntPtr MeshBindings;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_curve_data_header
        {
            public byte Format;
            public byte Degree;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_curve_data_da_k32f_c32f
        {
            public unm_curve_data_header CurveDataHeader;
            public short Padding;
            public int KnotCount;
            public IntPtr Knots;
            public int ControlCount;
            public IntPtr Controls;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_curve2
        {
            public unm_variant CurveData;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_vector_track
        {
            public IntPtr Name;
            public uint TrackKey;
            public int Dimension;
            public unm_curve2 ValueCurve;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_transform_track
        {
            public IntPtr Name;
            public int Flags;
            public unm_curve2 OrientationCurve;
            public unm_curve2 PositionCurve;
            public unm_curve2 ScaleShearCurve;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_text_track_entry
        {
            public float TimeStamp;
            public IntPtr Text;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_text_track
        {
            public IntPtr Name;
            public int EntryCount;
            public IntPtr Entries;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_animation
        {
            public IntPtr Name;
            public float Duration;
            public float TimeStep;
            public float Oversampling;
            public int TrackGroupCount;
            public IntPtr TrackGroups;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct unm_grn_reference
        {
            public uint SectionIndex;
            public uint Offset;
        };

        [StructLayout(LayoutKind.Sequential)]
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

        [StructLayout(LayoutKind.Sequential)]
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
        // TODO: add mesh support, not needed for animations/skinning, but good to have for completeness
        public struct file_info
        {
            public art_tool_info ArtToolInfo;
            public exporter_info ExporterInfo;

            public string FromFileName;

            // model and animation are the highest level of data, I use them exclusively to make parsing easier
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
            public curve_data OrientationCurve;
            public curve_data PositionCurve;
            public curve_data ScaleShearCurve;
        }

        public struct curve_data
        {
            public List<float> Knots;
            public List<float> Controls;
        }
    }
}