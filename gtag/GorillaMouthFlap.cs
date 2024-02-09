using System;
using UnityEngine;

public class GorillaMouthFlap : MonoBehaviour
{
	private void Update()
	{
		GorillaSpeakerLoudness component = base.GetComponent<GorillaSpeakerLoudness>();
		if (component == null)
		{
			return;
		}
		float num = 0f;
		if (component.IsSpeaking)
		{
			num = component.Louddess;
		}
		this.CheckMouthflapChange(num);
		this.UpdateMouthFlapFlipbook();
	}

	private void CheckMouthflapChange(float currentLoudness)
	{
		int num = this.mouthFlapLevels.Length - 1;
		while (num >= 0 && currentLoudness < this.mouthFlapLevels[num].maxRequiredVolume)
		{
			if (currentLoudness > this.mouthFlapLevels[num].minRequiredVolume)
			{
				if (this.activeFlipbookIndex != num)
				{
					this.activeFlipbookIndex = num;
					this.activeFlipbookPlayTime = 0f;
					return;
				}
				break;
			}
			else
			{
				num--;
			}
		}
	}

	private void UpdateMouthFlapFlipbook()
	{
		Material material = this.targetFace.GetComponent<Renderer>().material;
		this.activeFlipbookPlayTime += Time.deltaTime;
		this.activeFlipbookPlayTime %= this.mouthFlapLevels[this.activeFlipbookIndex].cycleDuration;
		int num = Mathf.FloorToInt(this.activeFlipbookPlayTime * (float)this.mouthFlapLevels[this.activeFlipbookIndex].faces.Length / this.mouthFlapLevels[this.activeFlipbookIndex].cycleDuration);
		material.SetTextureOffset(this._MouthMap, this.mouthFlapLevels[this.activeFlipbookIndex].faces[num]);
	}

	public GameObject targetFace;

	public MouthFlapLevel[] mouthFlapLevels;

	private int activeFlipbookIndex;

	private float activeFlipbookPlayTime;

	private ShaderHashId _MouthMap = "_MouthMap";
}
