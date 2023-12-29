using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	public class PlanarSound : MonoBehaviour
	{
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		protected void LateUpdate()
		{
			if (!this.hasCamera)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 localPosition = transform.parent.InverseTransformPoint(this.cameraXform.position);
			localPosition.y = 0f;
			transform.localPosition = localPosition;
		}

		private Transform cameraXform;

		private bool hasCamera;
	}
}
