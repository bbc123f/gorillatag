using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework;

public class HandsManager : MonoBehaviour
{
	public enum HandsVisualMode
	{
		Mesh,
		Skeleton,
		Both
	}

	private const string SKELETON_VISUALIZER_NAME = "SkeletonRenderer";

	[SerializeField]
	private GameObject _leftHand;

	[SerializeField]
	private GameObject _rightHand;

	public HandsVisualMode VisualMode;

	private OVRHand[] _hand = new OVRHand[2];

	private OVRSkeleton[] _handSkeleton = new OVRSkeleton[2];

	private OVRSkeletonRenderer[] _handSkeletonRenderer = new OVRSkeletonRenderer[2];

	private OVRMesh[] _handMesh = new OVRMesh[2];

	private OVRMeshRenderer[] _handMeshRenderer = new OVRMeshRenderer[2];

	private SkinnedMeshRenderer _leftMeshRenderer;

	private SkinnedMeshRenderer _rightMeshRenderer;

	private GameObject _leftSkeletonVisual;

	private GameObject _rightSkeletonVisual;

	private float _currentHandAlpha = 1f;

	private int HandAlphaId = Shader.PropertyToID("_HandAlpha");

	public OVRHand RightHand
	{
		get
		{
			return _hand[1];
		}
		private set
		{
			_hand[1] = value;
		}
	}

	public OVRSkeleton RightHandSkeleton
	{
		get
		{
			return _handSkeleton[1];
		}
		private set
		{
			_handSkeleton[1] = value;
		}
	}

	public OVRSkeletonRenderer RightHandSkeletonRenderer
	{
		get
		{
			return _handSkeletonRenderer[1];
		}
		private set
		{
			_handSkeletonRenderer[1] = value;
		}
	}

	public OVRMesh RightHandMesh
	{
		get
		{
			return _handMesh[1];
		}
		private set
		{
			_handMesh[1] = value;
		}
	}

	public OVRMeshRenderer RightHandMeshRenderer
	{
		get
		{
			return _handMeshRenderer[1];
		}
		private set
		{
			_handMeshRenderer[1] = value;
		}
	}

	public OVRHand LeftHand
	{
		get
		{
			return _hand[0];
		}
		private set
		{
			_hand[0] = value;
		}
	}

	public OVRSkeleton LeftHandSkeleton
	{
		get
		{
			return _handSkeleton[0];
		}
		private set
		{
			_handSkeleton[0] = value;
		}
	}

	public OVRSkeletonRenderer LeftHandSkeletonRenderer
	{
		get
		{
			return _handSkeletonRenderer[0];
		}
		private set
		{
			_handSkeletonRenderer[0] = value;
		}
	}

	public OVRMesh LeftHandMesh
	{
		get
		{
			return _handMesh[0];
		}
		private set
		{
			_handMesh[0] = value;
		}
	}

	public OVRMeshRenderer LeftHandMeshRenderer
	{
		get
		{
			return _handMeshRenderer[0];
		}
		private set
		{
			_handMeshRenderer[0] = value;
		}
	}

	public static HandsManager Instance { get; private set; }

