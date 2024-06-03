using System;
using UnityEngine;

public class GTPosRotConstraints : MonoBehaviour
{
	protected void OnEnable()
	{
		GTPosRotConstraintManager.Register(this);
	}

	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	public GTPosRotConstraints()
	{
	}

	public GorillaPosRotConstraint[] constraints;
}
