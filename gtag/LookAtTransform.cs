﻿using System;
using UnityEngine;

public class LookAtTransform : MonoBehaviour
{
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(this.lookAt.position - base.transform.position);
	}

	public LookAtTransform()
	{
	}

	[SerializeField]
	private Transform lookAt;
}
