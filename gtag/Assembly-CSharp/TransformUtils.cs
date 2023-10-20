using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022B RID: 555
public static class TransformUtils
{
	// Token: 0x06000DCF RID: 3535 RVA: 0x00050678 File Offset: 0x0004E878
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

	// Token: 0x06000DD0 RID: 3536 RVA: 0x000506CC File Offset: 0x0004E8CC
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

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00050718 File Offset: 0x0004E918
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

	// Token: 0x040010C5 RID: 4293
	private const string kFwdSlash = "/";
}
