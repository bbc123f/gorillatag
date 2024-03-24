using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public class FingerTipPokeTool : InteractableTool
	{
		public override InteractableToolTags ToolTags
		{
			get
			{
				return InteractableToolTags.Poke;
			}
		}

		public override ToolInputState ToolInputState
		{
			get
			{
				return ToolInputState.Inactive;
			}
		}

		public override bool IsFarFieldTool
		{
			get
			{
				return false;
			}
		}

		public override bool EnableState
		{
			get
			{
				return this._fingerTipPokeToolView.gameObject.activeSelf;
			}
			set
			{
				this._fingerTipPokeToolView.gameObject.SetActive(value);
			}
		}

		public override void Initialize()
		{
			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			this._fingerTipPokeToolView.InteractableTool = this;
			this._velocityFrames = new Vector3[10];
			Array.Clear(this._velocityFrames, 0, 10);
			base.StartCoroutine(this.AttachTriggerLogic());
		}

		private IEnumerator AttachTriggerLogic()
		{
			while (!HandsManager.Instance || !HandsManager.Instance.IsInitialized())
			{
				yield return null;
			}
			OVRSkeleton ovrskeleton = (base.IsRightHandedTool ? HandsManager.Instance.RightHandSkeleton : HandsManager.Instance.LeftHandSkeleton);
			OVRSkeleton.BoneId boneId;
			switch (this._fingerToFollow)
			{
			case OVRPlugin.HandFinger.Thumb:
				boneId = OVRSkeleton.BoneId.Hand_Thumb3;
				break;
			case OVRPlugin.HandFinger.Index:
				boneId = OVRSkeleton.BoneId.Hand_Index3;
				break;
			case OVRPlugin.HandFinger.Middle:
				boneId = OVRSkeleton.BoneId.Hand_Middle3;
				break;
			case OVRPlugin.HandFinger.Ring:
				boneId = OVRSkeleton.BoneId.Hand_Ring3;
				break;
			default:
				boneId = OVRSkeleton.BoneId.Hand_Pinky3;
				break;
			}
			List<BoneCapsuleTriggerLogic> list = new List<BoneCapsuleTriggerLogic>();
			List<OVRBoneCapsule> capsulesPerBone = HandsManager.GetCapsulesPerBone(ovrskeleton, boneId);
			foreach (OVRBoneCapsule ovrboneCapsule in capsulesPerBone)
			{
				BoneCapsuleTriggerLogic boneCapsuleTriggerLogic = ovrboneCapsule.CapsuleRigidbody.gameObject.AddComponent<BoneCapsuleTriggerLogic>();
				ovrboneCapsule.CapsuleCollider.isTrigger = true;
				boneCapsuleTriggerLogic.ToolTags = this.ToolTags;
				list.Add(boneCapsuleTriggerLogic);
			}
			this._boneCapsuleTriggerLogic = list.ToArray();
			if (capsulesPerBone.Count > 0)
			{
				this._capsuleToTrack = capsulesPerBone[0];
			}
			this._isInitialized = true;
			yield break;
		}

		private void Update()
		{
			if (!HandsManager.Instance || !HandsManager.Instance.IsInitialized() || !this._isInitialized || this._capsuleToTrack == null)
			{
				return;
			}
			float handScale = (base.IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand).HandScale;
			Transform transform = this._capsuleToTrack.CapsuleCollider.transform;
			Vector3 right = transform.right;
			Vector3 vector = transform.position + this._capsuleToTrack.CapsuleCollider.height * 0.5f * right;
			Vector3 vector2 = handScale * this._fingerTipPokeToolView.SphereRadius * right;
			Vector3 vector3 = vector + vector2;
			base.transform.position = vector3;
			base.transform.rotation = transform.rotation;
			base.InteractionPosition = vector;
			this.UpdateAverageVelocity();
			this.CheckAndUpdateScale();
		}

		private void UpdateAverageVelocity()
		{
			Vector3 position = this._position;
			Vector3 position2 = base.transform.position;
			Vector3 vector = (position2 - position) / Time.deltaTime;
			this._position = position2;
			this._velocityFrames[this._currVelocityFrame] = vector;
			this._currVelocityFrame = (this._currVelocityFrame + 1) % 10;
			base.Velocity = Vector3.zero;
			if (!this._sampledMaxFramesAlready && this._currVelocityFrame == 9)
			{
				this._sampledMaxFramesAlready = true;
			}
			int num = (this._sampledMaxFramesAlready ? 10 : (this._currVelocityFrame + 1));
			for (int i = 0; i < num; i++)
			{
				base.Velocity += this._velocityFrames[i];
			}
			base.Velocity /= (float)num;
		}

		private void CheckAndUpdateScale()
		{
			float num = (base.IsRightHandedTool ? HandsManager.Instance.RightHand.HandScale : HandsManager.Instance.LeftHand.HandScale);
			if (Mathf.Abs(num - this._lastScale) > Mathf.Epsilon)
			{
				base.transform.localScale = new Vector3(num, num, num);
				this._lastScale = num;
			}
		}

		public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
		{
			this._currentIntersectingObjects.Clear();
			BoneCapsuleTriggerLogic[] boneCapsuleTriggerLogic = this._boneCapsuleTriggerLogic;
			for (int i = 0; i < boneCapsuleTriggerLogic.Length; i++)
			{
				foreach (ColliderZone colliderZone in boneCapsuleTriggerLogic[i].CollidersTouchingUs)
				{
					this._currentIntersectingObjects.Add(new InteractableCollisionInfo(colliderZone, colliderZone.CollisionDepth, this));
				}
			}
			return this._currentIntersectingObjects;
		}

		public override void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone)
		{
		}

		public override void DeFocus()
		{
		}

		public FingerTipPokeTool()
		{
		}

		private const int NUM_VELOCITY_FRAMES = 10;

		[SerializeField]
		private FingerTipPokeToolView _fingerTipPokeToolView;

		[SerializeField]
		private OVRPlugin.HandFinger _fingerToFollow = OVRPlugin.HandFinger.Index;

		private Vector3[] _velocityFrames;

		private int _currVelocityFrame;

		private bool _sampledMaxFramesAlready;

		private Vector3 _position;

		private BoneCapsuleTriggerLogic[] _boneCapsuleTriggerLogic;

		private float _lastScale = 1f;

		private bool _isInitialized;

		private OVRBoneCapsule _capsuleToTrack;

		[CompilerGenerated]
		private sealed class <AttachTriggerLogic>d__21 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <AttachTriggerLogic>d__21(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				FingerTipPokeTool fingerTipPokeTool = this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
				}
				if (HandsManager.Instance && HandsManager.Instance.IsInitialized())
				{
					OVRSkeleton ovrskeleton = (fingerTipPokeTool.IsRightHandedTool ? HandsManager.Instance.RightHandSkeleton : HandsManager.Instance.LeftHandSkeleton);
					OVRSkeleton.BoneId boneId;
					switch (fingerTipPokeTool._fingerToFollow)
					{
					case OVRPlugin.HandFinger.Thumb:
						boneId = OVRSkeleton.BoneId.Hand_Thumb3;
						break;
					case OVRPlugin.HandFinger.Index:
						boneId = OVRSkeleton.BoneId.Hand_Index3;
						break;
					case OVRPlugin.HandFinger.Middle:
						boneId = OVRSkeleton.BoneId.Hand_Middle3;
						break;
					case OVRPlugin.HandFinger.Ring:
						boneId = OVRSkeleton.BoneId.Hand_Ring3;
						break;
					default:
						boneId = OVRSkeleton.BoneId.Hand_Pinky3;
						break;
					}
					List<BoneCapsuleTriggerLogic> list = new List<BoneCapsuleTriggerLogic>();
					List<OVRBoneCapsule> capsulesPerBone = HandsManager.GetCapsulesPerBone(ovrskeleton, boneId);
					foreach (OVRBoneCapsule ovrboneCapsule in capsulesPerBone)
					{
						BoneCapsuleTriggerLogic boneCapsuleTriggerLogic = ovrboneCapsule.CapsuleRigidbody.gameObject.AddComponent<BoneCapsuleTriggerLogic>();
						ovrboneCapsule.CapsuleCollider.isTrigger = true;
						boneCapsuleTriggerLogic.ToolTags = fingerTipPokeTool.ToolTags;
						list.Add(boneCapsuleTriggerLogic);
					}
					fingerTipPokeTool._boneCapsuleTriggerLogic = list.ToArray();
					if (capsulesPerBone.Count > 0)
					{
						fingerTipPokeTool._capsuleToTrack = capsulesPerBone[0];
					}
					fingerTipPokeTool._isInitialized = true;
					return false;
				}
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public FingerTipPokeTool <>4__this;
		}
	}
}
