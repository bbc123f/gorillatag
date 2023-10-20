using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200014C RID: 332
public class AutomaticAdjustIPD : MonoBehaviour
{
	// Token: 0x0600084A RID: 2122 RVA: 0x000335C4 File Offset: 0x000317C4
	private void Update()
	{
		InputDevice inputDevice = this.headset;
		if (!this.headset.isValid)
		{
			this.headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		}
		if (this.headset.isValid && this.headset.TryGetFeatureValue(CommonUsages.leftEyePosition, out this.leftEyePosition) && this.headset.TryGetFeatureValue(CommonUsages.rightEyePosition, out this.rightEyePosition))
		{
			this.currentIPD = (this.rightEyePosition - this.leftEyePosition).magnitude;
			if (Mathf.Abs(this.lastIPD - this.currentIPD) < 0.01f)
			{
				return;
			}
			this.lastIPD = this.currentIPD;
			Transform[] array = this.adjustXScaleObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].localScale = new Vector3(Mathf.LerpUnclamped(1f, 1.12f, (this.currentIPD - 0.058f) / 0.0050000027f), 1f, 1f);
			}
		}
	}

	// Token: 0x04000A30 RID: 2608
	public InputDevice headset;

	// Token: 0x04000A31 RID: 2609
	public float currentIPD;

	// Token: 0x04000A32 RID: 2610
	public Vector3 leftEyePosition;

	// Token: 0x04000A33 RID: 2611
	public Vector3 rightEyePosition;

	// Token: 0x04000A34 RID: 2612
	public bool testOverride;

	// Token: 0x04000A35 RID: 2613
	public Transform[] adjustXScaleObjects;

	// Token: 0x04000A36 RID: 2614
	public float sizeAt58mm = 1f;

	// Token: 0x04000A37 RID: 2615
	public float sizeAt63mm = 1.12f;

	// Token: 0x04000A38 RID: 2616
	public float lastIPD;
}
