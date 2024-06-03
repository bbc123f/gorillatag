using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class DayNightCycle : MonoBehaviour
{
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

	public DayNightCycle()
	{
	}

	public Texture2D _dayMap;

	private Texture2D fromMap;

	public Texture2D _sunriseMap;

	private Texture2D toMap;

	public DayNightCycle.LerpBakedLightingJob job;

	public JobHandle jobHandle;

	public bool isComplete;

	private float startTime;

	public float timeTakenStartingJob;

	public float timeTakenPostJob;

	public float timeTakenDuringJob;

	public LightmapData newData;

	private Color[] fromPixels;

	private Color[] toPixels;

	private Color[] mixedPixels;

	private LightmapData[] newDatas;

	public Texture2D newTexture;

	public int textureWidth;

	public int textureHeight;

	private Color[] workBlockFrom;

	private Color[] workBlockTo;

	private Color[] workBlockMix;

	public int subTextureSize = 1024;

	public Texture2D[] subTextureArray;

	public bool startCoroutine;

	public bool startedCoroutine;

	public bool finishedCoroutine;

	public bool startJob;

	public float switchTimeTaken;

	public bool jobStarted;

	public float lerpAmount;

	public int currentRow;

	public int currentColumn;

	public int currentSubTexture;

	public int currentRowInSubtexture;

	public struct LerpBakedLightingJob : IJob
	{
		public void Execute()
		{
			for (int i = 0; i < this.fromPixels.Length; i++)
			{
				this.mixedPixels[i] = Color.Lerp(this.fromPixels[i], this.toPixels[i], 0.5f);
			}
		}

		public NativeArray<Color> fromPixels;

		public NativeArray<Color> toPixels;

		public NativeArray<Color> mixedPixels;

		public float lerpValue;
	}

	[CompilerGenerated]
	private sealed class <UpdateWork>d__37 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateWork>d__37(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			DayNightCycle dayNightCycle = this;
			int num2;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				this.<>2__current = 0;
				this.<>1__state = 1;
				return true;
			case 1:
				this.<>1__state = -1;
				dayNightCycle.timeTakenStartingJob = Time.realtimeSinceStartup - dayNightCycle.startTime;
				dayNightCycle.startTime = Time.realtimeSinceStartup;
				dayNightCycle.startedCoroutine = true;
				dayNightCycle.currentSubTexture = 0;
				i = 0;
				break;
			case 2:
				this.<>1__state = -1;
				num2 = i;
				i = num2 + 1;
				break;
			case 3:
				this.<>1__state = -1;
				num2 = j;
				j = num2 + 1;
				goto IL_239;
			case 4:
				this.<>1__state = -1;
				Graphics.CopyTexture(dayNightCycle.subTextureArray[i], 0, 0, 0, 0, dayNightCycle.subTextureSize, dayNightCycle.subTextureSize, dayNightCycle.newTexture, 0, 0, i * dayNightCycle.subTextureSize % dayNightCycle.textureHeight, (int)Mathf.Floor((float)(dayNightCycle.subTextureSize * i / dayNightCycle.textureHeight)) * dayNightCycle.subTextureSize);
				this.<>2__current = 0;
				this.<>1__state = 5;
				return true;
			case 5:
				this.<>1__state = -1;
				num2 = i;
				i = num2 + 1;
				goto IL_34B;
			default:
				return false;
			}
			if (i >= dayNightCycle.subTextureArray.Length)
			{
				i = 0;
				goto IL_261;
			}
			dayNightCycle.subTextureArray[i] = new Texture2D(dayNightCycle.subTextureSize, dayNightCycle.subTextureSize, dayNightCycle.fromMap.graphicsFormat, TextureCreationFlags.None);
			this.<>2__current = 0;
			this.<>1__state = 2;
			return true;
			IL_239:
			if (j < dayNightCycle.textureHeight / dayNightCycle.subTextureSize)
			{
				dayNightCycle.currentRow = j;
				dayNightCycle.workBlockFrom = dayNightCycle.fromMap.GetPixels(i * dayNightCycle.subTextureSize, j * dayNightCycle.subTextureSize, dayNightCycle.subTextureSize, dayNightCycle.subTextureSize);
				dayNightCycle.workBlockTo = dayNightCycle.toMap.GetPixels(i * dayNightCycle.subTextureSize, j * dayNightCycle.subTextureSize, dayNightCycle.subTextureSize, dayNightCycle.subTextureSize);
				for (int k = 0; k < dayNightCycle.subTextureSize * dayNightCycle.subTextureSize - 1; k++)
				{
					dayNightCycle.workBlockMix[k] = Color.Lerp(dayNightCycle.workBlockFrom[k], dayNightCycle.workBlockTo[k], dayNightCycle.lerpAmount);
				}
				dayNightCycle.subTextureArray[j * (dayNightCycle.textureWidth / dayNightCycle.subTextureSize) + i].SetPixels(0, 0, dayNightCycle.subTextureSize, dayNightCycle.subTextureSize, dayNightCycle.workBlockMix);
				this.<>2__current = 0;
				this.<>1__state = 3;
				return true;
			}
			num2 = i;
			i = num2 + 1;
			IL_261:
			if (i < dayNightCycle.textureWidth / dayNightCycle.subTextureSize)
			{
				dayNightCycle.currentColumn = i;
				j = 0;
				goto IL_239;
			}
			i = 0;
			IL_34B:
			if (i >= dayNightCycle.subTextureArray.Length)
			{
				dayNightCycle.finishedCoroutine = true;
				return false;
			}
			dayNightCycle.currentSubTexture = i;
			dayNightCycle.subTextureArray[i].Apply();
			this.<>2__current = 0;
			this.<>1__state = 4;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public DayNightCycle <>4__this;

		private int <i>5__2;

		private int <j>5__3;
	}
}
