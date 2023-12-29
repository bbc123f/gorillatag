using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPointManager : MonoBehaviour
{
	protected void Awake()
	{
		if (InteractionPointManager.hasInstance && InteractionPointManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		InteractionPointManager.SetInstance(this);
	}

	protected void OnDestroy()
	{
		if (InteractionPointManager.instance == this)
		{
			InteractionPointManager.hasInstance = false;
			InteractionPointManager.instance = null;
		}
	}

	protected void LateUpdate()
	{
		for (int i = 0; i < InteractionPointManager.interactionPoints.Count; i++)
		{
		}
	}

	public static void CreateManager()
	{
		InteractionPointManager.SetInstance(new GameObject("InteractionPointManager").AddComponent<InteractionPointManager>());
	}

	private static void SetInstance(InteractionPointManager manager)
	{
		InteractionPointManager.instance = manager;
		InteractionPointManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	public static void Register(InteractionPoint interactionPoint)
	{
		if (!InteractionPointManager.hasInstance)
		{
			InteractionPointManager.CreateManager();
		}
		if (!InteractionPointManager.interactionPoints.Contains(interactionPoint))
		{
			InteractionPointManager.interactionPoints.Add(interactionPoint);
		}
	}

	public static void Unregister(InteractionPoint interactionPoint)
	{
		if (!InteractionPointManager.hasInstance)
		{
			InteractionPointManager.CreateManager();
		}
		if (InteractionPointManager.interactionPoints.Contains(interactionPoint))
		{
			InteractionPointManager.interactionPoints.Remove(interactionPoint);
		}
	}

	public static InteractionPointManager instance;

	public static bool hasInstance = false;

	public static readonly List<InteractionPoint> interactionPoints = new List<InteractionPoint>(1024);
}
