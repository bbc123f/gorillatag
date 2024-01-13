using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(1300)]
public class GTPosRotConstraintManager : MonoBehaviour
{
	public struct Range
	{
		public int start;

		public int end;
	}

	public static GTPosRotConstraintManager instance;

	public static bool hasInstance = false;

	public static readonly List<GorillaPosRotConstraint> constraints = new List<GorillaPosRotConstraint>(1024);

	public static readonly Dictionary<int, Range> componentRanges = new Dictionary<int, Range>(256);

	protected void Awake()
	{
		if (hasInstance && instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			SetInstance(this);
		}
	}

	protected void OnDestroy()
	{
		if (instance == this)
		{
			hasInstance = false;
			instance = null;
		}
	}

	protected void LateUpdate()
	{
		for (int i = 0; i < constraints.Count; i++)
		{
			Transform source = constraints[i].source;
			constraints[i].follower.SetPositionAndRotation(source.position, source.rotation);
		}
	}

	public static void CreateManager()
	{
		SetInstance(new GameObject("GTPosRotConstraintManager").AddComponent<GTPosRotConstraintManager>());
	}

	private static void SetInstance(GTPosRotConstraintManager manager)
	{
		instance = manager;
		hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	public static void Register(GTPosRotConstraints component)
	{
		if (!hasInstance)
		{
			CreateManager();
		}
		int instanceID = component.GetInstanceID();
		if (!componentRanges.ContainsKey(instanceID))
		{
			Range range = default(Range);
			range.start = constraints.Count;
			range.end = constraints.Count + component.constraints.Length - 1;
			Range value = range;
			componentRanges.Add(instanceID, value);
			constraints.AddRange(component.constraints);
		}
	}

	public static void Unregister(GTPosRotConstraints component)
	{
		int instanceID = component.GetInstanceID();
		if (!hasInstance || !componentRanges.TryGetValue(instanceID, out var value))
		{
			return;
		}
		constraints.RemoveRange(value.start, 1 + value.end - value.start);
		componentRanges.Remove(instanceID);
		int[] array = componentRanges.Keys.ToArray();
		foreach (int key in array)
		{
			Range range = componentRanges[key];
			if (range.start > value.end)
			{
				componentRanges[key] = new Range
				{
					start = range.start - value.end + value.start - 1,
					end = range.end - value.end + value.start - 1
				};
			}
		}
	}
}
