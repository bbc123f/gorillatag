using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000152 RID: 338
internal class ConnectedControllerHandler : MonoBehaviour
{
	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000859 RID: 2137 RVA: 0x00033F26 File Offset: 0x00032126
	// (set) Token: 0x0600085A RID: 2138 RVA: 0x00033F2D File Offset: 0x0003212D
	public static ConnectedControllerHandler Instance { get; private set; }

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600085B RID: 2139 RVA: 0x00033F35 File Offset: 0x00032135
	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600085C RID: 2140 RVA: 0x00033F3D File Offset: 0x0003213D
	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x00033F48 File Offset: 0x00032148
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

	// Token: 0x0600085E RID: 2142 RVA: 0x00034070 File Offset: 0x00032270
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

	// Token: 0x0600085F RID: 2143 RVA: 0x000340F7 File Offset: 0x000322F7
	private void OnEnable()
	{
		base.StartCoroutine(this.ControllerValidator());
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x00034106 File Offset: 0x00032306
	private void ONDisable()
	{
		base.StopCoroutine(this.ControllerValidator());
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x00034114 File Offset: 0x00032314
	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
		InputDevices.deviceConnected -= this.DeviceConnected;
		InputDevices.deviceDisconnected -= this.DeviceDisconnected;
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x00034163 File Offset: 0x00032363
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

	// Token: 0x06000863 RID: 2147 RVA: 0x0003418B File Offset: 0x0003238B
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

	// Token: 0x06000864 RID: 2148 RVA: 0x0003419A File Offset: 0x0003239A
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

	// Token: 0x06000865 RID: 2149 RVA: 0x000341D8 File Offset: 0x000323D8
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

	// Token: 0x06000866 RID: 2150 RVA: 0x00034218 File Offset: 0x00032418
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

	// Token: 0x06000867 RID: 2151 RVA: 0x000342B8 File Offset: 0x000324B8
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

	// Token: 0x06000868 RID: 2152 RVA: 0x0003431C File Offset: 0x0003251C
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

	// Token: 0x04000A6B RID: 2667
	[SerializeField]
	private HandTransformFollowOffest rightHandFollower;

	// Token: 0x04000A6C RID: 2668
	[SerializeField]
	private HandTransformFollowOffest leftHandFollower;

	// Token: 0x04000A6D RID: 2669
	[SerializeField]
	private XRController rightXRController;

	// Token: 0x04000A6E RID: 2670
	[SerializeField]
	private XRController leftXRController;

	// Token: 0x04000A6F RID: 2671
	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	// Token: 0x04000A70 RID: 2672
	private List<XRController> rightControllerList;

	// Token: 0x04000A71 RID: 2673
	private List<XRController> leftcontrollerList;

	// Token: 0x04000A72 RID: 2674
	private const InputDeviceCharacteristics rightCharecteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;

	// Token: 0x04000A73 RID: 2675
	private const InputDeviceCharacteristics leftCharecteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;

	// Token: 0x04000A74 RID: 2676
	private bool rightControllerValid = true;

	// Token: 0x04000A75 RID: 2677
	private bool leftControllerValid = true;

	// Token: 0x04000A76 RID: 2678
	[SerializeField]
	private bool rightValid = true;

	// Token: 0x04000A77 RID: 2679
	[SerializeField]
	private bool leftValid = true;

	// Token: 0x04000A78 RID: 2680
	[SerializeField]
	private Vector3 lastRightPos;

	// Token: 0x04000A79 RID: 2681
	[SerializeField]
	private Vector3 lastLeftPos;

	// Token: 0x04000A7A RID: 2682
	private Vector3 tempRightPos;

	// Token: 0x04000A7B RID: 2683
	private Vector3 tempLeftPos;

	// Token: 0x04000A7C RID: 2684
	private bool updateControllers;

	// Token: 0x04000A7D RID: 2685
	private Player playerHandler;

	// Token: 0x04000A7E RID: 2686
	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float overridePollRate = 15f;

	// Token: 0x04000A7F RID: 2687
	[SerializeField]
	private bool overrideEnabled;

	// Token: 0x04000A80 RID: 2688
	[SerializeField]
	private OverrideControllers overrideController;
}
