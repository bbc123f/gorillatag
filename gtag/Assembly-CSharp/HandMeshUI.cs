﻿using System;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class HandMeshUI : MonoBehaviour
{
	// Token: 0x060003E6 RID: 998 RVA: 0x000192F4 File Offset: 0x000174F4
	private void Start()
	{
		this.SetSliderValue(0, (float)this.rightMask.radialDivisions, false);
		this.SetSliderValue(1, this.rightMask.borderSize, false);
		this.SetSliderValue(2, this.rightMask.fingerTaper, false);
		this.SetSliderValue(3, this.rightMask.fingerTipLength, false);
		this.SetSliderValue(4, this.rightMask.webOffset, false);
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00019364 File Offset: 0x00017564
	private void Update()
	{
		this.CheckForHands();
		Vector3 position = this.rightHand.Bones[20].Transform.position;
		Vector3 position2 = this.leftHand.Bones[20].Transform.position;
		if (this.rightHeldKnob >= 0)
		{
			Vector3 vector = this.knobs[this.rightHeldKnob].transform.parent.InverseTransformPoint(position);
			this.SetSliderValue(this.rightHeldKnob, Mathf.Clamp01(vector.x * 10f), true);
			if (vector.z < -0.02f)
			{
				this.rightHeldKnob = -1;
			}
		}
		else
		{
			for (int i = 0; i < this.knobs.Length; i++)
			{
				if (Vector3.Distance(position, this.knobs[i].transform.position) <= 0.02f && this.leftHeldKnob != i)
				{
					this.rightHeldKnob = i;
					break;
				}
			}
		}
		if (this.leftHeldKnob >= 0)
		{
			Vector3 vector2 = this.knobs[this.leftHeldKnob].transform.parent.InverseTransformPoint(position2);
			this.SetSliderValue(this.leftHeldKnob, Mathf.Clamp01(vector2.x * 10f), true);
			if (vector2.z < -0.02f)
			{
				this.leftHeldKnob = -1;
				return;
			}
		}
		else
		{
			for (int j = 0; j < this.knobs.Length; j++)
			{
				if (Vector3.Distance(position2, this.knobs[j].transform.position) <= 0.02f && this.rightHeldKnob != j)
				{
					this.leftHeldKnob = j;
					return;
				}
			}
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x000194F8 File Offset: 0x000176F8
	private void SetSliderValue(int sliderID, float value, bool isNormalized)
	{
		float num = 0f;
		float num2 = 1f;
		float d = 0.1f;
		string format = "";
		switch (sliderID)
		{
		case 0:
			num = 2f;
			num2 = 16f;
			format = "{0, 0:0}";
			break;
		case 1:
			num = 0f;
			num2 = 0.05f;
			format = "{0, 0:0.000}";
			break;
		case 2:
			num = 0f;
			num2 = 0.3333f;
			format = "{0, 0:0.00}";
			break;
		case 3:
			num = 0.5f;
			num2 = 1.5f;
			format = "{0, 0:0.00}";
			break;
		case 4:
			num = 0f;
			num2 = 1f;
			format = "{0, 0:0.00}";
			break;
		}
		float num3 = isNormalized ? (value * (num2 - num) + num) : value;
		float d2 = isNormalized ? value : ((value - num) / (num2 - num));
		this.knobs[sliderID].transform.localPosition = Vector3.right * d2 * d;
		this.readouts[sliderID].text = string.Format(format, num3);
		switch (sliderID)
		{
		case 0:
			this.rightMask.radialDivisions = (int)num3;
			this.leftMask.radialDivisions = (int)num3;
			return;
		case 1:
			this.rightMask.borderSize = num3;
			this.leftMask.borderSize = num3;
			return;
		case 2:
			this.rightMask.fingerTaper = num3;
			this.leftMask.fingerTaper = num3;
			return;
		case 3:
			this.rightMask.fingerTipLength = num3;
			this.leftMask.fingerTipLength = num3;
			return;
		case 4:
			this.rightMask.webOffset = num3;
			this.leftMask.webOffset = num3;
			return;
		default:
			return;
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x0001969C File Offset: 0x0001789C
	private void CheckForHands()
	{
		bool flag = OVRInput.GetActiveController() == OVRInput.Controller.Hands || OVRInput.GetActiveController() == OVRInput.Controller.LHand || OVRInput.GetActiveController() == OVRInput.Controller.RHand;
		if (base.transform.GetChild(0).gameObject.activeSelf)
		{
			if (!flag)
			{
				base.transform.GetChild(0).gameObject.SetActive(false);
				this.leftHeldKnob = -1;
				this.rightHeldKnob = -1;
				return;
			}
		}
		else if (flag)
		{
			base.transform.GetChild(0).gameObject.SetActive(true);
			base.transform.position = (this.rightHand.Bones[20].Transform.position + this.rightHand.Bones[20].Transform.position) * 0.5f;
			base.transform.position += (base.transform.position - Camera.main.transform.position).normalized * 0.1f;
			base.transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z));
		}
	}

	// Token: 0x0400048B RID: 1163
	public SphereCollider[] knobs;

	// Token: 0x0400048C RID: 1164
	public TextMesh[] readouts;

	// Token: 0x0400048D RID: 1165
	private int rightHeldKnob = -1;

	// Token: 0x0400048E RID: 1166
	private int leftHeldKnob = -1;

	// Token: 0x0400048F RID: 1167
	public OVRSkeleton leftHand;

	// Token: 0x04000490 RID: 1168
	public OVRSkeleton rightHand;

	// Token: 0x04000491 RID: 1169
	public HandMeshMask leftMask;

	// Token: 0x04000492 RID: 1170
	public HandMeshMask rightMask;
}
