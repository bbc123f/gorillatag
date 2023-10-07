using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class OrbitCamera : MonoBehaviour
{
	// Token: 0x06000031 RID: 49 RVA: 0x000031DD File Offset: 0x000013DD
	public void Start()
	{
	}

	// Token: 0x06000032 RID: 50 RVA: 0x000031E0 File Offset: 0x000013E0
	public void Update()
	{
		this.m_phase += OrbitCamera.kOrbitSpeed * MathUtil.TwoPi * Time.deltaTime;
		base.transform.position = new Vector3(-4f * Mathf.Cos(this.m_phase), 6f, 4f * Mathf.Sin(this.m_phase));
		base.transform.rotation = Quaternion.LookRotation((new Vector3(0f, 3f, 0f) - base.transform.position).normalized);
	}

	// Token: 0x04000037 RID: 55
	private static readonly float kOrbitSpeed = 0.01f;

	// Token: 0x04000038 RID: 56
	private float m_phase;
}
