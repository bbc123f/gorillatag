﻿using System;
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
			num = component.Loudness;
		}
		this.CheckMouthflapChange(component.IsMicEnabled, num);
		MouthFlapLevel mouthFlapLevel = this.noMicFace;
		if (this.useMicEnabled)
		{
			mouthFlapLevel = this.mouthFlapLevels[this.activeFlipbookIndex];
		}
		this.UpdateMouthFlapFlipbook(mouthFlapLevel);
	}

	private void CheckMouthflapChange(bool isMicEnabled, float currentLoudness)
	{
		if (isMicEnabled)
		{
			this.useMicEnabled = true;
			int i = this.mouthFlapLevels.Length - 1;
			while (i >= 0)
			{
				if (currentLoudness >= this.mouthFlapLevels[i].maxRequiredVolume)
				{
					return;
				}
				if (currentLoudness > this.mouthFlapLevels[i].minRequiredVolume)
				{
					if (this.activeFlipbookIndex != i)
					{
						this.activeFlipbookIndex = i;
						this.activeFlipbookPlayTime = 0f;
						return;
					}
					return;
				}
				else
				{
					i--;
				}
			}
			return;
		}
		if (this.useMicEnabled)
		{
			this.useMicEnabled = false;
			this.activeFlipbookPlayTime = 0f;
		}
	}

	private void UpdateMouthFlapFlipbook(MouthFlapLevel mouthFlap)
	{
		Material material = this.targetFace.GetComponent<Renderer>().material;
		this.activeFlipbookPlayTime += Time.deltaTime;
		this.activeFlipbookPlayTime %= mouthFlap.cycleDuration;
		int num = Mathf.FloorToInt(this.activeFlipbookPlayTime * (float)mouthFlap.faces.Length / mouthFlap.cycleDuration);
		material.SetTextureOffset(this._MouthMap, mouthFlap.faces[num]);
	}

	public GameObject targetFace;

	public MouthFlapLevel[] mouthFlapLevels;

	public MouthFlapLevel noMicFace;

	private bool useMicEnabled;

	private int activeFlipbookIndex;

	private float activeFlipbookPlayTime;

	private ShaderHashId _MouthMap = "_MouthMap";
}
