using System;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class MagicCauldronLiquid : MonoBehaviour
{
	// Token: 0x06000A5E RID: 2654 RVA: 0x00040C31 File Offset: 0x0003EE31
	private void Test()
	{
		this._animProgress = 0f;
		this._animating = true;
		base.enabled = true;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00040C4C File Offset: 0x0003EE4C
	public void AnimateColorFromTo(Color a, Color b, float length = 1f)
	{
		this._colorStart = a;
		this._colorEnd = b;
		this._animProgress = 0f;
		this._animating = true;
		this.animLength = length;
		base.enabled = true;
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00040C7C File Offset: 0x0003EE7C
	private void ApplyColor(Color color)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetColor("_BaseColor", color);
		this._applyMaterial.Apply();
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00040CA8 File Offset: 0x0003EEA8
	private void ApplyWaveParams(float amplitude, float frequency, float scale, float rotation)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetFloat("_WaveAmplitude", amplitude);
		this._applyMaterial.SetFloat("_WaveFrequency", frequency);
		this._applyMaterial.SetFloat("_WaveScale", scale);
		this._applyMaterial.Apply();
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00040D01 File Offset: 0x0003EF01
	private void OnEnable()
	{
		if (this._applyMaterial)
		{
			this._applyMaterial.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00040D1C File Offset: 0x0003EF1C
	private void OnDisable()
	{
		this._animating = false;
		this._animProgress = 0f;
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x00040D30 File Offset: 0x0003EF30
	private void Update()
	{
		if (!this._animating)
		{
			return;
		}
		float num = this._animationCurve.Evaluate(this._animProgress / this.animLength);
		float t = this._waveCurve.Evaluate(this._animProgress / this.animLength);
		if (num >= 1f)
		{
			this.ApplyColor(this._colorEnd);
			this._animating = false;
			base.enabled = false;
			return;
		}
		Color color = Color.Lerp(this._colorStart, this._colorEnd, num);
		Mathf.Lerp(this.waveNormal.frequency, this.waveAnimating.frequency, t);
		Mathf.Lerp(this.waveNormal.amplitude, this.waveAnimating.amplitude, t);
		Mathf.Lerp(this.waveNormal.scale, this.waveAnimating.scale, t);
		Mathf.Lerp(this.waveNormal.rotation, this.waveAnimating.rotation, t);
		this.ApplyColor(color);
		this._animProgress += Time.deltaTime;
	}

	// Token: 0x04000D1A RID: 3354
	[SerializeField]
	private ApplyMaterialProperty _applyMaterial;

	// Token: 0x04000D1B RID: 3355
	[SerializeField]
	private Color _colorStart;

	// Token: 0x04000D1C RID: 3356
	[SerializeField]
	private Color _colorEnd;

	// Token: 0x04000D1D RID: 3357
	[SerializeField]
	private bool _animating;

	// Token: 0x04000D1E RID: 3358
	[SerializeField]
	private float _animProgress;

	// Token: 0x04000D1F RID: 3359
	[SerializeField]
	private AnimationCurve _animationCurve = AnimationCurves.EaseOutCubic;

	// Token: 0x04000D20 RID: 3360
	[SerializeField]
	private AnimationCurve _waveCurve = AnimationCurves.EaseInElastic;

	// Token: 0x04000D21 RID: 3361
	public float animLength = 1f;

	// Token: 0x04000D22 RID: 3362
	public MagicCauldronLiquid.WaveParams waveNormal;

	// Token: 0x04000D23 RID: 3363
	public MagicCauldronLiquid.WaveParams waveAnimating;

	// Token: 0x02000440 RID: 1088
	[Serializable]
	public struct WaveParams
	{
		// Token: 0x04001D9A RID: 7578
		public float amplitude;

		// Token: 0x04001D9B RID: 7579
		public float frequency;

		// Token: 0x04001D9C RID: 7580
		public float scale;

		// Token: 0x04001D9D RID: 7581
		public float rotation;
	}
}
