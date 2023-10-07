using System;
using UnityEngine;

// Token: 0x020000FF RID: 255
public static class JSonHelper
{
	// Token: 0x06000638 RID: 1592 RVA: 0x00027424 File Offset: 0x00025624
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JSonHelper.Wrapper<T>>(json).Items;
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x00027431 File Offset: 0x00025631
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x00027444 File Offset: 0x00025644
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x020003F6 RID: 1014
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x04001C83 RID: 7299
		public T[] Items;
	}
}
