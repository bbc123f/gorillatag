using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	public class DiceHoldable : TransferrableObject
	{
		public override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				Photon.Realtime.Player player = (this.myOnlineRig != null) ? this.myOnlineRig.creator : ((this.myRig != null) ? ((this.myRig.creator != null) ? this.myRig.creator : PhotonNetwork.LocalPlayer) : null);
				if (player != null)
				{
					this._events.Init(player);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnDiceEvent;
			}
		}

		public override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= this.OnDiceEvent;
				Object.Destroy(this._events);
				this._events = null;
			}
		}

		private void OnDiceEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "OnDiceEvent");
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creatorWrapped.ID)
			{
				return;
			}
			if ((bool)args[0])
			{
				Vector3 position = base.transform.position;
				Vector3 forward = base.transform.forward;
				Vector3 vector = (Vector3)args[1];
				ref position.SetValueSafe(vector);
				vector = (Vector3)args[2];
				ref forward.SetValueSafe(vector);
				float playerScale = ((float)args[3]).ClampSafe(0.01f, 1f);
				int landingSide = Mathf.Clamp((int)args[4], 1, 20);
				double finite = ((double)args[5]).GetFinite();
				this.ThrowDiceLocal(position, forward, playerScale, landingSide, finite);
				return;
			}
			this.dicePhysics.EndThrow();
		}

		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (this.dicePhysics.enabled)
			{
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						false
					};
					this._events.Activate.RaiseOthers(args);
				}
				base.transform.position = this.dicePhysics.transform.position;
				base.transform.rotation = this.dicePhysics.transform.rotation;
				this.dicePhysics.EndThrow();
				if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
				{
					this.canAutoGrabLeft = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
				else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
				{
					this.canAutoGrabRight = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (zoneReleased == null)
			{
				Vector3 position = base.transform.position;
				Vector3 vector = (releasingHand == EquipmentInteractor.instance.leftHand) ? GorillaLocomotion.Player.Instance.leftInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false) : GorillaLocomotion.Player.Instance.rightInteractPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
				int randomSide = this.dicePhysics.GetRandomSide();
				double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
				float scale = GorillaLocomotion.Player.Instance.scale;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						true,
						position,
						vector,
						scale,
						randomSide,
						num
					};
					this._events.Activate.RaiseOthers(args);
				}
				this.ThrowDiceLocal(position, vector, scale, randomSide, num);
			}
			return true;
		}

		private void ThrowDiceLocal(Vector3 startPosition, Vector3 throwVelocity, float playerScale, int landingSide, double startTime)
		{
			this.dicePhysics.StartThrow(this, startPosition, throwVelocity, playerScale, landingSide, startTime);
		}

		public DiceHoldable()
		{
		}

		[SerializeField]
		private DicePhysics dicePhysics;

		private RubberDuckEvents _events;
	}
}
