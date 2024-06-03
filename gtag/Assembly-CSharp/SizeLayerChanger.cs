﻿using System;
using UnityEngine;

public class SizeLayerChanger : MonoBehaviour
{
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			component.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			component.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	public SizeLayerChanger()
	{
	}

	public float maxScale;

	public float minScale;

	public bool isAssurance;

	public bool affectLayerA = true;

	public bool affectLayerB = true;

	public bool affectLayerC = true;

	public bool affectLayerD = true;

	[SerializeField]
	private bool applyOnTriggerEnter = true;

	[SerializeField]
	private bool applyOnTriggerExit;
}
