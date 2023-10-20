using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002D3 RID: 723
	public class HandsManager : MonoBehaviour
	{
		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600136B RID: 4971 RVA: 0x00070120 File Offset: 0x0006E320
		// (set) Token: 0x0600136C RID: 4972 RVA: 0x0007012A File Offset: 0x0006E32A
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

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600136D RID: 4973 RVA: 0x00070135 File Offset: 0x0006E335
		// (set) Token: 0x0600136E RID: 4974 RVA: 0x0007013F File Offset: 0x0006E33F
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

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x0007014A File Offset: 0x0006E34A
		// (set) Token: 0x06001370 RID: 4976 RVA: 0x00070154 File Offset: 0x0006E354
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

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06001371 RID: 4977 RVA: 0x0007015F File Offset: 0x0006E35F
		// (set) Token: 0x06001372 RID: 4978 RVA: 0x00070169 File Offset: 0x0006E369
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

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06001373 RID: 4979 RVA: 0x00070174 File Offset: 0x0006E374
		// (set) Token: 0x06001374 RID: 4980 RVA: 0x0007017E File Offset: 0x0006E37E
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

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06001375 RID: 4981 RVA: 0x00070189 File Offset: 0x0006E389
		// (set) Token: 0x06001376 RID: 4982 RVA: 0x00070193 File Offset: 0x0006E393
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

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06001377 RID: 4983 RVA: 0x0007019E File Offset: 0x0006E39E
		// (set) Token: 0x06001378 RID: 4984 RVA: 0x000701A8 File Offset: 0x0006E3A8
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

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06001379 RID: 4985 RVA: 0x000701B3 File Offset: 0x0006E3B3
		// (set) Token: 0x0600137A RID: 4986 RVA: 0x000701BD File Offset: 0x0006E3BD
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

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x000701C8 File Offset: 0x0006E3C8
		// (set) Token: 0x0600137C RID: 4988 RVA: 0x000701D2 File Offset: 0x0006E3D2
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

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x000701DD File Offset: 0x0006E3DD
		// (set) Token: 0x0600137E RID: 4990 RVA: 0x000701E7 File Offset: 0x0006E3E7
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

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600137F RID: 4991 RVA: 0x000701F2 File Offset: 0x0006E3F2
		// (set) Token: 0x06001380 RID: 4992 RVA: 0x000701F9 File Offset: 0x0006E3F9
		public static HandsManager Instance { get; private set; }

		// Token: 0x06001381 RID: 4993 RVA: 0x00070204 File Offset: 0x0006E404
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

		// Token: 0x06001382 RID: 4994 RVA: 0x00070310 File Offset: 0x0006E510
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

		// Token: 0x06001383 RID: 4995 RVA: 0x0007038B File Offset: 0x0006E58B
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

		// Token: 0x06001384 RID: 4996 RVA: 0x0007039A File Offset: 0x0006E59A
		public void SwitchVisualization()
		{
			if (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				return;
			}
			this.VisualMode = (this.VisualMode + 1) % (HandsManager.HandsVisualMode)3;
			this.SetToCurrentVisualMode();
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x000703D0 File Offset: 0x0006E5D0
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

		// Token: 0x06001386 RID: 4998 RVA: 0x000704F0 File Offset: 0x0006E6F0
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

		// Token: 0x06001387 RID: 4999 RVA: 0x0007053C File Offset: 0x0006E73C
		public bool IsInitialized()
		{
			return this.LeftHandSkeleton && this.LeftHandSkeleton.IsInitialized && this.RightHandSkeleton && this.RightHandSkeleton.IsInitialized && this.LeftHandMesh && this.LeftHandMesh.IsInitialized && this.RightHandMesh && this.RightHandMesh.IsInitialized;
		}

		// Token: 0x04001649 RID: 5705
		private const string SKELETON_VISUALIZER_NAME = "SkeletonRenderer";

		// Token: 0x0400164A RID: 5706
		[SerializeField]
		private GameObject _leftHand;

		// Token: 0x0400164B RID: 5707
		[SerializeField]
		private GameObject _rightHand;

		// Token: 0x0400164C RID: 5708
		public HandsManager.HandsVisualMode VisualMode;

		// Token: 0x0400164D RID: 5709
		private OVRHand[] _hand = new OVRHand[2];

		// Token: 0x0400164E RID: 5710
		private OVRSkeleton[] _handSkeleton = new OVRSkeleton[2];

		// Token: 0x0400164F RID: 5711
		private OVRSkeletonRenderer[] _handSkeletonRenderer = new OVRSkeletonRenderer[2];

		// Token: 0x04001650 RID: 5712
		private OVRMesh[] _handMesh = new OVRMesh[2];

		// Token: 0x04001651 RID: 5713
		private OVRMeshRenderer[] _handMeshRenderer = new OVRMeshRenderer[2];

		// Token: 0x04001652 RID: 5714
		private SkinnedMeshRenderer _leftMeshRenderer;

		// Token: 0x04001653 RID: 5715
		private SkinnedMeshRenderer _rightMeshRenderer;

		// Token: 0x04001654 RID: 5716
		private GameObject _leftSkeletonVisual;

		// Token: 0x04001655 RID: 5717
		private GameObject _rightSkeletonVisual;

		// Token: 0x04001656 RID: 5718
		private float _currentHandAlpha = 1f;

		// Token: 0x04001657 RID: 5719
		private int HandAlphaId = Shader.PropertyToID("_HandAlpha");

		// Token: 0x020004E8 RID: 1256
		public enum HandsVisualMode
		{
			// Token: 0x04002070 RID: 8304
			Mesh,
			// Token: 0x04002071 RID: 8305
			Skeleton,
			// Token: 0x04002072 RID: 8306
			Both
		}
	}
}
