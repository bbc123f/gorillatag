using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OVRTouchSample
{
	[RequireComponent(typeof(OVRGrabber))]
	public class Hand : MonoBehaviour
	{
		private void Awake()
		{
			this.m_grabber = base.GetComponent<OVRGrabber>();
		}

		private void Start()
		{
			this.m_showAfterInputFocusAcquired = new List<Renderer>();
			this.m_colliders = (from childCollider in base.GetComponentsInChildren<Collider>()
			where !childCollider.isTrigger
			select childCollider).ToArray<Collider>();
			this.CollisionEnable(false);
			this.m_animLayerIndexPoint = this.m_animator.GetLayerIndex("Point Layer");
			this.m_animLayerIndexThumb = this.m_animator.GetLayerIndex("Thumb Layer");
			this.m_animParamIndexFlex = Animator.StringToHash("Flex");
			this.m_animParamIndexPose = Animator.StringToHash("Pose");
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		private void OnDestroy()
		{
			OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
			OVRManager.InputFocusLost -= this.OnInputFocusLost;
		}

		private void Update()
		{
			this.UpdateCapTouchStates();
			this.m_pointBlend = this.InputValueRateChange(this.m_isPointing, this.m_pointBlend);
			this.m_thumbsUpBlend = this.InputValueRateChange(this.m_isGivingThumbsUp, this.m_thumbsUpBlend);
			float num = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller);
			bool enabled = this.m_grabber.grabbedObject == null && num >= 0.9f;
			this.CollisionEnable(enabled);
			this.UpdateAnimStates();
		}

		private void UpdateCapTouchStates()
		{
			this.m_isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, this.m_controller);
			this.m_isGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, this.m_controller);
		}

		private void LateUpdate()
		{
			if (this.m_collisionEnabled && this.m_collisionScaleCurrent + Mathf.Epsilon < 1f)
			{
				this.m_collisionScaleCurrent = Mathf.Min(1f, this.m_collisionScaleCurrent + Time.deltaTime * 1f);
				for (int i = 0; i < this.m_colliders.Length; i++)
				{
					this.m_colliders[i].transform.localScale = new Vector3(this.m_collisionScaleCurrent, this.m_collisionScaleCurrent, this.m_collisionScaleCurrent);
				}
			}
		}

		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				this.m_showAfterInputFocusAcquired.Clear();
				Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].enabled)
					{
						componentsInChildren[i].enabled = false;
						this.m_showAfterInputFocusAcquired.Add(componentsInChildren[i]);
					}
				}
				this.CollisionEnable(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				for (int i = 0; i < this.m_showAfterInputFocusAcquired.Count; i++)
				{
					if (this.m_showAfterInputFocusAcquired[i])
					{
						this.m_showAfterInputFocusAcquired[i].enabled = true;
					}
				}
				this.m_showAfterInputFocusAcquired.Clear();
				this.m_restoreOnInputAcquired = false;
			}
		}

		private float InputValueRateChange(bool isDown, float value)
		{
			float num = Time.deltaTime * 20f;
			float num2 = isDown ? 1f : -1f;
			return Mathf.Clamp01(value + num * num2);
		}

		private void UpdateAnimStates()
		{
			bool flag = this.m_grabber.grabbedObject != null;
			HandPose handPose = this.m_defaultGrabPose;
			if (flag)
			{
				HandPose component = this.m_grabber.grabbedObject.GetComponent<HandPose>();
				if (component != null)
				{
					handPose = component;
				}
			}
			HandPoseId poseId = handPose.PoseId;
			this.m_animator.SetInteger(this.m_animParamIndexPose, (int)poseId);
			float value = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller);
			this.m_animator.SetFloat(this.m_animParamIndexFlex, value);
			float weight = (!flag || handPose.AllowPointing) ? this.m_pointBlend : 0f;
			this.m_animator.SetLayerWeight(this.m_animLayerIndexPoint, weight);
			float weight2 = (!flag || handPose.AllowThumbsUp) ? this.m_thumbsUpBlend : 0f;
			this.m_animator.SetLayerWeight(this.m_animLayerIndexThumb, weight2);
			float value2 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller);
			this.m_animator.SetFloat("Pinch", value2);
		}

		private void CollisionEnable(bool enabled)
		{
			if (this.m_collisionEnabled == enabled)
			{
				return;
			}
			this.m_collisionEnabled = enabled;
			if (enabled)
			{
				this.m_collisionScaleCurrent = 0.01f;
				for (int i = 0; i < this.m_colliders.Length; i++)
				{
					Collider collider = this.m_colliders[i];
					collider.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
					collider.enabled = true;
				}
				return;
			}
			this.m_collisionScaleCurrent = 1f;
			for (int j = 0; j < this.m_colliders.Length; j++)
			{
				Collider collider2 = this.m_colliders[j];
				collider2.enabled = false;
				collider2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			}
		}

		public const string ANIM_LAYER_NAME_POINT = "Point Layer";

		public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";

		public const string ANIM_PARAM_NAME_FLEX = "Flex";

		public const string ANIM_PARAM_NAME_POSE = "Pose";

		public const float THRESH_COLLISION_FLEX = 0.9f;

		public const float INPUT_RATE_CHANGE = 20f;

		public const float COLLIDER_SCALE_MIN = 0.01f;

		public const float COLLIDER_SCALE_MAX = 1f;

		public const float COLLIDER_SCALE_PER_SECOND = 1f;

		public const float TRIGGER_DEBOUNCE_TIME = 0.05f;

		public const float THUMB_DEBOUNCE_TIME = 0.15f;

		[SerializeField]
		private OVRInput.Controller m_controller;

		[SerializeField]
		private Animator m_animator;

		[SerializeField]
		private HandPose m_defaultGrabPose;

		private Collider[] m_colliders;

		private bool m_collisionEnabled = true;

		private OVRGrabber m_grabber;

		private List<Renderer> m_showAfterInputFocusAcquired;

		private int m_animLayerIndexThumb = -1;

		private int m_animLayerIndexPoint = -1;

		private int m_animParamIndexFlex = -1;

		private int m_animParamIndexPose = -1;

		private bool m_isPointing;

		private bool m_isGivingThumbsUp;

		private float m_pointBlend;

		private float m_thumbsUpBlend;

		private bool m_restoreOnInputAcquired;

		private float m_collisionScaleCurrent;
	}
}
