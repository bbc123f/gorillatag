using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200014A RID: 330
public class PrimaryButtonWatcher : MonoBehaviour
{
	// Token: 0x06000840 RID: 2112 RVA: 0x000334F4 File Offset: 0x000316F4
	private void Awake()
	{
		if (this.primaryButtonPress == null)
		{
			this.primaryButtonPress = new PrimaryButtonEvent();
		}
		this.devicesWithPrimaryButton = new List<InputDevice>();
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00033514 File Offset: 0x00031714
	private void OnEnable()
	{
		List<InputDevice> list = new List<InputDevice>();
		InputDevices.GetDevices(list);
		foreach (InputDevice device in list)
		{
			this.InputDevices_deviceConnected(device);
		}
		InputDevices.deviceConnected += this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected += this.InputDevices_deviceDisconnected;
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x00033590 File Offset: 0x00031790
	private void OnDisable()
	{
		InputDevices.deviceConnected -= this.InputDevices_deviceConnected;
		InputDevices.deviceDisconnected -= this.InputDevices_deviceDisconnected;
		this.devicesWithPrimaryButton.Clear();
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x000335C0 File Offset: 0x000317C0
	private void InputDevices_deviceConnected(InputDevice device)
	{
		bool flag;
		if (device.TryGetFeatureValue(CommonUsages.primaryButton, out flag))
		{
			this.devicesWithPrimaryButton.Add(device);
		}
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x000335E9 File Offset: 0x000317E9
	private void InputDevices_deviceDisconnected(InputDevice device)
	{
		if (this.devicesWithPrimaryButton.Contains(device))
		{
			this.devicesWithPrimaryButton.Remove(device);
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00033608 File Offset: 0x00031808
	private void Update()
	{
		bool flag = false;
		foreach (InputDevice inputDevice in this.devicesWithPrimaryButton)
		{
			bool flag2 = false;
			flag = ((inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out flag2) && flag2) || flag);
		}
		if (flag != this.lastButtonState)
		{
			this.primaryButtonPress.Invoke(flag);
			this.lastButtonState = flag;
		}
	}

	// Token: 0x04000A29 RID: 2601
	public PrimaryButtonEvent primaryButtonPress;

	// Token: 0x04000A2A RID: 2602
	private bool lastButtonState;

	// Token: 0x04000A2B RID: 2603
	private List<InputDevice> devicesWithPrimaryButton;
}
