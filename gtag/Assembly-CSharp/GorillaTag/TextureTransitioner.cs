using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000316 RID: 790
	[ExecuteAlways]
	public class TextureTransitioner : MonoBehaviour, IResettableItem
	{
		// Token: 0x060015C7 RID: 5575 RVA: 0x000782B1 File Offset: 0x000764B1
		protected void Awake()
		{
			if (Application.isPlaying || this.editorPreview)
			{
				TextureTransitionerManager.EnsureInstanceIsAvailable();
			}
			this.RefreshShaderParams();
			this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
			this.ResetToDefaultState();
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x000782E4 File Offset: 0x000764E4
		protected void OnEnable()
		{
			TextureTransitionerManager.Register(this);
			if (Application.isPlaying && !this.remapInfo.IsValid())
			{
				Debug.LogError("Bad min/max values for remapRanges: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.textures.Length == 0)
			{
				Debug.LogError("Textures array is empty: " + this.GetComponentPath(int.MaxValue), this);
				base.enabled = false;
			}
			if (Application.isPlaying && this.iDynamicFloat == null)
			{
				if (this.dynamicFloatComponent == null)
				{
					Debug.LogError("dynamicFloatComponent cannot be null: " + this.GetComponentPath(int.MaxValue), this);
				}
				this.iDynamicFloat = (IDynamicFloat)this.dynamicFloatComponent;
				if (this.iDynamicFloat == null)
				{
					Debug.LogError("Component assigned to dynamicFloatComponent does not implement IDynamicFloat: " + this.GetComponentPath(int.MaxValue), this);
					base.enabled = false;
				}
			}
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x000783D2 File Offset: 0x000765D2
		protected void OnDisable()
		{
			TextureTransitionerManager.Unregister(this);
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x000783DA File Offset: 0x000765DA
		private void RefreshShaderParams()
		{
			this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
			this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
			this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x0007840F File Offset: 0x0007660F
		public void ResetToDefaultState()
		{
			this.wasReset = true;
			this.normalizedValue = 0f;
			this.transitionPercent = 0;
			this.tex1Index = 0;
			this.tex2Index = 0;
		}

		// Token: 0x040017C8 RID: 6088
		public bool editorPreview;

		// Token: 0x040017C9 RID: 6089
		[Tooltip("The component that will drive the texture transitions.")]
		public MonoBehaviour dynamicFloatComponent;

		// Token: 0x040017CA RID: 6090
		[Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
		public GorillaMath.RemapFloatInfo remapInfo;

		// Token: 0x040017CB RID: 6091
		public TextureTransitioner.DirectionRetentionMode directionRetentionMode;

		// Token: 0x040017CC RID: 6092
		public string texTransitionShaderParamName = "_TexTransition";

		// Token: 0x040017CD RID: 6093
		public string tex1ShaderParamName = "_MainTex";

		// Token: 0x040017CE RID: 6094
		public string tex2ShaderParamName = "_Tex2";

		// Token: 0x040017CF RID: 6095
		public Texture[] textures;

		// Token: 0x040017D0 RID: 6096
		public Renderer[] renderers;

		// Token: 0x040017D1 RID: 6097
		[NonSerialized]
		private bool wasReset;

		// Token: 0x040017D2 RID: 6098
		[NonSerialized]
		public IDynamicFloat iDynamicFloat;

		// Token: 0x040017D3 RID: 6099
		[NonSerialized]
		public int texTransitionShaderParam;

		// Token: 0x040017D4 RID: 6100
		[NonSerialized]
		public int tex1ShaderParam;

		// Token: 0x040017D5 RID: 6101
		[NonSerialized]
		public int tex2ShaderParam;

		// Token: 0x040017D6 RID: 6102
		[DebugReadout]
		[NonSerialized]
		public float normalizedValue;

		// Token: 0x040017D7 RID: 6103
		[DebugReadout]
		[NonSerialized]
		public int transitionPercent;

		// Token: 0x040017D8 RID: 6104
		[DebugReadout]
		[NonSerialized]
		public int tex1Index;

		// Token: 0x040017D9 RID: 6105
		[DebugReadout]
		[NonSerialized]
		public int tex2Index;

		// Token: 0x02000501 RID: 1281
		public enum DirectionRetentionMode
		{
			// Token: 0x040020D9 RID: 8409
			None,
			// Token: 0x040020DA RID: 8410
			IncreaseOnly,
			// Token: 0x040020DB RID: 8411
			DecreaseOnly
		}
	}
}
