using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	public class DreidelHoldable : TransferrableObject
	{
		public override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				RubberDuckEvents events = this._events;
				VRRig myOnlineRig = this.myOnlineRig;
				Photon.Realtime.Player player;
				if ((player = ((myOnlineRig != null) ? myOnlineRig.creator : null)) == null)
				{
					VRRig myRig = this.myRig;
					player = ((myRig != null) ? myRig.creator : null);
				}
				events.Init(player);
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnDreidelSpin;
			}
		}

		public override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				Object.Destroy(this._events);
			}
		}

		private void OnDreidelSpin(int sender, int target, object[] args)
		{
			if (sender != target)
			{
				return;
			}
			this.StartSpinLocal((Vector3)args[0], (Vector3)args[1], (float)args[2], (bool)args[3], (Dreidel.Side)args[4], (Dreidel.Variation)args[5], (double)args[6]);
		}

		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TryCheckForSurfaces();
			}
		}

		public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			base.OnRelease(zoneReleased, releasingHand);
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.TrySetIdle();
			}
		}

		public override void OnActivate()
		{
			base.OnActivate();
			Vector3 vector;
			Vector3 vector2;
			float num;
			Dreidel.Side side;
			Dreidel.Variation variation;
			double num2;
			if (this.dreidelAnimation != null && this.dreidelAnimation.TryGetSpinStartData(out vector, out vector2, out num, out side, out variation, out num2))
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						vector,
						vector2,
						num,
						flag,
						(int)side,
						(int)variation,
						num2
					};
					this._events.Activate.RaiseAll(args);
					return;
				}
				this.StartSpinLocal(vector, vector2, num, flag, side, variation, num2);
			}
		}

		private void StartSpinLocal(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			if (this.dreidelAnimation != null)
			{
				this.dreidelAnimation.SetSpinStartData(surfacePoint, surfaceNormal, duration, counterClockwise, side, variation, startTime);
				this.dreidelAnimation.Spin();
			}
		}

		public void DebugSpinDreidel()
		{
			Transform transform = GorillaLocomotion.Player.Instance.headCollider.transform;
			Vector3 origin = transform.position + transform.forward * 0.5f;
			float maxDistance = 2f;
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, Vector3.down, out raycastHit, maxDistance, GorillaLocomotion.Player.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				Vector3 point = raycastHit.point;
				Vector3 normal = raycastHit.normal;
				float num = Random.Range(7f, 10f);
				Dreidel.Side side = (Dreidel.Side)Random.Range(0, 4);
				Dreidel.Variation variation = (Dreidel.Variation)Random.Range(0, 5);
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
				double num2 = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						point,
						normal,
						num,
						flag,
						(int)side,
						(int)variation,
						num2
					};
					this._events.Activate.RaiseAll(args);
					return;
				}
				this.StartSpinLocal(point, normal, num, flag, side, variation, num2);
			}
		}

		[SerializeField]
		private Dreidel dreidelAnimation;

		private RubberDuckEvents _events;
	}
}
