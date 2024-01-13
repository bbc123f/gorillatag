using UnityEngine;

namespace OculusSampleFramework;

public class PauseOnInputLoss : MonoBehaviour
{
	private void Start()
	{
		OVRManager.InputFocusAcquired += OnInputFocusAcquired;
		OVRManager.InputFocusLost += OnInputFocusLost;
	}

	private void OnInputFocusLost()
	{
		Time.timeScale = 0f;
	}

	private void OnInputFocusAcquired()
	{
		Time.timeScale = 1f;
	}
}
