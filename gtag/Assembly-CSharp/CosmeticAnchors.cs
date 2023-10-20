using System;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class CosmeticAnchors : MonoBehaviour
{
	// Token: 0x06000681 RID: 1665 RVA: 0x00028A82 File Offset: 0x00026C82
	protected void Awake()
	{
		this.anchorEnabled = false;
		this.vrRig = base.GetComponentInParent<VRRig>();
		if (this.vrRig != null)
		{
			this.anchorOverrides = this.vrRig.gameObject.GetComponent<VRRigAnchorOverrides>();
		}
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00028ABC File Offset: 0x00026CBC
	protected void Update()
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

	// Token: 0x06000683 RID: 1667 RVA: 0x00028B8C File Offset: 0x00026D8C
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

	// Token: 0x06000684 RID: 1668 RVA: 0x00028C9C File Offset: 0x00026E9C
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

	// Token: 0x06000685 RID: 1669 RVA: 0x00028D14 File Offset: 0x00026F14
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

	// Token: 0x06000686 RID: 1670 RVA: 0x00028D82 File Offset: 0x00026F82
	public Transform GetNameAnchor()
	{
		if (!this.nameAnchor)
		{
			return null;
		}
		return this.nameAnchor.transform;
	}

	// Token: 0x040007CA RID: 1994
	[SerializeField]
	protected GameObject nameAnchor;

	// Token: 0x040007CB RID: 1995
	[SerializeField]
	protected GameObject leftArmAnchor;

	// Token: 0x040007CC RID: 1996
	[SerializeField]
	protected GameObject rightArmAnchor;

	// Token: 0x040007CD RID: 1997
	[SerializeField]
	protected GameObject chestAnchor;

	// Token: 0x040007CE RID: 1998
	[SerializeField]
	protected GameObject huntComputerAnchor;

	// Token: 0x040007CF RID: 1999
	private VRRig vrRig;

	// Token: 0x040007D0 RID: 2000
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x040007D1 RID: 2001
	private bool anchorEnabled;
}
