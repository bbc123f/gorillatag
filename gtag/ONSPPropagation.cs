using System;
using System.Runtime.InteropServices;
using Oculus.Spatializer.Propagation;
using UnityEngine;

internal class ONSPPropagation
{
	public enum ovrAudioScalarType : uint
	{
		Int8,
		UInt8,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Float16,
		Float32,
		Float64
	}

	public class ClientType
	{
		public const uint OVRA_CLIENT_TYPE_NATIVE = 0u;

		public const uint OVRA_CLIENT_TYPE_WWISE_2016 = 1u;

		public const uint OVRA_CLIENT_TYPE_WWISE_2017_1 = 2u;

		public const uint OVRA_CLIENT_TYPE_WWISE_2017_2 = 3u;

		public const uint OVRA_CLIENT_TYPE_WWISE_2018_1 = 4u;

		public const uint OVRA_CLIENT_TYPE_FMOD = 5u;

		public const uint OVRA_CLIENT_TYPE_UNITY = 6u;

		public const uint OVRA_CLIENT_TYPE_UE4 = 7u;

		public const uint OVRA_CLIENT_TYPE_VST = 8u;

		public const uint OVRA_CLIENT_TYPE_AAX = 9u;

		public const uint OVRA_CLIENT_TYPE_TEST = 10u;

		public const uint OVRA_CLIENT_TYPE_OTHER = 11u;

		public const uint OVRA_CLIENT_TYPE_WWISE_UNKNOWN = 12u;

		public const uint OVRA_CLIENT_TYPE_WWISE_2019_1 = 13u;

		public const uint OVRA_CLIENT_TYPE_WWISE_2019_2 = 14u;
	}

	public interface PropagationInterface
	{
		int SetPropagationQuality(float quality);

		int SetPropagationThreadAffinity(ulong cpuMask);

		int CreateAudioGeometry(out IntPtr geometry);

		int DestroyAudioGeometry(IntPtr geometry);

		int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount);

		int AudioGeometrySetTransform(IntPtr geometry, float[] matrix4x4);

		int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		int AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		int CreateAudioMaterial(out IntPtr material);

		int DestroyAudioMaterial(IntPtr material);

