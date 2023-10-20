using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class GorillaVRConstraint : MonoBehaviour
{
	// Token: 0x06000A2D RID: 2605 RVA: 0x0003EF04 File Offset: 0x0003D104
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

	// Token: 0x04000C96 RID: 3222
	public bool isConstrained;

	// Token: 0x04000C97 RID: 3223
	public float angle = 3600f;
}
