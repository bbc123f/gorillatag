using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class GorillaDayNight : MonoBehaviour
{
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

	public void Update()
	{
		if (this.test)
		{
			this.test = false;
			base.StartCoroutine(this.LightMapSet(this.firstData, this.secondData, this.lerpValue));
		}
	}

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

	public GorillaDayNight()
	{
	}

	[CompilerGenerated]
	private bool <LightMapSet>b__25_0()
	{
		return this.finishedStep;
	}

	[CompilerGenerated]
	private bool <LightMapSet>b__25_1()
	{
		return this.finishedStep;
	}

	[OnEnterPlay_SetNull]
	public static volatile GorillaDayNight instance;

	public GorillaLightmapData[] lightmapDatas;

	private LightmapData[] workingLightMapDatas;

	private LightmapData workingLightMapData;

	public float lerpValue;

	public bool done;

	public bool finishedStep;

	private Color[] fromPixels;

	private Color[] toPixels;

	private Color[] mixedPixels;

	public int firstData;

	public int secondData;

	public int i;

	public int j;

	public int k;

	public int l;

	private Thread lightsThread;

	private Thread dirsThread;

	public bool test;

	public bool working;

	[CompilerGenerated]
	private sealed class <LightMapSet>d__25 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <LightMapSet>d__25(int <>1__state)
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
			GorillaDayNight gorillaDayNight = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				gorillaDayNight.working = true;
				gorillaDayNight.firstData = setFirstData;
				gorillaDayNight.secondData = setSecondData;
				gorillaDayNight.lerpValue = setLerp;
				gorillaDayNight.k = 0;
				break;
			case 1:
				this.<>1__state = -1;
				gorillaDayNight.finishedStep = false;
				gorillaDayNight.workingLightMapData.lightmapColor.SetPixels(gorillaDayNight.mixedPixels);
				gorillaDayNight.workingLightMapData.lightmapColor.Apply(false);
				gorillaDayNight.dirsThread = new Thread(new ThreadStart(gorillaDayNight.DoDirsStep));
				gorillaDayNight.dirsThread.Start();
				this.<>2__current = new WaitUntil(() => gorillaDayNight.finishedStep);
				this.<>1__state = 2;
				return true;
			case 2:
				this.<>1__state = -1;
				gorillaDayNight.finishedStep = false;
				gorillaDayNight.workingLightMapData.lightmapDir.SetPixels(gorillaDayNight.mixedPixels);
				gorillaDayNight.workingLightMapData.lightmapDir.Apply(false);
				gorillaDayNight.workingLightMapDatas[gorillaDayNight.k] = gorillaDayNight.workingLightMapData;
				gorillaDayNight.k++;
				break;
			default:
				return false;
			}
			if (gorillaDayNight.k >= gorillaDayNight.lightmapDatas[gorillaDayNight.firstData].lights.Length)
			{
				LightmapSettings.lightmaps = gorillaDayNight.workingLightMapDatas;
				gorillaDayNight.working = false;
				gorillaDayNight.done = true;
				return false;
			}
			gorillaDayNight.lightsThread = new Thread(new ThreadStart(gorillaDayNight.DoLightsStep));
			gorillaDayNight.lightsThread.Start();
			this.<>2__current = new WaitUntil(() => gorillaDayNight.finishedStep);
			this.<>1__state = 1;
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

		public GorillaDayNight <>4__this;

		public int setFirstData;

		public int setSecondData;

		public float setLerp;
	}
}
