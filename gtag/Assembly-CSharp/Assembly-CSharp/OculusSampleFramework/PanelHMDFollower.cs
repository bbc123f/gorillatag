using System;
using System.Collections;
using UnityEngine;

namespace OculusSampleFramework
{
	public class PanelHMDFollower : MonoBehaviour
	{
		private void Awake()
		{
			this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			this._panelInitialPosition = base.transform.position;
		}

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

		private Vector3 CalculateIdealAnchorPosition()
		{
			return this._cameraRig.centerEyeAnchor.position + this._panelInitialPosition;
		}

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

		private const float TOTAL_DURATION = 3f;

		private const float HMD_MOVEMENT_THRESHOLD = 0.3f;

		[SerializeField]
		private float _maxDistance = 0.3f;

		[SerializeField]
		private float _minDistance = 0.05f;

		[SerializeField]
		private float _minZDistance = 0.05f;

		private OVRCameraRig _cameraRig;

		private Vector3 _panelInitialPosition = Vector3.zero;

		private Coroutine _coroutine;

		private Vector3 _prevPos = Vector3.zero;

		private Vector3 _lastMovedToPos = Vector3.zero;
	}
}
