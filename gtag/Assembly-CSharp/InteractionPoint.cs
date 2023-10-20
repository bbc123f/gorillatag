using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
public class InteractionPoint : MonoBehaviour
{
	// Token: 0x06000511 RID: 1297 RVA: 0x000204E4 File Offset: 0x0001E6E4
	private void Awake()
	{
		this.interactor = EquipmentInteractor.instance;
		this.myCollider = base.GetComponent<Collider>();
		this.forLocalPlayer = false;
		if (this.parentTransferrableObject.isSceneObject || this.parentTransferrableObject.canDrop)
		{
			this.forLocalPlayer = true;
			return;
		}
		if (this.parentTransferrableObject.IsLocalObject())
		{
			this.forLocalPlayer = true;
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00020547 File Offset: 0x0001E747
	private void OnEnable()
	{
		this.wasInLeft = false;
		this.wasInRight = false;
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00020558 File Offset: 0x0001E758
	public void OnDisable()
	{
		if (!this.forLocalPlayer || this.interactor == null)
		{
			return;
		}
		if (this.interactor.overlapInteractionPointsLeft != null)
		{
			this.interactor.overlapInteractionPointsLeft.Remove(this);
		}
		if (this.interactor.overlapInteractionPointsRight != null)
		{
			this.interactor.overlapInteractionPointsRight.Remove(this);
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x000205BC File Offset: 0x0001E7BC
	public void LateUpdate()
	{
		if (!this.forLocalPlayer)
		{
			base.enabled = false;
			this.myCollider.enabled = false;
			return;
		}
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
			return;
		}
		if (this.myCollider != null)
		{
			if (this.myCollider.bounds.Contains(this.interactor.leftHand.transform.position) != this.wasInLeft)
			{
				if (!this.wasInLeft && !this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Add(this);
					this.wasInLeft = true;
				}
				else if (this.wasInLeft && this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Remove(this);
					this.wasInLeft = false;
				}
			}
			if (this.myCollider.bounds.Contains(this.interactor.rightHand.transform.position) != this.wasInRight)
			{
				if (!this.wasInRight && !this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Add(this);
					this.wasInRight = true;
					return;
				}
				if (this.wasInRight && this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Remove(this);
					this.wasInRight = false;
				}
			}
		}
	}

	// Token: 0x04000607 RID: 1543
	public TransferrableObject parentTransferrableObject;

	// Token: 0x04000608 RID: 1544
	public Collider myCollider;

	// Token: 0x04000609 RID: 1545
	public EquipmentInteractor interactor;

	// Token: 0x0400060A RID: 1546
	public bool wasInLeft;

	// Token: 0x0400060B RID: 1547
	public bool wasInRight;

	// Token: 0x0400060C RID: 1548
	public bool forLocalPlayer;
}
