using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000128 RID: 296
public class BubbleGumEvents : MonoBehaviour
{
	// Token: 0x060007C1 RID: 1985 RVA: 0x000312D4 File Offset: 0x0002F4D4
	private void OnEnable()
	{
		this._edible.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0003130E File Offset: 0x0002F50E
	private void OnDisable()
	{
		this._edible.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x00031348 File Offset: 0x0002F548
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x00031353 File Offset: 0x0002F553
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x00031360 File Offset: 0x0002F560
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		GorillaTagger instance = GorillaTagger.Instance;
		GameObject gameObject = null;
		if (isViewRig && instance != null)
		{
			gameObject = instance.gameObject;
		}
		else if (!isViewRig)
		{
			gameObject = rig.gameObject;
		}
		if (!BubbleGumEvents.gTargetCache.TryGetValue(gameObject, out this._bubble))
		{
			this._bubble = gameObject.GetComponentsInChildren<GumBubble>(true).FirstOrDefault((GumBubble g) => g.transform.parent.name == "$gum");
			if (isViewRig)
			{
				this._bubble.audioSource = instance.offlineVRRig.tagSound;
				this._bubble.targetScale = Vector3.one * 1.36f;
			}
			else
			{
				this._bubble.audioSource = rig.tagSound;
				this._bubble.targetScale = Vector3.one * 2f;
			}
			BubbleGumEvents.gTargetCache.Add(gameObject, this._bubble);
		}
		GumBubble bubble = this._bubble;
		if (bubble != null)
		{
			bubble.transform.parent.gameObject.SetActive(true);
		}
		GumBubble bubble2 = this._bubble;
		if (bubble2 == null)
		{
			return;
		}
		bubble2.InflateDelayed();
	}

	// Token: 0x04000959 RID: 2393
	[SerializeField]
	private EdibleHoldable _edible;

	// Token: 0x0400095A RID: 2394
	[SerializeField]
	private GumBubble _bubble;

	// Token: 0x0400095B RID: 2395
	private static Dictionary<GameObject, GumBubble> gTargetCache = new Dictionary<GameObject, GumBubble>(16);

	// Token: 0x0200040E RID: 1038
	public enum EdibleState
	{
		// Token: 0x04001CD9 RID: 7385
		A = 1,
		// Token: 0x04001CDA RID: 7386
		B,
		// Token: 0x04001CDB RID: 7387
		C = 4,
		// Token: 0x04001CDC RID: 7388
		D = 8
	}
}
