using UnityEngine;

public class CosmeticAnchors : MonoBehaviour
{
	[SerializeField]
	protected GameObject nameAnchor;

	[SerializeField]
	protected GameObject leftArmAnchor;

	[SerializeField]
	protected GameObject rightArmAnchor;

	[SerializeField]
	protected GameObject chestAnchor;

	[SerializeField]
	protected GameObject huntComputerAnchor;

	private VRRig vrRig;

	private VRRigAnchorOverrides anchorOverrides;

	private bool anchorEnabled;

	protected void Awake()
	{
		anchorEnabled = false;
		vrRig = GetComponentInParent<VRRig>();
		if (vrRig != null)
		{
			anchorOverrides = vrRig.gameObject.GetComponent<VRRigAnchorOverrides>();
		}
	}

	protected void Update()
	{
		if (anchorEnabled && (bool)huntComputerAnchor && !GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && anchorOverrides.HuntComputer.parent != anchorOverrides.HuntDefaultAnchor)
		{
			anchorOverrides.HuntComputer.parent = anchorOverrides.HuntDefaultAnchor;
		}
		else if (anchorEnabled && (bool)huntComputerAnchor && GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && anchorOverrides.HuntComputer.parent == anchorOverrides.HuntDefaultAnchor)
		{
			SetHuntComputerAnchor(anchorEnabled);
		}
	}

	public void EnableAnchor(bool enable)
	{
		anchorEnabled = enable;
		if (!(anchorOverrides == null))
		{
			if ((bool)leftArmAnchor)
			{
				anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnLeftArm, enable ? leftArmAnchor.transform : null);
			}
			if ((bool)rightArmAnchor)
			{
				anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnRightArm, enable ? rightArmAnchor.transform : null);
			}
			if ((bool)chestAnchor)
			{
				anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnChest, enable ? chestAnchor.transform : null);
			}
			if ((bool)nameAnchor)
			{
				Transform nameTransform = anchorOverrides.NameTransform;
				nameTransform.parent = (enable ? nameAnchor.transform : anchorOverrides.NameDefaultAnchor);
				nameTransform.transform.localPosition = Vector3.zero;
				nameTransform.transform.localRotation = Quaternion.identity;
			}
			if ((bool)huntComputerAnchor)
			{
				SetHuntComputerAnchor(enable);
			}
		}
	}

	private void SetHuntComputerAnchor(bool enable)
	{
		Transform huntComputer = anchorOverrides.HuntComputer;
		if (!GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf || !enable)
		{
			huntComputer.parent = anchorOverrides.HuntDefaultAnchor;
		}
		else
		{
			huntComputer.parent = huntComputerAnchor.transform;
		}
		huntComputer.transform.localPosition = Vector3.zero;
		huntComputer.transform.localRotation = Quaternion.identity;
	}

	public Transform GetPositionAnchor(TransferrableObject.PositionState pos)
	{
		switch (pos)
		{
		case TransferrableObject.PositionState.OnLeftArm:
			if (!leftArmAnchor)
			{
				return null;
			}
			return leftArmAnchor.transform;
		case TransferrableObject.PositionState.OnRightArm:
			if (!rightArmAnchor)
			{
				return null;
			}
			return rightArmAnchor.transform;
		case TransferrableObject.PositionState.OnChest:
			if (!chestAnchor)
			{
				return null;
			}
			return chestAnchor.transform;
		default:
			return null;
		}
	}

	public Transform GetNameAnchor()
	{
		if (!nameAnchor)
		{
			return null;
		}
		return nameAnchor.transform;
	}
}
