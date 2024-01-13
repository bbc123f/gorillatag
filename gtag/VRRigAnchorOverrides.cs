using UnityEngine;

public class VRRigAnchorOverrides : MonoBehaviour
{
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

	public Transform NameDefaultAnchor => nameDefaultAnchor;

	public Transform NameTransform => nameTransform;

	public Transform HuntDefaultAnchor => huntComputerDefaultAnchor;

	public Transform HuntComputer => huntComputer;

	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			overrideAnchors[i] = null;
		}
		huntDefaultTransform = huntComputer;
	}

	private void OnEnable()
	{
		nameTransform.parent = nameDefaultAnchor.parent;
		huntComputer = huntDefaultTransform;
		huntComputer.parent = huntComputerDefaultAnchor.parent;
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
		int num = MapPositionToIndex(pos);
		if ((bool)overrideAnchors[num])
		{
			foreach (Transform item in overrideAnchors[num])
			{
				item.parent = null;
			}
		}
		overrideAnchors[num] = anchor;
	}

	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = MapPositionToIndex(pos);
		Transform transform = overrideAnchors[num];
		if ((object)transform != null)
		{
			return transform;
		}
		return fallback;
	}
}
