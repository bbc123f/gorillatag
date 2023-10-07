using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200012A RID: 298
public class HotPepperEvents : MonoBehaviour
{
	// Token: 0x060007D0 RID: 2000 RVA: 0x000316E7 File Offset: 0x0002F8E7
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x00031721 File Offset: 0x0002F921
	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0003175B File Offset: 0x0002F95B
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00031766 File Offset: 0x0002F966
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00031771 File Offset: 0x0002F971
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

	// Token: 0x02000410 RID: 1040
	public enum EdibleState
	{
		// Token: 0x04001CE0 RID: 7392
		A = 1,
		// Token: 0x04001CE1 RID: 7393
		B,
		// Token: 0x04001CE2 RID: 7394
		C = 4,
		// Token: 0x04001CE3 RID: 7395
		D = 8
	}
}
