using System;
using Cinemachine;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000139 RID: 313
public class GorillaCameraFollow : MonoBehaviour
{
	// Token: 0x06000815 RID: 2069 RVA: 0x00032C98 File Offset: 0x00030E98
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			this.cameraParent.SetActive(false);
		}
		if (this.cinemachineCamera != null)
		{
			this.cinemachineFollow = this.cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			this.baseCameraRadius = this.cinemachineFollow.CameraRadius;
			this.baseFollowDistance = this.cinemachineFollow.CameraDistance;
			this.baseVerticalArmLength = this.cinemachineFollow.VerticalArmLength;
			this.baseShoulderOffset = this.cinemachineFollow.ShoulderOffset;
		}
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x00032D20 File Offset: 0x00030F20
	private void LateUpdate()
	{
		if (this.cinemachineFollow != null)
		{
			float scale = Player.Instance.scale;
			this.cinemachineFollow.CameraRadius = this.baseCameraRadius * scale;
			this.cinemachineFollow.CameraDistance = this.baseFollowDistance * scale;
			this.cinemachineFollow.VerticalArmLength = this.baseVerticalArmLength * scale;
			this.cinemachineFollow.ShoulderOffset = this.baseShoulderOffset * scale;
		}
	}

	// Token: 0x040009CA RID: 2506
	public Transform playerHead;

	// Token: 0x040009CB RID: 2507
	public GameObject cameraParent;

	// Token: 0x040009CC RID: 2508
	public Vector3 headOffset;

	// Token: 0x040009CD RID: 2509
	public Vector3 eulerRotationOffset;

	// Token: 0x040009CE RID: 2510
	public CinemachineVirtualCamera cinemachineCamera;

	// Token: 0x040009CF RID: 2511
	private Cinemachine3rdPersonFollow cinemachineFollow;

	// Token: 0x040009D0 RID: 2512
	private float baseCameraRadius = 0.2f;

	// Token: 0x040009D1 RID: 2513
	private float baseFollowDistance = 2f;

	// Token: 0x040009D2 RID: 2514
	private float baseVerticalArmLength = 0.4f;

	// Token: 0x040009D3 RID: 2515
	private Vector3 baseShoulderOffset = new Vector3(0.5f, -0.4f, 0f);
}
