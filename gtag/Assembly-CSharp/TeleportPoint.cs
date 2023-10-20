﻿using System;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class TeleportPoint : MonoBehaviour
{
	// Token: 0x0600031B RID: 795 RVA: 0x000130D3 File Offset: 0x000112D3
	private void Start()
	{
	}

	// Token: 0x0600031C RID: 796 RVA: 0x000130D5 File Offset: 0x000112D5
	public Transform GetDestTransform()
	{
		return this.destTransform;
	}

	// Token: 0x0600031D RID: 797 RVA: 0x000130E0 File Offset: 0x000112E0
	private void Update()
	{
		float value = Mathf.SmoothStep(this.fullIntensity, this.lowIntensity, (Time.time - this.lastLookAtTime) * this.dimmingSpeed);
		base.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", value);
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00013128 File Offset: 0x00011328
	public void OnLookAt()
	{
		this.lastLookAtTime = Time.time;
	}

	// Token: 0x040003D4 RID: 980
	public float dimmingSpeed = 1f;

	// Token: 0x040003D5 RID: 981
	public float fullIntensity = 1f;

	// Token: 0x040003D6 RID: 982
	public float lowIntensity = 0.5f;

	// Token: 0x040003D7 RID: 983
	public Transform destTransform;

	// Token: 0x040003D8 RID: 984
	private float lastLookAtTime;
}