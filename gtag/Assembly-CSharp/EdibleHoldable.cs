using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200010C RID: 268
public class EdibleHoldable : TransferrableObject
{
	// Token: 0x0600068E RID: 1678 RVA: 0x00029418 File Offset: 0x00027618
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.previousEdibleState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		this.lastFullyEatenTime = -this.respawnTime;
		this.iResettableItems = base.GetComponentsInChildren<IResettableItem>(true);
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0002944D File Offset: 0x0002764D
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.lastEatTime = Time.time - this.eatMinimumCooldown;
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00029469 File Offset: 0x00027669
	public override void OnActivate()
	{
		base.OnActivate();
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00029471 File Offset: 0x00027671
	public override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00029479 File Offset: 0x00027679
	public override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00029481 File Offset: 0x00027681
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00029489 File Offset: 0x00027689
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		base.InHand();
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0002949C File Offset: 0x0002769C
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

	// Token: 0x06000696 RID: 1686 RVA: 0x000296D4 File Offset: 0x000278D4
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

	// Token: 0x06000697 RID: 1687 RVA: 0x00029704 File Offset: 0x00027904
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

	// Token: 0x06000698 RID: 1688 RVA: 0x0002981B File Offset: 0x00027A1B
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0002981E File Offset: 0x00027A1E
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x040007EC RID: 2028
	public AudioClip[] eatSounds;

	// Token: 0x040007ED RID: 2029
	public GameObject[] edibleMeshObjects;

	// Token: 0x040007EE RID: 2030
	public EdibleHoldable.BiteEvent onBiteView;

	// Token: 0x040007EF RID: 2031
	public EdibleHoldable.BiteEvent onBiteWorld;

	// Token: 0x040007F0 RID: 2032
	[DebugReadout]
	public float lastEatTime;

	// Token: 0x040007F1 RID: 2033
	[DebugReadout]
	public float lastFullyEatenTime;

	// Token: 0x040007F2 RID: 2034
	public float eatMinimumCooldown = 1f;

	// Token: 0x040007F3 RID: 2035
	public float respawnTime = 7f;

	// Token: 0x040007F4 RID: 2036
	public float biteDistance = 0.1666667f;

	// Token: 0x040007F5 RID: 2037
	public Vector3 biteOffset = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x040007F6 RID: 2038
	public Transform biteSpot;

	// Token: 0x040007F7 RID: 2039
	public bool inBiteZone;

	// Token: 0x040007F8 RID: 2040
	public AudioSource eatSoundSource;

	// Token: 0x040007F9 RID: 2041
	private EdibleHoldable.EdibleHoldableStates previousEdibleState;

	// Token: 0x040007FA RID: 2042
	private IResettableItem[] iResettableItems;

	// Token: 0x020003FB RID: 1019
	private enum EdibleHoldableStates
	{
		// Token: 0x04001C94 RID: 7316
		EatingState0 = 1,
		// Token: 0x04001C95 RID: 7317
		EatingState1,
		// Token: 0x04001C96 RID: 7318
		EatingState2 = 4,
		// Token: 0x04001C97 RID: 7319
		EatingState3 = 8
	}

	// Token: 0x020003FC RID: 1020
	[Serializable]
	public class BiteEvent : UnityEvent<VRRig, int>
	{
	}
}
