using System;
using UnityEngine;

public class CosmeticAnchors : MonoBehaviour
{
	protected void Awake()
	{
		this.anchorEnabled = false;
		this.vrRig = base.GetComponentInParent<VRRig>();
		if (this.vrRig != null)
		{
			this.anchorOverrides = this.vrRig.gameObject.GetComponent<VRRigAnchorOverrides>();
		}
	}

	private void OnEnable()
	{
		if (this.huntComputerAnchor)
		{
			CosmeticAnchorManager.RegisterCosmeticAnchor(this);
		}
	}

	private void OnDisable()
	{
		if (this.huntComputerAnchor)
		{
			CosmeticAnchorManager.UnregisterCosmeticAnchor(this);
		}
	}

	public void TryUpdate()
	{
		if (this.anchorEnabled && this.huntComputerAnchor && !GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && this.anchorOverrides.HuntComputer.parent != this.anchorOverrides.HuntDefaultAnchor)
		{
			this.anchorOverrides.HuntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
			return;
		}
		if (this.anchorEnabled && this.huntComputerAnchor && GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf && this.anchorOverrides.HuntComputer.parent == this.anchorOverrides.HuntDefaultAnchor)
		{
			this.SetHuntComputerAnchor(this.anchorEnabled);
		}
	}

	public void EnableAnchor(bool enable)
	{
		this.anchorEnabled = enable;
		if (this.anchorOverrides == null)
		{
			return;
		}
		if (this.leftArmAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnLeftArm, enable ? this.leftArmAnchor.transform : null);
		}
		if (this.rightArmAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnRightArm, enable ? this.rightArmAnchor.transform : null);
		}
		if (this.chestAnchor)
		{
			this.anchorOverrides.OverrideAnchor(TransferrableObject.PositionState.OnChest, enable ? this.chestAnchor.transform : null);
		}
		if (this.nameAnchor)
		{
			Transform nameTransform = this.anchorOverrides.NameTransform;
			nameTransform.parent = (enable ? this.nameAnchor.transform : this.anchorOverrides.NameDefaultAnchor);
			nameTransform.transform.localPosition = Vector3.zero;
			nameTransform.transform.localRotation = Quaternion.identity;
		}
		if (this.huntComputerAnchor)
		{
			this.SetHuntComputerAnchor(enable);
		}
	}

	private void SetHuntComputerAnchor(bool enable)
	{
		Transform huntComputer = this.anchorOverrides.HuntComputer;
		if (!GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf || !enable)
		{
			huntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
		}
		else
		{
			huntComputer.parent = this.huntComputerAnchor.transform;
		}
		huntComputer.transform.localPosition = Vector3.zero;
		huntComputer.transform.localRotation = Quaternion.identity;
	}

	public Transform GetPositionAnchor(TransferrableObject.PositionState pos)
	{
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					return null;
				}
				if (!this.chestAnchor)
				{
					return null;
				}
				return this.chestAnchor.transform;
			}
			else
			{
				if (!this.rightArmAnchor)
				{
					return null;
				}
				return this.rightArmAnchor.transform;
			}
		}
		else
		{
			if (!this.leftArmAnchor)
			{
				return null;
			}
			return this.leftArmAnchor.transform;
		}
	}

	public Transform GetNameAnchor()
	{
		if (!this.nameAnchor)
		{
			return null;
		}
		return this.nameAnchor.transform;
	}

	public bool AffectedByHunt()
	{
		return this.huntComputerAnchor != null;
	}

	public CosmeticAnchors()
	{
	}

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
}
