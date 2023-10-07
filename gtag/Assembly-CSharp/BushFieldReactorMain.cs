using System;
using System.Collections.Generic;
using BoingKit;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class BushFieldReactorMain : MonoBehaviour
{
	// Token: 0x06000047 RID: 71 RVA: 0x00003F3C File Offset: 0x0000213C
	public void Start()
	{
		Random.InitState(0);
		for (int i = 0; i < this.NumBushes; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.Bush);
			float num = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
			gameObject.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject.transform.localScale = num * Vector3.one;
			BoingBehavior component = gameObject.GetComponent<BoingBehavior>();
			if (component != null)
			{
				component.Reboot();
			}
		}
		for (int j = 0; j < this.NumBlossoms; j++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.Blossom);
			float num2 = Random.Range(this.BlossomScaleRange.x, this.BlossomScaleRange.y);
			gameObject2.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.y), 0.2f * num2, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject2.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject2.transform.localScale = num2 * Vector3.one;
			BoingBehavior component2 = gameObject2.GetComponent<BoingBehavior>();
			if (component2 != null)
			{
				component2.Reboot();
			}
		}
		this.m_aSphere = new List<GameObject>(this.NumSpheresPerCircle * this.NumCircles);
		for (int k = 0; k < this.NumCircles; k++)
		{
			for (int l = 0; l < this.NumSpheresPerCircle; l++)
			{
				this.m_aSphere.Add(Object.Instantiate<GameObject>(this.Sphere));
			}
		}
		this.m_basePhase = 0f;
	}

	// Token: 0x06000048 RID: 72 RVA: 0x000041A8 File Offset: 0x000023A8
	public void Update()
	{
		int num = 0;
		for (int i = 0; i < this.NumCircles; i++)
		{
			float num2 = this.MaxCircleRadius / (float)(i + 1);
			for (int j = 0; j < this.NumSpheresPerCircle; j++)
			{
				float num3 = this.m_basePhase + (float)j / (float)this.NumSpheresPerCircle * 2f * 3.1415927f;
				num3 *= ((i % 2 == 0) ? 1f : -1f);
				this.m_aSphere[num].transform.position = new Vector3(num2 * Mathf.Cos(num3), 0.2f, num2 * Mathf.Sin(num3));
				num++;
			}
		}
		this.m_basePhase -= this.CircleSpeed / this.MaxCircleRadius * Time.deltaTime;
	}

	// Token: 0x04000063 RID: 99
	public GameObject Bush;

	// Token: 0x04000064 RID: 100
	public GameObject Blossom;

	// Token: 0x04000065 RID: 101
	public GameObject Sphere;

	// Token: 0x04000066 RID: 102
	public int NumBushes;

	// Token: 0x04000067 RID: 103
	public Vector2 BushScaleRange;

	// Token: 0x04000068 RID: 104
	public int NumBlossoms;

	// Token: 0x04000069 RID: 105
	public Vector2 BlossomScaleRange;

	// Token: 0x0400006A RID: 106
	public Vector2 FieldBounds;

	// Token: 0x0400006B RID: 107
	public int NumSpheresPerCircle;

	// Token: 0x0400006C RID: 108
	public int NumCircles;

	// Token: 0x0400006D RID: 109
	public float MaxCircleRadius;

	// Token: 0x0400006E RID: 110
	public float CircleSpeed;

	// Token: 0x0400006F RID: 111
	private List<GameObject> m_aSphere;

	// Token: 0x04000070 RID: 112
	private float m_basePhase;
}
