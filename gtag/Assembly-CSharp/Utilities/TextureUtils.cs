﻿using System;
using UnityEngine;

namespace Utilities
{
	// Token: 0x02000289 RID: 649
	public static class TextureUtils
	{
		// Token: 0x060010BB RID: 4283 RVA: 0x000592C0 File Offset: 0x000574C0
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

		// Token: 0x060010BC RID: 4284 RVA: 0x00059354 File Offset: 0x00057554
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
	}
}