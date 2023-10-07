using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001B3 RID: 435
public class GorillaPressableButton : MonoBehaviour
{
	// Token: 0x06000B1C RID: 2844 RVA: 0x00044B87 File Offset: 0x00042D87
	public virtual void Start()
	{
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x00044B89 File Offset: 0x00042D89
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x00044B9F File Offset: 0x00042D9F
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x00044BAE File Offset: 0x00042DAE
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.ButtonActivation();
				this.ButtonActivationWithHand(this.testHandLeft);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x00044BC0 File Offset: 0x00042DC0
	private void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touchTime = Time.time;
		GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.onPressButton.Invoke();
		this.ButtonActivation();
		this.ButtonActivationWithHand(component.isLeftHand);
		if (component == null)
		{
			return;
		}
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, new object[]
			{
				67,
				component.isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x00044CD4 File Offset: 0x00042ED4
	public virtual void UpdateColor()
	{
		if (this.isOn)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.onText;
				return;
			}
		}
		else
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = this.offText;
			}
		}
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x00044D4A File Offset: 0x00042F4A
	public virtual void ButtonActivation()
	{
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x00044D4C File Offset: 0x00042F4C
	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x04000E72 RID: 3698
	public Material pressedMaterial;

	// Token: 0x04000E73 RID: 3699
	public Material unpressedMaterial;

	// Token: 0x04000E74 RID: 3700
	public MeshRenderer buttonRenderer;

	// Token: 0x04000E75 RID: 3701
	public bool isOn;

	// Token: 0x04000E76 RID: 3702
	public float debounceTime = 0.25f;

	// Token: 0x04000E77 RID: 3703
	public float touchTime;

	// Token: 0x04000E78 RID: 3704
	public bool testPress;

	// Token: 0x04000E79 RID: 3705
	public bool testHandLeft;

	// Token: 0x04000E7A RID: 3706
	[TextArea]
	public string offText;

	// Token: 0x04000E7B RID: 3707
	[TextArea]
	public string onText;

	// Token: 0x04000E7C RID: 3708
	public Text myText;

	// Token: 0x04000E7D RID: 3709
	[Space]
	public UnityEvent onPressButton;
}
