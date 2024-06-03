using System;
using UnityEngine;

public class VoiceLoudnessReactor : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.transformPositionTargets.Length; i++)
		{
			this.transformPositionTargets[i].Initial = this.transformPositionTargets[i].transform.localPosition;
		}
		for (int j = 0; j < this.transformScaleTargets.Length; j++)
		{
			this.transformScaleTargets[j].Initial = this.transformScaleTargets[j].transform.localScale;
		}
		for (int k = 0; k < this.transformRotationTargets.Length; k++)
		{
			this.transformRotationTargets[k].Initial = this.transformRotationTargets[k].transform.localRotation;
		}
		for (int l = 0; l < this.particleTargets.Length; l++)
		{
			this.particleTargets[l].Main = this.particleTargets[l].particleSystem.main;
			this.particleTargets[l].InitialSpeed = this.particleTargets[l].Main.startSpeedMultiplier;
			this.particleTargets[l].InitialSize = this.particleTargets[l].Main.startSizeMultiplier;
			this.particleTargets[l].Emission = this.particleTargets[l].particleSystem.emission;
			this.particleTargets[l].InitialRate = this.particleTargets[l].Emission.rateOverTimeMultiplier;
			this.particleTargets[l].Main.startSpeedMultiplier = 0f;
			this.particleTargets[l].Main.startSizeMultiplier = 0f;
			this.particleTargets[l].Emission.rateOverTimeMultiplier = 0f;
		}
		for (int m = 0; m < this.gameObjectEnableTargets.Length; m++)
		{
			this.gameObjectEnableTargets[m].GameObject.SetActive(!this.gameObjectEnableTargets[m].TurnOnAtThreshhold);
		}
	}

	private void OnEnable()
	{
		if (this.loudness != null)
		{
			return;
		}
		this.loudness = base.GetComponentInParent<GorillaSpeakerLoudness>(true);
		if (this.loudness == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.loudness = componentInParent.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
			}
		}
	}

	private void Update()
	{
		if (this.loudness == null)
		{
			return;
		}
		for (int i = 0; i < this.blendShapeTargets.Length; i++)
		{
			float t = this.blendShapeTargets[i].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness;
			this.blendShapeTargets[i].SkinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeTargets[i].BlendShapeIndex, Mathf.Lerp(this.blendShapeTargets[i].minValue, this.blendShapeTargets[i].maxValue, t));
		}
		for (int j = 0; j < this.transformPositionTargets.Length; j++)
		{
			float t2 = (this.transformPositionTargets[j].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness) * this.transformPositionTargets[j].Scale;
			this.transformPositionTargets[j].transform.localPosition = Vector3.Lerp(this.transformPositionTargets[j].Initial, this.transformPositionTargets[j].Max, t2);
		}
		for (int k = 0; k < this.transformScaleTargets.Length; k++)
		{
			float t3 = (this.transformScaleTargets[k].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness) * this.transformScaleTargets[k].Scale;
			this.transformScaleTargets[k].transform.localScale = Vector3.Lerp(this.transformScaleTargets[k].Initial, this.transformScaleTargets[k].Max, t3);
		}
		for (int l = 0; l < this.transformRotationTargets.Length; l++)
		{
			float t4 = (this.transformRotationTargets[l].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness) * this.transformRotationTargets[l].Scale;
			this.transformRotationTargets[l].transform.localRotation = Quaternion.Slerp(this.transformRotationTargets[l].Initial, this.transformRotationTargets[l].Max, t4);
		}
		for (int m = 0; m < this.particleTargets.Length; m++)
		{
			float time = (this.particleTargets[m].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness) * this.particleTargets[m].Scale;
			this.particleTargets[m].Main.startSpeedMultiplier = this.particleTargets[m].InitialSpeed * this.particleTargets[m].speed.Evaluate(time);
			this.particleTargets[m].Main.startSizeMultiplier = this.particleTargets[m].InitialSize * this.particleTargets[m].size.Evaluate(time);
			this.particleTargets[m].Emission.rateOverTimeMultiplier = this.particleTargets[m].InitialRate * this.particleTargets[m].rate.Evaluate(time);
		}
		for (int n = 0; n < this.gameObjectEnableTargets.Length; n++)
		{
			bool flag = (this.gameObjectEnableTargets[n].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : (this.loudness.Loudness * this.gameObjectEnableTargets[n].Scale)) >= this.gameObjectEnableTargets[n].Threshold;
			if (!this.gameObjectEnableTargets[n].TurnOnAtThreshhold)
			{
				flag = !flag;
			}
			if (this.gameObjectEnableTargets[n].GameObject.activeInHierarchy != flag)
			{
				this.gameObjectEnableTargets[n].GameObject.SetActive(flag);
			}
		}
	}

	public VoiceLoudnessReactor()
	{
	}

	private GorillaSpeakerLoudness loudness;

	[SerializeField]
	private VoiceLoudnessReactorBlendShapeTarget[] blendShapeTargets;

	[SerializeField]
	private VoiceLoudnessReactorTransformTarget[] transformPositionTargets;

	[SerializeField]
	private VoiceLoudnessReactorTransformRotationTarget[] transformRotationTargets;

	[SerializeField]
	private VoiceLoudnessReactorTransformTarget[] transformScaleTargets;

	[SerializeField]
	private VoiceLoudnessReactorParticleSystemTarget[] particleTargets;

	[SerializeField]
	private VoiceLoudnessReactorGameObjectEnableTarget[] gameObjectEnableTargets;
}
