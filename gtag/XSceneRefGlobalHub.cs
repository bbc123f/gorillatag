using System;
using System.Collections.Generic;

public static class XSceneRefGlobalHub
{
	public static void Register(int ID, XSceneRefTarget obj)
	{
		if (ID > 0)
		{
			int sceneIndex = (int)obj.GetSceneIndex();
			if (sceneIndex >= 0)
			{
				XSceneRefGlobalHub.registry[sceneIndex][ID] = obj;
			}
		}
	}

	public static void Unregister(int ID, XSceneRefTarget obj)
	{
		int sceneIndex = (int)obj.GetSceneIndex();
		if (ID > 0 && sceneIndex >= 0)
		{
			XSceneRefGlobalHub.registry[sceneIndex].Remove(ID);
		}
	}

	public static bool TryResolve(SceneIndex sceneIndex, int ID, out XSceneRefTarget result)
	{
		return XSceneRefGlobalHub.registry[(int)sceneIndex].TryGetValue(ID, out result);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static XSceneRefGlobalHub()
	{
	}

	private static List<Dictionary<int, XSceneRefTarget>> registry = new List<Dictionary<int, XSceneRefTarget>>
	{
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } },
		new Dictionary<int, XSceneRefTarget> { { 0, null } }
	};
}
