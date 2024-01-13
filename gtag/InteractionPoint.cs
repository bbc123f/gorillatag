using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
	public TransferrableObject parentTransferrableObject;

	public Collider myCollider;

	public EquipmentInteractor interactor;

	public bool wasInLeft;

	public bool wasInRight;

	public bool forLocalPlayer;

	private void Awake()
	{
		interactor = EquipmentInteractor.instance;
		myCollider = GetComponent<Collider>();
		forLocalPlayer = false;
		if (parentTransferrableObject.isSceneObject || parentTransferrableObject.canDrop)
		{
			forLocalPlayer = true;
		}
		else if (parentTransferrableObject.IsLocalObject())
		{
			forLocalPlayer = true;
		}
	}

	private void OnEnable()
	{
		wasInLeft = false;
		wasInRight = false;
	}

	public void OnDisable()
	{
		if (forLocalPlayer && !(interactor == null))
		{
			if (interactor.overlapInteractionPointsLeft != null)
			{
				interactor.overlapInteractionPointsLeft.Remove(this);
			}
			if (interactor.overlapInteractionPointsRight != null)
			{
				interactor.overlapInteractionPointsRight.Remove(this);
			}
		}
	}

	public void LateUpdate()
	{
		if (!forLocalPlayer)
		{
			base.enabled = false;
			myCollider.enabled = false;
		}
		else if (interactor == null)
		{
			interactor = EquipmentInteractor.instance;
		}
		else
		{
			if (!(myCollider != null))
			{
				return;
			}
			if (myCollider.bounds.Contains(interactor.leftHand.transform.position) != wasInLeft)
			{
				if (!wasInLeft && !interactor.overlapInteractionPointsLeft.Contains(this))
				{
					interactor.overlapInteractionPointsLeft.Add(this);
					wasInLeft = true;
				}
				else if (wasInLeft && interactor.overlapInteractionPointsLeft.Contains(this))
				{
					interactor.overlapInteractionPointsLeft.Remove(this);
					wasInLeft = false;
				}
			}
			if (myCollider.bounds.Contains(interactor.rightHand.transform.position) != wasInRight)
			{
				if (!wasInRight && !interactor.overlapInteractionPointsRight.Contains(this))
				{
					interactor.overlapInteractionPointsRight.Add(this);
					wasInRight = true;
				}
				else if (wasInRight && interactor.overlapInteractionPointsRight.Contains(this))
				{
					interactor.overlapInteractionPointsRight.Remove(this);
					wasInRight = false;
				}
			}
		}
	}
}
