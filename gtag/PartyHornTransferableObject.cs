using UnityEngine;

public class PartyHornTransferableObject : TransferrableObject
{
	private enum PartyHornState
	{
		None = 1,
		CoolingDown
	}

	[Tooltip("This GameObject will activate when held to any gorilla's mouth.")]
	public GameObject effectsGameObject;

	public float cooldown = 2f;

	public float mouthPieceZOffset = -0.18f;

	public float mouthPieceRadius = 0.05f;

	public Vector3 mouthOffset = new Vector3(0f, 0.02f, 0.17f);

	private float cooldownRemaining;

	private Transform localHead;

	private PartyHornState partyHornStateLastFrame;

	private bool localWasActivated;

	public override void OnEnable()
	{
		base.OnEnable();
		localHead = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
		InitToDefault();
	}

	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		InitToDefault();
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!InHand() || itemState != ItemStates.State0 || !GorillaParent.hasInstance)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3 vector = transform.position + transform.forward * mouthPieceZOffset;
		float num = mouthPieceRadius * mouthPieceRadius;
		bool flag = (localHead.TransformPoint(mouthOffset) - vector).sqrMagnitude < num;
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			if (vrrig.head == null || vrrig.head.rigTarget == null || flag)
			{
				break;
			}
			flag = (vrrig.head.rigTarget.transform.TransformPoint(mouthOffset) - vector).sqrMagnitude < num;
		}
		itemState = (flag ? ItemStates.State1 : itemState);
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (ItemStates.State1 == itemState)
		{
			if (!localWasActivated)
			{
				effectsGameObject.SetActive(value: true);
				cooldownRemaining = cooldown;
				localWasActivated = true;
			}
			cooldownRemaining -= Time.deltaTime;
			if (cooldownRemaining <= 0f)
			{
				InitToDefault();
			}
		}
	}

	private void InitToDefault()
	{
		itemState = ItemStates.State0;
		effectsGameObject.SetActive(value: false);
		cooldownRemaining = cooldown;
		localWasActivated = false;
	}
}
