using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Backtrace.Unity;
using Backtrace.Unity.Model;
using GorillaNetworking;
using PlayFab;
using Unity.Mathematics;
using UnityEngine;

public class BacktraceManager : MonoBehaviour
{
	public virtual void Awake()
	{
		base.GetComponent<BacktraceClient>().BeforeSend = delegate(BacktraceData data)
		{
			if (default(Unity.Mathematics.Random).NextDouble() > this.backtraceSampleRate)
			{
				return null;
			}
			return data;
		};
	}

	private void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData("BacktraceSampleRate", delegate(string data)
		{
			if (data != null)
			{
				double.TryParse(data.Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture, out this.backtraceSampleRate);
				Debug.Log(string.Format("Set backtrace sample rate to: {0}", this.backtraceSampleRate));
			}
		}, delegate(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting Backtrace sample rate: {0}", e));
		});
	}

	public BacktraceManager()
	{
	}

	[CompilerGenerated]
	private BacktraceData <Awake>b__1_0(BacktraceData data)
	{
		if (default(Unity.Mathematics.Random).NextDouble() > this.backtraceSampleRate)
		{
			return null;
		}
		return data;
	}

	[CompilerGenerated]
	private void <Start>b__2_0(string data)
	{
		if (data != null)
		{
			double.TryParse(data.Trim('"'), NumberStyles.Any, CultureInfo.InvariantCulture, out this.backtraceSampleRate);
			Debug.Log(string.Format("Set backtrace sample rate to: {0}", this.backtraceSampleRate));
		}
	}

	public double backtraceSampleRate = 0.01;

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal void <Start>b__2_1(PlayFabError e)
		{
			Debug.LogError(string.Format("Error getting Backtrace sample rate: {0}", e));
		}

		public static readonly BacktraceManager.<>c <>9 = new BacktraceManager.<>c();

		public static Action<PlayFabError> <>9__2_1;
	}
}
