using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000065 RID: 101
public class LocalChestController : MonoBehaviour
{
	// Token: 0x060001E4 RID: 484 RVA: 0x0000D584 File Offset: 0x0000B784
	private void OnTriggerEnter(Collider other)
	{
		if (this.isOpen)
		{
			return;
		}
		TransformFollow component = other.GetComponent<TransformFollow>();
		if (component == null)
		{
			return;
		}
		Transform transformToFollow = component.transformToFollow;
		if (transformToFollow == null)
		{
			return;
		}
		VRRig componentInParent = transformToFollow.GetComponentInParent<VRRig>();
		if (componentInParent == null)
		{
			return;
		}
		if (this.playerCollectionVolume != null && !this.playerCollectionVolume.containedRigs.Contains(componentInParent))
		{
			return;
		}
		this.isOpen = true;
		this.director.Play();
	}

	// Token: 0x040002B5 RID: 693
	public PlayableDirector director;

	// Token: 0x040002B6 RID: 694
	public MazePlayerCollection playerCollectionVolume;

	// Token: 0x040002B7 RID: 695
	private bool isOpen;
}
