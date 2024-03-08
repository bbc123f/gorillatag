using System;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[OrderBefore(new Type[] { typeof(NetworkTransform) })]
[DisallowMultipleComponent]
[NetworkBehaviourWeaved(24)]
public class NetworkCharacterControllerPrototype : NetworkTransform
{
	[Networked]
	[HideInInspector]
	[NetworkedWeaved(20, 1)]
	public bool IsGrounded
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.IsGrounded. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(this.Ptr + 20);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.IsGrounded. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(this.Ptr + 20, value);
		}
	}

	[Networked]
	[HideInInspector]
	[NetworkedWeaved(21, 3)]
	public Vector3 Velocity
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.Velocity. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadVector3(this.Ptr + 21, 0.001f);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing NetworkCharacterControllerPrototype.Velocity. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteVector3(this.Ptr + 21, 999.99994f, value);
		}
	}

	protected override Vector3 DefaultTeleportInterpolationVelocity
	{
		get
		{
			return this.Velocity;
		}
	}

	protected override Vector3 DefaultTeleportInterpolationAngularVelocity
	{
		get
		{
			return new Vector3(0f, 0f, this.rotationSpeed);
		}
	}

	public CharacterController Controller { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		this.CacheController();
	}

	public override void Spawned()
	{
		base.Spawned();
		this.CacheController();
	}

	private void CacheController()
	{
		if (this.Controller == null)
		{
			this.Controller = base.GetComponent<CharacterController>();
		}
	}

	protected override void CopyFromBufferToEngine()
	{
		this.Controller.enabled = false;
		base.CopyFromBufferToEngine();
		this.Controller.enabled = true;
	}

	public virtual void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
	{
		if (this.IsGrounded || ignoreGrounded)
		{
			Vector3 velocity = this.Velocity;
			velocity.y += overrideImpulse ?? this.jumpImpulse;
			this.Velocity = velocity;
		}
	}

	public virtual void Move(Vector3 direction)
	{
		float deltaTime = this.Runner.DeltaTime;
		Vector3 position = base.transform.position;
		Vector3 velocity = this.Velocity;
		direction = direction.normalized;
		if (this.IsGrounded && velocity.y < 0f)
		{
			velocity.y = 0f;
		}
		velocity.y += this.gravity * this.Runner.DeltaTime;
		Vector3 vector = default(Vector3);
		vector.x = velocity.x;
		vector.z = velocity.z;
		if (direction == default(Vector3))
		{
			vector = Vector3.Lerp(vector, default(Vector3), this.braking * deltaTime);
		}
		else
		{
			vector = Vector3.ClampMagnitude(vector + direction * this.acceleration * deltaTime, this.maxSpeed);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(direction), this.rotationSpeed * this.Runner.DeltaTime);
		}
		velocity.x = vector.x;
		velocity.z = vector.z;
		this.Controller.Move(velocity * deltaTime);
		this.Velocity = (base.transform.position - position) * (float)this.Runner.Simulation.Config.TickRate;
		this.IsGrounded = this.Controller.isGrounded;
	}

	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.IsGrounded = this._IsGrounded;
		this.Velocity = this._Velocity;
	}

	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._IsGrounded = this.IsGrounded;
		this._Velocity = this.Velocity;
	}

	[Header("Character Controller Settings")]
	public float gravity = -20f;

	public float jumpImpulse = 8f;

	public float acceleration = 10f;

	public float braking = 10f;

	public float maxSpeed = 2f;

	public float rotationSpeed = 15f;

	[HideInInspector]
	[SerializeField]
	[DefaultForProperty("IsGrounded", 20, 1)]
	private bool _IsGrounded;

	[HideInInspector]
	[SerializeField]
	[DefaultForProperty("Velocity", 21, 3)]
	private Vector3 _Velocity;

	static Changed<NetworkCharacterControllerPrototype> $IL2CPP_CHANGED;

	static ChangedDelegate<NetworkCharacterControllerPrototype> $IL2CPP_CHANGED_DELEGATE;

	static NetworkBehaviourCallbacks<NetworkCharacterControllerPrototype> $IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;
}
