using System;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class MagicCauldronLiquid : MonoBehaviour
{
	// Token: 0x06000A63 RID: 2659 RVA: 0x00040D69 File Offset: 0x0003EF69
	private void Test()
	{
		this._animProgress = 0f;
		this._animating = true;
		base.enabled = true;
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x00040D84 File Offset: 0x0003EF84
	public void AnimateColorFromTo(Color a, Color b, float length = 1f)
	{
		this._colorStart = a;
		this._colorEnd = b;
		this._animProgress = 0f;
		this._animating = true;
		this.animLength = length;
		base.enabled = true;
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x00040DB4 File Offset: 0x0003EFB4
	private void ApplyColor(Color color)
	{
		if (!this._applyMaterial)
		{
			return;
		}
		this._applyMaterial.SetColor("_BaseColor", color);
		this._applyMaterial.Apply();
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00040DE0 File Offset: 0x0003EFE0
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

	// Token: 0x06000A67 RID: 2663 RVA: 0x00040E39 File Offset: 0x0003F039
	private void OnEnable()
	{
		if (this._applyMaterial)
		{
			this._applyMaterial.mode = ApplyMaterialProperty.ApplyMode.MaterialPropertyBlock;
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00040E54 File Offset: 0x0003F054
	private void OnDisable()
	{
		this._animating = false;
		this._animProgress = 0f;
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x00040E68 File Offset: 0x0003F068
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

	// Token: 0x04000D1E RID: 3358
	[SerializeField]
	private ApplyMaterialProperty _applyMaterial;

	// Token: 0x04000D1F RID: 3359
	[SerializeField]
	private Color _colorStart;

	// Token: 0x04000D20 RID: 3360
	[SerializeField]
	private Color _colorEnd;

	// Token: 0x04000D21 RID: 3361
	[SerializeField]
	private bool _animating;

	// Token: 0x04000D22 RID: 3362
	[SerializeField]
	private float _animProgress;

	// Token: 0x04000D23 RID: 3363
	[SerializeField]
	private AnimationCurve _animationCurve = AnimationCurves.EaseOutCubic;

	// Token: 0x04000D24 RID: 3364
	[SerializeField]
	private AnimationCurve _waveCurve = AnimationCurves.EaseInElastic;

	// Token: 0x04000D25 RID: 3365
	public float animLength = 1f;

	// Token: 0x04000D26 RID: 3366
	public MagicCauldronLiquid.WaveParams waveNormal;

	// Token: 0x04000D27 RID: 3367
	public MagicCauldronLiquid.WaveParams waveAnimating;

	// Token: 0x02000442 RID: 1090
	[Serializable]
	public struct WaveParams
	{
		// Token: 0x04001DA7 RID: 7591
		public float amplitude;

		// Token: 0x04001DA8 RID: 7592
		public float frequency;

		// Token: 0x04001DA9 RID: 7593
		public float scale;

		// Token: 0x04001DAA RID: 7594
		public float rotation;
	}
}
