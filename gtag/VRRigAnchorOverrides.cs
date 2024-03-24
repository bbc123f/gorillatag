using System;
using UnityEngine;

public class VRRigAnchorOverrides : MonoBehaviour
{
	public Transform NameDefaultAnchor
	{
		get
		{
			return this.nameDefaultAnchor;
		}
	}

	public Transform NameTransform
	{
		get
		{
			return this.nameTransform;
		}
	}

	public Transform HuntDefaultAnchor
	{
		get
		{
			return this.huntComputerDefaultAnchor;
		}
	}

	public Transform HuntComputer
	{
		get
		{
			return this.huntComputer;
		}
	}

	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			this.overrideAnchors[i] = null;
		}
		this.huntDefaultTransform = this.huntComputer;
	}

	private void OnEnable()
	{
		this.nameTransform.parent = this.nameDefaultAnchor.parent;
		this.huntComputer = this.huntDefaultTransform;
		this.huntComputer.parent = this.huntComputerDefaultAnchor.parent;
	}

	private int MapPositionToIndex(TransferrableObject.PositionState pos)
	{
		int num = (int)pos;
		int num2 = 0;
		while ((num >>= 1) != 0)
		{
			num2++;
		}
		return num2;
	}

	public void OverrideAnchor(TransferrableObject.PositionState pos, Transform anchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (this.overrideAnchors[num])
		{
			foreach (object obj in this.overrideAnchors[num])
			{
				((Transform)obj).parent = null;
			}
		}
		this.overrideAnchors[num] = anchor;
	}

	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = this.MapPositionToIndex(pos);
		Transform transform = this.overrideAnchors[num];
		if (transform != null)
		{
			return transform;
		}
		return fallback;
	}

	public VRRigAnchorOverrides()
	{
	}

	[SerializeField]
	protected Transform nameDefaultAnchor;

	[SerializeField]
	protected Transform nameTransform;

	[SerializeField]
	protected Transform huntComputer;

	[SerializeField]
	protected Transform huntComputerDefaultAnchor;

	private Transform huntDefaultTransform;

	private readonly Transform[] overrideAnchors = new Transform[8];

	private GameObject nameLastObjectToAttach;
}
