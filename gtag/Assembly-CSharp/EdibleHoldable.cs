﻿using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

public class EdibleHoldable : TransferrableObject
{
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.previousEdibleState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		this.lastFullyEatenTime = -this.respawnTime;
		this.iResettableItems = base.GetComponentsInChildren<IResettableItem>(true);
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.lastEatTime = Time.time - this.eatMinimumCooldown;
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

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && !base.InHand();
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			if (Time.time > this.lastFullyEatenTime + this.respawnTime)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				return;
			}
		}
		else if (Time.time > this.lastEatTime + this.eatMinimumCooldown)
		{
			bool flag = false;
			bool flag2 = false;
			float num = this.biteDistance * this.biteDistance;
			if (!GorillaParent.hasInstance)
			{
				return;
			}
			VRRig arg = null;
			VRRig arg2 = null;
			for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
			{
				VRRig vrrig = GorillaParent.instance.vrrigs[i];
				if (!vrrig.isOfflineVRRig)
				{
					if (vrrig.head == null || vrrig.head.rigTarget == null)
					{
						break;
					}
					Transform transform = vrrig.head.rigTarget.transform;
					if ((transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
					{
						flag = true;
						arg2 = vrrig;
					}
				}
			}
			Transform transform2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
			if ((transform2.position + transform2.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
			{
				flag = true;
				flag2 = true;
				arg = GorillaTagger.Instance.offlineVRRig;
			}
			if (flag && !this.inBiteZone && (!flag2 || base.InHand()) && this.itemState != TransferrableObject.ItemStates.State3)
			{
				if (this.itemState == TransferrableObject.ItemStates.State0)
				{
					this.itemState = TransferrableObject.ItemStates.State1;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State1)
				{
					this.itemState = TransferrableObject.ItemStates.State2;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State2)
				{
					this.itemState = TransferrableObject.ItemStates.State3;
				}
				this.lastEatTime = Time.time;
				this.lastFullyEatenTime = Time.time;
			}
			if (flag)
			{
				if (flag2)
				{
					EdibleHoldable.BiteEvent biteEvent = this.onBiteView;
					if (biteEvent != null)
					{
						biteEvent.Invoke(arg, (int)this.itemState);
					}
				}
				else
				{
					EdibleHoldable.BiteEvent biteEvent2 = this.onBiteWorld;
					if (biteEvent2 != null)
					{
						biteEvent2.Invoke(arg2, (int)this.itemState);
					}
				}
			}
			this.inBiteZone = flag;
		}
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		EdibleHoldable.EdibleHoldableStates itemState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		if (itemState != this.previousEdibleState)
		{
			this.OnEdibleHoldableStateChange();
		}
		this.previousEdibleState = itemState;
	}

	protected virtual void OnEdibleHoldableStateChange()
	{
		float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float volumeScale = 0.08f;
		int num = 0;
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			num = 0;
			if (this.iResettableItems != null)
			{
				foreach (IResettableItem resettableItem in this.iResettableItems)
				{
					if (resettableItem != null)
					{
						resettableItem.ResetToDefaultState();
					}
				}
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num = 1;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			num = 2;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			num = 3;
		}
		int num2 = num - 1;
		if (num2 < 0)
		{
			num2 = this.edibleMeshObjects.Length - 1;
		}
		this.edibleMeshObjects[num2].SetActive(false);
		this.edibleMeshObjects[num].SetActive(true);
		if ((this.itemState != TransferrableObject.ItemStates.State0 && this.onBiteView != null) || this.onBiteWorld != null)
		{
			VRRig vrrig = null;
			float num3 = float.PositiveInfinity;
			for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
			{
				VRRig vrrig2 = GorillaParent.instance.vrrigs[j];
				if (vrrig2.head == null || vrrig2.head.rigTarget == null)
				{
					break;
				}
				Transform transform = vrrig2.head.rigTarget.transform;
				float sqrMagnitude = (transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude;
				if (sqrMagnitude < num3)
				{
					num3 = sqrMagnitude;
					vrrig = vrrig2;
				}
			}
			if (vrrig != null)
			{
				EdibleHoldable.BiteEvent biteEvent = vrrig.isOfflineVRRig ? this.onBiteView : this.onBiteWorld;
				if (biteEvent != null)
				{
					biteEvent.Invoke(vrrig, (int)this.itemState);
				}
			}
		}
		this.eatSoundSource.PlayOneShot(this.eatSounds[num], volumeScale);
		if (this.IsMyItem())
		{
			if (base.InHand())
			{
				GorillaTagger.Instance.StartVibration(base.InLeftHand(), amplitude, fixedDeltaTime);
				return;
			}
			GorillaTagger.Instance.StartVibration(false, amplitude, fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(true, amplitude, fixedDeltaTime);
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

	public EdibleHoldable()
	{
	}

	public AudioClip[] eatSounds;

	public GameObject[] edibleMeshObjects;

	public EdibleHoldable.BiteEvent onBiteView;

	public EdibleHoldable.BiteEvent onBiteWorld;

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

	private EdibleHoldable.EdibleHoldableStates previousEdibleState;

	private IResettableItem[] iResettableItems;

	private enum EdibleHoldableStates
	{
		EatingState0 = 1,
		EatingState1,
		EatingState2 = 4,
		EatingState3 = 8
	}

	[Serializable]
	public class BiteEvent : UnityEvent<VRRig, int>
	{
		public BiteEvent()
		{
		}
	}
}
