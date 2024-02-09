using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(1300)]
public class GTPosRotConstraintManager : MonoBehaviour
{
	protected void Awake()
	{
		if (GTPosRotConstraintManager.hasInstance && GTPosRotConstraintManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GTPosRotConstraintManager.SetInstance(this);
	}

	protected void OnDestroy()
	{
		if (GTPosRotConstraintManager.instance == this)
		{
			GTPosRotConstraintManager.hasInstance = false;
			GTPosRotConstraintManager.instance = null;
		}
	}

	protected void LateUpdate()
	{
		for (int i = 0; i < GTPosRotConstraintManager.constraints.Count; i++)
		{
			Transform source = GTPosRotConstraintManager.constraints[i].source;
			GTPosRotConstraintManager.constraints[i].follower.SetPositionAndRotation(source.position, source.rotation);
		}
	}

	public static void CreateManager()
	{
		GTPosRotConstraintManager.SetInstance(new GameObject("GTPosRotConstraintManager").AddComponent<GTPosRotConstraintManager>());
	}

	private static void SetInstance(GTPosRotConstraintManager manager)
	{
		GTPosRotConstraintManager.instance = manager;
		GTPosRotConstraintManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

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
		GTPosRotConstraintManager.Range range = new GTPosRotConstraintManager.Range
		{
			start = GTPosRotConstraintManager.constraints.Count,
			end = GTPosRotConstraintManager.constraints.Count + component.constraints.Length - 1
		};
		GTPosRotConstraintManager.componentRanges.Add(instanceID, range);
		GTPosRotConstraintManager.constraints.AddRange(component.constraints);
	}

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
		foreach (int num in GTPosRotConstraintManager.componentRanges.Keys.ToArray<int>())
		{
			GTPosRotConstraintManager.Range range2 = GTPosRotConstraintManager.componentRanges[num];
			if (range2.start > range.end)
			{
				GTPosRotConstraintManager.componentRanges[num] = new GTPosRotConstraintManager.Range
				{
					start = range2.start - range.end + range.start - 1,
					end = range2.end - range.end + range.start - 1
				};
			}
		}
	}

	public static GTPosRotConstraintManager instance;

	public static bool hasInstance = false;

	[OnEnterPlay_Clear]
	public static readonly List<GorillaPosRotConstraint> constraints = new List<GorillaPosRotConstraint>(1024);

	[OnEnterPlay_Clear]
	public static readonly Dictionary<int, GTPosRotConstraintManager.Range> componentRanges = new Dictionary<int, GTPosRotConstraintManager.Range>(256);

	public struct Range
	{
		public int start;

		public int end;
	}
}
