using System;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
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

	private void OnEnable()
	{
		this.wasInLeft = false;
		this.wasInRight = false;
	}

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

	private void OnDestroy()
	{
	}

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

	public InteractionPoint()
	{
	}

	public TransferrableObject parentTransferrableObject;

	public Collider myCollider;

	public EquipmentInteractor interactor;

	public bool wasInLeft;

	public bool wasInRight;

	public bool forLocalPlayer;
}
