using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200012A RID: 298
public class HotPepperEvents : MonoBehaviour
{
	// Token: 0x060007D1 RID: 2001 RVA: 0x00031527 File Offset: 0x0002F727
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x00031561 File Offset: 0x0002F761
	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0003159B File Offset: 0x0002F79B
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x000315A6 File Offset: 0x0002F7A6
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x000315B1 File Offset: 0x0002F7B1
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		if (nextState != 8)
		{
			return;
		}
		rig.transform.Find("rig/body/head/gorillaface/spicy").gameObject.GetComponent<HotPepperFace>().PlayFX(1f);
	}

	// Token: 0x04000969 RID: 2409
	[SerializeField]
	private EdibleHoldable _pepper;

	// Token: 0x02000412 RID: 1042
	public enum EdibleState
	{
		// Token: 0x04001CED RID: 7405
		A = 1,
		// Token: 0x04001CEE RID: 7406
		B,
		// Token: 0x04001CEF RID: 7407
		C = 4,
		// Token: 0x04001CF0 RID: 7408
		D = 8
	}
}
