using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200009C RID: 156
public class MediaPlayerImage : Image
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x0600035C RID: 860 RVA: 0x00013D5A File Offset: 0x00011F5A
	// (set) Token: 0x0600035D RID: 861 RVA: 0x00013D62 File Offset: 0x00011F62
	public MediaPlayerImage.ButtonType buttonType
	{
		get
		{
			return this.m_ButtonType;
		}
		set
		{
			if (this.m_ButtonType != value)
			{
				this.m_ButtonType = value;
				this.SetAllDirty();
			}
		}
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00013D7C File Offset: 0x00011F7C
	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
		Vector4 vector = new Vector4(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
		Color32 color = this.color;
		toFill.Clear();
		switch (this.m_ButtonType)
		{
		case MediaPlayerImage.ButtonType.Play:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(vector.z, Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(1f, 0.5f));
			toFill.AddTriangle(0, 1, 2);
			return;
		case MediaPlayerImage.ButtonType.Pause:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.35f), vector.w), color, new Vector2(0.35f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.35f), vector.y), color, new Vector2(0.35f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.65f), vector.y), color, new Vector2(0.65f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.65f), vector.w), color, new Vector2(0.65f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(2, 3, 0);
			toFill.AddTriangle(4, 5, 6);
			toFill.AddTriangle(6, 7, 4);
			return;
		case MediaPlayerImage.ButtonType.FastForward:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.5f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.y), color, new Vector2(0.5f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.w), color, new Vector2(0.5f, 1f));
			toFill.AddVert(new Vector3(vector.z, Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(1f, 0.5f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(3, 4, 5);
			return;
		case MediaPlayerImage.ButtonType.Rewind:
			toFill.AddVert(new Vector3(vector.x, Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.w), color, new Vector2(0.5f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), vector.y), color, new Vector2(0.5f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.5f, 0.5f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(3, 4, 5);
			return;
		case MediaPlayerImage.ButtonType.SkipForward:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.4375f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.4375f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.4375f), vector.y), color, new Vector2(0.4375f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.4375f), vector.w), color, new Vector2(0.4375f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.875f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.875f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.875f), vector.y), color, new Vector2(0.875f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.875f), vector.w), color, new Vector2(0.875f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(3, 4, 5);
			toFill.AddTriangle(6, 7, 8);
			toFill.AddTriangle(8, 9, 6);
			return;
		case MediaPlayerImage.ButtonType.SkipBack:
			toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.125f), vector.w), color, new Vector2(0.125f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.125f), vector.y), color, new Vector2(0.125f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.125f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.125f, 0.5f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5625f), vector.w), color, new Vector2(0.5625f, 1f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5625f), vector.y), color, new Vector2(0.5625f, 0f));
			toFill.AddVert(new Vector3(Mathf.Lerp(vector.x, vector.z, 0.5625f), Mathf.Lerp(vector.y, vector.w, 0.5f)), color, new Vector2(0.5625f, 0.5f));
			toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			toFill.AddTriangle(0, 1, 2);
			toFill.AddTriangle(2, 3, 0);
			toFill.AddTriangle(4, 5, 6);
			toFill.AddTriangle(7, 8, 9);
			return;
		}
		toFill.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
		toFill.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
		toFill.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
		toFill.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
		toFill.AddTriangle(0, 1, 2);
		toFill.AddTriangle(2, 3, 0);
	}

	// Token: 0x04000401 RID: 1025
	[SerializeField]
	private MediaPlayerImage.ButtonType m_ButtonType;

	// Token: 0x020003C6 RID: 966
	public enum ButtonType
	{
		// Token: 0x04001BDF RID: 7135
		Play,
		// Token: 0x04001BE0 RID: 7136
		Pause,
		// Token: 0x04001BE1 RID: 7137
		FastForward,
		// Token: 0x04001BE2 RID: 7138
		Rewind,
		// Token: 0x04001BE3 RID: 7139
		SkipForward,
		// Token: 0x04001BE4 RID: 7140
		SkipBack,
		// Token: 0x04001BE5 RID: 7141
		Stop
	}
}
