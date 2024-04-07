using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnJump : MonoBehaviour, ITickSystemTick
{
	private void OnEnable()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		if (this._events == null && componentInParent != null && componentInParent.creator != null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			this._events.Init(componentInParent.creator);
		}
		if (this._events != null)
		{
			this._events.Activate += this.OnActivate;
		}
		bool flag = !PhotonNetwork.InRoom && componentInParent != null && componentInParent.isOfflineVRRig;
		RigContainer rigContainer;
		bool flag2 = PhotonNetwork.InRoom && componentInParent != null && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer != null && rigContainer.Rig != null && rigContainer.Rig == componentInParent;
		if (flag || flag2)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		this.playerOnGround = false;
		this.jumpStartTime = 0f;
		this.lastActivationTime = 0f;
		this.waitingForGrounding = false;
	}

	private void OnActivate(int sender, int target, object[] args)
	{
		if (sender != target)
		{
			return;
		}
		this.onJumping.Invoke();
	}

	public void Tick()
	{
		Player instance = Player.Instance;
		if (instance != null)
		{
			bool flag = this.playerOnGround;
			this.playerOnGround = instance.BodyOnGround || instance.IsHandTouching(true) || instance.IsHandTouching(false);
			float time = Time.time;
			if (this.playerOnGround)
			{
				this.waitingForGrounding = false;
			}
			if (!this.playerOnGround && flag)
			{
				this.jumpStartTime = time;
			}
			if (!this.playerOnGround && !this.waitingForGrounding && instance.Velocity.sqrMagnitude > this.minJumpStrength * this.minJumpStrength && instance.Velocity.y > this.minJumpVertical && time > this.jumpStartTime + this.minJumpTime)
			{
				this.waitingForGrounding = true;
				if (time > this.lastActivationTime + this.cooldownTime)
				{
					this.lastActivationTime = time;
					if (PhotonNetwork.InRoom)
					{
						this._events.Activate.RaiseAll(Array.Empty<object>());
						return;
					}
					this.onJumping.Invoke();
				}
			}
		}
	}

	public bool TickRunning
	{
		[CompilerGenerated]
		get
		{
			return this.<TickRunning>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<TickRunning>k__BackingField = value;
		}
	}

	public TriggerOnJump()
	{
	}

	[SerializeField]
	private float minJumpStrength = 1f;

	[SerializeField]
	private float minJumpVertical = 1f;

	[SerializeField]
	private float cooldownTime = 1f;

	[SerializeField]
	private UnityEvent onJumping;

	private RubberDuckEvents _events;

	private bool playerOnGround;

	private float minJumpTime = 0.05f;

	private bool waitingForGrounding;

	private float jumpStartTime;

	private float lastActivationTime;

	[CompilerGenerated]
	private bool <TickRunning>k__BackingField;
}