	private void Awake()
	{
		if ((bool)Instance && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		LeftHand = _leftHand.GetComponent<OVRHand>();
		LeftHandSkeleton = _leftHand.GetComponent<OVRSkeleton>();
		LeftHandSkeletonRenderer = _leftHand.GetComponent<OVRSkeletonRenderer>();
		LeftHandMesh = _leftHand.GetComponent<OVRMesh>();
		LeftHandMeshRenderer = _leftHand.GetComponent<OVRMeshRenderer>();
		RightHand = _rightHand.GetComponent<OVRHand>();
		RightHandSkeleton = _rightHand.GetComponent<OVRSkeleton>();
		RightHandSkeletonRenderer = _rightHand.GetComponent<OVRSkeletonRenderer>();
		RightHandMesh = _rightHand.GetComponent<OVRMesh>();
		RightHandMeshRenderer = _rightHand.GetComponent<OVRMeshRenderer>();
		_leftMeshRenderer = LeftHand.GetComponent<SkinnedMeshRenderer>();
		_rightMeshRenderer = RightHand.GetComponent<SkinnedMeshRenderer>();
		StartCoroutine(FindSkeletonVisualGameObjects());
	}

	private void Update()
	{
		switch (VisualMode)
		{
		case HandsVisualMode.Mesh:
		case HandsVisualMode.Skeleton:
			_currentHandAlpha = 1f;
			break;
		case HandsVisualMode.Both:
			_currentHandAlpha = 0.6f;
			break;
		default:
			_currentHandAlpha = 1f;
			break;
		}
		_rightMeshRenderer.sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
		_leftMeshRenderer.sharedMaterial.SetFloat(HandAlphaId, _currentHandAlpha);
	}

	private IEnumerator FindSkeletonVisualGameObjects()
	{
		while (!_leftSkeletonVisual || !_rightSkeletonVisual)
		{
			if (!_leftSkeletonVisual)
			{
				Transform transform = LeftHand.transform.Find("SkeletonRenderer");
				if ((bool)transform)
				{
					_leftSkeletonVisual = transform.gameObject;
				}
			}
			if (!_rightSkeletonVisual)
			{
				Transform transform2 = RightHand.transform.Find("SkeletonRenderer");
				if ((bool)transform2)
				{
					_rightSkeletonVisual = transform2.gameObject;
				}
			}
			yield return null;
		}
		SetToCurrentVisualMode();
	}

	public void SwitchVisualization()
	{
		if ((bool)_leftSkeletonVisual && (bool)_rightSkeletonVisual)
		{
			VisualMode = (HandsVisualMode)((int)(VisualMode + 1) % 3);
			SetToCurrentVisualMode();
		}
	}

	private void SetToCurrentVisualMode()
	{
		switch (VisualMode)
		{
		case HandsVisualMode.Mesh:
			RightHandMeshRenderer.enabled = true;
			_rightMeshRenderer.enabled = true;
			_rightSkeletonVisual.gameObject.SetActive(value: false);
			LeftHandMeshRenderer.enabled = true;
			_leftMeshRenderer.enabled = true;
			_leftSkeletonVisual.gameObject.SetActive(value: false);
			break;
		case HandsVisualMode.Skeleton:
			RightHandMeshRenderer.enabled = false;
			_rightMeshRenderer.enabled = false;
			_rightSkeletonVisual.gameObject.SetActive(value: true);
			LeftHandMeshRenderer.enabled = false;
			_leftMeshRenderer.enabled = false;
			_leftSkeletonVisual.gameObject.SetActive(value: true);
			break;
		case HandsVisualMode.Both:
			RightHandMeshRenderer.enabled = true;
			_rightMeshRenderer.enabled = true;
			_rightSkeletonVisual.gameObject.SetActive(value: true);
			LeftHandMeshRenderer.enabled = true;
			_leftMeshRenderer.enabled = true;
			_leftSkeletonVisual.gameObject.SetActive(value: true);
			break;
		}
	}

	public static List<OVRBoneCapsule> GetCapsulesPerBone(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId)
	{
		List<OVRBoneCapsule> list = new List<OVRBoneCapsule>();
		IList<OVRBoneCapsule> capsules = skeleton.Capsules;
		for (int i = 0; i < capsules.Count; i++)
		{
			if (capsules[i].BoneIndex == (short)boneId)
			{
				list.Add(capsules[i]);
			}
		}
		return list;
	}

	public bool IsInitialized()
	{
		if ((bool)LeftHandSkeleton && LeftHandSkeleton.IsInitialized && (bool)RightHandSkeleton && RightHandSkeleton.IsInitialized && (bool)LeftHandMesh && LeftHandMesh.IsInitialized && (bool)RightHandMesh)
		{
			return RightHandMesh.IsInitialized;
		}
		return false;
	}
}
