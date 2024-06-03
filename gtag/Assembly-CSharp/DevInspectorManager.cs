using System;
using UnityEngine;

public class DevInspectorManager : MonoBehaviour
{
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

	public DevInspectorManager()
	{
	}

	private static DevInspectorManager _instance;
}
