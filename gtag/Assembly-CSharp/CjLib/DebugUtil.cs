using System;
using System.Collections.Generic;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000352 RID: 850
	public class DebugUtil
	{
		// Token: 0x06001882 RID: 6274 RVA: 0x0008364C File Offset: 0x0008184C
		private static Material GetMaterial(DebugUtil.Style style, bool depthTest, bool capShiftScale)
		{
			int num = 0;
			if (style - DebugUtil.Style.FlatShaded <= 1)
			{
				num |= 1;
			}
			if (capShiftScale)
			{
				num |= 2;
			}
			if (depthTest)
			{
				num |= 4;
			}
			if (DebugUtil.s_materialPool == null)
			{
				DebugUtil.s_materialPool = new Dictionary<int, Material>();
			}
			Material material;
			if (!DebugUtil.s_materialPool.TryGetValue(num, out material) || material == null)
			{
				if (material == null)
				{
					DebugUtil.s_materialPool.Remove(num);
				}
				Shader shader = Shader.Find(depthTest ? "CjLib/Primitive" : "CjLib/PrimitiveNoZTest");
				if (shader == null)
				{
					return null;
				}
				material = new Material(shader);
				if ((num & 1) != 0)
				{
					material.EnableKeyword("NORMAL_ON");
				}
				if ((num & 2) != 0)
				{
					material.EnableKeyword("CAP_SHIFT_SCALE");
				}
				DebugUtil.s_materialPool.Add(num, material);
			}
			return material;
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x00083705 File Offset: 0x00081905
		private static MaterialPropertyBlock GetMaterialPropertyBlock()
		{
			if (DebugUtil.s_materialProperties == null)
			{
				return DebugUtil.s_materialProperties = new MaterialPropertyBlock();
			}
			return DebugUtil.s_materialProperties;
		}

		// Token: 0x06001884 RID: 6276 RVA: 0x00083720 File Offset: 0x00081920
		public static void DrawLine(Vector3 v0, Vector3 v1, Color color, bool depthTest = true)
		{
			Mesh mesh = PrimitiveMeshFactory.Line(v0, v1);
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(DebugUtil.Style.Wireframe, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(1f, 1f, 1f, 0f));
			materialPropertyBlock.SetFloat("_ZBias", DebugUtil.s_wireframeZBias);
			Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x000837A8 File Offset: 0x000819A8
		public static void DrawLines(Vector3[] aVert, Color color, bool depthTest = true)
		{
			Mesh mesh = PrimitiveMeshFactory.Lines(aVert);
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(DebugUtil.Style.Wireframe, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(1f, 1f, 1f, 0f));
			materialPropertyBlock.SetFloat("_ZBias", DebugUtil.s_wireframeZBias);
			Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x06001886 RID: 6278 RVA: 0x00083830 File Offset: 0x00081A30
		public static void DrawLineStrip(Vector3[] aVert, Color color, bool depthTest = true)
		{
			Mesh mesh = PrimitiveMeshFactory.LineStrip(aVert);
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(DebugUtil.Style.Wireframe, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(1f, 1f, 1f, 0f));
			materialPropertyBlock.SetFloat("_ZBias", DebugUtil.s_wireframeZBias);
			Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x000838B8 File Offset: 0x00081AB8
		public static void DrawArc(Vector3 center, Vector3 from, Vector3 normal, float angle, float radius, int numSegments, Color color, bool depthTest = true)
		{
			if (numSegments <= 0)
			{
				return;
			}
			from.Normalize();
			from *= radius;
			Vector3[] array = new Vector3[numSegments + 1];
			array[0] = center + from;
			float num = 1f / (float)numSegments;
			Quaternion rotation = QuaternionUtil.AxisAngle(normal, angle * num);
			Vector3 vector = rotation * from;
			for (int i = 1; i <= numSegments; i++)
			{
				array[i] = center + vector;
				vector = rotation * vector;
			}
			DebugUtil.DrawLineStrip(array, color, depthTest);
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x00083944 File Offset: 0x00081B44
		public static void DrawLocator(Vector3 position, Vector3 right, Vector3 up, Vector3 forward, Color rightColor, Color upColor, Color forwardColor, float size = 0.5f)
		{
			DebugUtil.DrawLine(position, position + right * size, rightColor, true);
			DebugUtil.DrawLine(position, position + up * size, upColor, true);
			DebugUtil.DrawLine(position, position + forward * size, forwardColor, true);
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x00083996 File Offset: 0x00081B96
		public static void DrawLocator(Vector3 position, Vector3 right, Vector3 up, Vector3 forward, float size = 0.5f)
		{
			DebugUtil.DrawLocator(position, right, up, forward, Color.red, Color.green, Color.blue, size);
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x000839B4 File Offset: 0x00081BB4
		public static void DrawLocator(Vector3 position, Quaternion rotation, Color rightColor, Color upColor, Color forwardColor, float size = 0.5f)
		{
			Vector3 right = rotation * Vector3.right;
			Vector3 up = rotation * Vector3.up;
			Vector3 forward = rotation * Vector3.forward;
			DebugUtil.DrawLocator(position, right, up, forward, rightColor, upColor, forwardColor, size);
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x000839F4 File Offset: 0x00081BF4
		public static void DrawLocator(Vector3 position, Quaternion rotation, float size = 0.5f)
		{
			DebugUtil.DrawLocator(position, rotation, Color.red, Color.green, Color.blue, size);
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x00083A10 File Offset: 0x00081C10
		public static void DrawBox(Vector3 center, Quaternion rotation, Vector3 dimensions, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (dimensions.x < MathUtil.Epsilon || dimensions.y < MathUtil.Epsilon || dimensions.z < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.BoxWireframe();
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.BoxSolidColor();
				break;
			case DebugUtil.Style.FlatShaded:
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.BoxFlatShaded();
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(dimensions.x, dimensions.y, dimensions.z, 0f));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, center, rotation, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x00083AF0 File Offset: 0x00081CF0
		public static void DrawRect(Vector3 center, Quaternion rotation, Vector2 dimensions, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (dimensions.x < MathUtil.Epsilon || dimensions.y < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.RectWireframe();
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.RectSolidColor();
				break;
			case DebugUtil.Style.FlatShaded:
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.RectFlatShaded();
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(dimensions.x, 1f, dimensions.y, 0f));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, center, rotation, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x00083BC4 File Offset: 0x00081DC4
		public static void DrawRect2D(Vector3 center, float rotationDeg, Vector2 dimensions, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			Quaternion rotation = Quaternion.AngleAxis(rotationDeg, Vector3.forward) * Quaternion.AngleAxis(90f, Vector3.right);
			DebugUtil.DrawRect(center, rotation, dimensions, color, depthTest, style);
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x00083C00 File Offset: 0x00081E00
		public static void DrawCircle(Vector3 center, Quaternion rotation, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CircleWireframe(numSegments);
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.CircleSolidColor(numSegments);
				break;
			case DebugUtil.Style.FlatShaded:
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CircleFlatShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(radius, radius, radius, 0f));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, center, rotation, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x00083CB8 File Offset: 0x00081EB8
		public static void DrawCircle(Vector3 center, Vector3 normal, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.5f) ? Vector3.up : Vector3.forward, normal)), normal);
			DebugUtil.DrawCircle(center, rotation, radius, numSegments, color, depthTest, style);
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x00083D09 File Offset: 0x00081F09
		public static void DrawCircle2D(Vector3 center, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			DebugUtil.DrawCircle(center, Vector3.forward, radius, numSegments, color, depthTest, style);
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x00083D20 File Offset: 0x00081F20
		public static void DrawCylinder(Vector3 center, Quaternion rotation, float height, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CylinderWireframe(numSegments);
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.CylinderSolidColor(numSegments);
				break;
			case DebugUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CylinderFlatShaded(numSegments);
				break;
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CylinderSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, true);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(radius, radius, radius, height));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, center, rotation, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x00083DE8 File Offset: 0x00081FE8
		public static void DrawCylinder(Vector3 point0, Vector3 point1, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 center = 0.5f * (point0 + point1);
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Mathf.Abs(Vector3.Dot(vector.normalized, Vector3.up)) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			DebugUtil.DrawCylinder(center, rotation, magnitude, radius, numSegments, color, depthTest, style);
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x00083E70 File Offset: 0x00082070
		public static void DrawSphere(Vector3 center, Quaternion rotation, float radius, int latSegments, int longSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.SphereWireframe(latSegments, longSegments);
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.SphereSolidColor(latSegments, longSegments);
				break;
			case DebugUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.SphereFlatShaded(latSegments, longSegments);
				break;
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.SphereSmoothShaded(latSegments, longSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(radius, radius, radius, 0f));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, center, rotation, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x00083F36 File Offset: 0x00082136
		public static void DrawSphere(Vector3 center, float radius, int latSegments, int longSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			DebugUtil.DrawSphere(center, Quaternion.identity, radius, latSegments, longSegments, color, depthTest, style);
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x00083F4C File Offset: 0x0008214C
		public static void DrawSphereTripleCircles(Vector3 center, Quaternion rotation, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			Vector3 normal = rotation * Vector3.right;
			Vector3 normal2 = rotation * Vector3.up;
			Vector3 normal3 = rotation * Vector3.forward;
			DebugUtil.DrawCircle(center, normal, radius, numSegments, color, depthTest, style);
			DebugUtil.DrawCircle(center, normal2, radius, numSegments, color, depthTest, style);
			DebugUtil.DrawCircle(center, normal3, radius, numSegments, color, depthTest, style);
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x00083FAA File Offset: 0x000821AA
		public static void DrawSphereTripleCircles(Vector3 center, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			DebugUtil.DrawSphereTripleCircles(center, Quaternion.identity, radius, numSegments, color, depthTest, style);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x00083FC0 File Offset: 0x000821C0
		public static void DrawCapsule(Vector3 center, Quaternion rotation, float height, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.CapsuleWireframe(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.CapsuleSolidColor(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				break;
			case DebugUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.CapsuleFlatShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				break;
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.CapsuleSmoothShaded(latSegmentsPerCap, longSegmentsPerCap, true, true, false);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, true);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(radius, radius, radius, height));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, center, rotation, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x0008409C File Offset: 0x0008229C
		public static void DrawCapsule(Vector3 point0, Vector3 point1, float radius, int latSegmentsPerCap, int longSegmentsPerCap, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			Vector3 vector = point1 - point0;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Vector3 center = 0.5f * (point0 + point1);
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Mathf.Abs(Vector3.Dot(vector.normalized, Vector3.up)) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			DebugUtil.DrawCapsule(center, rotation, magnitude, radius, latSegmentsPerCap, longSegmentsPerCap, color, depthTest, style);
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x00084128 File Offset: 0x00082328
		public static void DrawCapsule2D(Vector3 center, float rotationDeg, float height, float radius, int capSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.Capsule2DWireframe(capSegments);
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.Capsule2DSolidColor(capSegments);
				break;
			case DebugUtil.Style.FlatShaded:
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.Capsule2DFlatShaded(capSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, true);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(radius, radius, radius, height));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, center, Quaternion.AngleAxis(rotationDeg, Vector3.forward), material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x000841F0 File Offset: 0x000823F0
		public static void DrawCone(Vector3 baseCenter, Quaternion rotation, float height, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			if (height < MathUtil.Epsilon || radius < MathUtil.Epsilon)
			{
				return;
			}
			Mesh mesh = null;
			switch (style)
			{
			case DebugUtil.Style.Wireframe:
				mesh = PrimitiveMeshFactory.ConeWireframe(numSegments);
				break;
			case DebugUtil.Style.SolidColor:
				mesh = PrimitiveMeshFactory.ConeSolidColor(numSegments);
				break;
			case DebugUtil.Style.FlatShaded:
				mesh = PrimitiveMeshFactory.ConeFlatShaded(numSegments);
				break;
			case DebugUtil.Style.SmoothShaded:
				mesh = PrimitiveMeshFactory.ConeSmoothShaded(numSegments);
				break;
			}
			if (mesh == null)
			{
				return;
			}
			Material material = DebugUtil.GetMaterial(style, depthTest, false);
			MaterialPropertyBlock materialPropertyBlock = DebugUtil.GetMaterialPropertyBlock();
			materialPropertyBlock.SetColor("_Color", color);
			materialPropertyBlock.SetVector("_Dimensions", new Vector4(radius, height, radius, 0f));
			materialPropertyBlock.SetFloat("_ZBias", (style == DebugUtil.Style.Wireframe) ? DebugUtil.s_wireframeZBias : 0f);
			Graphics.DrawMesh(mesh, baseCenter, rotation, material, 0, null, 0, materialPropertyBlock, false, false, false);
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x000842BC File Offset: 0x000824BC
		public static void DrawCone(Vector3 baseCenter, Vector3 top, float radius, int numSegments, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			Vector3 vector = top - baseCenter;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Mathf.Abs(Vector3.Dot(vector, Vector3.up)) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			DebugUtil.DrawCone(baseCenter, rotation, magnitude, radius, numSegments, color, depthTest, style);
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x00084330 File Offset: 0x00082530
		public static void DrawArrow(Vector3 from, Vector3 to, float coneRadius, float coneHeight, int numSegments, float stemThickness, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			Vector3 vector = to - from;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return;
			}
			vector.Normalize();
			Quaternion rotation = Quaternion.LookRotation(Vector3.Normalize(Vector3.Cross((Mathf.Abs(Vector3.Dot(vector, Vector3.up)) < 0.5f) ? Vector3.up : Vector3.forward, vector)), vector);
			DebugUtil.DrawCone(to - coneHeight * vector, rotation, coneHeight, coneRadius, numSegments, color, depthTest, style);
			if (stemThickness <= 0f)
			{
				if (style == DebugUtil.Style.Wireframe)
				{
					to -= coneHeight * vector;
				}
				DebugUtil.DrawLine(from, to, color, depthTest);
				return;
			}
			if (coneHeight < magnitude)
			{
				to -= coneHeight * vector;
				DebugUtil.DrawCylinder(from, to, 0.5f * stemThickness, numSegments, color, depthTest, style);
			}
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x00084404 File Offset: 0x00082604
		public static void DrawArrow(Vector3 from, Vector3 to, float size, Color color, bool depthTest = true, DebugUtil.Style style = DebugUtil.Style.Wireframe)
		{
			DebugUtil.DrawArrow(from, to, 0.5f * size, size, 8, 0f, color, depthTest, style);
		}

		// Token: 0x04001975 RID: 6517
		private static float s_wireframeZBias = 0.0001f;

		// Token: 0x04001976 RID: 6518
		private const int kNormalFlag = 1;

		// Token: 0x04001977 RID: 6519
		private const int kCapShiftScaleFlag = 2;

		// Token: 0x04001978 RID: 6520
		private const int kDepthTestFlag = 4;

		// Token: 0x04001979 RID: 6521
		private static Dictionary<int, Material> s_materialPool;

		// Token: 0x0400197A RID: 6522
		private static MaterialPropertyBlock s_materialProperties;

		// Token: 0x02000515 RID: 1301
		public enum Style
		{
			// Token: 0x0400213A RID: 8506
			Wireframe,
			// Token: 0x0400213B RID: 8507
			SolidColor,
			// Token: 0x0400213C RID: 8508
			FlatShaded,
			// Token: 0x0400213D RID: 8509
			SmoothShaded
		}
	}
}
