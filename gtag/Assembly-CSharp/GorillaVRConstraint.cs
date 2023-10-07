using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class GorillaVRConstraint : MonoBehaviour
{
	// Token: 0x06000A28 RID: 2600 RVA: 0x0003EDD4 File Offset: 0x0003CFD4
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

	// Token: 0x04000C92 RID: 3218
	public bool isConstrained;

	// Token: 0x04000C93 RID: 3219
	public float angle = 3600f;
}
