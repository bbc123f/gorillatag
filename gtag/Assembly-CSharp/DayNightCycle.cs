using System;
using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x020001AB RID: 427
public class DayNightCycle : MonoBehaviour
{
	// Token: 0x06000AFB RID: 2811 RVA: 0x00043CCC File Offset: 0x00041ECC
	public void Awake()
	{
		this.fromMap = new Texture2D(this._sunriseMap.width, this._sunriseMap.height);
		this.fromMap = LightmapSettings.lightmaps[0].lightmapColor;
		this.toMap = new Texture2D(this._dayMap.width, this._dayMap.height);
		this.toMap.SetPixels(this._dayMap.GetPixels());
		this.toMap.Apply();
		this.workBlockMix = new Color[this.subTextureSize * this.subTextureSize];
		this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height, this.fromMap.graphicsFormat, TextureCreationFlags.None);
		this.newData = new LightmapData();
		this.textureHeight = this.fromMap.height;
		this.textureWidth = this.fromMap.width;
		this.subTextureArray = new Texture2D[(int)Mathf.Pow((float)(this.textureHeight / this.subTextureSize), 2f)];
		Debug.Log("aaaa " + this.fromMap.format.ToString());
		Debug.Log("aaaa " + this.fromMap.graphicsFormat.ToString());
		this.startJob = false;
		this.startCoroutine = false;
		this.startedCoroutine = false;
		this.finishedCoroutine = false;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00043E50 File Offset: 0x00042050
	public void Update()
	{
		if (this.startJob)
		{
			this.startJob = false;
			this.startTime = Time.realtimeSinceStartup;
			base.StartCoroutine(this.UpdateWork());
			this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
		}
		if (this.jobStarted && this.jobHandle.IsCompleted)
		{
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.jobHandle.Complete();
			this.jobStarted = false;
			this.newTexture.SetPixels(this.job.mixedPixels.ToArray());
			this.newData.lightmapDir = LightmapSettings.lightmaps[0].lightmapDir;
			LightmapSettings.lightmaps = new LightmapData[]
			{
				this.newData
			};
			this.job.fromPixels.Dispose();
			this.job.toPixels.Dispose();
			this.job.mixedPixels.Dispose();
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
		if (this.startCoroutine)
		{
			this.startCoroutine = false;
			this.startTime = Time.realtimeSinceStartup;
			this.newTexture = new Texture2D(this.fromMap.width, this.fromMap.height);
			base.StartCoroutine(this.UpdateWork());
		}
		if (this.startedCoroutine && this.finishedCoroutine)
		{
			this.startedCoroutine = false;
			this.finishedCoroutine = false;
			this.timeTakenDuringJob = Time.realtimeSinceStartup - this.startTime;
			this.startTime = Time.realtimeSinceStartup;
			this.newData = LightmapSettings.lightmaps[0];
			this.newData.lightmapColor = this.fromMap;
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			lightmaps[0].lightmapColor = this.fromMap;
			LightmapSettings.lightmaps = lightmaps;
			this.timeTakenPostJob = Time.realtimeSinceStartup - this.startTime;
		}
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0004403E File Offset: 0x0004223E
	public IEnumerator UpdateWork()
	{
		yield return 0;
		this.timeTakenStartingJob = Time.realtimeSinceStartup - this.startTime;
		this.startTime = Time.realtimeSinceStartup;
		this.startedCoroutine = true;
		this.currentSubTexture = 0;
		int num;
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.subTextureArray[i] = new Texture2D(this.subTextureSize, this.subTextureSize, this.fromMap.graphicsFormat, TextureCreationFlags.None);
			yield return 0;
			num = i;
		}
		for (int i = 0; i < this.textureWidth / this.subTextureSize; i = num + 1)
		{
			this.currentColumn = i;
			for (int j = 0; j < this.textureHeight / this.subTextureSize; j = num + 1)
			{
				this.currentRow = j;
				this.workBlockFrom = this.fromMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				this.workBlockTo = this.toMap.GetPixels(i * this.subTextureSize, j * this.subTextureSize, this.subTextureSize, this.subTextureSize);
				for (int k = 0; k < this.subTextureSize * this.subTextureSize - 1; k++)
				{
					this.workBlockMix[k] = Color.Lerp(this.workBlockFrom[k], this.workBlockTo[k], this.lerpAmount);
				}
				this.subTextureArray[j * (this.textureWidth / this.subTextureSize) + i].SetPixels(0, 0, this.subTextureSize, this.subTextureSize, this.workBlockMix);
				yield return 0;
				num = j;
			}
			num = i;
		}
		for (int i = 0; i < this.subTextureArray.Length; i = num + 1)
		{
			this.currentSubTexture = i;
			this.subTextureArray[i].Apply();
			yield return 0;
			Graphics.CopyTexture(this.subTextureArray[i], 0, 0, 0, 0, this.subTextureSize, this.subTextureSize, this.newTexture, 0, 0, i * this.subTextureSize % this.textureHeight, (int)Mathf.Floor((float)(this.subTextureSize * i / this.textureHeight)) * this.subTextureSize);
			yield return 0;
			num = i;
		}
		this.finishedCoroutine = true;
		yield break;
	}

	// Token: 0x04000E01 RID: 3585
	public Texture2D _dayMap;

	// Token: 0x04000E02 RID: 3586
	private Texture2D fromMap;

	// Token: 0x04000E03 RID: 3587
	public Texture2D _sunriseMap;

	// Token: 0x04000E04 RID: 3588
	private Texture2D toMap;

	// Token: 0x04000E05 RID: 3589
	public DayNightCycle.LerpBakedLightingJob job;

	// Token: 0x04000E06 RID: 3590
	public JobHandle jobHandle;

	// Token: 0x04000E07 RID: 3591
	public bool isComplete;

	// Token: 0x04000E08 RID: 3592
	private float startTime;

	// Token: 0x04000E09 RID: 3593
	public float timeTakenStartingJob;

	// Token: 0x04000E0A RID: 3594
	public float timeTakenPostJob;

	// Token: 0x04000E0B RID: 3595
	public float timeTakenDuringJob;

	// Token: 0x04000E0C RID: 3596
	public LightmapData newData;

	// Token: 0x04000E0D RID: 3597
	private Color[] fromPixels;

	// Token: 0x04000E0E RID: 3598
	private Color[] toPixels;

	// Token: 0x04000E0F RID: 3599
	private Color[] mixedPixels;

	// Token: 0x04000E10 RID: 3600
	private LightmapData[] newDatas;

	// Token: 0x04000E11 RID: 3601
	public Texture2D newTexture;

	// Token: 0x04000E12 RID: 3602
	public int textureWidth;

	// Token: 0x04000E13 RID: 3603
	public int textureHeight;

	// Token: 0x04000E14 RID: 3604
	private Color[] workBlockFrom;

	// Token: 0x04000E15 RID: 3605
	private Color[] workBlockTo;

	// Token: 0x04000E16 RID: 3606
	private Color[] workBlockMix;

	// Token: 0x04000E17 RID: 3607
	public int subTextureSize = 1024;

	// Token: 0x04000E18 RID: 3608
	public Texture2D[] subTextureArray;

	// Token: 0x04000E19 RID: 3609
	public bool startCoroutine;

	// Token: 0x04000E1A RID: 3610
	public bool startedCoroutine;

	// Token: 0x04000E1B RID: 3611
	public bool finishedCoroutine;

	// Token: 0x04000E1C RID: 3612
	public bool startJob;

	// Token: 0x04000E1D RID: 3613
	public float switchTimeTaken;

	// Token: 0x04000E1E RID: 3614
	public bool jobStarted;

	// Token: 0x04000E1F RID: 3615
	public float lerpAmount;

	// Token: 0x04000E20 RID: 3616
	public int currentRow;

	// Token: 0x04000E21 RID: 3617
	public int currentColumn;

	// Token: 0x04000E22 RID: 3618
	public int currentSubTexture;

	// Token: 0x04000E23 RID: 3619
	public int currentRowInSubtexture;

	// Token: 0x0200044F RID: 1103
	public struct LerpBakedLightingJob : IJob
	{
		// Token: 0x06001CE3 RID: 7395 RVA: 0x0009972C File Offset: 0x0009792C
		public void Execute()
		{
			for (int i = 0; i < this.fromPixels.Length; i++)
			{
				this.mixedPixels[i] = Color.Lerp(this.fromPixels[i], this.toPixels[i], 0.5f);
			}
		}

		// Token: 0x04001DD8 RID: 7640
		public NativeArray<Color> fromPixels;

		// Token: 0x04001DD9 RID: 7641
		public NativeArray<Color> toPixels;

		// Token: 0x04001DDA RID: 7642
		public NativeArray<Color> mixedPixels;

		// Token: 0x04001DDB RID: 7643
		public float lerpValue;
	}
}
