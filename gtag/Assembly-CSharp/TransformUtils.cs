using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022A RID: 554
public static class TransformUtils
{
	// Token: 0x06000DC9 RID: 3529 RVA: 0x00050418 File Offset: 0x0004E618
	public static Hash128 ComputePathHash(Transform t)
	{
		if (t == null)
		{
			return default(Hash128);
		}
		Hash128 result = default(Hash128);
		Transform transform = t;
		while (transform != null)
		{
			Hash128 hash = Hash128.Compute(transform.name);
			HashUtilities.AppendHash(ref hash, ref result);
			transform = transform.parent;
		}
		return result;
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x0005046C File Offset: 0x0004E66C
	public static string GetScenePath(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform parent = t.parent;
		while (parent != null)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	// Token: 0x06000DCB RID: 3531 RVA: 0x000504B8 File Offset: 0x0004E6B8
	public static string GetScenePathReverse(Transform t)
	{
		if (t == null)
		{
			return null;
		}
		string text = t.name;
		Transform parent = t.parent;
		Queue<string> queue = new Queue<string>(16);
		while (parent != null)
		{
			queue.Enqueue(parent.name);
			parent = parent.parent;
		}
		while (queue.Count > 0)
		{
			text = text + "/" + queue.Dequeue();
		}
		return text;
	}

	// Token: 0x040010C0 RID: 4288
	private const string kFwdSlash = "/";
}
