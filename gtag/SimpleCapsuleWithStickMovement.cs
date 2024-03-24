using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	public event Action CameraUpdated
	{
		[CompilerGenerated]
		add
		{
			Action action = this.CameraUpdated;
			Action action2;
			do
			{
				action2 = action;
				Action action3 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.CameraUpdated, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action action = this.CameraUpdated;
			Action action2;
			do
			{
				action2 = action;
				Action action3 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.CameraUpdated, action3, action2);
			}
			while (action != action2);
		}
	}

	public event Action PreCharacterMove
	{
		[CompilerGenerated]
		add
		{
			Action action = this.PreCharacterMove;
			Action action2;
			do
			{
				action2 = action;
				Action action3 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.PreCharacterMove, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action action = this.PreCharacterMove;
			Action action2;
			do
			{
				action2 = action;
				Action action3 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.PreCharacterMove, action3, action2);
			}
			while (action != action2);
		}
	}

	private void Awake()
	{
		this._rigidbody = base.GetComponent<Rigidbody>();
		if (this.CameraRig == null)
		{
			this.CameraRig = base.GetComponentInChildren<OVRCameraRig>();
		}
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (this.CameraUpdated != null)
		{
			this.CameraUpdated();
		}
		if (this.PreCharacterMove != null)
		{
			this.PreCharacterMove();
		}
		if (this.HMDRotatesPlayer)
		{
			this.RotatePlayerToHMD();
		}
		if (this.EnableLinearMovement)
		{
			this.StickMovement();
		}
		if (this.EnableRotation)
		{
			this.SnapTurn();
		}
	}

	private void RotatePlayerToHMD()
	{
		Transform trackingSpace = this.CameraRig.trackingSpace;
		Transform centerEyeAnchor = this.CameraRig.centerEyeAnchor;
		Vector3 position = trackingSpace.position;
		Quaternion rotation = trackingSpace.rotation;
		base.transform.rotation = Quaternion.Euler(0f, centerEyeAnchor.rotation.eulerAngles.y, 0f);
		trackingSpace.position = position;
		trackingSpace.rotation = rotation;
	}

	private void StickMovement()
	{
		Vector3 eulerAngles = this.CameraRig.centerEyeAnchor.rotation.eulerAngles;
		eulerAngles.z = (eulerAngles.x = 0f);
		Quaternion quaternion = Quaternion.Euler(eulerAngles);
		Vector3 vector = Vector3.zero;
		Vector2 vector2 = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Active);
		vector += quaternion * (vector2.x * Vector3.right);
		vector += quaternion * (vector2.y * Vector3.forward);
		this._rigidbody.MovePosition(this._rigidbody.position + vector * this.Speed * Time.fixedDeltaTime);
	}

	private void SnapTurn()
	{
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, -this.RotationAngle);
				return;
			}
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight, OVRInput.Controller.Active) || (this.RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.Active)))
		{
			if (this.ReadyToSnapTurn)
			{
				this.ReadyToSnapTurn = false;
				base.transform.RotateAround(this.CameraRig.centerEyeAnchor.position, Vector3.up, this.RotationAngle);
				return;
			}
		}
		else
		{
			this.ReadyToSnapTurn = true;
		}
	}

	public SimpleCapsuleWithStickMovement()
	{
	}

	public bool EnableLinearMovement = true;

	public bool EnableRotation = true;

	public bool HMDRotatesPlayer = true;

	public bool RotationEitherThumbstick;

	public float RotationAngle = 45f;

	public float Speed;

	public OVRCameraRig CameraRig;

	private bool ReadyToSnapTurn;

	private Rigidbody _rigidbody;

	[CompilerGenerated]
	private Action CameraUpdated;

	[CompilerGenerated]
	private Action PreCharacterMove;
}
