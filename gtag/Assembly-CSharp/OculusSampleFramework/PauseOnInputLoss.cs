using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class PauseOnInputLoss : MonoBehaviour
	{
		private void Start()
		{
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		private void OnInputFocusLost()
		{
			Time.timeScale = 0f;
		}

		private void OnInputFocusAcquired()
		{
			Time.timeScale = 1f;
		}

		public PauseOnInputLoss()
		{
		}
	}
}
