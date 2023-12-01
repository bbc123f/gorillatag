using System;
using UnityEngine;

public class PlantablePoint : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.planted = true;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.planted = false;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public bool shouldBeSet;

	public LayerMask floorMask;

	public PlantableObject plantableObject;
}
