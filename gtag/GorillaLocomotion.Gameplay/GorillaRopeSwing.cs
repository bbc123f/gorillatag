using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay;

public class GorillaRopeSwing : MonoBehaviourPun
{
	public const float ropeBitGenOffset = 1f;

	[SerializeField]
	private GameObject prefabRopeBit;

	public Transform[] nodes = Array.Empty<Transform>();

	private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

	[NonSerialized]
	public float lastGrabTime;

	[SerializeField]
	private AudioSource ropeCreakSFX;

	public GorillaVelocityTracker velocityTracker;

	private bool localPlayerOn;

	private int localPlayerBoneIndex;

	private XRNode localPlayerXRNode;

	private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

	private const float TIME_FOR_IDLE = 2f;

	private float potentialIdleTimer;

	[SerializeField]
	private int ropeLength = 8;

	[SerializeField]
	private GorillaRopeSwingSettings settings;

	[NonSerialized]
	public int ropeDataStartIndex;

	[NonSerialized]
	public int ropeDataIndexOffset;

	[NonSerialized]
	public Transform trans;

	[SerializeField]
	private LayerMask wallLayerMask;

	private RaycastHit[] nodeHits = new RaycastHit[1];

	private int lastNodeCheckIndex = 2;

	public bool isIdle { get; private set; }

	public bool isFullyIdle { get; private set; }

	public bool hasPlayers
	{
		get
		{
			if (!localPlayerOn)
			{
				return remotePlayers.Count > 0;
			}
			return true;
		}
	}

	private void Awake()
	{
		trans = base.transform;
		SetIsIdle(idle: true);
	}

	private void OnEnable()
	{
		VectorizedCustomRopeSimulation.Register(this);
	}

	private void OnDisable()
	{
		if (!isIdle)
		{
			SetIsIdle(idle: true, resetPos: true);
		}
		VectorizedCustomRopeSimulation.Unregister(this);
	}

