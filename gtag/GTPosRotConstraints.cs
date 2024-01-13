using UnityEngine;

public class GTPosRotConstraints : MonoBehaviour
{
	public GorillaPosRotConstraint[] constraints;

	protected void OnEnable()
	{
		GTPosRotConstraintManager.Register(this);
	}

	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}
}
