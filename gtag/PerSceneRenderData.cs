using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PerSceneRenderData : MonoBehaviour
{
	private void RefreshRenderer()
	{
		int sceneIndex = this.sceneIndex;
		new List<Renderer>();
		foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
		{
			if (renderer.gameObject.scene.buildIndex == sceneIndex)
			{
				this.representativeRenderer = renderer;
				return;
			}
		}
	}

	public string sceneName
	{
		get
		{
			return base.gameObject.scene.name;
		}
	}

	public int sceneIndex
	{
		get
		{
			return base.gameObject.scene.buildIndex;
		}
	}

	private void OnEnable()
	{
		BetterDayNightManager.Register(this);
	}

	private void OnDisable()
	{
		BetterDayNightManager.Unregister(this);
	}

	public bool CheckShouldRepopulate()
	{
		return this.representativeRenderer.lightmapIndex != this.lastLightmapIndex;
	}

	private Texture2D GetLightmap(string timeOfDay)
	{
		if (this.singleLightmap != null)
		{
			return this.singleLightmap;
		}
		Texture2D texture2D;
		if (!this.lightmapsCache.TryGetValue(timeOfDay, out texture2D))
		{
			ResourceRequest request;
			if (this.resourceRequests.TryGetValue(timeOfDay, out request))
			{
				return null;
			}
			request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, timeOfDay));
			this.resourceRequests.Add(timeOfDay, request);
			request.completed += delegate(AsyncOperation ao)
			{
				if (this == null)
				{
					return;
				}
				this.lightmapsCache.Add(timeOfDay, (Texture2D)request.asset);
				this.resourceRequests.Remove(timeOfDay);
				if (BetterDayNightManager.instance != null)
				{
					BetterDayNightManager.instance.RequestRepopulateLightmaps();
				}
			};
		}
		return texture2D;
	}

	public void PopulateLightmaps(string fromTimeOfDay, string toTimeOfDay, LightmapData[] lightmaps)
	{
		LightmapData lightmapData = new LightmapData();
		lightmapData.lightmapColor = this.GetLightmap(fromTimeOfDay);
		lightmapData.lightmapDir = this.GetLightmap(toTimeOfDay);
		if (lightmapData.lightmapColor != null && lightmapData.lightmapDir != null && this.representativeRenderer.lightmapIndex < lightmaps.Length)
		{
			lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
		}
		this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
	}

	public void ReleaseLightmap(string oldTimeOfDay)
	{
		Texture2D texture2D;
		if (this.lightmapsCache.Remove(oldTimeOfDay, out texture2D))
		{
			Resources.UnloadAsset(texture2D);
		}
	}

	public PerSceneRenderData()
	{
	}

	public Renderer representativeRenderer;

	public string lightmapsResourcePath;

	public Texture2D singleLightmap;

	private int lastLightmapIndex = -1;

	private Dictionary<string, ResourceRequest> resourceRequests = new Dictionary<string, ResourceRequest>();

	private Dictionary<string, Texture2D> lightmapsCache = new Dictionary<string, Texture2D>();

	[CompilerGenerated]
	private sealed class <>c__DisplayClass14_0
	{
		public <>c__DisplayClass14_0()
		{
		}

		internal void <GetLightmap>b__0(AsyncOperation ao)
		{
			if (this.<>4__this == null)
			{
				return;
			}
			this.<>4__this.lightmapsCache.Add(this.timeOfDay, (Texture2D)this.request.asset);
			this.<>4__this.resourceRequests.Remove(this.timeOfDay);
			if (BetterDayNightManager.instance != null)
			{
				BetterDayNightManager.instance.RequestRepopulateLightmaps();
			}
		}

		public PerSceneRenderData <>4__this;

		public string timeOfDay;

		public ResourceRequest request;
	}
}
