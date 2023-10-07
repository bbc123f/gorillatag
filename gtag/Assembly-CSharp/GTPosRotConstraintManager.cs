using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200005F RID: 95
[DefaultExecutionOrder(1300)]
public class GTPosRotConstraintManager : MonoBehaviour
{
	// Token: 0x060001D1 RID: 465 RVA: 0x0000D168 File Offset: 0x0000B368
	protected void Awake()
	{
		if (GTPosRotConstraintManager.hasInstance && GTPosRotConstraintManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GTPosRotConstraintManager.SetInstance(this);
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000D18B File Offset: 0x0000B38B
	protected void OnDestroy()
	{
		if (GTPosRotConstraintManager.instance == this)
		{
			GTPosRotConstraintManager.hasInstance = false;
			GTPosRotConstraintManager.instance = null;
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000D1A8 File Offset: 0x0000B3A8
	protected void LateUpdate()
	{
		for (int i = 0; i < GTPosRotConstraintManager.constraints.Count; i++)
		{
			Transform source = GTPosRotConstraintManager.constraints[i].source;
			GTPosRotConstraintManager.constraints[i].follower.SetPositionAndRotation(source.position, source.rotation);
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000D1FC File Offset: 0x0000B3FC
	public static void CreateManager()
	{
		GTPosRotConstraintManager.SetInstance(new GameObject("GTPosRotConstraintManager").AddComponent<GTPosRotConstraintManager>());
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000D212 File Offset: 0x0000B412
	private static void SetInstance(GTPosRotConstraintManager manager)
	{
		GTPosRotConstraintManager.instance = manager;
		GTPosRotConstraintManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000D230 File Offset: 0x0000B430
	public static void Register(GTPosRotConstraints component)
	{
		if (!GTPosRotConstraintManager.hasInstance)
		{
			GTPosRotConstraintManager.CreateManager();
		}
		int instanceID = component.GetInstanceID();
		if (GTPosRotConstraintManager.componentRanges.ContainsKey(instanceID))
		{
			return;
		}
		GTPosRotConstraintManager.Range value = new GTPosRotConstraintManager.Range
		{
			start = GTPosRotConstraintManager.constraints.Count,
			end = GTPosRotConstraintManager.constraints.Count + component.constraints.Length - 1
		};
		GTPosRotConstraintManager.componentRanges.Add(instanceID, value);
		GTPosRotConstraintManager.constraints.AddRange(component.constraints);
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
	public static void Unregister(GTPosRotConstraints component)
	{
		int instanceID = component.GetInstanceID();
		GTPosRotConstraintManager.Range range;
		if (!GTPosRotConstraintManager.hasInstance || !GTPosRotConstraintManager.componentRanges.TryGetValue(instanceID, out range))
		{
			return;
		}
		GTPosRotConstraintManager.constraints.RemoveRange(range.start, 1 + range.end - range.start);
		GTPosRotConstraintManager.componentRanges.Remove(instanceID);
		foreach (int key in GTPosRotConstraintManager.componentRanges.Keys.ToArray<int>())
		{
			GTPosRotConstraintManager.Range range2 = GTPosRotConstraintManager.componentRanges[key];
			if (range2.start > range.end)
			{
				GTPosRotConstraintManager.componentRanges[key] = new GTPosRotConstraintManager.Range
				{
					start = range2.start - range.end + range.start - 1,
					end = range2.end - range.end + range.start - 1
				};
			}
		}
	}

	// Token: 0x0400029C RID: 668
	public static GTPosRotConstraintManager instance;

	// Token: 0x0400029D RID: 669
	public static bool hasInstance = false;

	// Token: 0x0400029E RID: 670
	public static readonly List<GorillaPosRotConstraint> constraints = new List<GorillaPosRotConstraint>(1024);

	// Token: 0x0400029F RID: 671
	public static readonly Dictionary<int, GTPosRotConstraintManager.Range> componentRanges = new Dictionary<int, GTPosRotConstraintManager.Range>(256);

	// Token: 0x0200039F RID: 927
	public struct Range
	{
		// Token: 0x04001B57 RID: 6999
		public int start;

		// Token: 0x04001B58 RID: 7000
		public int end;
	}
}
