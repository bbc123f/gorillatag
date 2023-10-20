using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000318 RID: 792
	[ExecuteAlways]
	public class TextureTransitioner : MonoBehaviour, IResettableItem
	{
		// Token: 0x060015D0 RID: 5584 RVA: 0x00078799 File Offset: 0x00076999
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

		// Token: 0x060015D1 RID: 5585 RVA: 0x000787CC File Offset: 0x000769CC
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

		// Token: 0x060015D2 RID: 5586 RVA: 0x000788BA File Offset: 0x00076ABA
		protected void OnDisable()
		{
			TextureTransitionerManager.Unregister(this);
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x000788C2 File Offset: 0x00076AC2
		private void RefreshShaderParams()
		{
			this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
			this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
			this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x000788F7 File Offset: 0x00076AF7
		public void ResetToDefaultState()
		{
			this.wasReset = true;
			this.normalizedValue = 0f;
			this.transitionPercent = 0;
			this.tex1Index = 0;
			this.tex2Index = 0;
		}

		// Token: 0x040017D5 RID: 6101
		public bool editorPreview;

		// Token: 0x040017D6 RID: 6102
		[Tooltip("The component that will drive the texture transitions.")]
		public MonoBehaviour dynamicFloatComponent;

		// Token: 0x040017D7 RID: 6103
		[Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
		public GorillaMath.RemapFloatInfo remapInfo;

		// Token: 0x040017D8 RID: 6104
		public TextureTransitioner.DirectionRetentionMode directionRetentionMode;

		// Token: 0x040017D9 RID: 6105
		public string texTransitionShaderParamName = "_TexTransition";

		// Token: 0x040017DA RID: 6106
		public string tex1ShaderParamName = "_MainTex";

		// Token: 0x040017DB RID: 6107
		public string tex2ShaderParamName = "_Tex2";

		// Token: 0x040017DC RID: 6108
		public Texture[] textures;

		// Token: 0x040017DD RID: 6109
		public Renderer[] renderers;

		// Token: 0x040017DE RID: 6110
		[NonSerialized]
		private bool wasReset;

		// Token: 0x040017DF RID: 6111
		[NonSerialized]
		public IDynamicFloat iDynamicFloat;

		// Token: 0x040017E0 RID: 6112
		[NonSerialized]
		public int texTransitionShaderParam;

		// Token: 0x040017E1 RID: 6113
		[NonSerialized]
		public int tex1ShaderParam;

		// Token: 0x040017E2 RID: 6114
		[NonSerialized]
		public int tex2ShaderParam;

		// Token: 0x040017E3 RID: 6115
		[DebugReadout]
		[NonSerialized]
		public float normalizedValue;

		// Token: 0x040017E4 RID: 6116
		[DebugReadout]
		[NonSerialized]
		public int transitionPercent;

		// Token: 0x040017E5 RID: 6117
		[DebugReadout]
		[NonSerialized]
		public int tex1Index;

		// Token: 0x040017E6 RID: 6118
		[DebugReadout]
		[NonSerialized]
		public int tex2Index;

		// Token: 0x02000503 RID: 1283
		public enum DirectionRetentionMode
		{
			// Token: 0x040020E6 RID: 8422
			None,
			// Token: 0x040020E7 RID: 8423
			IncreaseOnly,
			// Token: 0x040020E8 RID: 8424
			DecreaseOnly
		}
	}
}
