using System;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
[NetworkBehaviourWeaved(3)]
public class ControllerPrototype : NetworkBehaviour
{
	[Networked]
	[NetworkedWeaved(0, 3)]
	public Vector3 MovementDirection
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ControllerPrototype.MovementDirection. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadVector3(this.Ptr + 0, 0.001f);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ControllerPrototype.MovementDirection. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteVector3(this.Ptr + 0, 999.99994f, value);
		}
	}

	private bool ShowSpeed
	{
		get
		{
			NetworkCharacterControllerPrototype networkCharacterControllerPrototype;
			return this && !base.TryGetComponent<NetworkCharacterControllerPrototype>(out networkCharacterControllerPrototype);
		}
	}

	public void Awake()
	{
		this.CacheComponents();
	}

	public override void Spawned()
	{
		this.CacheComponents();
	}

	private void CacheComponents()
	{
		if (!this._ncc)
		{
			this._ncc = base.GetComponent<NetworkCharacterControllerPrototype>();
		}
		if (!this._nrb)
		{
			this._nrb = base.GetComponent<NetworkRigidbody>();
		}
		if (!this._nrb2d)
		{
			this._nrb2d = base.GetComponent<NetworkRigidbody2D>();
		}
		if (!this._nt)
		{
			this._nt = base.GetComponent<NetworkTransform>();
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (this.Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.None)
		{
			return;
		}
		NetworkInputPrototype networkInputPrototype;
		Vector3 vector;
		if (base.GetInput<NetworkInputPrototype>(out networkInputPrototype))
		{
			vector = default(Vector3);
			if (networkInputPrototype.IsDown(3))
			{
				vector += (this.TransformLocal ? base.transform.forward : Vector3.forward);
			}
			if (networkInputPrototype.IsDown(4))
			{
				vector -= (this.TransformLocal ? base.transform.forward : Vector3.forward);
			}
			if (networkInputPrototype.IsDown(5))
			{
				vector -= (this.TransformLocal ? base.transform.right : Vector3.right);
			}
			if (networkInputPrototype.IsDown(6))
			{
				vector += (this.TransformLocal ? base.transform.right : Vector3.right);
			}
			vector = vector.normalized;
			this.MovementDirection = vector;
			if (networkInputPrototype.IsDown(7))
			{
				if (this._ncc)
				{
					this._ncc.Jump(false, null);
				}
				else
				{
					vector += (this.TransformLocal ? base.transform.up : Vector3.up);
				}
			}
		}
		else
		{
			vector = this.MovementDirection;
		}
		if (this._ncc)
		{
			this._ncc.Move(vector);
			return;
		}
		if (this._nrb && !this._nrb.Rigidbody.isKinematic)
		{
			this._nrb.Rigidbody.AddForce(vector * this.Speed);
			return;
		}
		if (this._nrb2d && !this._nrb2d.Rigidbody.isKinematic)
		{
			Vector2 a = new Vector2(vector.x, vector.y + vector.z);
			this._nrb2d.Rigidbody.AddForce(a * this.Speed);
			return;
		}
		base.transform.position += vector * this.Speed * this.Runner.DeltaTime;
	}

	public ControllerPrototype()
	{
	}

	public override void CopyBackingFieldsToState(bool A_1)
	{
		this.MovementDirection = this._MovementDirection;
	}

	public override void CopyStateToBackingFields()
	{
		this._MovementDirection = this.MovementDirection;
	}

	protected NetworkCharacterControllerPrototype _ncc;

	protected NetworkRigidbody _nrb;

	protected NetworkRigidbody2D _nrb2d;

	protected NetworkTransform _nt;

	[SerializeField]
	[DefaultForProperty("MovementDirection", 0, 3)]
	private Vector3 _MovementDirection;

	public bool TransformLocal;

	[DrawIf("ShowSpeed", Hide = true)]
	public float Speed = 6f;

	static Changed<ControllerPrototype> $IL2CPP_CHANGED;

	static ChangedDelegate<ControllerPrototype> $IL2CPP_CHANGED_DELEGATE;

	static NetworkBehaviourCallbacks<ControllerPrototype> $IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;
}
