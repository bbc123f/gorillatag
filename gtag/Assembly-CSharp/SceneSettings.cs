using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class SceneSettings : MonoBehaviour
{
	// Token: 0x06000483 RID: 1155 RVA: 0x0001CEDB File Offset: 0x0001B0DB
	private void Awake()
	{
		Time.fixedDeltaTime = this.m_fixedTimeStep;
		Physics.gravity = Vector3.down * 9.81f * this.m_gravityScalar;
		Physics.defaultContactOffset = this.m_defaultContactOffset;
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x0001CF12 File Offset: 0x0001B112
	private void Start()
	{
		SceneSettings.CollidersSetContactOffset(this.m_defaultContactOffset);
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001CF20 File Offset: 0x0001B120
	private static void CollidersSetContactOffset(float contactOffset)
	{
		Collider[] array = Object.FindObjectsOfType<Collider>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].contactOffset = contactOffset;
		}
	}

	// Token: 0x04000526 RID: 1318
	[Header("Time")]
	[SerializeField]
	private float m_fixedTimeStep = 0.01f;

	// Token: 0x04000527 RID: 1319
	[Header("Physics")]
	[SerializeField]
	private float m_gravityScalar = 0.75f;

	// Token: 0x04000528 RID: 1320
	[SerializeField]
	private float m_defaultContactOffset = 0.001f;
}
