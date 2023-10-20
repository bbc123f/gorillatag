﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x0200005E RID: 94
public class GTDoorTrigger : MonoBehaviour
{
	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060001CB RID: 459 RVA: 0x0000D080 File Offset: 0x0000B280
	public int overlapCount
	{
		get
		{
			return this.overlappingColliders.Count;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x060001CC RID: 460 RVA: 0x0000D08D File Offset: 0x0000B28D
	public bool TriggeredThisFrame
	{
		get
		{
			return this.lastTriggeredFrame == Time.frameCount;
		}
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000D09C File Offset: 0x0000B29C
	public void ValidateOverlappingColliders()
	{
		for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
		{
			if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
			{
				this.overlappingColliders.RemoveAt(i);
			}
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000D10C File Offset: 0x0000B30C
	private void OnTriggerEnter(Collider other)
	{
		if (!this.overlappingColliders.Contains(other))
		{
			this.overlappingColliders.Add(other);
		}
		this.lastTriggeredFrame = Time.frameCount;
		if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
		{
			this.timeline.Play();
		}
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000D185 File Offset: 0x0000B385
	private void OnTriggerExit(Collider other)
	{
		this.overlappingColliders.Remove(other);
	}

	// Token: 0x04000299 RID: 665
	[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
	public PlayableDirector timeline;

	// Token: 0x0400029A RID: 666
	private int lastTriggeredFrame = -1;

	// Token: 0x0400029B RID: 667
	private List<Collider> overlappingColliders = new List<Collider>(20);
}