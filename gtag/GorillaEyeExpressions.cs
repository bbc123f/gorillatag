using System;
using UnityEngine;

public class GorillaEyeExpressions : MonoBehaviour
{
	private void Start()
	{
		this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
	}

	private void Update()
	{
		this.CheckEyeEffects();
		this.UpdateEyeExpression();
	}

	private void CheckEyeEffects()
	{
		if (this.loudness.IsSpeaking && this.loudness.Loudness > this.screamVolume)
		{
			this.IsEyeExpressionOverriden = true;
			this.overrideDuration = this.screamDuration;
			this.overrideUV = this.ScreamUV;
			return;
		}
		if (this.IsEyeExpressionOverriden)
		{
			this.overrideDuration -= Time.deltaTime;
			if (this.overrideDuration < 0f)
			{
				this.IsEyeExpressionOverriden = false;
			}
		}
	}

	private void UpdateEyeExpression()
	{
		Material material = this.targetFace.GetComponent<Renderer>().material;
		material.SetFloat(this._EyeOverrideUV, this.IsEyeExpressionOverriden ? 1f : 0f);
		material.SetVector(this._EyeOverrideUVTransform, new Vector4(1f, 1f, this.overrideUV.x, this.overrideUV.y));
	}

	public GorillaEyeExpressions()
	{
	}

	public GameObject targetFace;

	[Space]
	[SerializeField]
	private float screamVolume = 0.2f;

	[SerializeField]
	private float screamDuration = 0.5f;

	[SerializeField]
	private Vector2 ScreamUV = new Vector2(0.8f, 0f);

	private GorillaSpeakerLoudness loudness;

	private bool IsEyeExpressionOverriden;

	private float overrideDuration;

	private Vector2 overrideUV;

	private ShaderHashId _EyeOverrideUV = "_EyeOverrideUV";

	private ShaderHashId _EyeOverrideUVTransform = "_EyeOverrideUVTransform";
}
