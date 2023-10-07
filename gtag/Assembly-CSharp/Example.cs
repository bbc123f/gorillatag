using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class Example : MonoBehaviour
{
	// Token: 0x060000A1 RID: 161 RVA: 0x00006B34 File Offset: 0x00004D34
	private void OnDrawGizmos()
	{
		if (this.debugPoint)
		{
			DebugExtension.DrawPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale);
		}
		if (this.debugBounds)
		{
			DebugExtension.DrawBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color);
		}
		if (this.debugCircle)
		{
			DebugExtension.DrawCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius);
		}
		if (this.debugWireSphere)
		{
			Gizmos.color = this.debugWireSphere_Color;
			Gizmos.DrawWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Radius);
		}
		if (this.debugCylinder)
		{
			DebugExtension.DrawCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius);
		}
		if (this.debugCone)
		{
			DebugExtension.DrawCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle);
		}
		if (this.debugArrow)
		{
			DebugExtension.DrawArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color);
		}
		if (this.debugCapsule)
		{
			DebugExtension.DrawCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius);
		}
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00006CC0 File Offset: 0x00004EC0
	private void Update()
	{
		DebugExtension.DebugPoint(this.debugPoint_Position, this.debugPoint_Color, this.debugPoint_Scale, 0f, true);
		DebugExtension.DebugBounds(new Bounds(new Vector3(10f, 0f, 0f), this.debugBounds_Size), this.debugBounds_Color, 0f, true);
		DebugExtension.DebugCircle(new Vector3(20f, 0f, 0f), this.debugCircle_Up, this.debugCircle_Color, this.debugCircle_Radius, 0f, true);
		DebugExtension.DebugWireSphere(new Vector3(30f, 0f, 0f), this.debugWireSphere_Color, this.debugWireSphere_Radius, 0f, true);
		DebugExtension.DebugCylinder(new Vector3(40f, 0f, 0f), this.debugCylinder_End, this.debugCylinder_Color, this.debugCylinder_Radius, 0f, true);
		DebugExtension.DebugCone(new Vector3(50f, 0f, 0f), this.debugCone_Direction, this.debugCone_Color, this.debugCone_Angle, 0f, true);
		DebugExtension.DebugArrow(new Vector3(60f, 0f, 0f), this.debugArrow_Direction, this.debugArrow_Color, 0f, true);
		DebugExtension.DebugCapsule(new Vector3(70f, 0f, 0f), this.debugCapsule_End, this.debugCapsule_Color, this.debugCapsule_Radius, 0f, true);
	}

	// Token: 0x040000A7 RID: 167
	public bool debugPoint;

	// Token: 0x040000A8 RID: 168
	public Vector3 debugPoint_Position;

	// Token: 0x040000A9 RID: 169
	public float debugPoint_Scale;

	// Token: 0x040000AA RID: 170
	public Color debugPoint_Color;

	// Token: 0x040000AB RID: 171
	public bool debugBounds;

	// Token: 0x040000AC RID: 172
	public Vector3 debugBounds_Position;

	// Token: 0x040000AD RID: 173
	public Vector3 debugBounds_Size;

	// Token: 0x040000AE RID: 174
	public Color debugBounds_Color;

	// Token: 0x040000AF RID: 175
	public bool debugCircle;

	// Token: 0x040000B0 RID: 176
	public Vector3 debugCircle_Up;

	// Token: 0x040000B1 RID: 177
	public float debugCircle_Radius;

	// Token: 0x040000B2 RID: 178
	public Color debugCircle_Color;

	// Token: 0x040000B3 RID: 179
	public bool debugWireSphere;

	// Token: 0x040000B4 RID: 180
	public float debugWireSphere_Radius;

	// Token: 0x040000B5 RID: 181
	public Color debugWireSphere_Color;

	// Token: 0x040000B6 RID: 182
	public bool debugCylinder;

	// Token: 0x040000B7 RID: 183
	public Vector3 debugCylinder_End;

	// Token: 0x040000B8 RID: 184
	public float debugCylinder_Radius;

	// Token: 0x040000B9 RID: 185
	public Color debugCylinder_Color;

	// Token: 0x040000BA RID: 186
	public bool debugCone;

	// Token: 0x040000BB RID: 187
	public Vector3 debugCone_Direction;

	// Token: 0x040000BC RID: 188
	public float debugCone_Angle;

	// Token: 0x040000BD RID: 189
	public Color debugCone_Color;

	// Token: 0x040000BE RID: 190
	public bool debugArrow;

	// Token: 0x040000BF RID: 191
	public Vector3 debugArrow_Direction;

	// Token: 0x040000C0 RID: 192
	public Color debugArrow_Color;

	// Token: 0x040000C1 RID: 193
	public bool debugCapsule;

	// Token: 0x040000C2 RID: 194
	public Vector3 debugCapsule_End;

	// Token: 0x040000C3 RID: 195
	public float debugCapsule_Radius;

	// Token: 0x040000C4 RID: 196
	public Color debugCapsule_Color;
}
