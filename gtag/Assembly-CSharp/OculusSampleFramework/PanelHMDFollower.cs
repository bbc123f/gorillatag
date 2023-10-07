using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E6 RID: 742
	public class PanelHMDFollower : MonoBehaviour
	{
		// Token: 0x06001412 RID: 5138 RVA: 0x00071B26 File Offset: 0x0006FD26
		private void Awake()
		{
			this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			this._panelInitialPosition = base.transform.position;
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x00071B44 File Offset: 0x0006FD44
		private void Update()
		{
			Vector3 position = this._cameraRig.centerEyeAnchor.position;
			Vector3 position2 = base.transform.position;
			float num = Vector3.Distance(position, this._lastMovedToPos);
			float num2 = (this._cameraRig.centerEyeAnchor.position - this._prevPos).magnitude / Time.deltaTime;
			Vector3 vector = base.transform.position - position;
			float magnitude = vector.magnitude;
			if ((num > this._maxDistance || this._minZDistance > vector.z || this._minDistance > magnitude) && num2 < 0.3f && this._coroutine == null && this._coroutine == null)
			{
				this._coroutine = base.StartCoroutine(this.LerpToHMD());
			}
			this._prevPos = this._cameraRig.centerEyeAnchor.position;
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x00071C1E File Offset: 0x0006FE1E
		private Vector3 CalculateIdealAnchorPosition()
		{
			return this._cameraRig.centerEyeAnchor.position + this._panelInitialPosition;
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x00071C3B File Offset: 0x0006FE3B
		private IEnumerator LerpToHMD()
		{
			Vector3 newPanelPosition = this.CalculateIdealAnchorPosition();
			this._lastMovedToPos = this._cameraRig.centerEyeAnchor.position;
			float startTime = Time.time;
			float endTime = Time.time + 3f;
			while (Time.time < endTime)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, newPanelPosition, (Time.time - startTime) / 3f);
				yield return null;
			}
			base.transform.position = newPanelPosition;
			this._coroutine = null;
			yield break;
		}

		// Token: 0x040016B5 RID: 5813
		private const float TOTAL_DURATION = 3f;

		// Token: 0x040016B6 RID: 5814
		private const float HMD_MOVEMENT_THRESHOLD = 0.3f;

		// Token: 0x040016B7 RID: 5815
		[SerializeField]
		private float _maxDistance = 0.3f;

		// Token: 0x040016B8 RID: 5816
		[SerializeField]
		private float _minDistance = 0.05f;

		// Token: 0x040016B9 RID: 5817
		[SerializeField]
		private float _minZDistance = 0.05f;

		// Token: 0x040016BA RID: 5818
		private OVRCameraRig _cameraRig;

		// Token: 0x040016BB RID: 5819
		private Vector3 _panelInitialPosition = Vector3.zero;

		// Token: 0x040016BC RID: 5820
		private Coroutine _coroutine;

		// Token: 0x040016BD RID: 5821
		private Vector3 _prevPos = Vector3.zero;

		// Token: 0x040016BE RID: 5822
		private Vector3 _lastMovedToPos = Vector3.zero;
	}
}
