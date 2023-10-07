using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x020002F4 RID: 756
	[RequireComponent(typeof(OVRGrabber))]
	public class Hand : MonoBehaviour
	{
		// Token: 0x0600148A RID: 5258 RVA: 0x00073C68 File Offset: 0x00071E68
		private void Awake()
		{
			this.m_grabber = base.GetComponent<OVRGrabber>();
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x00073C78 File Offset: 0x00071E78
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

		// Token: 0x0600148C RID: 5260 RVA: 0x00073D3A File Offset: 0x00071F3A
		private void OnDestroy()
		{
			OVRManager.InputFocusAcquired -= this.OnInputFocusAcquired;
			OVRManager.InputFocusLost -= this.OnInputFocusLost;
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x00073D60 File Offset: 0x00071F60
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

		// Token: 0x0600148E RID: 5262 RVA: 0x00073DDF File Offset: 0x00071FDF
		private void UpdateCapTouchStates()
		{
			this.m_isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, this.m_controller);
			this.m_isGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, this.m_controller);
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x00073E0C File Offset: 0x0007200C
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

		// Token: 0x06001490 RID: 5264 RVA: 0x00073E94 File Offset: 0x00072094
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

		// Token: 0x06001491 RID: 5265 RVA: 0x00073F00 File Offset: 0x00072100
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

		// Token: 0x06001492 RID: 5266 RVA: 0x00073F64 File Offset: 0x00072164
		private float InputValueRateChange(bool isDown, float value)
		{
			float num = Time.deltaTime * 20f;
			float num2 = isDown ? 1f : -1f;
			return Mathf.Clamp01(value + num * num2);
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x00073F98 File Offset: 0x00072198
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

		// Token: 0x06001494 RID: 5268 RVA: 0x00074094 File Offset: 0x00072294
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

		// Token: 0x0400174D RID: 5965
		public const string ANIM_LAYER_NAME_POINT = "Point Layer";

		// Token: 0x0400174E RID: 5966
		public const string ANIM_LAYER_NAME_THUMB = "Thumb Layer";

		// Token: 0x0400174F RID: 5967
		public const string ANIM_PARAM_NAME_FLEX = "Flex";

		// Token: 0x04001750 RID: 5968
		public const string ANIM_PARAM_NAME_POSE = "Pose";

		// Token: 0x04001751 RID: 5969
		public const float THRESH_COLLISION_FLEX = 0.9f;

		// Token: 0x04001752 RID: 5970
		public const float INPUT_RATE_CHANGE = 20f;

		// Token: 0x04001753 RID: 5971
		public const float COLLIDER_SCALE_MIN = 0.01f;

		// Token: 0x04001754 RID: 5972
		public const float COLLIDER_SCALE_MAX = 1f;

		// Token: 0x04001755 RID: 5973
		public const float COLLIDER_SCALE_PER_SECOND = 1f;

		// Token: 0x04001756 RID: 5974
		public const float TRIGGER_DEBOUNCE_TIME = 0.05f;

		// Token: 0x04001757 RID: 5975
		public const float THUMB_DEBOUNCE_TIME = 0.15f;

		// Token: 0x04001758 RID: 5976
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x04001759 RID: 5977
		[SerializeField]
		private Animator m_animator;

		// Token: 0x0400175A RID: 5978
		[SerializeField]
		private HandPose m_defaultGrabPose;

		// Token: 0x0400175B RID: 5979
		private Collider[] m_colliders;

		// Token: 0x0400175C RID: 5980
		private bool m_collisionEnabled = true;

		// Token: 0x0400175D RID: 5981
		private OVRGrabber m_grabber;

		// Token: 0x0400175E RID: 5982
		private List<Renderer> m_showAfterInputFocusAcquired;

		// Token: 0x0400175F RID: 5983
		private int m_animLayerIndexThumb = -1;

		// Token: 0x04001760 RID: 5984
		private int m_animLayerIndexPoint = -1;

		// Token: 0x04001761 RID: 5985
		private int m_animParamIndexFlex = -1;

		// Token: 0x04001762 RID: 5986
		private int m_animParamIndexPose = -1;

		// Token: 0x04001763 RID: 5987
		private bool m_isPointing;

		// Token: 0x04001764 RID: 5988
		private bool m_isGivingThumbsUp;

		// Token: 0x04001765 RID: 5989
		private float m_pointBlend;

		// Token: 0x04001766 RID: 5990
		private float m_thumbsUpBlend;

		// Token: 0x04001767 RID: 5991
		private bool m_restoreOnInputAcquired;

		// Token: 0x04001768 RID: 5992
		private float m_collisionScaleCurrent;
	}
}
