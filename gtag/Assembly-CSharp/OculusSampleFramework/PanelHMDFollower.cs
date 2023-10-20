using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E8 RID: 744
	public class PanelHMDFollower : MonoBehaviour
	{
		// Token: 0x06001419 RID: 5145 RVA: 0x00071FF2 File Offset: 0x000701F2
		private void Awake()
		{
			this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			this._panelInitialPosition = base.transform.position;
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x00072010 File Offset: 0x00070210
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

		// Token: 0x0600141B RID: 5147 RVA: 0x000720EA File Offset: 0x000702EA
		private Vector3 CalculateIdealAnchorPosition()
		{
			return this._cameraRig.centerEyeAnchor.position + this._panelInitialPosition;
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x00072107 File Offset: 0x00070307
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

		// Token: 0x040016C2 RID: 5826
		private const float TOTAL_DURATION = 3f;

		// Token: 0x040016C3 RID: 5827
		private const float HMD_MOVEMENT_THRESHOLD = 0.3f;

		// Token: 0x040016C4 RID: 5828
		[SerializeField]
		private float _maxDistance = 0.3f;

		// Token: 0x040016C5 RID: 5829
		[SerializeField]
		private float _minDistance = 0.05f;

		// Token: 0x040016C6 RID: 5830
		[SerializeField]
		private float _minZDistance = 0.05f;

		// Token: 0x040016C7 RID: 5831
		private OVRCameraRig _cameraRig;

		// Token: 0x040016C8 RID: 5832
		private Vector3 _panelInitialPosition = Vector3.zero;

		// Token: 0x040016C9 RID: 5833
		private Coroutine _coroutine;

		// Token: 0x040016CA RID: 5834
		private Vector3 _prevPos = Vector3.zero;

		// Token: 0x040016CB RID: 5835
		private Vector3 _lastMovedToPos = Vector3.zero;
	}
}
