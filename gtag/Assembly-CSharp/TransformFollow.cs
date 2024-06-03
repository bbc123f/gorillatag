using System;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	public TransformFollow()
	{
	}

	public Transform transformToFollow;

	public Vector3 offset;

	public Vector3 prevPos;
}
