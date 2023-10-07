using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class CurveBall : MonoBehaviour
{
	// Token: 0x0600003F RID: 63 RVA: 0x0000388C File Offset: 0x00001A8C
	public void Reset()
	{
		float f = Random.Range(0f, MathUtil.TwoPi);
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		this.m_speedX = 40f * num;
		this.m_speedZ = 40f * num2;
		this.m_timer = 0f;
		Vector3 position = base.transform.position;
		position.x = -10f * num;
		position.z = -10f * num2;
		base.transform.position = position;
	}

	// Token: 0x06000040 RID: 64 RVA: 0x0000390E File Offset: 0x00001B0E
	public void Start()
	{
		this.Reset();
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00003918 File Offset: 0x00001B18
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.m_timer > this.Interval)
		{
			this.Reset();
		}
		Vector3 position = base.transform.position;
		position.x += this.m_speedX * deltaTime;
		position.z += this.m_speedZ * deltaTime;
		base.transform.position = position;
		this.m_timer += deltaTime;
	}

	// Token: 0x0400004D RID: 77
	public float Interval = 2f;

	// Token: 0x0400004E RID: 78
	private float m_speedX;

	// Token: 0x0400004F RID: 79
	private float m_speedZ;

	// Token: 0x04000050 RID: 80
	private float m_timer;
}
