using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

public class FingerTorch : MonoBehaviour
{
	protected void Awake()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	protected void OnEnable()
	{
		int num = this.attachedToLeftHand ? 1 : 2;
		this.stateBitIndex = VRRig.WearablePackedStatesBitWriteInfos[num].index;
		this.OnExtendStateChanged(false);
	}

	protected void OnDisable()
	{
	}

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

	[Header("Audio")]
	public AudioSource audioSource;

	public AudioClip extendAudioClip;

	public AudioClip retractAudioClip;

	[Header("Vibration")]
	public float extendVibrationDuration = 0.05f;

	public float extendVibrationStrength = 0.2f;

	public float retractVibrationDuration = 0.05f;

	public float retractVibrationStrength = 0.2f;

	[Header("Particle FX")]
	public GameObject particleFX;

	private bool networkedExtended;

	private bool extended;

	private bool fullyRetracted;

	private float retractExtendTime;

	private InputDevice inputDevice;

	private VRRig myRig;

	private int stateBitIndex;
}
