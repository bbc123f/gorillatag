using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

public class FingerFlagWearable : MonoBehaviour
{
	protected void Awake()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	protected void OnEnable()
	{
		int num = (this.attachedToLeftHand ? 1 : 2);
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

	private void UpdateLocal()
	{
		int num = (this.attachedToLeftHand ? 4 : 5);
		bool flag = ControllerInputPoller.GripFloat((XRNode)num) > 0.25f;
		bool flag2 = ControllerInputPoller.PrimaryButtonPress((XRNode)num);
		bool flag3 = ControllerInputPoller.SecondaryButtonPress((XRNode)num);
		bool flag4 = flag && (flag2 || flag3);
		this.networkedExtended = flag4;
		if (PhotonNetwork.InRoom && this.myRig)
		{
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, this.stateBitIndex, this.networkedExtended);
		}
	}

	private void UpdateShared()
	{
		if (this.extended != this.networkedExtended)
		{
			this.extended = this.networkedExtended;
			this.OnExtendStateChanged(true);
		}
		bool flag = this.fullyRetracted;
		this.fullyRetracted = this.extended && this.retractExtendTime <= 0f;
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

	private void UpdateReplicated()
	{
		if (this.myRig != null && !this.myRig.isOfflineVRRig)
		{
			this.networkedExtended = GTBitOps.ReadBit(this.myRig.WearablePackedStates, this.stateBitIndex);
		}
	}

	public bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

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

	private void UpdateAnimation()
	{
		float num = (this.extended ? this.extendSpeed : (-this.retractSpeed));
		this.retractExtendTime = Mathf.Clamp01(this.retractExtendTime + Time.deltaTime * num);
		this.animator.SetFloat(this.retractExtendTimeAnimParam, this.retractExtendTime);
	}

	private void UpdateBones()
	{
		for (int i = 0; i < this.clothBones.Length; i++)
		{
			this.clothBones[i].rotation = this.clothRigidbodies[i].rotation;
		}
		this.pinkyRingBone.SetPositionAndRotation(this.pinkyRingAttachPoint.position, this.pinkyRingAttachPoint.rotation);
		this.thumbRingBone.SetPositionAndRotation(this.thumbRingAttachPoint.position, this.thumbRingAttachPoint.rotation);
	}

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

	[Header("Wearable Settings")]
	public bool attachedToLeftHand = true;

	[Header("Bones")]
	public Transform pinkyRingBone;

	public Transform pinkyRingAttachPoint;

	public Transform thumbRingBone;

	public Transform thumbRingAttachPoint;

	public Transform[] clothBones;

	public Transform[] clothRigidbodies;

	[Header("Animation")]
	public Animator animator;

	public float extendSpeed = 1.5f;

	public float retractSpeed = 2.25f;

	[Header("Audio")]
	public AudioSource audioSource;

	public AudioClip extendAudioClip;

	public AudioClip retractAudioClip;

	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	public float extendVibrationStrength = 0.2f;

	public float retractVibrationDuration = 0.05f;

	public float retractVibrationStrength = 0.2f;

	private readonly int retractExtendTimeAnimParam = Animator.StringToHash("retractExtendTime");

	private bool networkedExtended;

	private bool extended;

	private bool fullyRetracted;

	private float retractExtendTime;

	private InputDevice inputDevice;

	private VRRig myRig;

	private int stateBitIndex;
}
