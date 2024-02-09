using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

public class GorillaVRConstraint : MonoBehaviour
{
	private void Update()
	{
		if (PhotonNetworkController.Instance.wrongVersion)
		{
			this.isConstrained = true;
		}
		if (this.isConstrained && Time.realtimeSinceStartup > this.angle)
		{
			Application.Quit();
			Object.DestroyImmediate(PhotonNetworkController.Instance);
			Object.DestroyImmediate(Player.Instance);
			GameObject[] array = Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
			}
		}
	}

	public bool isConstrained;

	public float angle = 3600f;
}
