using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

public class GorillaVRConstraint : MonoBehaviour
{
	public bool isConstrained;

	public float angle = 3600f;

	private void Update()
	{
		if (PhotonNetworkController.Instance.wrongVersion)
		{
			isConstrained = true;
		}
		if (isConstrained && Time.realtimeSinceStartup > angle)
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
}
