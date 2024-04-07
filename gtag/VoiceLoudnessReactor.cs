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
	}

	private void OnEnable()
	{
		if (this.loudness != null)
		{
			return;
		}
		this.loudness = base.GetComponentInParent<GorillaSpeakerLoudness>(true);
	}

	private void Update()
	{
		if (this.loudness == null)
		{
			return;
		}
		for (int i = 0; i < this.blendShapeTargets.Length; i++)
		{
			this.blendShapeTargets[i].SkinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeTargets[i].BlendShapeIndex, Mathf.Lerp(this.blendShapeTargets[i].minValue, this.blendShapeTargets[i].maxValue, this.blendShapeTargets[i].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness));
		}
		for (int j = 0; j < this.transformPositionTargets.Length; j++)
		{
			this.transformPositionTargets[j].transform.localPosition = Vector3.Lerp(this.transformPositionTargets[j].Initial, this.transformPositionTargets[j].Max, (this.transformPositionTargets[j].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness) * this.transformPositionTargets[j].Scale);
		}
		for (int k = 0; k < this.transformScaleTargets.Length; k++)
		{
			this.transformScaleTargets[k].transform.localScale = Vector3.Lerp(this.transformScaleTargets[k].Initial, this.transformScaleTargets[k].Max, (this.transformScaleTargets[k].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness) * this.transformScaleTargets[k].Scale);
		}
		for (int l = 0; l < this.transformRotationTargets.Length; l++)
		{
			this.transformRotationTargets[l].transform.localRotation = Quaternion.Slerp(this.transformRotationTargets[l].Initial, this.transformRotationTargets[l].Max, (this.transformRotationTargets[l].UseSmoothedLoudness ? this.loudness.SmoothedLoudness : this.loudness.Loudness) * this.transformRotationTargets[l].Scale);
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
}
