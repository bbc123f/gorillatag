using System;
using System.Collections;
using System.Threading;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class GorillaDayNight : MonoBehaviour
{
	// Token: 0x0600092F RID: 2351 RVA: 0x000379FC File Offset: 0x00035BFC
	public void Awake()
	{
		if (GorillaDayNight.instance == null)
		{
			GorillaDayNight.instance = this;
		}
		else if (GorillaDayNight.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.test = false;
		this.working = false;
		this.lerpValue = 0.5f;
		this.workingLightMapDatas = new LightmapData[3];
		this.workingLightMapData = new LightmapData();
		this.workingLightMapData.lightmapColor = this.lightmapDatas[0].lightTextures[0];
		this.workingLightMapData.lightmapDir = this.lightmapDatas[0].dirTextures[0];
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00037AA0 File Offset: 0x00035CA0
	public void Update()
	{
		if (this.test)
		{
			this.test = false;
			base.StartCoroutine(this.LightMapSet(this.firstData, this.secondData, this.lerpValue));
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00037AD0 File Offset: 0x00035CD0
	public void DoWork()
	{
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
			this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
			this.mixedPixels = this.fromPixels;
			this.j = 0;
			while (this.j < this.mixedPixels.Length)
			{
				this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
				this.j++;
			}
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		this.done = true;
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00037CDC File Offset: 0x00035EDC
	public void DoLightsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].lights[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].lights[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00037DA0 File Offset: 0x00035FA0
	public void DoDirsStep()
	{
		this.fromPixels = this.lightmapDatas[this.firstData].dirs[this.k];
		this.toPixels = this.lightmapDatas[this.secondData].dirs[this.k];
		this.mixedPixels = this.fromPixels;
		this.j = 0;
		while (this.j < this.mixedPixels.Length)
		{
			this.mixedPixels[this.j] = Color.Lerp(this.fromPixels[this.j], this.toPixels[this.j], this.lerpValue);
			this.j++;
		}
		this.finishedStep = true;
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00037E63 File Offset: 0x00036063
	private IEnumerator LightMapSet(int setFirstData, int setSecondData, float setLerp)
	{
		this.working = true;
		this.firstData = setFirstData;
		this.secondData = setSecondData;
		this.lerpValue = setLerp;
		this.k = 0;
		while (this.k < this.lightmapDatas[this.firstData].lights.Length)
		{
			this.lightsThread = new Thread(new ThreadStart(this.DoLightsStep));
			this.lightsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapColor.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapColor.Apply(false);
			this.dirsThread = new Thread(new ThreadStart(this.DoDirsStep));
			this.dirsThread.Start();
			yield return new WaitUntil(() => this.finishedStep);
			this.finishedStep = false;
			this.workingLightMapData.lightmapDir.SetPixels(this.mixedPixels);
			this.workingLightMapData.lightmapDir.Apply(false);
			this.workingLightMapDatas[this.k] = this.workingLightMapData;
			this.k++;
		}
		LightmapSettings.lightmaps = this.workingLightMapDatas;
		this.working = false;
		this.done = true;
		yield break;
	}

	// Token: 0x04000B2F RID: 2863
	public static volatile GorillaDayNight instance;

	// Token: 0x04000B30 RID: 2864
	public GorillaLightmapData[] lightmapDatas;

	// Token: 0x04000B31 RID: 2865
	private LightmapData[] workingLightMapDatas;

	// Token: 0x04000B32 RID: 2866
	private LightmapData workingLightMapData;

	// Token: 0x04000B33 RID: 2867
	public float lerpValue;

	// Token: 0x04000B34 RID: 2868
	public bool done;

	// Token: 0x04000B35 RID: 2869
	public bool finishedStep;

	// Token: 0x04000B36 RID: 2870
	private Color[] fromPixels;

	// Token: 0x04000B37 RID: 2871
	private Color[] toPixels;

	// Token: 0x04000B38 RID: 2872
	private Color[] mixedPixels;

	// Token: 0x04000B39 RID: 2873
	public int firstData;

	// Token: 0x04000B3A RID: 2874
	public int secondData;

	// Token: 0x04000B3B RID: 2875
	public int i;

	// Token: 0x04000B3C RID: 2876
	public int j;

	// Token: 0x04000B3D RID: 2877
	public int k;

	// Token: 0x04000B3E RID: 2878
	public int l;

	// Token: 0x04000B3F RID: 2879
	private Thread lightsThread;

	// Token: 0x04000B40 RID: 2880
	private Thread dirsThread;

	// Token: 0x04000B41 RID: 2881
	public bool test;

	// Token: 0x04000B42 RID: 2882
	public bool working;
}
