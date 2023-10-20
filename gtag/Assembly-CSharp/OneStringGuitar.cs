using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class OneStringGuitar : TransferrableObject
{
	// Token: 0x060006DB RID: 1755 RVA: 0x0002AAAB File Offset: 0x00028CAB
	public override Matrix4x4 GetDefaultTransformationMatrix()
	{
		return Matrix4x4.identity;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x0002AAB4 File Offset: 0x00028CB4
	protected override void Start()
	{
		base.Start();
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.nullHit = default(RaycastHit);
		this.strumList.Add(this.strumCollider);
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.startingLeftChestOffset = this.chestOffsetLeft;
		this.startingRightChestOffset = this.chestOffsetRight;
		this.startingUnsnapDistance = this.unsnapDistance;
		for (int i = 0; i < this.frets.Length; i++)
		{
			this.fretsList.Add(this.frets[i]);
		}
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x0002AB7C File Offset: 0x00028D7C
	public override void OnEnable()
	{
		base.OnEnable();
		if (this.currentState == TransferrableObject.PositionState.InLeftHand)
		{
			this.fretHandIndicator = this.leftHandIndicator;
			this.strumHandIndicator = this.rightHandIndicator;
			if (base.IsLocalObject())
			{
				this.parentHand = Player.Instance.leftHandFollower;
			}
		}
		else
		{
			this.fretHandIndicator = this.rightHandIndicator;
			this.strumHandIndicator = this.leftHandIndicator;
			if (base.IsLocalObject())
			{
				this.parentHand = Player.Instance.rightHandFollower;
			}
		}
		this.initOffset = Vector3.zero;
		this.initRotation = Quaternion.identity;
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x0002AC10 File Offset: 0x00028E10
	public override void OnDisable()
	{
		base.OnDisable();
		this.angleSnapped = false;
		this.positionSnapped = false;
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x0002AC34 File Offset: 0x00028E34
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (!this.CanDeactivate())
		{
			return;
		}
		if (base.InHand())
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x0002AC58 File Offset: 0x00028E58
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.lastState != (OneStringGuitar.GuitarStates)this.itemState)
		{
			this.angleSnapped = false;
			this.positionSnapped = false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			Vector3 positionTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startPositionLeft : this.startPositionRight;
			Quaternion rotationTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startQuatLeft : this.startQuatRight;
			this.UpdateNonPlayingPosition(positionTarget, rotationTarget);
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			Vector3 positionTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripPositionLeft : this.reverseGripPositionRight;
			Quaternion rotationTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripQuatLeft : this.reverseGripQuatRight;
			this.UpdateNonPlayingPosition(positionTarget2, rotationTarget2);
			if (this.IsMyItem() && (this.chestTouch.transform.position - this.currentChestCollider.transform.position).magnitude < this.snapDistance)
			{
				this.itemState = TransferrableObject.ItemStates.State2;
				this.angleSnapped = false;
				this.positionSnapped = false;
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			Quaternion rhs = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.holdingOffsetRotationLeft : this.holdingOffsetRotationRight;
			Vector3 point = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.chestOffsetLeft : this.chestOffsetRight;
			Quaternion quaternion = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * rhs;
			if (!this.angleSnapped && Quaternion.Angle(base.transform.rotation, quaternion) > this.angleLerpSnap)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.lerpValue);
			}
			else
			{
				this.angleSnapped = true;
				base.transform.rotation = quaternion;
			}
			Vector3 vector = this.currentChestCollider.transform.position + base.transform.rotation * point;
			if (!this.positionSnapped && (base.transform.position - vector).magnitude > this.vectorLerpSnap)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.currentChestCollider.transform.position + base.transform.rotation * point, this.lerpValue);
			}
			else
			{
				this.positionSnapped = true;
				base.transform.position = vector;
			}
			if (this.currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.parentHand = this.parentHandRight;
			}
			else
			{
				this.parentHand = this.parentHandLeft;
			}
			if (this.IsMyItem())
			{
				this.unsnapDistance = this.startingUnsnapDistance * this.myRig.transform.localScale.x;
				if (this.currentState == TransferrableObject.PositionState.InRightHand)
				{
					this.chestOffsetRight = Vector3.Scale(this.startingRightChestOffset, this.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderRight;
					this.fretHandIndicator = this.rightHandIndicator;
					this.strumHandIndicator = this.leftHandIndicator;
				}
				else
				{
					this.chestOffsetLeft = Vector3.Scale(this.startingLeftChestOffset, this.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderLeft;
					this.fretHandIndicator = this.leftHandIndicator;
					this.strumHandIndicator = this.rightHandIndicator;
				}
				if (this.Unsnap())
				{
					this.itemState = TransferrableObject.ItemStates.State1;
					this.angleSnapped = false;
					this.positionSnapped = false;
					if (this.currentState == TransferrableObject.PositionState.InLeftHand)
					{
						EquipmentInteractor.instance.wasLeftGrabPressed = true;
					}
					else
					{
						EquipmentInteractor.instance.wasRightGrabPressed = true;
					}
				}
				else
				{
					if (!this.handIn)
					{
						this.CheckFretFinger(this.fretHandIndicator.transform);
						HitChecker.CheckHandHit(ref this.collidersHitCount, this.interactableMask, this.sphereRadius, ref this.nullHit, ref this.raycastHits, ref this.raycastHitList, ref this.spherecastSweep, ref this.strumHandIndicator);
						if (this.collidersHitCount > 0)
						{
							int i = 0;
							while (i < this.collidersHitCount)
							{
								if (this.raycastHits[i].collider != null && this.strumCollider == this.raycastHits[i].collider)
								{
									GorillaTagger.Instance.StartVibration(this.strumHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
									this.PlayNote(this.currentFretIndex, Mathf.Max(Mathf.Min(1f, this.strumHandIndicator.currentVelocity.magnitude / this.maxVelocity) * this.maxVolume, this.minVolume));
									if (!PhotonNetwork.InRoom)
									{
										break;
									}
									PhotonView myVRRig = GorillaTagger.Instance.myVRRig;
									if (myVRRig == null)
									{
										break;
									}
									myVRRig.RPC("PlaySelfOnlyInstrument", RpcTarget.Others, new object[]
									{
										this.selfInstrumentIndex,
										this.currentFretIndex,
										this.audioSource.volume
									});
									break;
								}
								else
								{
									i++;
								}
							}
						}
					}
					this.handIn = HitChecker.CheckHandIn(ref this.anyHit, ref this.collidersHit, this.sphereRadius * base.transform.lossyScale.x, this.interactableMask, ref this.strumHandIndicator, ref this.strumList);
				}
			}
		}
		this.lastState = (OneStringGuitar.GuitarStates)this.itemState;
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x0002B1E8 File Offset: 0x000293E8
	public override void PlayNote(int note, float volume)
	{
		this.audioSource.time = 0.005f;
		this.audioSource.clip = this.audioClips[note];
		this.audioSource.volume = volume;
		this.audioSource.Play();
		base.PlayNote(note, volume);
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x0002B238 File Offset: 0x00029438
	private bool Unsnap()
	{
		return (this.parentHand.position - this.chestTouch.position).magnitude > this.unsnapDistance;
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x0002B270 File Offset: 0x00029470
	private void CheckFretFinger(Transform finger)
	{
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = null;
		}
		this.collidersHitCount = Physics.OverlapSphereNonAlloc(finger.position, this.sphereRadius, this.collidersHit, this.interactableMask, QueryTriggerInteraction.Collide);
		this.currentFretIndex = 5;
		if (this.collidersHitCount > 0)
		{
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.fretsList.Contains(this.collidersHit[j]))
				{
					this.currentFretIndex = this.fretsList.IndexOf(this.collidersHit[j]);
					if (this.currentFretIndex != this.lastFretIndex)
					{
						GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
					}
					this.lastFretIndex = this.currentFretIndex;
					return;
				}
			}
			return;
		}
		if (this.lastFretIndex != -1)
		{
			GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
		}
		this.lastFretIndex = -1;
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x0002B3A4 File Offset: 0x000295A4
	public void UpdateNonPlayingPosition(Vector3 positionTarget, Quaternion rotationTarget)
	{
		if (!this.angleSnapped)
		{
			if (Quaternion.Angle(rotationTarget, base.transform.localRotation) < this.angleLerpSnap)
			{
				this.angleSnapped = true;
				base.transform.localRotation = rotationTarget;
			}
			else
			{
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, rotationTarget, this.lerpValue);
			}
		}
		if (!this.positionSnapped)
		{
			if ((base.transform.localPosition - positionTarget).magnitude < this.vectorLerpSnap)
			{
				this.positionSnapped = true;
				base.transform.localPosition = positionTarget;
				return;
			}
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, positionTarget, this.lerpValue);
		}
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x0002B468 File Offset: 0x00029668
	public override bool CanDeactivate()
	{
		return !base.gameObject.activeSelf || this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x0002B48B File Offset: 0x0002968B
	public override bool CanActivate()
	{
		return this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x0002B4A1 File Offset: 0x000296A1
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.itemState = TransferrableObject.ItemStates.State1;
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x0002B4C4 File Offset: 0x000296C4
	public void GenerateVectorOffsetLeft()
	{
		this.chestOffsetLeft = base.transform.position - this.chestColliderLeft.transform.position;
		this.holdingOffsetRotationLeft = Quaternion.LookRotation(base.transform.position - this.chestColliderLeft.transform.position);
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x0002B524 File Offset: 0x00029724
	public void GenerateVectorOffsetRight()
	{
		this.chestOffsetRight = base.transform.position - this.chestColliderRight.transform.position;
		this.holdingOffsetRotationRight = Quaternion.LookRotation(base.transform.position - this.chestColliderRight.transform.position);
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x0002B582 File Offset: 0x00029782
	public void GenerateReverseGripOffsetLeft()
	{
		this.reverseGripPositionLeft = base.transform.localPosition;
		this.reverseGripQuatLeft = base.transform.localRotation;
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0002B5A6 File Offset: 0x000297A6
	public void GenerateClubOffsetLeft()
	{
		this.startPositionLeft = base.transform.localPosition;
		this.startQuatLeft = base.transform.localRotation;
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x0002B5CA File Offset: 0x000297CA
	public void GenerateReverseGripOffsetRight()
	{
		this.reverseGripPositionRight = base.transform.localPosition;
		this.reverseGripQuatRight = base.transform.localRotation;
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x0002B5EE File Offset: 0x000297EE
	public void GenerateClubOffsetRight()
	{
		this.startPositionRight = base.transform.localPosition;
		this.startQuatRight = base.transform.localRotation;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x0002B612 File Offset: 0x00029812
	public void TestClubPositionRight()
	{
		base.transform.localPosition = this.startPositionRight;
		base.transform.localRotation = this.startQuatRight;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0002B636 File Offset: 0x00029836
	public void TestReverseGripPositionRight()
	{
		base.transform.localPosition = this.reverseGripPositionRight;
		base.transform.localRotation = this.reverseGripQuatRight;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0002B65C File Offset: 0x0002985C
	public void TestPlayingPositionRight()
	{
		base.transform.rotation = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * this.holdingOffsetRotationRight;
		base.transform.position = this.chestColliderRight.transform.position + base.transform.rotation * this.chestOffsetRight;
	}

	// Token: 0x0400083C RID: 2108
	public Vector3 chestOffsetLeft;

	// Token: 0x0400083D RID: 2109
	public Vector3 chestOffsetRight;

	// Token: 0x0400083E RID: 2110
	public Quaternion holdingOffsetRotationLeft;

	// Token: 0x0400083F RID: 2111
	public Quaternion holdingOffsetRotationRight;

	// Token: 0x04000840 RID: 2112
	public Quaternion chestRotationOffset;

	// Token: 0x04000841 RID: 2113
	public Collider currentChestCollider;

	// Token: 0x04000842 RID: 2114
	public Collider chestColliderLeft;

	// Token: 0x04000843 RID: 2115
	public Collider chestColliderRight;

	// Token: 0x04000844 RID: 2116
	public float lerpValue = 0.25f;

	// Token: 0x04000845 RID: 2117
	public AudioSource audioSource;

	// Token: 0x04000846 RID: 2118
	public Transform parentHand;

	// Token: 0x04000847 RID: 2119
	public Transform parentHandLeft;

	// Token: 0x04000848 RID: 2120
	public Transform parentHandRight;

	// Token: 0x04000849 RID: 2121
	public float unsnapDistance;

	// Token: 0x0400084A RID: 2122
	public float snapDistance;

	// Token: 0x0400084B RID: 2123
	public Vector3 startPositionLeft;

	// Token: 0x0400084C RID: 2124
	public Quaternion startQuatLeft;

	// Token: 0x0400084D RID: 2125
	public Vector3 reverseGripPositionLeft;

	// Token: 0x0400084E RID: 2126
	public Quaternion reverseGripQuatLeft;

	// Token: 0x0400084F RID: 2127
	public Vector3 startPositionRight;

	// Token: 0x04000850 RID: 2128
	public Quaternion startQuatRight;

	// Token: 0x04000851 RID: 2129
	public Vector3 reverseGripPositionRight;

	// Token: 0x04000852 RID: 2130
	public Quaternion reverseGripQuatRight;

	// Token: 0x04000853 RID: 2131
	public float angleLerpSnap = 1f;

	// Token: 0x04000854 RID: 2132
	public float vectorLerpSnap = 0.01f;

	// Token: 0x04000855 RID: 2133
	private bool angleSnapped;

	// Token: 0x04000856 RID: 2134
	private bool positionSnapped;

	// Token: 0x04000857 RID: 2135
	public Transform chestTouch;

	// Token: 0x04000858 RID: 2136
	private int collidersHitCount;

	// Token: 0x04000859 RID: 2137
	private Collider[] collidersHit = new Collider[20];

	// Token: 0x0400085A RID: 2138
	private RaycastHit[] raycastHits = new RaycastHit[20];

	// Token: 0x0400085B RID: 2139
	private List<RaycastHit> raycastHitList = new List<RaycastHit>();

	// Token: 0x0400085C RID: 2140
	private RaycastHit nullHit;

	// Token: 0x0400085D RID: 2141
	public Collider[] collidersToBeIn;

	// Token: 0x0400085E RID: 2142
	public LayerMask interactableMask;

	// Token: 0x0400085F RID: 2143
	public int currentFretIndex;

	// Token: 0x04000860 RID: 2144
	public int lastFretIndex;

	// Token: 0x04000861 RID: 2145
	public Collider[] frets;

	// Token: 0x04000862 RID: 2146
	private List<Collider> fretsList = new List<Collider>();

	// Token: 0x04000863 RID: 2147
	public AudioClip[] audioClips;

	// Token: 0x04000864 RID: 2148
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x04000865 RID: 2149
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04000866 RID: 2150
	private GorillaTriggerColliderHandIndicator fretHandIndicator;

	// Token: 0x04000867 RID: 2151
	private GorillaTriggerColliderHandIndicator strumHandIndicator;

	// Token: 0x04000868 RID: 2152
	private float sphereRadius;

	// Token: 0x04000869 RID: 2153
	private bool anyHit;

	// Token: 0x0400086A RID: 2154
	private bool handIn;

	// Token: 0x0400086B RID: 2155
	private Vector3 spherecastSweep;

	// Token: 0x0400086C RID: 2156
	public Collider strumCollider;

	// Token: 0x0400086D RID: 2157
	public float maxVolume = 1f;

	// Token: 0x0400086E RID: 2158
	public float minVolume = 0.05f;

	// Token: 0x0400086F RID: 2159
	public float maxVelocity = 2f;

	// Token: 0x04000870 RID: 2160
	private List<Collider> strumList = new List<Collider>();

	// Token: 0x04000871 RID: 2161
	public int selfInstrumentIndex;

	// Token: 0x04000872 RID: 2162
	private OneStringGuitar.GuitarStates lastState;

	// Token: 0x04000873 RID: 2163
	private Vector3 startingLeftChestOffset;

	// Token: 0x04000874 RID: 2164
	private Vector3 startingRightChestOffset;

	// Token: 0x04000875 RID: 2165
	private float startingUnsnapDistance;

	// Token: 0x02000400 RID: 1024
	private enum GuitarStates
	{
		// Token: 0x04001CA9 RID: 7337
		Club = 1,
		// Token: 0x04001CAA RID: 7338
		HeldReverseGrip,
		// Token: 0x04001CAB RID: 7339
		Playing = 4
	}
}
