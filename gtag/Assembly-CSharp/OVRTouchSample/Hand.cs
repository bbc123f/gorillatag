using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x020002F6 RID: 758
	[RequireComponent(typeof(OVRGrabber))]
	public class Hand : MonoBehaviour
	{
		// Token: 0x06001491 RID: 5265 RVA: 0x00074134 File Offset: 0x00072334
		private void Awake()
		{
			this.m_grabber = base.GetComponent<OVRGrabber>();
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x00074144 File Offset: 0x00072344
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

		// Token: 0x06001493 RID: 5267 RVA: 0x00074206 File Offset: 0x00072406
		private void OnDestroy()
		{
			OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
			OVRManager.InputFocusLost -= this.OnInputFocusLost;
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x0007422C File Offset: 0x0007242C
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

		// Token: 0x06001495 RID: 5269 RVA: 0x000742AB File Offset: 0x000724AB
		private void UpdateCapTouchStates()
		{
			this.m_isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, this.m_controller);
			this.m_isGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, this.m_controller);
		}

		// Token: 0x06001496 RID: 5270 RVA: 0x000742D8 File Offset: 0x000724D8
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

		// Token: 0x06001497 RID: 5271 RVA: 0x00074360 File Offset: 0x00072560
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

		// Token: 0x06001498 RID: 5272 RVA: 0x000743CC File Offset: 0x000725CC
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

		// Token: 0x06001499 RID: 5273 RVA: 0x00074430 File Offset: 0x00072630
		private float InputValueRateChange(bool isDown, float value)
		{
			float num = Time.deltaTime * 20f;
			float num2 = isDown ? 1f : -1f;
			return Mathf.Clamp01(value + num * num2);
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x00074464 File Offset: 0x00072664
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

		// Token: 0x0600149B RID: 5275 RVA: 0x00074560 File Offset: 0x00072760
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

		// Token: 0x0400175A RID: 5978
		public const string ANIM_LAYER_NAME_POINT = "Point Layer";

		// Token: 0x0400175B RID: 5979
		public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";

		// Token: 0x0400175C RID: 5980
		public const string ANIM_PARAM_NAME_FLEX = "Flex";

		// Token: 0x0400175D RID: 5981
		public const string ANIM_PARAM_NAME_POSE = "Pose";

		// Token: 0x0400175E RID: 5982
		public const float THRESH_COLLISION_FLEX = 0.9f;

		// Token: 0x0400175F RID: 5983
		public const float INPUT_RATE_CHANGE = 20f;

		// Token: 0x04001760 RID: 5984
		public const float COLLIDER_SCALE_MIN = 0.01f;

		// Token: 0x04001761 RID: 5985
		public const float COLLIDER_SCALE_MAX = 1f;

		// Token: 0x04001762 RID: 5986
		public const float COLLIDER_SCALE_PER_SECOND = 1f;

		// Token: 0x04001763 RID: 5987
		public const float TRIGGER_DEBOUNCE_TIME = 0.05f;

		// Token: 0x04001764 RID: 5988
		public const float THUMB_DEBOUNCE_TIME = 0.15f;

		// Token: 0x04001765 RID: 5989
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x04001766 RID: 5990
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04001767 RID: 5991
		[SerializeField]
		private HandPose m_defaultGrabPose;

		// Token: 0x04001768 RID: 5992
		private Collider[] m_colliders;

		// Token: 0x04001769 RID: 5993
		private bool m_collisionEnabled = true;

		// Token: 0x0400176A RID: 5994
		private OVRGrabber m_grabber;

		// Token: 0x0400176B RID: 5995
		private List<Renderer> m_showAfterInputFocusAcquired;

		// Token: 0x0400176C RID: 5996
		private int m_animLayerIndexThumb = -1;

		// Token: 0x0400176D RID: 5997
		private int m_animLayerIndexPoint = -1;

		// Token: 0x0400176E RID: 5998
		private int m_animParamIndexFlex = -1;

		// Token: 0x0400176F RID: 5999
		private int m_animParamIndexPose = -1;

		// Token: 0x04001770 RID: 6000
		private bool m_isPointing;

		// Token: 0x04001771 RID: 6001
		private bool m_isGivingThumbsUp;

		// Token: 0x04001772 RID: 6002
		private float m_pointBlend;

		// Token: 0x04001773 RID: 6003
		private float m_thumbsUpBlend;

		// Token: 0x04001774 RID: 6004
		private bool m_restoreOnInputAcquired;

		// Token: 0x04001775 RID: 6005
		private float m_collisionScaleCurrent;
	}
}
