using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class MagicRingCosmetic : MonoBehaviour
{
	// Token: 0x0600013D RID: 317 RVA: 0x0000ABC7 File Offset: 0x00008DC7
	protected void Awake()
	{
		this.materialPropertyBlock = new MaterialPropertyBlock();
		this.defaultEmissiveColor = this.ringRenderer.sharedMaterial.GetColor(this.emissionColorShaderPropID);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0000ABF0 File Offset: 0x00008DF0
	protected void LateUpdate()
	{
		float celsius = this.thermalReceiver.celsius;
		if (celsius >= this.fadeInTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedIn)
		{
			this.fadeInSounds.Play(null, null);
			this.fadeState = MagicRingCosmetic.FadeState.FadedIn;
		}
		else if (celsius <= this.fadeOutTemperatureThreshold && this.fadeState != MagicRingCosmetic.FadeState.FadedOut)
		{
			this.fadeOutSounds.Play(null, null);
			this.fadeState = MagicRingCosmetic.FadeState.FadedOut;
		}
		this.emissiveAmount = Mathf.MoveTowards(this.emissiveAmount, (this.fadeState == MagicRingCosmetic.FadeState.FadedIn) ? 1f : 0f, Time.deltaTime / this.fadeTime);
		this.ringRenderer.GetPropertyBlock(this.materialPropertyBlock);
		this.materialPropertyBlock.SetColor(this.emissionColorShaderPropID, new Color(this.defaultEmissiveColor.r, this.defaultEmissiveColor.g, this.defaultEmissiveColor.b, this.emissiveAmount));
		this.ringRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	// Token: 0x040001B2 RID: 434
	[Tooltip("The ring will fade in the emissive texture based on temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x040001B3 RID: 435
	public Renderer ringRenderer;

	// Token: 0x040001B4 RID: 436
	public float fadeInTemperatureThreshold = 200f;

	// Token: 0x040001B5 RID: 437
	public float fadeOutTemperatureThreshold = 190f;

	// Token: 0x040001B6 RID: 438
	public float fadeTime = 1.5f;

	// Token: 0x040001B7 RID: 439
	public SoundBankPlayer fadeInSounds;

	// Token: 0x040001B8 RID: 440
	public SoundBankPlayer fadeOutSounds;

	// Token: 0x040001B9 RID: 441
	private MagicRingCosmetic.FadeState fadeState;

	// Token: 0x040001BA RID: 442
	private int emissionColorShaderPropID = Shader.PropertyToID("_EmissionColor");

	// Token: 0x040001BB RID: 443
	private Color defaultEmissiveColor;

	// Token: 0x040001BC RID: 444
	private float emissiveAmount;

	// Token: 0x040001BD RID: 445
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x02000390 RID: 912
	private enum FadeState
	{
		// Token: 0x04001B13 RID: 6931
		FadedOut,
		// Token: 0x04001B14 RID: 6932
		FadedIn
	}
}
