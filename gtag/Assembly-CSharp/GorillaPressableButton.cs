using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001B4 RID: 436
public class GorillaPressableButton : MonoBehaviour
{
	// Token: 0x06000B22 RID: 2850 RVA: 0x00044DEF File Offset: 0x00042FEF
	public virtual void Start()
	{
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x00044DF1 File Offset: 0x00042FF1
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x00044E07 File Offset: 0x00043007
	private void OnDisable()
	{
		if (Application.isEditor)
		{
			base.StopAllCoroutines();
		}
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00044E16 File Offset: 0x00043016
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

	// Token: 0x06000B26 RID: 2854 RVA: 0x00044E28 File Offset: 0x00043028
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

	// Token: 0x06000B27 RID: 2855 RVA: 0x00044F3C File Offset: 0x0004313C
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

	// Token: 0x06000B28 RID: 2856 RVA: 0x00044FB2 File Offset: 0x000431B2
	public virtual void ButtonActivation()
	{
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x00044FB4 File Offset: 0x000431B4
	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x04000E76 RID: 3702
	public Material pressedMaterial;

	// Token: 0x04000E77 RID: 3703
	public Material unpressedMaterial;

	// Token: 0x04000E78 RID: 3704
	public MeshRenderer buttonRenderer;

	// Token: 0x04000E79 RID: 3705
	public bool isOn;

	// Token: 0x04000E7A RID: 3706
	public float debounceTime = 0.25f;

	// Token: 0x04000E7B RID: 3707
	public float touchTime;

	// Token: 0x04000E7C RID: 3708
	public bool testPress;

	// Token: 0x04000E7D RID: 3709
	public bool testHandLeft;

	// Token: 0x04000E7E RID: 3710
	[TextArea]
	public string offText;

	// Token: 0x04000E7F RID: 3711
	[TextArea]
	public string onText;

	// Token: 0x04000E80 RID: 3712
	public Text myText;

	// Token: 0x04000E81 RID: 3713
	[Space]
	public UnityEvent onPressButton;
}
