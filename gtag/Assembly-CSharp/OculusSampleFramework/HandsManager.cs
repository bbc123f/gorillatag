using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002D1 RID: 721
	public class HandsManager : MonoBehaviour
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x0006FC54 File Offset: 0x0006DE54
		// (set) Token: 0x06001365 RID: 4965 RVA: 0x0006FC5E File Offset: 0x0006DE5E
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

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06001366 RID: 4966 RVA: 0x0006FC69 File Offset: 0x0006DE69
		// (set) Token: 0x06001367 RID: 4967 RVA: 0x0006FC73 File Offset: 0x0006DE73
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

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06001368 RID: 4968 RVA: 0x0006FC7E File Offset: 0x0006DE7E
		// (set) Token: 0x06001369 RID: 4969 RVA: 0x0006FC88 File Offset: 0x0006DE88
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

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600136A RID: 4970 RVA: 0x0006FC93 File Offset: 0x0006DE93
		// (set) Token: 0x0600136B RID: 4971 RVA: 0x0006FC9D File Offset: 0x0006DE9D
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

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600136C RID: 4972 RVA: 0x0006FCA8 File Offset: 0x0006DEA8
		// (set) Token: 0x0600136D RID: 4973 RVA: 0x0006FCB2 File Offset: 0x0006DEB2
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

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x0006FCBD File Offset: 0x0006DEBD
		// (set) Token: 0x0600136F RID: 4975 RVA: 0x0006FCC7 File Offset: 0x0006DEC7
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

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06001370 RID: 4976 RVA: 0x0006FCD2 File Offset: 0x0006DED2
		// (set) Token: 0x06001371 RID: 4977 RVA: 0x0006FCDC File Offset: 0x0006DEDC
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

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06001372 RID: 4978 RVA: 0x0006FCE7 File Offset: 0x0006DEE7
		// (set) Token: 0x06001373 RID: 4979 RVA: 0x0006FCF1 File Offset: 0x0006DEF1
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

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06001374 RID: 4980 RVA: 0x0006FCFC File Offset: 0x0006DEFC
		// (set) Token: 0x06001375 RID: 4981 RVA: 0x0006FD06 File Offset: 0x0006DF06
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

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06001376 RID: 4982 RVA: 0x0006FD11 File Offset: 0x0006DF11
		// (set) Token: 0x06001377 RID: 4983 RVA: 0x0006FD1B File Offset: 0x0006DF1B
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

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06001378 RID: 4984 RVA: 0x0006FD26 File Offset: 0x0006DF26
		// (set) Token: 0x06001379 RID: 4985 RVA: 0x0006FD2D File Offset: 0x0006DF2D
		public static HandsManager Instance { get; private set; }

		// Token: 0x0600137A RID: 4986 RVA: 0x0006FD38 File Offset: 0x0006DF38
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

		// Token: 0x0600137B RID: 4987 RVA: 0x0006FE44 File Offset: 0x0006E044
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

		// Token: 0x0600137C RID: 4988 RVA: 0x0006FEBF File Offset: 0x0006E0BF
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

		// Token: 0x0600137D RID: 4989 RVA: 0x0006FECE File Offset: 0x0006E0CE
		public void SwitchVisualization()
		{
			if (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				return;
			}
			this.VisualMode = (this.VisualMode + 1) % (HandsManager.HandsVisualMode)3;
			this.SetToCurrentVisualMode();
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x0006FF04 File Offset: 0x0006E104
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

		// Token: 0x0600137F RID: 4991 RVA: 0x00070024 File Offset: 0x0006E224
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

		// Token: 0x06001380 RID: 4992 RVA: 0x00070070 File Offset: 0x0006E270
		public bool IsInitialized()
		{
			return this.LeftHandSkeleton && this.LeftHandSkeleton.IsInitialized && this.RightHandSkeleton && this.RightHandSkeleton.IsInitialized && this.LeftHandMesh && this.LeftHandMesh.IsInitialized && this.RightHandMesh && this.RightHandMesh.IsInitialized;
		}

		// Token: 0x0400163C RID: 5692
		private const string SKELETON_VISUALIZER_NAME = "SkeletonRenderer";

		// Token: 0x0400163D RID: 5693
		[SerializeField]
		private GameObject _leftHand;

		// Token: 0x0400163E RID: 5694
		[SerializeField]
		private GameObject _rightHand;

		// Token: 0x0400163F RID: 5695
		public HandsManager.HandsVisualMode VisualMode;

		// Token: 0x04001640 RID: 5696
		private OVRHand[] _hand = new OVRHand[2];

		// Token: 0x04001641 RID: 5697
		private OVRSkeleton[] _handSkeleton = new OVRSkeleton[2];

		// Token: 0x04001642 RID: 5698
		private OVRSkeletonRenderer[] _handSkeletonRenderer = new OVRSkeletonRenderer[2];

		// Token: 0x04001643 RID: 5699
		private OVRMesh[] _handMesh = new OVRMesh[2];

		// Token: 0x04001644 RID: 5700
		private OVRMeshRenderer[] _handMeshRenderer = new OVRMeshRenderer[2];

		// Token: 0x04001645 RID: 5701
		private SkinnedMeshRenderer _leftMeshRenderer;

		// Token: 0x04001646 RID: 5702
		private SkinnedMeshRenderer _rightMeshRenderer;

		// Token: 0x04001647 RID: 5703
		private GameObject _leftSkeletonVisual;

		// Token: 0x04001648 RID: 5704
		private GameObject _rightSkeletonVisual;

		// Token: 0x04001649 RID: 5705
		private float _currentHandAlpha = 1f;

		// Token: 0x0400164A RID: 5706
		private int HandAlphaId = Shader.PropertyToID("_HandAlpha");

		// Token: 0x020004E6 RID: 1254
		public enum HandsVisualMode
		{
			// Token: 0x04002063 RID: 8291
			Mesh,
			// Token: 0x04002064 RID: 8292
			Skeleton,
			// Token: 0x04002065 RID: 8293
			Both
		}
	}
}
