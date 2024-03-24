using System;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utilities
{
	public static class TextureUtils
	{
		public static Color CalcAverageColor(Texture2D tex)
		{
			if (tex == null)
			{
				return default(Color);
			}
			Color32[] pixels = tex.GetPixels32();
			int num = pixels.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 += (int)pixels[i].r;
				num3 += (int)pixels[i].g;
				num4 += (int)pixels[i].b;
			}
			return new Color32((byte)(num2 / num), (byte)(num3 / num), (byte)(num4 / num), byte.MaxValue);
		}

		public static void SaveToFile(Texture source, string filePath, int width, int height, SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95, bool asynchronous = true, Action<bool> done = null)
		{
			if (source is Texture2D || source is RenderTexture)
			{
				if (width < 0 || height < 0)
				{
					width = source.width;
					height = source.height;
				}
				RenderTexture resizeRT = RenderTexture.GetTemporary(width, height, 0);
				Graphics.Blit(source, resizeRT);
				NativeArray<byte> narray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				AsyncGPUReadbackRequest asyncGPUReadbackRequest = AsyncGPUReadback.RequestIntoNativeArray<byte>(ref narray, resizeRT, 0, delegate(AsyncGPUReadbackRequest request)
				{
					if (!request.hasError)
					{
						NativeArray<byte> nativeArray;
						switch (fileFormat)
						{
						case SaveTextureFileFormat.EXR:
							nativeArray = ImageConversion.EncodeNativeArrayToEXR<byte>(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0U, Texture2D.EXRFlags.None);
							goto IL_C8;
						case SaveTextureFileFormat.JPG:
							nativeArray = ImageConversion.EncodeNativeArrayToJPG<byte>(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0U, jpgQuality);
							goto IL_C8;
						case SaveTextureFileFormat.TGA:
							nativeArray = ImageConversion.EncodeNativeArrayToTGA<byte>(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0U);
							goto IL_C8;
						}
						nativeArray = ImageConversion.EncodeNativeArrayToPNG<byte>(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0U);
						IL_C8:
						File.WriteAllBytes(filePath, nativeArray.ToArray());
						nativeArray.Dispose();
					}
					narray.Dispose();
					Action<bool> done3 = done;
					if (done3 == null)
					{
						return;
					}
					done3(!request.hasError);
				});
				if (!asynchronous)
				{
					asyncGPUReadbackRequest.WaitForCompletion();
				}
				return;
			}
			Action<bool> done2 = done;
			if (done2 == null)
			{
				return;
			}
			done2(false);
		}

		public static Texture2D CreateCopy(Texture2D tex)
		{
			if (tex == null)
			{
				throw new ArgumentNullException("tex");
			}
			RenderTexture temporary = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
			Graphics.Blit(tex, temporary);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(tex.width, tex.height);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}

		[CompilerGenerated]
		private sealed class <>c__DisplayClass1_0
		{
			public <>c__DisplayClass1_0()
			{
			}

			internal void <SaveToFile>b__0(AsyncGPUReadbackRequest request)
			{
				if (!request.hasError)
				{
					NativeArray<byte> nativeArray;
					switch (this.fileFormat)
					{
					case SaveTextureFileFormat.EXR:
						nativeArray = ImageConversion.EncodeNativeArrayToEXR<byte>(this.narray, this.resizeRT.graphicsFormat, (uint)this.width, (uint)this.height, 0U, Texture2D.EXRFlags.None);
						goto IL_C8;
					case SaveTextureFileFormat.JPG:
						nativeArray = ImageConversion.EncodeNativeArrayToJPG<byte>(this.narray, this.resizeRT.graphicsFormat, (uint)this.width, (uint)this.height, 0U, this.jpgQuality);
						goto IL_C8;
					case SaveTextureFileFormat.TGA:
						nativeArray = ImageConversion.EncodeNativeArrayToTGA<byte>(this.narray, this.resizeRT.graphicsFormat, (uint)this.width, (uint)this.height, 0U);
						goto IL_C8;
					}
					nativeArray = ImageConversion.EncodeNativeArrayToPNG<byte>(this.narray, this.resizeRT.graphicsFormat, (uint)this.width, (uint)this.height, 0U);
					IL_C8:
					File.WriteAllBytes(this.filePath, nativeArray.ToArray());
					nativeArray.Dispose();
				}
				this.narray.Dispose();
				Action<bool> action = this.done;
				if (action == null)
				{
					return;
				}
				action(!request.hasError);
			}

			public SaveTextureFileFormat fileFormat;

			public NativeArray<byte> narray;

			public RenderTexture resizeRT;

			public int width;

			public int height;

			public int jpgQuality;

			public string filePath;

			public Action<bool> done;
		}
	}
}
