using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class Spinner : MonoBehaviour
{
	// Token: 0x06000058 RID: 88 RVA: 0x0000468F File Offset: 0x0000288F
	public void OnEnable()
	{
		this.m_angle = Random.Range(0f, 360f);
	}

	// Token: 0x06000059 RID: 89 RVA: 0x000046A8 File Offset: 0x000028A8
	public void Update()
	{
		this.m_angle += this.Speed * 360f * Time.deltaTime;
		base.transform.rotation = Quaternion.Euler(0f, -this.m_angle, 0f);
	}

	// Token: 0x04000083 RID: 131
	public float Speed;

	// Token: 0x04000084 RID: 132
	private float m_angle;
}
