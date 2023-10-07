﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000044 RID: 68
public class ApplyMaterialProperty : MonoBehaviour
{
	// Token: 0x0600016E RID: 366 RVA: 0x0000BB88 File Offset: 0x00009D88
	private void Start()
	{
		if (this.applyOnStart)
		{
			this.Apply();
		}
	}

	// Token: 0x0600016F RID: 367 RVA: 0x0000BB98 File Offset: 0x00009D98
	public void Apply()
	{
		if (!this._renderer)
		{
			this._renderer = base.GetComponent<Renderer>();
		}
		ApplyMaterialProperty.ApplyMode applyMode = this.mode;
		if (applyMode == ApplyMaterialProperty.ApplyMode.MaterialInstance)
		{
			this.ApplyMaterialInstance();
			return;
		}
		if (applyMode != ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock)
		{
			return;
		}
		this.ApplyMaterialPropertyBlock();
	}

	// Token: 0x06000170 RID: 368 RVA: 0x0000BBDA File Offset: 0x00009DDA
	public void SetColor(string propertyName, Color color)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyName);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Color;
		orCreateData.color = color;
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000BBF0 File Offset: 0x00009DF0
	public void SetFloat(string propertyName, float value)
	{
		ApplyMaterialProperty.CustomMaterialData orCreateData = this.GetOrCreateData(propertyName);
		orCreateData.dataType = ApplyMaterialProperty.SuportedTypes.Float;
		orCreateData.@float = value;
	}

	// Token: 0x06000172 RID: 370 RVA: 0x0000BC08 File Offset: 0x00009E08
	private ApplyMaterialProperty.CustomMaterialData GetOrCreateData(string propertyName)
	{
		for (int i = 0; i < this.customData.Count; i++)
		{
			ApplyMaterialProperty.CustomMaterialData customMaterialData = this.customData[i];
			if (customMaterialData.name == propertyName)
			{
				return customMaterialData;
			}
		}
		ApplyMaterialProperty.CustomMaterialData customMaterialData2 = new ApplyMaterialProperty.CustomMaterialData
		{
			name = propertyName
		};
		this.customData.Add(customMaterialData2);
		return customMaterialData2;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x0000BC64 File Offset: 0x00009E64
	private void ApplyMaterialInstance()
	{
		if (!this._instance)
		{
			this._instance = base.GetComponent<MaterialInstance>();
			if (this._instance == null)
			{
				this._instance = base.gameObject.AddComponent<MaterialInstance>();
			}
		}
		Material material = this.targetMaterial = this._instance.Material;
		for (int i = 0; i < this.customData.Count; i++)
		{
			ApplyMaterialProperty.CustomMaterialData customMaterialData = this.customData[i];
			switch (customMaterialData.dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				material.SetColor(customMaterialData.name, customMaterialData.color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				material.SetFloat(customMaterialData.name, customMaterialData.@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				material.SetVector(customMaterialData.name, customMaterialData.vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				material.SetVector(customMaterialData.name, customMaterialData.vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				material.SetVector(customMaterialData.name, customMaterialData.vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				material.SetTexture(customMaterialData.name, customMaterialData.texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x06000174 RID: 372 RVA: 0x0000BDA4 File Offset: 0x00009FA4
	private void ApplyMaterialPropertyBlock()
	{
		if (this._block == null)
		{
			this._block = new MaterialPropertyBlock();
		}
		this._renderer.GetPropertyBlock(this._block);
		for (int i = 0; i < this.customData.Count; i++)
		{
			ApplyMaterialProperty.CustomMaterialData customMaterialData = this.customData[i];
			switch (customMaterialData.dataType)
			{
			case ApplyMaterialProperty.SuportedTypes.Color:
				this._block.SetColor(customMaterialData.name, customMaterialData.color);
				break;
			case ApplyMaterialProperty.SuportedTypes.Float:
				this._block.SetFloat(customMaterialData.name, customMaterialData.@float);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector2:
				this._block.SetVector(customMaterialData.name, customMaterialData.vector2);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector3:
				this._block.SetVector(customMaterialData.name, customMaterialData.vector3);
				break;
			case ApplyMaterialProperty.SuportedTypes.Vector4:
				this._block.SetVector(customMaterialData.name, customMaterialData.vector4);
				break;
			case ApplyMaterialProperty.SuportedTypes.Texture2D:
				this._block.SetTexture(customMaterialData.name, customMaterialData.texture2D);
				break;
			}
		}
		this._renderer.SetPropertyBlock(this._block);
	}

	// Token: 0x04000212 RID: 530
	public ApplyMaterialProperty.ApplyMode mode;

	// Token: 0x04000213 RID: 531
	[FormerlySerializedAs("materialToApplyBlock")]
	public Material targetMaterial;

	// Token: 0x04000214 RID: 532
	[SerializeField]
	private MaterialInstance _instance;

	// Token: 0x04000215 RID: 533
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04000216 RID: 534
	public List<ApplyMaterialProperty.CustomMaterialData> customData;

	// Token: 0x04000217 RID: 535
	[SerializeField]
	private bool applyOnStart;

	// Token: 0x04000218 RID: 536
	[NonSerialized]
	private MaterialPropertyBlock _block;

	// Token: 0x02000393 RID: 915
	public enum ApplyMode
	{
		// Token: 0x04001B1B RID: 6939
		MaterialInstance,
		// Token: 0x04001B1C RID: 6940
		MaterialPropertyBlock
	}

	// Token: 0x02000394 RID: 916
	public enum SuportedTypes
	{
		// Token: 0x04001B1E RID: 6942
		Color,
		// Token: 0x04001B1F RID: 6943
		Float,
		// Token: 0x04001B20 RID: 6944
		Vector2,
		// Token: 0x04001B21 RID: 6945
		Vector3,
		// Token: 0x04001B22 RID: 6946
		Vector4,
		// Token: 0x04001B23 RID: 6947
		Texture2D
	}

	// Token: 0x02000395 RID: 917
	[Serializable]
	public class CustomMaterialData
	{
		// Token: 0x06001AC7 RID: 6855 RVA: 0x00094718 File Offset: 0x00092918
		public override int GetHashCode()
		{
			return new ValueTuple<string, ApplyMaterialProperty.SuportedTypes, Color, float, Vector2, Vector3, Vector4, ValueTuple<Texture2D>>(this.name, this.dataType, this.color, this.@float, this.vector2, this.vector3, this.vector4, new ValueTuple<Texture2D>(this.texture2D)).GetHashCode();
		}

		// Token: 0x04001B24 RID: 6948
		public string name;

		// Token: 0x04001B25 RID: 6949
		public ApplyMaterialProperty.SuportedTypes dataType;

		// Token: 0x04001B26 RID: 6950
		public Color color;

		// Token: 0x04001B27 RID: 6951
		public float @float;

		// Token: 0x04001B28 RID: 6952
		public Vector2 vector2;

		// Token: 0x04001B29 RID: 6953
		public Vector3 vector3;

		// Token: 0x04001B2A RID: 6954
		public Vector4 vector4;

		// Token: 0x04001B2B RID: 6955
		public Texture2D texture2D;
	}
}
