using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HalloweenGhostChaser : MonoBehaviourPunCallbacks, IInRoomCallbacks, IPunObservable, IOnPhotonViewOwnerChange, IPhotonViewCallback
{
	public enum ChaseState
	{
		Dormant = 1,
		InitialRise = 2,
		Gong = 4,
		Chasing = 8,
		Grabbing = 0x10
	}

	public Transform followTarget;

	public Transform childGhost;

	public float velocityStep = 1f;

	public float currentSpeed;

	public float velocityIncreaseTime = 20f;

	public float riseDistance = 2f;

	public float summonDistance = 5f;

	public float timeEncircled;

	public float lastSummonCheck;

	public float timeGongStarted;

	public float summoningDuration = 30f;

	public float summoningCheckCountdown = 5f;

	public float gongDuration = 5f;

	public int summonCount = 5;

	public bool wasSurroundedLastCheck;

	public AudioSource laugh;

	public List<Player> possibleTarget;

	public AudioClip defaultLaugh;

	public AudioClip deepLaugh;

	public AudioClip gong;

	public Vector3 noisyOffset;

	public Vector3 leftArmGrabbingLocal;

	public Vector3 rightArmGrabbingLocal;

	public Vector3 leftHandGrabbingLocal;

	public Vector3 rightHandGrabbingLocal;

	public Vector3 leftHandStartingLocal;

	public Vector3 rightHandStartingLocal;

	public Vector3 ghostOffsetGrabbingLocal;

	public Vector3 ghostStartingEulerRotation;

	public Vector3 ghostGrabbingEulerRotation;

	public float maxTimeToNextHeadAngle;

	public float lastHeadAngleTime;

	public float nextHeadAngleTime;

	public float nextTimeToChasePlayer;

	public float maxNextTimeToChasePlayer;

	public float timeRiseStarted;

	public float totalTimeToRise;

	public float catchDistance;

	public float grabTime;

	public float grabDuration;

	public float grabSpeed = 1f;

	public float minGrabCooldown;

	public float lastSpeedIncreased;

	public Vector3[] headEulerAngles;

	public Transform skullTransform;

	public Transform leftArm;

	public Transform rightArm;

	public Transform leftHand;

	public Transform rightHand;

	public Transform[] spawnTransforms;

	public Transform[] spawnTransformOffsets;

	public Player targetPlayer;

	public GameObject ghostBody;

	public ChaseState currentState;

	public ChaseState lastState;

	public int spawnIndex;

	public Player grabbedPlayer;

	public Material ghostMaterial;

	public Color defaultColor;

	public Color summonedColor;

	public bool isSummoned;

	private void Awake()
	{
		spawnIndex = 0;
		targetPlayer = null;
		currentState = ChaseState.Dormant;
		grabTime = 0f - minGrabCooldown;
		possibleTarget = new List<Player>();
	}

	private void InitializeGhost()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			lastHeadAngleTime = 0f;
			nextHeadAngleTime = lastHeadAngleTime + Random.value * maxTimeToNextHeadAngle;
			nextTimeToChasePlayer = Time.time + Random.Range(minGrabCooldown, maxNextTimeToChasePlayer);
			ghostBody.transform.localPosition = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			lastSpeedIncreased = 0f;
			currentSpeed = 0f;
		}
	}

	private void LateUpdate()
	{
		if (!PhotonNetwork.InRoom)
		{
			currentState = ChaseState.Dormant;
			UpdateState();
			return;
		}
		if (base.photonView.IsMine)
		{
			switch (currentState)
			{
			case ChaseState.Dormant:
			{
				if (Time.time >= nextTimeToChasePlayer)
				{
					currentState = ChaseState.InitialRise;
				}
				if (!(Time.time >= lastSummonCheck + summoningDuration))
				{
					break;
				}
				lastSummonCheck = Time.time;
				possibleTarget.Clear();
				int num = 0;
				for (int i = 0; i < spawnTransforms.Length; i++)
				{
					int num2 = 0;
					for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
					{
						if ((GorillaParent.instance.vrrigs[j].transform.position - spawnTransforms[i].position).magnitude < summonDistance)
						{
							possibleTarget.Add(GorillaParent.instance.vrrigs[j].creator);
							num2++;
							if (num2 >= summonCount)
							{
								break;
							}
						}
					}
					if (num2 >= summonCount)
					{
						if (!wasSurroundedLastCheck)
						{
							wasSurroundedLastCheck = true;
							break;
						}
						wasSurroundedLastCheck = false;
						isSummoned = true;
						currentState = ChaseState.Gong;
						break;
					}
					num++;
				}
				if (num == spawnTransforms.Length)
				{
					wasSurroundedLastCheck = false;
				}
				break;
			}
			case ChaseState.Gong:
				if (Time.time > timeGongStarted + gongDuration)
				{
					currentState = ChaseState.InitialRise;
				}
				break;
			case ChaseState.InitialRise:
				if (Time.time > timeRiseStarted + totalTimeToRise)
				{
					currentState = ChaseState.Chasing;
				}
				break;
			case ChaseState.Chasing:
				if (followTarget == null || targetPlayer == null)
				{
					ChooseRandomTarget();
				}
				if (!(followTarget == null) && (followTarget.position - ghostBody.transform.position).magnitude < catchDistance)
				{
					currentState = ChaseState.Grabbing;
				}
				break;
			case ChaseState.Grabbing:
				if (Time.time > grabTime + grabDuration)
				{
					currentState = ChaseState.Dormant;
				}
				break;
			}
		}
		if (lastState != currentState)
		{
			ChangeState(currentState);
			lastState = currentState;
		}
		UpdateState();
	}

	public void UpdateState()
	{
		switch (currentState)
		{
		case ChaseState.Dormant:
			isSummoned = false;
			if (ghostMaterial.color == summonedColor)
			{
				ghostMaterial.color = defaultColor;
			}
			break;
		case ChaseState.InitialRise:
			if (PhotonNetwork.InRoom)
			{
				if (base.photonView.IsMine)
				{
					RiseHost();
				}
				MoveHead();
			}
			break;
		case ChaseState.Chasing:
			if (PhotonNetwork.InRoom)
			{
				if (base.photonView.IsMine)
				{
					ChaseHost();
				}
				MoveBodyShared();
				MoveHead();
			}
			break;
		case ChaseState.Grabbing:
			if (PhotonNetwork.InRoom)
			{
				if (targetPlayer == PhotonNetwork.LocalPlayer)
				{
					RiseGrabbedPlayer();
				}
				GrabBodyShared();
				MoveHead();
			}
			break;
		}
	}

	private void ChangeState(ChaseState newState)
	{
		switch (newState)
		{
		case ChaseState.Dormant:
			if (ghostBody.activeSelf)
			{
				ghostBody.SetActive(value: false);
			}
			if (base.photonView.IsMine)
			{
				targetPlayer = null;
				InitializeGhost();
			}
			else
			{
				nextTimeToChasePlayer = Time.time + Random.Range(minGrabCooldown, maxNextTimeToChasePlayer);
			}
			SetInitialRotations();
			break;
		case ChaseState.Gong:
			if (!ghostBody.activeSelf)
			{
				ghostBody.SetActive(value: true);
			}
			if (base.photonView.IsMine)
			{
				ChooseRandomTarget();
				SetInitialSpawnPoint();
				base.transform.position = spawnTransforms[spawnIndex].position;
			}
			timeGongStarted = Time.time;
			laugh.volume = 1f;
			laugh.PlayOneShot(gong);
			isSummoned = true;
			break;
		case ChaseState.InitialRise:
			timeRiseStarted = Time.time;
			if (!ghostBody.activeSelf)
			{
				ghostBody.SetActive(value: true);
			}
			if (base.photonView.IsMine)
			{
				if (!isSummoned)
				{
					currentSpeed = 0f;
					ChooseRandomTarget();
					SetInitialSpawnPoint();
				}
				else
				{
					currentSpeed = 3f;
				}
			}
			if (isSummoned)
			{
				laugh.volume = 0.25f;
				laugh.PlayOneShot(deepLaugh);
				ghostMaterial.color = summonedColor;
			}
			else
			{
				laugh.volume = 0.25f;
				laugh.Play();
				ghostMaterial.color = defaultColor;
			}
			SetInitialRotations();
			break;
		case ChaseState.Grabbing:
		{
			if (!ghostBody.activeSelf)
			{
				ghostBody.SetActive(value: true);
			}
			grabTime = Time.time;
			if (isSummoned)
			{
				laugh.volume = 0.25f;
				laugh.PlayOneShot(deepLaugh);
			}
			else
			{
				laugh.volume = 0.25f;
				laugh.Play();
			}
			leftArm.localEulerAngles = leftArmGrabbingLocal;
			rightArm.localEulerAngles = rightArmGrabbingLocal;
			leftHand.localEulerAngles = leftHandGrabbingLocal;
			rightHand.localEulerAngles = rightHandGrabbingLocal;
			ghostBody.transform.localPosition = ghostOffsetGrabbingLocal;
			ghostBody.transform.localEulerAngles = ghostGrabbingEulerRotation;
			VRRig vRRig = GorillaGameManager.StaticFindRigForPlayer(targetPlayer);
			if (vRRig != null)
			{
				followTarget = vRRig.transform;
			}
			break;
		}
		case ChaseState.Chasing:
			if (!ghostBody.activeSelf)
			{
				ghostBody.SetActive(value: true);
			}
			break;
		}
	}

	private void SetInitialSpawnPoint()
	{
		float num = 1000f;
		spawnIndex = 0;
		if (followTarget == null)
		{
			return;
		}
		for (int i = 0; i < spawnTransforms.Length; i++)
		{
			float magnitude = (followTarget.position - spawnTransformOffsets[i].position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				spawnIndex = i;
			}
		}
	}

	private void ChooseRandomTarget()
	{
		int num = -1;
		if (possibleTarget.Count >= summonCount)
		{
			int randomTarget = Random.Range(0, possibleTarget.Count);
			num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == possibleTarget[randomTarget]);
			currentSpeed = 3f;
		}
		if (num == -1)
		{
			num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
		}
		possibleTarget.Clear();
		if (num < GorillaParent.instance.vrrigs.Count)
		{
			targetPlayer = GorillaParent.instance.vrrigs[num].creator;
			followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
		}
		else
		{
			targetPlayer = null;
			followTarget = null;
		}
	}

	private void SetInitialRotations()
	{
		leftArm.localEulerAngles = Vector3.zero;
		rightArm.localEulerAngles = Vector3.zero;
		leftHand.localEulerAngles = leftHandStartingLocal;
		rightHand.localEulerAngles = rightHandStartingLocal;
		ghostBody.transform.localPosition = Vector3.zero;
		ghostBody.transform.localEulerAngles = ghostStartingEulerRotation;
	}

	private void MoveHead()
	{
		if (Time.time > nextHeadAngleTime)
		{
			skullTransform.localEulerAngles = headEulerAngles[Random.Range(0, headEulerAngles.Length)];
			lastHeadAngleTime = Time.time;
			nextHeadAngleTime = lastHeadAngleTime + Mathf.Max(Random.value * maxTimeToNextHeadAngle, 0.05f);
		}
	}

	private void RiseHost()
	{
		if (Time.time < timeRiseStarted + totalTimeToRise)
		{
			if (spawnIndex == -1)
			{
				spawnIndex = 0;
			}
			base.transform.position = spawnTransforms[spawnIndex].position + Vector3.up * (Time.time - timeRiseStarted) / totalTimeToRise * riseDistance;
			base.transform.rotation = spawnTransforms[spawnIndex].rotation;
		}
	}

	private void RiseGrabbedPlayer()
	{
		if (Time.time > grabTime + minGrabCooldown)
		{
			grabTime = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		}
		if (Time.time < grabTime + grabDuration)
		{
			GorillaTagger.Instance.rigidbody.velocity = Vector3.up * grabSpeed;
		}
	}

	private void ChaseHost()
	{
		if (followTarget != null)
		{
			if (Time.time > lastSpeedIncreased + velocityIncreaseTime)
			{
				lastSpeedIncreased = Time.time;
				currentSpeed += velocityStep;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, followTarget.position, currentSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.LookRotation(followTarget.position - base.transform.position, Vector3.up);
		}
	}

	private void MoveBodyShared()
	{
		noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		childGhost.localPosition = noisyOffset;
		leftArm.localEulerAngles = noisyOffset * 20f;
		rightArm.localEulerAngles = noisyOffset * -20f;
	}

	private void GrabBodyShared()
	{
		if (followTarget != null)
		{
			base.transform.rotation = followTarget.rotation;
			base.transform.position = followTarget.position;
		}
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(targetPlayer);
			stream.SendNext(currentState);
			stream.SendNext(spawnIndex);
			stream.SendNext(currentSpeed);
			stream.SendNext(isSummoned);
		}
		else
		{
			targetPlayer = (Player)stream.ReceiveNext();
			currentState = (ChaseState)stream.ReceiveNext();
			spawnIndex = (int)stream.ReceiveNext();
			currentSpeed = (float)stream.ReceiveNext();
			isSummoned = (bool)stream.ReceiveNext();
		}
	}

	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			ChangeState(currentState);
		}
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (PhotonNetwork.IsMasterClient)
		{
			InitializeGhost();
		}
		else
		{
			nextTimeToChasePlayer = Time.time + Random.Range(minGrabCooldown, maxNextTimeToChasePlayer);
		}
	}
}
