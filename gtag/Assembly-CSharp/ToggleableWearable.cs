using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000075 RID: 117
public class ToggleableWearable : MonoBehaviour
{
	// Token: 0x0600024F RID: 591 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	protected void Awake()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
		if (this.ownerRig == null)
		{
			GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				this.ownerRig = componentInParent.offlineVRRig;
				this.ownerIsLocal = (this.ownerRig != null);
			}
		}
		if (this.ownerRig == null)
		{
			Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
			base.enabled = false;
			return;
		}
		foreach (Renderer renderer in this.renderers)
		{
			if (renderer == null)
			{
				Debug.LogError("TriggerToggler: Disabling because a renderer is null.");
				base.enabled = false;
				break;
			}
			renderer.enabled = this.startOn;
		}
		this.hasAudioSource = (this.audioSource != null);
		this.assignedSlotBitIndex = (int)this.assignedSlot;
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000F68C File Offset: 0x0000D88C
	protected void LateUpdate()
	{
		if (this.ownerIsLocal)
		{
			this.toggleCooldownTimer -= Time.deltaTime;
			Transform transform = base.transform;
			if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(this.triggerOffset), this.triggerRadius * transform.localScale.x, this.colliders, this.layerMask) > 0)
			{
				if (this.toggleCooldownTimer < 0f)
				{
					XRController componentInParent = this.colliders[0].GetComponentInParent<XRController>();
					if (componentInParent != null)
					{
						this.LocalToggle(componentInParent.controllerNode == XRNode.LeftHand);
					}
				}
				this.toggleCooldownTimer = 0.2f;
			}
		}
		else
		{
			bool flag = (this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0;
			if (this.isOn != flag)
			{
				this.SharedSetState(flag);
			}
		}
		this.progress = Mathf.MoveTowards(this.progress, this.isOn ? 1f : 0f, Time.deltaTime / this.animationTransitionDuration);
		Animator[] array = this.animators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(ToggleableWearable.animParam_Progress, this.progress);
		}
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000F7C0 File Offset: 0x0000D9C0
	private void LocalToggle(bool isLeftHand)
	{
		this.ownerRig.WearablePackedStates ^= 1 << this.assignedSlotBitIndex;
		this.SharedSetState((this.ownerRig.WearablePackedStates & 1 << this.assignedSlotBitIndex) != 0);
		if (GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, this.isOn ? this.turnOnVibrationDuration : this.turnOffVibrationDuration, this.isOn ? this.turnOnVibrationStrength : this.turnOffVibrationStrength);
		}
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000F850 File Offset: 0x0000DA50
	private void SharedSetState(bool state)
	{
		this.isOn = state;
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.isOn;
		}
		if (this.hasAudioSource)
		{
			this.audioSource.PlayOneShot(this.isOn ? this.toggleOnSound : this.toggleOffSound);
		}
	}

	// Token: 0x040002F6 RID: 758
	public Renderer[] renderers;

	// Token: 0x040002F7 RID: 759
	public Animator[] animators;

	// Token: 0x040002F8 RID: 760
	public float animationTransitionDuration = 1f;

	// Token: 0x040002F9 RID: 761
	[Tooltip("Whether the wearable state is toggled on by default.")]
	public bool startOn;

	// Token: 0x040002FA RID: 762
	[Tooltip("AudioSource to play toggle sounds.")]
	public AudioSource audioSource;

	// Token: 0x040002FB RID: 763
	[Tooltip("Sound to play when toggled on.")]
	public AudioClip toggleOnSound;

	// Token: 0x040002FC RID: 764
	[Tooltip("Sound to play when toggled off.")]
	public AudioClip toggleOffSound;

	// Token: 0x040002FD RID: 765
	[Tooltip("Layer to check for trigger sphere collisions.")]
	public LayerMask layerMask;

	// Token: 0x040002FE RID: 766
	[Tooltip("Radius of the trigger sphere.")]
	public float triggerRadius = 0.2f;

	// Token: 0x040002FF RID: 767
	[Tooltip("Position in local space to move the trigger sphere.")]
	public Vector3 triggerOffset = Vector3.zero;

	// Token: 0x04000300 RID: 768
	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	public VRRig.WearablePackedStateSlots assignedSlot;

	// Token: 0x04000301 RID: 769
	[Header("Vibration")]
	public float turnOnVibrationDuration = 0.05f;

	// Token: 0x04000302 RID: 770
	public float turnOnVibrationStrength = 0.2f;

	// Token: 0x04000303 RID: 771
	public float turnOffVibrationDuration = 0.05f;

	// Token: 0x04000304 RID: 772
	public float turnOffVibrationStrength = 0.2f;

	// Token: 0x04000305 RID: 773
	private VRRig ownerRig;

	// Token: 0x04000306 RID: 774
	private bool ownerIsLocal;

	// Token: 0x04000307 RID: 775
	private bool isOn;

	// Token: 0x04000308 RID: 776
	private const float toggleCooldown = 0.2f;

	// Token: 0x04000309 RID: 777
	private bool hasAudioSource;

	// Token: 0x0400030A RID: 778
	private readonly Collider[] colliders = new Collider[1];

	// Token: 0x0400030B RID: 779
	private int framesSinceCooldownAndExitingVolume;

	// Token: 0x0400030C RID: 780
	private float toggleCooldownTimer;

	// Token: 0x0400030D RID: 781
	private int assignedSlotBitIndex;

	// Token: 0x0400030E RID: 782
	private static readonly int animParam_Progress = Animator.StringToHash("Progress");

	// Token: 0x0400030F RID: 783
	private float progress;
}
