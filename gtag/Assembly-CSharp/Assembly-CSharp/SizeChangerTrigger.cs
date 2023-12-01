using System;
using UnityEngine;

public class SizeChangerTrigger : MonoBehaviour
{
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	private Collider myCollider;

	public delegate void SizeChangerTriggerEvent(Collider other);
}
