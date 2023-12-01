using System;
using UnityEngine;

public class SceneSettings : MonoBehaviour
{
	private void Awake()
	{
		Time.fixedDeltaTime = this.m_fixedTimeStep;
		Physics.gravity = Vector3.down * 9.81f * this.m_gravityScalar;
		Physics.defaultContactOffset = this.m_defaultContactOffset;
	}

	private void Start()
	{
		SceneSettings.CollidersSetContactOffset(this.m_defaultContactOffset);
	}

	private static void CollidersSetContactOffset(float contactOffset)
	{
		Collider[] array = Object.FindObjectsOfType<Collider>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].contactOffset = contactOffset;
		}
	}

	[Header("Time")]
	[SerializeField]
	private float m_fixedTimeStep = 0.01f;

	[Header("Physics")]
	[SerializeField]
	private float m_gravityScalar = 0.75f;

	[SerializeField]
	private float m_defaultContactOffset = 0.001f;
}
