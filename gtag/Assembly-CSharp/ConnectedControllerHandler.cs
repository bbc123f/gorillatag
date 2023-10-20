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
	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600085A RID: 2138 RVA: 0x00033D66 File Offset: 0x00031F66
	// (set) Token: 0x0600085B RID: 2139 RVA: 0x00033D6D File Offset: 0x00031F6D
	public static ConnectedControllerHandler Instance { get; private set; }

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600085C RID: 2140 RVA: 0x00033D75 File Offset: 0x00031F75
	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600085D RID: 2141 RVA: 0x00033D7D File Offset: 0x00031F7D
	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x00033D88 File Offset: 0x00031F88
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

	// Token: 0x0600085F RID: 2143 RVA: 0x00033EB0 File Offset: 0x000320B0
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

	// Token: 0x06000860 RID: 2144 RVA: 0x00033F37 File Offset: 0x00032137
	private void OnEnable()
	{
		base.StartCoroutine(this.ControllerValidator());
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x00033F46 File Offset: 0x00032146
	private void ONDisable()
	{
		base.StopCoroutine(this.ControllerValidator());
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x00033F54 File Offset: 0x00032154
	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
		InputDevices.deviceConnected -= this.DeviceConnected;
		InputDevices.deviceDisconnected -= this.DeviceDisconnected;
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x00033FA3 File Offset: 0x000321A3
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

	// Token: 0x06000864 RID: 2148 RVA: 0x00033FCB File Offset: 0x000321CB
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

	// Token: 0x06000865 RID: 2149 RVA: 0x00033FDA File Offset: 0x000321DA
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

	// Token: 0x06000866 RID: 2150 RVA: 0x00034018 File Offset: 0x00032218
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

	// Token: 0x06000867 RID: 2151 RVA: 0x00034058 File Offset: 0x00032258
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

	// Token: 0x06000868 RID: 2152 RVA: 0x000340F8 File Offset: 0x000322F8
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

	// Token: 0x06000869 RID: 2153 RVA: 0x0003415C File Offset: 0x0003235C
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