	private void Update()
	{
		if (isIdle)
		{
			isFullyIdle = true;
		}
		if (isIdle)
		{
			return;
		}
		int num = -1;
		if (localPlayerOn)
		{
			num = localPlayerBoneIndex;
		}
		else if (remotePlayers.Count > 0)
		{
			num = remotePlayers.First().Value;
		}
		if (num >= 0 && VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, num).magnitude > 2f && !ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
		{
			ropeCreakSFX.Play();
		}
		if (localPlayerOn)
		{
			float num2 = Maths.Linear(velocityTracker.GetLatestVelocity(worldSpace: true).magnitude, 0f, 10f, -0.07f, 0.5f);
			if (num2 > 0f)
			{
				GorillaTagger.Instance.DoVibration(localPlayerXRNode, num2, Time.deltaTime);
			}
		}
		Transform bone = GetBone(lastNodeCheckIndex);
		Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, lastNodeCheckIndex);
		if (Physics.SphereCastNonAlloc(bone.position, 0.2f, nodeVelocity.normalized, nodeHits, 0.4f, wallLayerMask, QueryTriggerInteraction.Ignore) > 0)
		{
			SetVelocity(lastNodeCheckIndex, Vector3.zero, wholeRope: false, default(PhotonMessageInfo));
		}
		if (!(nodeVelocity.magnitude > 0.35f))
		{
			potentialIdleTimer += Time.deltaTime;
		}
		else
		{
			potentialIdleTimer = 0f;
		}
		if (potentialIdleTimer >= 2f)
		{
			SetIsIdle(idle: true);
			potentialIdleTimer = 0f;
		}
		lastNodeCheckIndex++;
		if (lastNodeCheckIndex > nodes.Length)
		{
			lastNodeCheckIndex = 2;
		}
	}

	private void SetIsIdle(bool idle, bool resetPos = false)
	{
		isIdle = idle;
		ropeCreakSFX.gameObject.SetActive(!idle);
		if (idle)
		{
			ToggleVelocityTracker(enable: false);
			if (resetPos)
			{
				Vector3 zero = Vector3.zero;
				for (int i = 0; i < nodes.Length; i++)
				{
					nodes[i].transform.localRotation = Quaternion.identity;
					nodes[i].transform.localPosition = zero;
					zero += new Vector3(0f, -1f, 0f);
				}
			}
		}
		else
		{
			isFullyIdle = false;
		}
	}

	public Transform GetBone(int index)
	{
		if (index >= nodes.Length)
		{
			return nodes.Last();
		}
		return nodes[index];
	}

	public int GetBoneIndex(Transform r)
	{
		for (int i = 0; i < nodes.Length; i++)
		{
			if (nodes[i] == r)
			{
				return i;
			}
		}
		return nodes.Length - 1;
	}

	public void AttachLocalPlayer(XRNode xrNode, Transform grabbedBone, Vector3 offset, Vector3 velocity)
	{
		int num = (localPlayerBoneIndex = GetBoneIndex(grabbedBone));
		velocity *= settings.inheritVelocityMultiplier;
		if (GorillaTagger.hasInstance && (bool)GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = base.photonView.ViewID;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = num;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
		}
		RefreshAllBonesMass();
		List<Vector3> list = new List<Vector3>();
		if (remotePlayers.Count <= 0)
		{
			Transform[] array = nodes;
			foreach (Transform transform in array)
			{
				list.Add(transform.position);
			}
		}
		velocity.y = 0f;
		if (Time.time - lastGrabTime > 1f && (remotePlayers.Count == 0 || velocity.magnitude > 2.5f))
		{
			SetVelocity_RPC(num, velocity, wholeRope: true);
		}
		lastGrabTime = Time.time;
		ropeCreakSFX.transform.parent = GetBone(Math.Max(0, num - 3)).transform;
		ropeCreakSFX.transform.localPosition = Vector3.zero;
		localPlayerOn = true;
		localPlayerXRNode = xrNode;
		ToggleVelocityTracker(enable: true, num, offset);
	}

	public void DetachLocalPlayer()
	{
		if (GorillaTagger.hasInstance && (bool)GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
		}
		localPlayerOn = false;
		localPlayerBoneIndex = 0;
		RefreshAllBonesMass();
	}

	private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default(Vector3))
	{
		if (enable)
		{
			velocityTracker.transform.SetParent(GetBone(boneIndex));
			velocityTracker.transform.localPosition = offset;
			velocityTracker.ResetState();
		}
		velocityTracker.gameObject.SetActive(enable);
		if (enable)
		{
			velocityTracker.Tick();
		}
	}

	private void RefreshAllBonesMass()
	{
		int num = 0;
		foreach (KeyValuePair<int, int> remotePlayer in remotePlayers)
		{
			if (remotePlayer.Value > num)
			{
				num = remotePlayer.Value;
			}
		}
		if (localPlayerBoneIndex > num)
		{
			num = localPlayerBoneIndex;
		}
		VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, hasPlayers, num);
	}

	public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
	{
		Transform bone = GetBone(boneIndex);
		if (bone == null)
		{
			return false;
		}
		offsetTransform.SetParent(bone.transform);
		offsetTransform.localPosition = offset;
		offsetTransform.localRotation = Quaternion.identity;
		if (remotePlayers.ContainsKey(playerId))
		{
			Debug.LogError("already on the list!");
			return false;
		}
		remotePlayers.Add(playerId, boneIndex);
		RefreshAllBonesMass();
		return true;
	}

	public void DetachRemotePlayer(int playerId)
	{
		remotePlayers.Remove(playerId);
		RefreshAllBonesMass();
	}

	public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope)
	{
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("SetVelocity", RpcTarget.All, boneIndex, velocity, wholeRope);
		}
		else
		{
			SetVelocity(boneIndex, velocity, wholeRope, default(PhotonMessageInfo));
		}
	}

	[PunRPC]
	public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfo info)
	{
		if (info.Sender != null)
		{
			GorillaNot.IncrementRPCCall(info, "SetVelocity");
		}
		if (!base.isActiveAndEnabled || !velocity.IsValid())
		{
			return;
		}
		boneIndex = Mathf.Clamp(boneIndex, 0, nodes.Length);
		Transform bone = GetBone(boneIndex);
		if (!bone)
		{
			return;
		}
		if (info.Sender != null && !info.Sender.IsLocal)
		{
			VRRig vRRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (!vRRig || Vector3.Distance(bone.position, vRRig.transform.position) > 5f)
			{
				return;
			}
		}
		SetIsIdle(idle: false);
		if ((bool)bone)
		{
			VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
		}
	}
}
