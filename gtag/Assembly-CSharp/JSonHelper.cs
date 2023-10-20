using System;
using UnityEngine;

// Token: 0x020000FF RID: 255
public static class JSonHelper
{
	// Token: 0x06000639 RID: 1593 RVA: 0x00027264 File Offset: 0x00025464
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JSonHelper.Wrapper<T>>(json).Items;
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00027271 File Offset: 0x00025471
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x00027284 File Offset: 0x00025484
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x020003F8 RID: 1016
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x04001C90 RID: 7312
		public T[] Items;
	}
}
