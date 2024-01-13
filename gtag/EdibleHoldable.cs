using GorillaTag;
using UnityEngine;

public class EdibleHoldable : TransferrableObject
{
	private enum EdibleHoldableStates
	{
		EatingState0 = 1,
		EatingState1 = 2,
		EatingState2 = 4,
		EatingState3 = 8
	}

	public AudioClip[] eatSounds;

	public GameObject[] edibleMeshObjects;

	[DebugReadout]
	public float lastEatTime;

	[DebugReadout]
	public float lastFullyEatenTime;

	public float eatMinimumCooldown = 1f;

	public float respawnTime = 7f;

	public float biteDistance = 0.1666667f;

	public Vector3 biteOffset = new Vector3(0f, 0.0208f, 0.171f);

	public Transform biteSpot;

	public bool inBiteZone;

	public AudioSource eatSoundSource;

	private EdibleHoldableStates previousEdibleState;

	private IResettableItem[] iResettableItems;

	protected override void Start()
	{
		base.Start();
		itemState = ItemStates.State0;
		previousEdibleState = (EdibleHoldableStates)itemState;
		lastFullyEatenTime = 0f - respawnTime;
		iResettableItems = GetComponentsInChildren<IResettableItem>(includeInactive: true);
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		lastEatTime = Time.time - eatMinimumCooldown;
	}

	public override void OnActivate()
	{
		base.OnActivate();
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}

	public override void OnDisable()
	{
		base.OnDisable();
	}

	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
	}

	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		InHand();
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (itemState == ItemStates.State3)
		{
			if (Time.time > lastFullyEatenTime + respawnTime)
			{
				itemState = ItemStates.State0;
			}
		}
		else
		{
			if (!(Time.time > lastEatTime + eatMinimumCooldown))
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			float num = biteDistance * biteDistance;
			if (!GorillaParent.hasInstance)
			{
				return;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!vrrig.isOfflineVRRig)
				{
					if (vrrig.head == null || vrrig.head.rigTarget == null)
					{
						break;
					}
					Transform transform = vrrig.head.rigTarget.transform;
					if ((transform.position + transform.rotation * biteOffset - biteSpot.position).sqrMagnitude < num)
					{
						flag = true;
					}
				}
			}
			Transform transform2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
			if ((transform2.position + transform2.rotation * biteOffset - biteSpot.position).sqrMagnitude < num)
			{
				flag = true;
				flag2 = true;
			}
			if (flag && !inBiteZone && (!flag2 || InHand()) && itemState != ItemStates.State3)
			{
				if (itemState == ItemStates.State0)
				{
					itemState = ItemStates.State1;
				}
				else if (itemState == ItemStates.State1)
				{
					itemState = ItemStates.State2;
				}
				else if (itemState == ItemStates.State2)
				{
					itemState = ItemStates.State3;
				}
				lastEatTime = Time.time;
				lastFullyEatenTime = Time.time;
			}
			inBiteZone = flag;
		}
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		EdibleHoldableStates edibleHoldableStates = (EdibleHoldableStates)itemState;
		if (edibleHoldableStates != previousEdibleState)
		{
			OnEdibleHoldableStateChange();
		}
		previousEdibleState = edibleHoldableStates;
	}

	protected virtual void OnEdibleHoldableStateChange()
	{
		float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float volumeScale = 0.08f;
		int num = 0;
		if (itemState == ItemStates.State0)
		{
			num = 0;
			if (iResettableItems != null)
			{
				IResettableItem[] array = iResettableItems;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.ResetToDefaultState();
				}
			}
		}
		else if (itemState == ItemStates.State1)
		{
			num = 1;
		}
		else if (itemState == ItemStates.State2)
		{
			num = 2;
		}
		else if (itemState == ItemStates.State3)
		{
			num = 3;
		}
		int num2 = num - 1;
		if (num2 < 0)
		{
			num2 = edibleMeshObjects.Length - 1;
		}
		edibleMeshObjects[num2].SetActive(value: false);
		edibleMeshObjects[num].SetActive(value: true);
		eatSoundSource.PlayOneShot(eatSounds[num], volumeScale);
		if (IsMyItem())
		{
			if (InHand())
			{
				GorillaTagger.Instance.StartVibration(InLeftHand(), amplitude, fixedDeltaTime);
				return;
			}
			GorillaTagger.Instance.StartVibration(forLeftController: false, amplitude, fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(forLeftController: true, amplitude, fixedDeltaTime);
		}
	}

	public override bool CanActivate()
	{
		return true;
	}

	public override bool CanDeactivate()
	{
		return true;
	}
}
