using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class WASD : MonoBehaviour
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600005B RID: 91 RVA: 0x000046FD File Offset: 0x000028FD
	public Vector3 Velocity
	{
		get
		{
			return this.m_velocity;
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00004708 File Offset: 0x00002908
	public void Update()
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		if (Input.GetKey(KeyCode.W))
		{
			zero.z += 1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			zero.x -= 1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			zero.z -= 1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			zero.x += 1f;
		}
		Vector3 vector = (zero.sqrMagnitude > 0f) ? (zero.normalized * this.Speed * Time.deltaTime) : Vector3.zero;
		Quaternion lhs = Quaternion.AngleAxis(num * this.Omega * 57.29578f * Time.deltaTime, Vector3.up);
		this.m_velocity = vector / Time.deltaTime;
		base.transform.position += vector;
		base.transform.rotation = lhs * base.transform.rotation;
	}

	// Token: 0x04000085 RID: 133
	public float Speed = 1f;

	// Token: 0x04000086 RID: 134
	public float Omega = 1f;

	// Token: 0x04000087 RID: 135
	public Vector3 m_velocity;
}
