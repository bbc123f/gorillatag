using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class DevInspectorManager : MonoBehaviour
{
	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000190 RID: 400 RVA: 0x0000C1C7 File Offset: 0x0000A3C7
	public static DevInspectorManager instance
	{
		get
		{
			if (DevInspectorManager._instance == null)
			{
				DevInspectorManager._instance = Object.FindObjectOfType<DevInspectorManager>();
			}
			return DevInspectorManager._instance;
		}
	}

	// Token: 0x04000270 RID: 624
	private static DevInspectorManager _instance;
}
