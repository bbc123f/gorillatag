using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class GeodeItem : TransferrableObject
{
	[Tooltip("This GameObject will activate when the geode hits the ground with enough force.")]
	public GameObject effectsGameObject;

	public LayerMask collisionLayerMask;

	[Tooltip("Used to calculate velocity of the geode.")]
	public GorillaVelocityEstimator velocityEstimator;

	public float cooldown = 5f;

	[Tooltip("The velocity of the geode must be greater than this value to activate the effect.")]
	public float minHitVelocity = 0.2f;

	[Tooltip("Geode's full mesh before cracking")]
	public GameObject geodeFullMesh;

	[Tooltip("Geode's cracked open half different meshes, picked randomly")]
	public GameObject[] geodeCrackedMeshes;

	[Tooltip("The distance between te geode and the layer mask to detect whether it hits it")]
	public float rayCastMaxDistance = 0.2f;

	[FormerlySerializedAs("collisionRadius")]
	public float sphereRayRadius = 0.05f;

	[DebugReadout]
	private float cooldownRemaining;

	[DebugReadout]
	private bool hitLastFrame;

	[SerializeField]
	private AudioSource audioSource;

	private bool hasEffectsGameObject;

	private bool effectsHaveBeenPlayed;

	private RaycastHit hit;

	private RaycastHit[] collidersHit = new RaycastHit[20];

	private ItemStates currentItemState;

	private ItemStates prevItemState;

	private int index;

	protected override void Awake()
	{
		base.Awake();
		hasEffectsGameObject = effectsGameObject != null;
		effectsHaveBeenPlayed = false;
	}

	protected override void Start()
	{
		base.Start();
		itemState = ItemStates.State0;
		prevItemState = ItemStates.State0;
		InitToDefault();
	}

	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		InitToDefault();
		itemState = ItemStates.State0;
	}

	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (itemState != ItemStates.State0)
		{
			InHand();
		}
	}

	private void InitToDefault()
	{
		cooldownRemaining = 0f;
		effectsHaveBeenPlayed = false;
		if (hasEffectsGameObject)
		{
			effectsGameObject.SetActive(value: false);
		}
		geodeFullMesh.SetActive(value: true);
		for (int i = 0; i < geodeCrackedMeshes.Length; i++)
		{
			geodeCrackedMeshes[i].SetActive(value: false);
		}
		hitLastFrame = false;
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (itemState == ItemStates.State1)
		{
			cooldownRemaining -= Time.deltaTime;
			if (cooldownRemaining <= 0f)
			{
				itemState = ItemStates.State0;
				OnItemStateChanged();
			}
		}
		else if (!(velocityEstimator.linearVelocity.magnitude < minHitVelocity))
		{
			if (InHand())
			{
				int num = Physics.SphereCastNonAlloc(geodeFullMesh.transform.position, sphereRayRadius * Mathf.Abs(geodeFullMesh.transform.lossyScale.x), geodeFullMesh.transform.TransformDirection(Vector3.forward), collidersHit, rayCastMaxDistance, collisionLayerMask, QueryTriggerInteraction.Collide);
				hitLastFrame = num > 0;
			}
			if (hitLastFrame && GorillaParent.hasInstance)
			{
				itemState = ItemStates.State1;
				cooldownRemaining = cooldown;
				index = RandomPickCrackedGeode();
			}
		}
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		currentItemState = itemState;
		if (currentItemState != prevItemState)
		{
			OnItemStateChanged();
		}
		prevItemState = currentItemState;
	}

	private void OnItemStateChanged()
	{
		if (itemState == ItemStates.State0)
		{
			InitToDefault();
			return;
		}
		geodeFullMesh.SetActive(value: false);
		for (int i = 0; i < geodeCrackedMeshes.Length; i++)
		{
			geodeCrackedMeshes[i].SetActive(i == index);
		}
		if (PhotonNetwork.InRoom && GorillaGameManager.instance != null && !effectsHaveBeenPlayed)
		{
			GorillaGameManager.instance.FindVRRigForPlayer(PhotonNetwork.LocalPlayer).RPC("PlayGeodeEffect", RpcTarget.All, geodeFullMesh.transform.position);
			effectsHaveBeenPlayed = true;
		}
		if (!PhotonNetwork.InRoom && !effectsHaveBeenPlayed)
		{
			if ((bool)audioSource)
			{
				audioSource.Play();
			}
			effectsHaveBeenPlayed = true;
		}
	}

	private int RandomPickCrackedGeode()
	{
		return Random.Range(0, geodeCrackedMeshes.Length);
	}
}
