using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200003F RID: 63
public class FingerTorch : MonoBehaviour
{
	// Token: 0x06000160 RID: 352 RVA: 0x0000B8BD File Offset: 0x00009ABD
	protected void Awake()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000B8CC File Offset: 0x00009ACC
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000B903 File Offset: 0x00009B03
	protected void OnDisable()
	{
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000B908 File Offset: 0x00009B08
	private void UpdateLocal()
	{
		int node = this.attachedToLeftHand ? 4 : 5;
		bool flag = ControllerInputPoller.GripFloat((XRNode)node) > 0.25f;
		bool flag2 = ControllerInputPoller.PrimaryButtonPress((XRNode)node);
		bool flag3 = ControllerInputPoller.SecondaryButtonPress((XRNode)node);
		bool flag4 = flag && (flag2 || flag3);
		this.networkedExtended = flag4;
		if (PhotonNetwork.InRoom && this.myRig)
		{
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.networkedExtended);
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000B988 File Offset: 0x00009B88
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
			this.particleFX.SetActive(this.extended);
		}
		this.pinkyRingBone.SetPositionAndRotation(this.pinkyRingAttachPoint.position, this.pinkyRingAttachPoint.rotation);
		this.thumbRingBone.SetPositionAndRotation(this.thumbRingAttachPoint.position, this.thumbRingAttachPoint.rotation);
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0000BA09 File Offset: 0x00009C09
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000BA42 File Offset: 0x00009C42
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000BA5F File Offset: 0x00009C5F
	protected void LateUpdate()
	{
		if (this.IsMyItem())
		{
			this.UpdateLocal();
		}
		else
		{
			this.UpdateReplicated();
		}
		this.UpdateShared();
	}

	// Token: 0x06000168 RID: 360 RVA: 0x0000BA80 File Offset: 0x00009C80
	private void OnExtendStateChanged(bool playAudio)
	{
		this.audioSource.clip = (this.extended ? this.extendAudioClip : this.retractAudioClip);
		if (playAudio)
		{
			this.audioSource.Play();
		}
		if (this.IsMyItem() && GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(this.attachedToLeftHand, this.extended ? this.extendVibrationDuration : this.retractVibrationDuration, this.extended ? this.extendVibrationStrength : this.retractVibrationStrength);
		}
	}

	// Token: 0x040001F4 RID: 500
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x040001F5 RID: 501
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x040001F6 RID: 502
	public Transform pinkyRingAttachPoint;

	// Token: 0x040001F7 RID: 503
	public Transform thumbRingBone;

	// Token: 0x040001F8 RID: 504
	public Transform thumbRingAttachPoint;

	// Token: 0x040001F9 RID: 505
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x040001FA RID: 506
	public AudioClip extendAudioClip;

	// Token: 0x040001FB RID: 507
	public AudioClip retractAudioClip;

	// Token: 0x040001FC RID: 508
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x040001FD RID: 509
	public float extendVibrationStrength = 0.2f;

	// Token: 0x040001FE RID: 510
	public float retractVibrationDuration = 0.05f;

	// Token: 0x040001FF RID: 511
	public float retractVibrationStrength = 0.2f;

	// Token: 0x04000200 RID: 512
	[Header("Particle FX")]
	public GameObject particleFX;

	// Token: 0x04000201 RID: 513
	private bool networkedExtended;

	// Token: 0x04000202 RID: 514
	private bool extended;

	// Token: 0x04000203 RID: 515
	private bool fullyRetracted;

	// Token: 0x04000204 RID: 516
	private float retractExtendTime;

	// Token: 0x04000205 RID: 517
	private InputDevice inputDevice;

	// Token: 0x04000206 RID: 518
	private VRRig myRig;

	// Token: 0x04000207 RID: 519
	private int stateBitIndex;
}
