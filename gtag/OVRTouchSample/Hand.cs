using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OVRTouchSample;

[RequireComponent(typeof(OVRGrabber))]
public class Hand : MonoBehaviour
{
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

	private void Awake()
	{
		m_grabber = GetComponent<OVRGrabber>();
	}

	private void Start()
	{
		m_showAfterInputFocusAcquired = new List<Renderer>();
		m_colliders = (from childCollider in GetComponentsInChildren<Collider>()
			where !childCollider.isTrigger
			select childCollider).ToArray();
		CollisionEnable(enabled: false);
		m_animLayerIndexPoint = m_animator.GetLayerIndex("Point Layer");
		m_animLayerIndexThumb = m_animator.GetLayerIndex("Thumb Layer");
		m_animParamIndexFlex = Animator.StringToHash("Flex");
		m_animParamIndexPose = Animator.StringToHash("Pose");
		OVRManager.InputFocusAcquired += OnInputFocusAcquired;
		OVRManager.InputFocusLost += OnInputFocusLost;
	}

	private void OnDestroy()
	{
		OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
		OVRManager.InputFocusLost -= OnInputFocusLost;
	}

	private void Update()
	{
		UpdateCapTouchStates();
		m_pointBlend = InputValueRateChange(m_isPointing, m_pointBlend);
		m_thumbsUpBlend = InputValueRateChange(m_isGivingThumbsUp, m_thumbsUpBlend);
		float num = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
		bool flag = m_grabber.grabbedObject == null && num >= 0.9f;
		CollisionEnable(flag);
		UpdateAnimStates();
	}

	private void UpdateCapTouchStates()
	{
		m_isPointing = !OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, m_controller);
		m_isGivingThumbsUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, m_controller);
	}

	private void LateUpdate()
	{
		if (m_collisionEnabled && m_collisionScaleCurrent + Mathf.Epsilon < 1f)
		{
			m_collisionScaleCurrent = Mathf.Min(1f, m_collisionScaleCurrent + Time.deltaTime * 1f);
			for (int i = 0; i < m_colliders.Length; i++)
			{
				m_colliders[i].transform.localScale = new Vector3(m_collisionScaleCurrent, m_collisionScaleCurrent, m_collisionScaleCurrent);
			}
		}
	}

	private void OnInputFocusLost()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		m_showAfterInputFocusAcquired.Clear();
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].enabled)
			{
				componentsInChildren[i].enabled = false;
				m_showAfterInputFocusAcquired.Add(componentsInChildren[i]);
			}
		}
		CollisionEnable(enabled: false);
		m_restoreOnInputAcquired = true;
	}

	private void OnInputFocusAcquired()
	{
		if (!m_restoreOnInputAcquired)
		{
			return;
		}
		for (int i = 0; i < m_showAfterInputFocusAcquired.Count; i++)
		{
			if ((bool)m_showAfterInputFocusAcquired[i])
			{
				m_showAfterInputFocusAcquired[i].enabled = true;
			}
		}
		m_showAfterInputFocusAcquired.Clear();
		m_restoreOnInputAcquired = false;
	}

	private float InputValueRateChange(bool isDown, float value)
	{
		float num = Time.deltaTime * 20f;
		float num2 = (isDown ? 1f : (-1f));
		return Mathf.Clamp01(value + num * num2);
	}

	private void UpdateAnimStates()
	{
		bool num = m_grabber.grabbedObject != null;
		HandPose handPose = m_defaultGrabPose;
		if (num)
		{
			HandPose component = m_grabber.grabbedObject.GetComponent<HandPose>();
			if (component != null)
			{
				handPose = component;
			}
		}
		HandPoseId poseId = handPose.PoseId;
		m_animator.SetInteger(m_animParamIndexPose, (int)poseId);
		float value = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
		m_animator.SetFloat(m_animParamIndexFlex, value);
		float weight = ((!num || handPose.AllowPointing) ? m_pointBlend : 0f);
		m_animator.SetLayerWeight(m_animLayerIndexPoint, weight);
		float weight2 = ((!num || handPose.AllowThumbsUp) ? m_thumbsUpBlend : 0f);
		m_animator.SetLayerWeight(m_animLayerIndexThumb, weight2);
		float value2 = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);
		m_animator.SetFloat("Pinch", value2);
	}

	private void CollisionEnable(bool enabled)
	{
		if (m_collisionEnabled == enabled)
		{
			return;
		}
		m_collisionEnabled = enabled;
		if (enabled)
		{
			m_collisionScaleCurrent = 0.01f;
			for (int i = 0; i < m_colliders.Length; i++)
			{
				Collider obj = m_colliders[i];
				obj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
				obj.enabled = true;
			}
		}
		else
		{
			m_collisionScaleCurrent = 1f;
			for (int j = 0; j < m_colliders.Length; j++)
			{
				Collider obj2 = m_colliders[j];
				obj2.enabled = false;
				obj2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			}
		}
	}
}
