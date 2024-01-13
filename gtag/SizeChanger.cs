using System.Collections.Generic;
using UnityEngine;

public class SizeChanger : GorillaTriggerBox
{
	public enum ChangerType
	{
		Static,
		Continuous,
		Radius
	}

	public VRRig rigRef;

	public ChangerType myType;

	public float maxScale;

	public float minScale;

	public Collider myCollider;

	public float insideThreshold = 0.01f;

	public List<VRRig> insideRigs;

	public List<VRRig> leftRigs;

	public float scaleLerp;

	public Transform startPos;

	public Transform endPos;

	public int priority;

	public bool aprilFoolsEnabled;

	public float startRadius;

	public float endRadius;

	public bool affectLayerA = true;

	public bool affectLayerB = true;

	public bool affectLayerC = true;

	public bool affectLayerD = true;

	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (affectLayerA)
			{
				num |= 1;
			}
			if (affectLayerB)
			{
				num |= 2;
			}
			if (affectLayerC)
			{
				num |= 4;
			}
			if (affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	private void Awake()
	{
		minScale = Mathf.Max(minScale, 0.01f);
		myCollider = GetComponent<Collider>();
	}

	public void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<SphereCollider>())
		{
			VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (!(component == null) && !component.sizeManager.touchingChangers.Contains(this))
			{
				component.sizeManager.touchingChangers.Add(this);
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<SphereCollider>())
		{
			VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (!(component == null) && component.sizeManager.touchingChangers.Contains(this))
			{
				component.sizeManager.touchingChangers.Remove(this);
			}
		}
	}
}
