using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleableWearable : MonoBehaviour
{
	public Renderer[] renderers;

	[Tooltip("Whether the wearable state is toggled on by default.")]
	public bool startOn;

	[Tooltip("AudioSource to play toggle sounds.")]
	public AudioSource audioSource;

	[Tooltip("Sound to play when toggled on.")]
	public AudioClip toggleOnSound;

	[Tooltip("Sound to play when toggled off.")]
	public AudioClip toggleOffSound;

	[Tooltip("Layer to check for trigger sphere collisions.")]
	public LayerMask layerMask;

	[Tooltip("Radius of the trigger sphere.")]
	public float triggerRadius = 0.2f;

	[Tooltip("Position in local space to move the trigger sphere.")]
	public Vector3 triggerOffset = Vector3.zero;

	[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
	public VRRig.WearablePackedStateSlots assignedSlot;

	[Header("Vibration")]
	public float turnOnVibrationDuration = 0.05f;

	public float turnOnVibrationStrength = 0.2f;

	public float turnOffVibrationDuration = 0.05f;

	public float turnOffVibrationStrength = 0.2f;

	private VRRig ownerRig;

	private bool ownerIsLocal;

	private bool isOn;

	private const float toggleCooldown = 0.2f;

	private bool hasAudioSource;

	private readonly Collider[] colliders = new Collider[1];

	private int framesSinceCooldownAndExitingVolume;

	private float toggleCooldownTimer;

	private int assignedSlotBitIndex;

	protected void Awake()
	{
		ownerRig = GetComponentInParent<VRRig>();
		if (ownerRig == null)
		{
			GorillaTagger componentInParent = GetComponentInParent<GorillaTagger>();
			if (componentInParent != null)
			{
				ownerRig = componentInParent.offlineVRRig;
				ownerIsLocal = ownerRig != null;
			}
		}
		if (ownerRig == null)
		{
			Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
			base.enabled = false;
			return;
		}
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			if (renderer == null)
			{
				Debug.LogError("TriggerToggler: Disabling because a renderer is null.");
				base.enabled = false;
				break;
			}
			renderer.enabled = startOn;
		}
		hasAudioSource = audioSource != null;
		assignedSlotBitIndex = (int)assignedSlot;
	}

	protected void LateUpdate()
	{
		if (ownerIsLocal)
		{
			toggleCooldownTimer -= Time.deltaTime;
			Transform transform = base.transform;
			if (Physics.OverlapSphereNonAlloc(transform.TransformPoint(triggerOffset), triggerRadius * transform.localScale.x, colliders, layerMask) <= 0)
			{
				return;
			}
			if (toggleCooldownTimer < 0f)
			{
				XRController componentInParent = colliders[0].GetComponentInParent<XRController>();
				if (componentInParent != null)
				{
					LocalToggle(componentInParent.controllerNode == XRNode.LeftHand);
				}
			}
			toggleCooldownTimer = 0.2f;
		}
		else
		{
			bool flag = (ownerRig.WearablePackedStates & (1 << assignedSlotBitIndex)) != 0;
			if (isOn != flag)
			{
				SharedSetState(flag);
			}
		}
	}

	private void LocalToggle(bool isLeftHand)
	{
		ownerRig.WearablePackedStates ^= 1 << assignedSlotBitIndex;
		SharedSetState((ownerRig.WearablePackedStates & (1 << assignedSlotBitIndex)) != 0);
		if ((bool)GorillaTagger.Instance)
		{
			GorillaTagger.Instance.StartVibration(isLeftHand, isOn ? turnOnVibrationDuration : turnOffVibrationDuration, isOn ? turnOnVibrationStrength : turnOffVibrationStrength);
		}
	}

	private void SharedSetState(bool state)
	{
		isOn = state;
		Renderer[] array = renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = isOn;
		}
		if (hasAudioSource)
		{
			audioSource.PlayOneShot(isOn ? toggleOnSound : toggleOffSound);
		}
	}
}
