﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

internal class ConnectedControllerHandler : MonoBehaviour
{
	public static ConnectedControllerHandler Instance
	{
		[CompilerGenerated]
		get
		{
			return ConnectedControllerHandler.<Instance>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			ConnectedControllerHandler.<Instance>k__BackingField = value;
		}
	}

	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	private void Awake()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ConnectedControllerHandler.Instance = this;
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.rightXRController == null || this.leftXRController == null || this.snapTurnController == null)
		{
			base.enabled = false;
			return;
		}
		this.rightControllerList = new List<XRController>();
		this.leftcontrollerList = new List<XRController>();
		this.rightControllerList.Add(this.rightXRController);
		this.leftcontrollerList.Add(this.leftXRController);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		InputDevice deviceAtXRNode2 = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		Debug.Log(string.Format("right controller? {0}", (deviceAtXRNode.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)));
		this.rightControllerValid = deviceAtXRNode.isValid;
		this.leftControllerValid = deviceAtXRNode2.isValid;
		InputDevices.deviceConnected += this.DeviceConnected;
		InputDevices.deviceDisconnected += this.DeviceDisconnected;
		this.UpdateControllerStates();
	}

	private void Start()
	{
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.leftXRController == null || this.rightXRController == null || this.snapTurnController == null)
		{
			return;
		}
		this.playerHandler = Player.Instance;
		this.rightHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
		this.leftHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.ControllerValidator());
	}

	private void ONDisable()
	{
		base.StopCoroutine(this.ControllerValidator());
	}

	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
		InputDevices.deviceConnected -= this.DeviceConnected;
		InputDevices.deviceDisconnected -= this.DeviceDisconnected;
	}

	private void LateUpdate()
	{
		if (!this.rightValid)
		{
			this.rightHandFollower.UpdatePositionRotation();
		}
		if (!this.leftValid)
		{
			this.leftHandFollower.UpdatePositionRotation();
		}
	}

	private IEnumerator ControllerValidator()
	{
		yield return null;
		this.lastRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
		this.lastLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
		for (;;)
		{
			yield return new WaitForSeconds(this.overridePollRate);
			this.updateControllers = false;
			if (!this.playerHandler.inOverlay)
			{
				if (this.rightControllerValid)
				{
					this.tempRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
					if (this.tempRightPos == this.lastRightPos)
					{
						if ((this.overrideController & OverrideControllers.RightController) != OverrideControllers.RightController)
						{
							this.overrideController |= OverrideControllers.RightController;
							this.updateControllers = true;
						}
					}
					else if ((this.overrideController & OverrideControllers.RightController) == OverrideControllers.RightController)
					{
						this.overrideController &= ~OverrideControllers.RightController;
						this.updateControllers = true;
					}
					this.lastRightPos = this.tempRightPos;
				}
				if (this.leftControllerValid)
				{
					this.tempLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
					if (this.tempLeftPos == this.lastLeftPos)
					{
						if ((this.overrideController & OverrideControllers.LeftController) != OverrideControllers.LeftController)
						{
							this.overrideController |= OverrideControllers.LeftController;
							this.updateControllers = true;
						}
					}
					else if ((this.overrideController & OverrideControllers.LeftController) == OverrideControllers.LeftController)
					{
						this.overrideController &= ~OverrideControllers.LeftController;
						this.updateControllers = true;
					}
					this.lastLeftPos = this.tempLeftPos;
				}
				if (this.updateControllers)
				{
					this.overrideEnabled = (this.overrideController > OverrideControllers.None);
					this.UpdateControllerStates();
				}
			}
		}
		yield break;
	}

	private void DeviceDisconnected(InputDevice device)
	{
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right))
		{
			this.rightControllerValid = false;
		}
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left))
		{
			this.leftControllerValid = false;
		}
		this.UpdateControllerStates();
	}

	private void DeviceConnected(InputDevice device)
	{
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right))
		{
			this.rightControllerValid = true;
		}
		if ((device.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left)) == (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left))
		{
			this.leftControllerValid = true;
		}
		this.UpdateControllerStates();
	}

	private void UpdateControllerStates()
	{
		if (this.overrideEnabled && this.overrideController != OverrideControllers.None)
		{
			this.rightValid = (this.rightControllerValid && (this.overrideController & OverrideControllers.RightController) != OverrideControllers.RightController);
			this.leftValid = (this.leftControllerValid && (this.overrideController & OverrideControllers.LeftController) != OverrideControllers.LeftController);
		}
		else
		{
			this.rightValid = this.rightControllerValid;
			this.leftValid = this.leftControllerValid;
		}
		this.rightXRController.enabled = this.rightValid;
		this.leftXRController.enabled = this.leftValid;
		this.AssignSnapturnController();
	}

	private void AssignSnapturnController()
	{
		if (!this.leftValid && this.rightValid)
		{
			this.snapTurnController.controllers = this.rightControllerList;
			return;
		}
		if (!this.rightValid && this.leftValid)
		{
			this.snapTurnController.controllers = this.leftcontrollerList;
			return;
		}
		this.snapTurnController.controllers = this.rightControllerList;
	}

	public bool GetValidForXRNode(XRNode controllerNode)
	{
		bool result;
		if (controllerNode != XRNode.LeftHand)
		{
			result = (controllerNode != XRNode.RightHand || this.rightValid);
		}
		else
		{
			result = this.leftValid;
		}
		return result;
	}

	public ConnectedControllerHandler()
	{
	}

	[CompilerGenerated]
	[OnEnterPlay_SetNull]
	private static ConnectedControllerHandler <Instance>k__BackingField;

	[SerializeField]
	private HandTransformFollowOffest rightHandFollower;

	[SerializeField]
	private HandTransformFollowOffest leftHandFollower;

	[SerializeField]
	private XRController rightXRController;

	[SerializeField]
	private XRController leftXRController;

	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	private List<XRController> rightControllerList;

	private List<XRController> leftcontrollerList;

	private const InputDeviceCharacteristics rightCharecteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;

	private const InputDeviceCharacteristics leftCharecteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;

	private bool rightControllerValid = true;

	private bool leftControllerValid = true;

	[SerializeField]
	private bool rightValid = true;

	[SerializeField]
	private bool leftValid = true;

	[SerializeField]
	private Vector3 lastRightPos;

	[SerializeField]
	private Vector3 lastLeftPos;

	private Vector3 tempRightPos;

	private Vector3 tempLeftPos;

	private bool updateControllers;

	private Player playerHandler;

	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float overridePollRate = 15f;

	[SerializeField]
	private bool overrideEnabled;

	[SerializeField]
	private OverrideControllers overrideController;

	[CompilerGenerated]
	private sealed class <ControllerValidator>d__36 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <ControllerValidator>d__36(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			ConnectedControllerHandler connectedControllerHandler = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				connectedControllerHandler.lastRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
				connectedControllerHandler.lastLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
				break;
			case 2:
				this.<>1__state = -1;
				connectedControllerHandler.updateControllers = false;
				if (!connectedControllerHandler.playerHandler.inOverlay)
				{
					if (connectedControllerHandler.rightControllerValid)
					{
						connectedControllerHandler.tempRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
						if (connectedControllerHandler.tempRightPos == connectedControllerHandler.lastRightPos)
						{
							if ((connectedControllerHandler.overrideController & OverrideControllers.RightController) != OverrideControllers.RightController)
							{
								connectedControllerHandler.overrideController |= OverrideControllers.RightController;
								connectedControllerHandler.updateControllers = true;
							}
						}
						else if ((connectedControllerHandler.overrideController & OverrideControllers.RightController) == OverrideControllers.RightController)
						{
							connectedControllerHandler.overrideController &= ~OverrideControllers.RightController;
							connectedControllerHandler.updateControllers = true;
						}
						connectedControllerHandler.lastRightPos = connectedControllerHandler.tempRightPos;
					}
					if (connectedControllerHandler.leftControllerValid)
					{
						connectedControllerHandler.tempLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
						if (connectedControllerHandler.tempLeftPos == connectedControllerHandler.lastLeftPos)
						{
							if ((connectedControllerHandler.overrideController & OverrideControllers.LeftController) != OverrideControllers.LeftController)
							{
								connectedControllerHandler.overrideController |= OverrideControllers.LeftController;
								connectedControllerHandler.updateControllers = true;
							}
						}
						else if ((connectedControllerHandler.overrideController & OverrideControllers.LeftController) == OverrideControllers.LeftController)
						{
							connectedControllerHandler.overrideController &= ~OverrideControllers.LeftController;
							connectedControllerHandler.updateControllers = true;
						}
						connectedControllerHandler.lastLeftPos = connectedControllerHandler.tempLeftPos;
					}
					if (connectedControllerHandler.updateControllers)
					{
						connectedControllerHandler.overrideEnabled = (connectedControllerHandler.overrideController > OverrideControllers.None);
						connectedControllerHandler.UpdateControllerStates();
					}
				}
				break;
			default:
				return false;
			}
			this.<>2__current = new WaitForSeconds(connectedControllerHandler.overridePollRate);
			this.<>1__state = 2;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public ConnectedControllerHandler <>4__this;
	}
}
