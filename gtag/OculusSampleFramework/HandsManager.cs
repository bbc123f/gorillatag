using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	public class HandsManager : MonoBehaviour
	{
		public OVRHand RightHand
		{
			get
			{
				return this._hand[1];
			}
			private set
			{
				this._hand[1] = value;
			}
		}

		public OVRSkeleton RightHandSkeleton
		{
			get
			{
				return this._handSkeleton[1];
			}
			private set
			{
				this._handSkeleton[1] = value;
			}
		}

		public OVRSkeletonRenderer RightHandSkeletonRenderer
		{
			get
			{
				return this._handSkeletonRenderer[1];
			}
			private set
			{
				this._handSkeletonRenderer[1] = value;
			}
		}

		public OVRMesh RightHandMesh
		{
			get
			{
				return this._handMesh[1];
			}
			private set
			{
				this._handMesh[1] = value;
			}
		}

		public OVRMeshRenderer RightHandMeshRenderer
		{
			get
			{
				return this._handMeshRenderer[1];
			}
			private set
			{
				this._handMeshRenderer[1] = value;
			}
		}

		public OVRHand LeftHand
		{
			get
			{
				return this._hand[0];
			}
			private set
			{
				this._hand[0] = value;
			}
		}

		public OVRSkeleton LeftHandSkeleton
		{
			get
			{
				return this._handSkeleton[0];
			}
			private set
			{
				this._handSkeleton[0] = value;
			}
		}

		public OVRSkeletonRenderer LeftHandSkeletonRenderer
		{
			get
			{
				return this._handSkeletonRenderer[0];
			}
			private set
			{
				this._handSkeletonRenderer[0] = value;
			}
		}

		public OVRMesh LeftHandMesh
		{
			get
			{
				return this._handMesh[0];
			}
			private set
			{
				this._handMesh[0] = value;
			}
		}

		public OVRMeshRenderer LeftHandMeshRenderer
		{
			get
			{
				return this._handMeshRenderer[0];
			}
			private set
			{
				this._handMeshRenderer[0] = value;
			}
		}

		public static HandsManager Instance { get; private set; }

		private void Awake()
		{
			if (HandsManager.Instance && HandsManager.Instance != this)
			{
				Object.Destroy(this);
				return;
			}
			HandsManager.Instance = this;
			this.LeftHand = this._leftHand.GetComponent<OVRHand>();
			this.LeftHandSkeleton = this._leftHand.GetComponent<OVRSkeleton>();
			this.LeftHandSkeletonRenderer = this._leftHand.GetComponent<OVRSkeletonRenderer>();
			this.LeftHandMesh = this._leftHand.GetComponent<OVRMesh>();
			this.LeftHandMeshRenderer = this._leftHand.GetComponent<OVRMeshRenderer>();
			this.RightHand = this._rightHand.GetComponent<OVRHand>();
			this.RightHandSkeleton = this._rightHand.GetComponent<OVRSkeleton>();
			this.RightHandSkeletonRenderer = this._rightHand.GetComponent<OVRSkeletonRenderer>();
			this.RightHandMesh = this._rightHand.GetComponent<OVRMesh>();
			this.RightHandMeshRenderer = this._rightHand.GetComponent<OVRMeshRenderer>();
			this._leftMeshRenderer = this.LeftHand.GetComponent<SkinnedMeshRenderer>();
			this._rightMeshRenderer = this.RightHand.GetComponent<SkinnedMeshRenderer>();
			base.StartCoroutine(this.FindSkeletonVisualGameObjects());
		}

		private void Update()
		{
			HandsManager.HandsVisualMode visualMode = this.VisualMode;
			if (visualMode > HandsManager.HandsVisualMode.Skeleton)
			{
				if (visualMode != HandsManager.HandsVisualMode.Both)
				{
					this._currentHandAlpha = 1f;
				}
				else
				{
					this._currentHandAlpha = 0.6f;
				}
			}
			else
			{
				this._currentHandAlpha = 1f;
			}
			this._rightMeshRenderer.sharedMaterial.SetFloat(this.HandAlphaId, this._currentHandAlpha);
			this._leftMeshRenderer.sharedMaterial.SetFloat(this.HandAlphaId, this._currentHandAlpha);
		}

		private IEnumerator FindSkeletonVisualGameObjects()
		{
			while (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				if (!this._leftSkeletonVisual)
				{
					Transform transform = this.LeftHand.transform.Find("SkeletonRenderer");
					if (transform)
					{
						this._leftSkeletonVisual = transform.gameObject;
					}
				}
				if (!this._rightSkeletonVisual)
				{
					Transform transform2 = this.RightHand.transform.Find("SkeletonRenderer");
					if (transform2)
					{
						this._rightSkeletonVisual = transform2.gameObject;
					}
				}
				yield return null;
			}
			this.SetToCurrentVisualMode();
			yield break;
		}

		public void SwitchVisualization()
		{
			if (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				return;
			}
			this.VisualMode = (this.VisualMode + 1) % (HandsManager.HandsVisualMode)3;
			this.SetToCurrentVisualMode();
		}

		private void SetToCurrentVisualMode()
		{
			switch (this.VisualMode)
			{
			case HandsManager.HandsVisualMode.Mesh:
				this.RightHandMeshRenderer.enabled = true;
				this._rightMeshRenderer.enabled = true;
				this._rightSkeletonVisual.gameObject.SetActive(false);
				this.LeftHandMeshRenderer.enabled = true;
				this._leftMeshRenderer.enabled = true;
				this._leftSkeletonVisual.gameObject.SetActive(false);
				return;
			case HandsManager.HandsVisualMode.Skeleton:
				this.RightHandMeshRenderer.enabled = false;
				this._rightMeshRenderer.enabled = false;
				this._rightSkeletonVisual.gameObject.SetActive(true);
				this.LeftHandMeshRenderer.enabled = false;
				this._leftMeshRenderer.enabled = false;
				this._leftSkeletonVisual.gameObject.SetActive(true);
				return;
			case HandsManager.HandsVisualMode.Both:
				this.RightHandMeshRenderer.enabled = true;
				this._rightMeshRenderer.enabled = true;
				this._rightSkeletonVisual.gameObject.SetActive(true);
				this.LeftHandMeshRenderer.enabled = true;
				this._leftMeshRenderer.enabled = true;
				this._leftSkeletonVisual.gameObject.SetActive(true);
				return;
			default:
				return;
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
			return this.LeftHandSkeleton && this.LeftHandSkeleton.IsInitialized && this.RightHandSkeleton && this.RightHandSkeleton.IsInitialized && this.LeftHandMesh && this.LeftHandMesh.IsInitialized && this.RightHandMesh && this.RightHandMesh.IsInitialized;
		}

		private const string SKELETON_VISUALIZER_NAME = "SkeletonRenderer";

		[SerializeField]
		private GameObject _leftHand;

		[SerializeField]
		private GameObject _rightHand;

		public HandsManager.HandsVisualMode VisualMode;

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

		public enum HandsVisualMode
		{
			Mesh,
			Skeleton,
			Both
		}
	}
}
