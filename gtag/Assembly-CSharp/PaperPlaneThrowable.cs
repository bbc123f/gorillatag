﻿using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PaperPlaneThrowable : TransferrableObject
{
	private void OnLaunchRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (info.senderID != this.ownerRig.creatorWrapped.ID)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnLaunchRPC");
		if (sender != receiver)
		{
			return;
		}
		if (!this)
		{
			return;
		}
		int num = PaperPlaneThrowable.FetchViewID(this);
		int num2 = (int)args[0];
		if (num == -1)
		{
			return;
		}
		if (num2 == -1)
		{
			return;
		}
		if (num != num2)
		{
			return;
		}
		Vector3 launchPos = (Vector3)args[1];
		Quaternion launchRot = (Quaternion)args[2];
		Vector3 releaseVel = (Vector3)args[3];
		if (!launchPos.IsValid() || !launchRot.IsValid() || !releaseVel.IsValid() || this._renderer.forceRenderingOff)
		{
			return;
		}
		this.LaunchProjectile(launchPos, launchRot, releaseVel);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		this._lastWorldPos = base.transform.position;
		if (PaperPlaneThrowable.gLaunchRPC == null)
		{
			PaperPlaneThrowable.gLaunchRPC = new PhotonEvent(StaticHash.Combine("PaperPlaneThrowable", "LaunchProjectile"));
			PaperPlaneThrowable.gLaunchRPC.reliable = false;
		}
		this._renderer.forceRenderingOff = false;
		PaperPlaneThrowable.gLaunchRPC += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnLaunchRPC);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		PaperPlaneThrowable.gLaunchRPC -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnLaunchRPC);
	}

	protected override void Start()
	{
		base.Start();
		if (PaperPlaneThrowable._playerView == null)
		{
			PaperPlaneThrowable._playerView = Camera.main;
		}
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this._renderer.forceRenderingOff)
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
	}

	private static int FetchViewID(PaperPlaneThrowable ppt)
	{
		Photon.Realtime.Player player = (ppt.myOnlineRig != null) ? ppt.myOnlineRig.creator : ((ppt.myRig != null) ? ((ppt.myRig.creator != null) ? ppt.myRig.creator : PhotonNetwork.LocalPlayer) : null);
		if (player == null)
		{
			return -1;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return -1;
		}
		if (rigContainer.Rig.photonView == null)
		{
			return -1;
		}
		return rigContainer.Rig.photonView.ViewID;
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this._renderer.forceRenderingOff)
		{
			return false;
		}
		GorillaVelocityTracker gorillaVelocityTracker = (releasingHand == EquipmentInteractor.instance.rightHand) ? GorillaLocomotion.Player.Instance.rightInteractPointVelocityTracker : GorillaLocomotion.Player.Instance.leftInteractPointVelocityTracker;
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = gorillaVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		int num = PaperPlaneThrowable.FetchViewID(this);
		this.LaunchProjectile(vector, rotation, averageVelocity);
		PaperPlaneThrowable.gLaunchRPC.RaiseOthers(new object[]
		{
			num,
			vector,
			rotation,
			averageVelocity
		});
		return true;
	}

	private void LaunchProjectile(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel)
	{
		if (releaseVel.sqrMagnitude <= this.minThrowSpeed * base.transform.lossyScale.z * base.transform.lossyScale.z)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this._projectilePrefab.gameObject, launchPos);
		gameObject.transform.localScale = base.transform.lossyScale;
		PaperPlaneProjectile component = gameObject.GetComponent<PaperPlaneProjectile>();
		component.OnHit += this.OnProjectileHit;
		component.ResetProjectile();
		component.SetVRRig(this.myRig);
		component.Launch(launchPos, launchRot, releaseVel);
		this._renderer.forceRenderingOff = true;
	}

	private void OnProjectileHit(Vector3 endPoint)
	{
		this._renderer.forceRenderingOff = false;
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		Transform transform = base.transform;
		Vector3 position = transform.position;
		this._itemWorldVel = (position - this._lastWorldPos) / Time.deltaTime;
		Quaternion localRotation = transform.localRotation;
		this._itemWorldAngVel = PaperPlaneThrowable.CalcAngularVelocity(this._lastWorldRot, localRotation, Time.deltaTime);
		this._lastWorldRot = localRotation;
		this._lastWorldPos = position;
	}

	private static Vector3 CalcAngularVelocity(Quaternion from, Quaternion to, float dt)
	{
		Vector3 vector = (to * Quaternion.Inverse(from)).eulerAngles;
		if (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		if (vector.y > 180f)
		{
			vector.y -= 360f;
		}
		if (vector.z > 180f)
		{
			vector.z -= 360f;
		}
		vector *= 0.017453292f / dt;
		return vector;
	}

	public override void DropItem()
	{
		base.DropItem();
	}

	public PaperPlaneThrowable()
	{
	}

	[SerializeField]
	private Renderer _renderer;

	[SerializeField]
	private GameObject _projectilePrefab;

	[SerializeField]
	private float minThrowSpeed;

	private static Camera _playerView;

	private static PhotonEvent gLaunchRPC;

	[Space]
	[DebugReadOnly]
	private Vector3 _lastWorldPos;

	[DebugReadOnly]
	private Quaternion _lastWorldRot;

	[Space]
	[DebugReadOnly]
	private Vector3 _itemWorldVel;

	[DebugReadOnly]
	private Vector3 _itemWorldAngVel;
}