		int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		int AudioMaterialReset(IntPtr material, MaterialProperty property);
	}

	public class UnityNativeInterface : PropagationInterface
	{
		public const string strOSPS = "AudioPluginOculusSpatializer";

		private IntPtr context_ = IntPtr.Zero;

		private IntPtr context
		{
			get
			{
				if (context_ == IntPtr.Zero)
				{
					ovrAudio_GetPluginContext(out context_, 6u);
				}
				return context_;
			}
		}

		[DllImport("AudioPluginOculusSpatializer")]
		public static extern int ovrAudio_GetPluginContext(out IntPtr context, uint clientType);

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_SetPropagationQuality(IntPtr context, float quality);

		public int SetPropagationQuality(float quality)
		{
			return ovrAudio_SetPropagationQuality(context, quality);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_SetPropagationThreadAffinity(IntPtr context, ulong cpuMask);

		public int SetPropagationThreadAffinity(ulong cpuMask)
		{
			return ovrAudio_SetPropagationThreadAffinity(context, cpuMask);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);

		public int CreateAudioGeometry(out IntPtr geometry)
		{
			return ovrAudio_CreateAudioGeometry(context, out geometry);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);

		public int DestroyAudioGeometry(IntPtr geometry)
		{
			return ovrAudio_DestroyAudioGeometry(geometry);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount);

		public int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount)
		{
			return ovrAudio_AudioGeometryUploadMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)(ulong)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)(ulong)indexCount, ovrAudioScalarType.UInt32, groups, (UIntPtr)(ulong)groupCount);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float[] matrix4x4);

		public int AudioGeometrySetTransform(IntPtr geometry, float[] matrix4x4)
		{
			return ovrAudio_AudioGeometrySetTransform(geometry, matrix4x4);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
		{
			return ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);

		public int CreateAudioMaterial(out IntPtr material)
		{
			return ovrAudio_CreateAudioMaterial(context, out material);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);

		public int DestroyAudioMaterial(IntPtr material)
		{
			return ovrAudio_DestroyAudioMaterial(material);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
		{
			return ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
		{
			return ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
		}

		[DllImport("AudioPluginOculusSpatializer")]
		private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);

		public int AudioMaterialReset(IntPtr material, MaterialProperty property)
		{
			return ovrAudio_AudioMaterialReset(material, property);
		}
	}

	public class WwisePluginInterface : PropagationInterface
	{
		public const string strOSPS = "OculusSpatializerWwise";

		private IntPtr context_ = IntPtr.Zero;

		private IntPtr context
		{
			get
			{
				if (context_ == IntPtr.Zero)
				{
					ovrAudio_GetPluginContext(out context_, 12u);
				}
				return context_;
			}
		}

		[DllImport("OculusSpatializerWwise")]
		public static extern int ovrAudio_GetPluginContext(out IntPtr context, uint clientType);

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_SetPropagationQuality(IntPtr context, float quality);

		public int SetPropagationQuality(float quality)
		{
			return ovrAudio_SetPropagationQuality(context, quality);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_SetPropagationThreadAffinity(IntPtr context, ulong cpuMask);

		public int SetPropagationThreadAffinity(ulong cpuMask)
		{
			return ovrAudio_SetPropagationThreadAffinity(context, cpuMask);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);

		public int CreateAudioGeometry(out IntPtr geometry)
		{
			return ovrAudio_CreateAudioGeometry(context, out geometry);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);

		public int DestroyAudioGeometry(IntPtr geometry)
		{
			return ovrAudio_DestroyAudioGeometry(geometry);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount);

		public int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount)
		{
			return ovrAudio_AudioGeometryUploadMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)(ulong)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)(ulong)indexCount, ovrAudioScalarType.UInt32, groups, (UIntPtr)(ulong)groupCount);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float[] matrix4x4);

		public int AudioGeometrySetTransform(IntPtr geometry, float[] matrix4x4)
		{
			return ovrAudio_AudioGeometrySetTransform(geometry, matrix4x4);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
		{
			return ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);

		public int CreateAudioMaterial(out IntPtr material)
		{
			return ovrAudio_CreateAudioMaterial(context, out material);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);

		public int DestroyAudioMaterial(IntPtr material)
		{
			return ovrAudio_DestroyAudioMaterial(material);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
		{
			return ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
		{
			return ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
		}

		[DllImport("OculusSpatializerWwise")]
		private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);

		public int AudioMaterialReset(IntPtr material, MaterialProperty property)
		{
			return ovrAudio_AudioMaterialReset(material, property);
		}
	}

	public class FMODPluginInterface : PropagationInterface
	{
		public const string strOSPS = "OculusSpatializerFMOD";

		private IntPtr context_ = IntPtr.Zero;

		private IntPtr context
		{
			get
			{
				if (context_ == IntPtr.Zero)
				{
					ovrAudio_GetPluginContext(out context_, 5u);
				}
				return context_;
			}
		}

		[DllImport("OculusSpatializerFMOD")]
		public static extern int ovrAudio_GetPluginContext(out IntPtr context, uint clientType);

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_SetPropagationQuality(IntPtr context, float quality);

		public int SetPropagationQuality(float quality)
		{
			return ovrAudio_SetPropagationQuality(context, quality);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_SetPropagationThreadAffinity(IntPtr context, ulong cpuMask);

		public int SetPropagationThreadAffinity(ulong cpuMask)
		{
			return ovrAudio_SetPropagationThreadAffinity(context, cpuMask);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_CreateAudioGeometry(IntPtr context, out IntPtr geometry);

		public int CreateAudioGeometry(out IntPtr geometry)
		{
			return ovrAudio_CreateAudioGeometry(context, out geometry);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_DestroyAudioGeometry(IntPtr geometry);

		public int DestroyAudioGeometry(IntPtr geometry)
		{
			return ovrAudio_DestroyAudioGeometry(geometry);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, UIntPtr verticesBytesOffset, UIntPtr vertexCount, UIntPtr vertexStride, ovrAudioScalarType vertexType, int[] indices, UIntPtr indicesByteOffset, UIntPtr indexCount, ovrAudioScalarType indexType, MeshGroup[] groups, UIntPtr groupCount);

		public int AudioGeometryUploadMeshArrays(IntPtr geometry, float[] vertices, int vertexCount, int[] indices, int indexCount, MeshGroup[] groups, int groupCount)
		{
			return ovrAudio_AudioGeometryUploadMeshArrays(geometry, vertices, UIntPtr.Zero, (UIntPtr)(ulong)vertexCount, UIntPtr.Zero, ovrAudioScalarType.Float32, indices, UIntPtr.Zero, (UIntPtr)(ulong)indexCount, ovrAudioScalarType.UInt32, groups, (UIntPtr)(ulong)groupCount);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioGeometrySetTransform(IntPtr geometry, float[] matrix4x4);

		public int AudioGeometrySetTransform(IntPtr geometry, float[] matrix4x4)
		{
			return ovrAudio_AudioGeometrySetTransform(geometry, matrix4x4);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4);

		public int AudioGeometryGetTransform(IntPtr geometry, out float[] matrix4x4)
		{
			return ovrAudio_AudioGeometryGetTransform(geometry, out matrix4x4);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFile(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryWriteMeshFile(geometry, filePath);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioGeometryReadMeshFile(IntPtr geometry, string filePath);

		public int AudioGeometryReadMeshFile(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryReadMeshFile(geometry, filePath);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath);

		public int AudioGeometryWriteMeshFileObj(IntPtr geometry, string filePath)
		{
			return ovrAudio_AudioGeometryWriteMeshFileObj(geometry, filePath);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_CreateAudioMaterial(IntPtr context, out IntPtr material);

		public int CreateAudioMaterial(out IntPtr material)
		{
			return ovrAudio_CreateAudioMaterial(context, out material);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_DestroyAudioMaterial(IntPtr material);

		public int DestroyAudioMaterial(IntPtr material)
		{
			return ovrAudio_DestroyAudioMaterial(material);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value);

		public int AudioMaterialSetFrequency(IntPtr material, MaterialProperty property, float frequency, float value)
		{
			return ovrAudio_AudioMaterialSetFrequency(material, property, frequency, value);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value);

		public int AudioMaterialGetFrequency(IntPtr material, MaterialProperty property, float frequency, out float value)
		{
			return ovrAudio_AudioMaterialGetFrequency(material, property, frequency, out value);
		}

		[DllImport("OculusSpatializerFMOD")]
		private static extern int ovrAudio_AudioMaterialReset(IntPtr material, MaterialProperty property);

		public int AudioMaterialReset(IntPtr material, MaterialProperty property)
		{
			return ovrAudio_AudioMaterialReset(material, property);
		}
	}

	private static PropagationInterface CachedInterface;

	public static PropagationInterface Interface
	{
		get
		{
			if (CachedInterface == null)
			{
				CachedInterface = FindInterface();
			}
			return CachedInterface;
		}
	}

	private static PropagationInterface FindInterface()
	{
		IntPtr context;
		try
		{
			WwisePluginInterface.ovrAudio_GetPluginContext(out context, 12u);
			Debug.Log("Propagation initialized with Wwise Oculus Spatializer plugin");
			return new WwisePluginInterface();
		}
		catch (DllNotFoundException)
		{
		}
		try
		{
			FMODPluginInterface.ovrAudio_GetPluginContext(out context, 5u);
			Debug.Log("Propagation initialized with FMOD Oculus Spatializer plugin");
			return new FMODPluginInterface();
		}
		catch (DllNotFoundException)
		{
		}
		Debug.Log("Propagation initialized with Unity Oculus Spatializer plugin");
		return new UnityNativeInterface();
	}
}
