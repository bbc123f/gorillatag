using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000029 RID: 41
public class FingerFlagWearable : MonoBehaviour
{
	// Token: 0x060000E5 RID: 229 RVA: 0x00008F55 File Offset: 0x00007155
	protected void Awake()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00008F64 File Offset: 0x00007164
	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		if (this.pinkyRingAttachPoint != null)
		{
			this.pinkyRingAttachPoint.gameObject.SetActive(true);
		}
		if (this.thumbRingAttachPoint != null)
		{
			this.thumbRingAttachPoint.gameObject.SetActive(true);
		}
		this.OnExtendStateChanged(false);
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00008FD9 File Offset: 0x000071D9
	protected void OnDisable()
	{
		if (this.pinkyRingAttachPoint != null)
		{
			this.pinkyRingAttachPoint.gameObject.SetActive(false);
		}
		if (this.thumbRingAttachPoint != null)
		{
			this.thumbRingAttachPoint.gameObject.SetActive(false);
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x0000901C File Offset: 0x0000721C
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

	// Token: 0x060000E9 RID: 233 RVA: 0x0000909C File Offset: 0x0000729C
	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
		}
		bool flag = this.fullyRetracted;
		this.fullyRetracted = (this.extended && this.retractExtendTime <= 0f);
		if (flag != this.fullyRetracted)
		{
			Transform[] array = this.clothRigidbodies;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(!this.fullyRetracted);
			}
		}
		this.UpdateAnimation();
		this.UpdateBones();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00009130 File Offset: 0x00007330
	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00009169 File Offset: 0x00007369
	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00009186 File Offset: 0x00007386
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

	// Token: 0x060000ED RID: 237 RVA: 0x000091A4 File Offset: 0x000073A4
	private void UpdateAnimation()
	{
		float num = this.extended ? this.extendSpeed : (-this.retractSpeed);
		this.retractExtendTime = Mathf.Clamp01(this.retractExtendTime + Time.deltaTime * num);
		this.animator.SetFloat(this.retractExtendTimeAnimParam, this.retractExtendTime);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x000091FC File Offset: 0x000073FC
	private void UpdateBones()
	{
		for (int i = 0; i < this.clothBones.Length; i++)
		{
			this.clothBones[i].rotation = this.clothRigidbodies[i].rotation;
		}
		this.pinkyRingBone.SetPositionAndRotation(this.pinkyRingAttachPoint.position, this.pinkyRingAttachPoint.rotation);
		this.thumbRingBone.SetPositionAndRotation(this.thumbRingAttachPoint.position, this.thumbRingAttachPoint.rotation);
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00009278 File Offset: 0x00007478
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

	// Token: 0x04000135 RID: 309
	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	// Token: 0x04000136 RID: 310
	[Header("Bones")]
	public Transform pinkyRingBone;

	// Token: 0x04000137 RID: 311
	public Transform pinkyRingAttachPoint;

	// Token: 0x04000138 RID: 312
	public Transform thumbRingBone;

	// Token: 0x04000139 RID: 313
	public Transform thumbRingAttachPoint;

	// Token: 0x0400013A RID: 314
	public Transform[] clothBones;

	// Token: 0x0400013B RID: 315
	public Transform[] clothRigidbodies;

	// Token: 0x0400013C RID: 316
	[Header("Animation")]
	public Animator animator;

	// Token: 0x0400013D RID: 317
	public float extendSpeed = 1.5f;

	// Token: 0x0400013E RID: 318
	public float retractSpeed = 2.25f;

	// Token: 0x0400013F RID: 319
	[Header("Audio")]
	public AudioSource audioSource;

	// Token: 0x04000140 RID: 320
	public AudioClip extendAudioClip;

	// Token: 0x04000141 RID: 321
	public AudioClip retractAudioClip;

	// Token: 0x04000142 RID: 322
	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	// Token: 0x04000143 RID: 323
	public float extendVibrationStrength = 0.2f;

	// Token: 0x04000144 RID: 324
	public float retractVibrationDuration = 0.05f;

	// Token: 0x04000145 RID: 325
	public float retractVibrationStrength = 0.2f;

	// Token: 0x04000146 RID: 326
	private readonly int retractExtendTimeAnimParam = Animator.StringToHash("retractExtendTime");

	// Token: 0x04000147 RID: 327
	private bool networkedExtended;

	// Token: 0x04000148 RID: 328
	private bool extended;

	// Token: 0x04000149 RID: 329
	private bool fullyRetracted;

	// Token: 0x0400014A RID: 330
	private float retractExtendTime;

	// Token: 0x0400014B RID: 331
	private InputDevice inputDevice;

	// Token: 0x0400014C RID: 332
	private VRRig myRig;

	// Token: 0x0400014D RID: 333
	private int stateBitIndex;
}
