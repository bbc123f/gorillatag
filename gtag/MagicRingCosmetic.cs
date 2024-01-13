using UnityEngine;

public class MagicRingCosmetic : MonoBehaviour
{
	private enum FadeState
	{
		FadedOut,
		FadedIn
	}

	[Tooltip("The ring will fade in the emissive texture based on temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	public Renderer ringRenderer;

	public string emissiveAmountShaderProperty;

	public float fadeInTemperatureThreshold = 200f;

	public float fadeOutTemperatureThreshold = 190f;

	public float fadeTime = 1.5f;

	public SoundBankPlayer fadeInSounds;

	public SoundBankPlayer fadeOutSounds;

	private FadeState fadeState;

	private int emissiveAmountShaderPropID;

	private float emissiveAmount;

	private MaterialPropertyBlock materialPropertyBlock;

	protected void Awake()
	{
		emissiveAmountShaderPropID = Shader.PropertyToID(emissiveAmountShaderProperty);
		materialPropertyBlock = new MaterialPropertyBlock();
	}

	protected void LateUpdate()
	{
		float celsius = thermalReceiver.celsius;
		if (celsius >= fadeInTemperatureThreshold && fadeState != FadeState.FadedIn)
		{
			fadeInSounds.Play();
			fadeState = FadeState.FadedIn;
		}
		else if (celsius <= fadeOutTemperatureThreshold && fadeState != 0)
		{
			fadeOutSounds.Play();
			fadeState = FadeState.FadedOut;
		}
		emissiveAmount = Mathf.MoveTowards(emissiveAmount, (fadeState == FadeState.FadedIn) ? 1f : 0f, Time.deltaTime / fadeTime);
		ringRenderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetFloat(emissiveAmountShaderPropID, emissiveAmount);
		ringRenderer.SetPropertyBlock(materialPropertyBlock);
	}
}
