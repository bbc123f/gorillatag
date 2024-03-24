using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Fusion
{
	[Serializable]
	public class FusionUnityLogger : ILogger
	{
		public Func<object, int> GetColor
		{
			[CompilerGenerated]
			get
			{
				return this.<GetColor>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<GetColor>k__BackingField = value;
			}
		}

		public FusionUnityLogger()
		{
			bool flag = false;
			this.MinRandomColor = (flag ? new Color32(158, 158, 158, byte.MaxValue) : new Color32(30, 30, 30, byte.MaxValue));
			this.MaxRandomColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(90, 90, 90, byte.MaxValue));
			this.ServerColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, 158, byte.MaxValue) : new Color32(30, 90, 200, byte.MaxValue));
			this.UseColorTags = true;
			this.UseGlobalPrefix = true;
			this.GlobalPrefixColor = FusionUnityLogger.Color32ToRGBString(flag ? new Color32(115, 172, 229, byte.MaxValue) : new Color32(20, 64, 120, byte.MaxValue));
			this.GetColor = delegate(object obj)
			{
				NetworkRunner networkRunner = obj as NetworkRunner;
				if (networkRunner != null)
				{
					int hashCodeForLogger = networkRunner.GetHashCodeForLogger();
					return this.GetRandomColor(hashCodeForLogger);
				}
				return 0;
			};
		}

		public void Log<T>(LogType logType, string prefix, ref T context, string message) where T : ILogBuilder
		{
			string text;
			try
			{
				if (logType == LogType.Debug)
				{
					this._builder.Append("[DEBUG] ");
				}
				else if (logType == LogType.Trace)
				{
					this._builder.Append("[TRACE] ");
				}
				if (this.UseGlobalPrefix)
				{
					if (this.UseColorTags)
					{
						this._builder.Append("<color=");
						this._builder.Append(this.GlobalPrefixColor);
						this._builder.Append(">");
					}
					this._builder.Append("[Fusion");
					if (!string.IsNullOrEmpty(prefix))
					{
						this._builder.Append("/");
						this._builder.Append(prefix);
					}
					this._builder.Append("]");
					if (this.UseColorTags)
					{
						this._builder.Append("</color>");
					}
					this._builder.Append(" ");
				}
				else if (!string.IsNullOrEmpty(prefix))
				{
					this._builder.Append(prefix);
					this._builder.Append(": ");
				}
				LogOptions logOptions = new LogOptions(this.UseColorTags, this.GetColor);
				context.BuildLogMessage(this._builder, message, logOptions);
				text = this._builder.ToString();
			}
			finally
			{
				this._builder.Clear();
			}
			Object @object = context as Object;
			if (logType == LogType.Error)
			{
				Debug.LogError(text, @object);
				return;
			}
			if (logType != LogType.Warn)
			{
				Debug.Log(text, @object);
				return;
			}
			Debug.LogWarning(text, @object);
		}

		public void LogException<T>(string prefix, ref T context, Exception ex) where T : ILogBuilder
		{
			this.Log<T>(LogType.Error, string.Empty, ref context, string.Format("{0}\n<i>See next error log entry for details.</i>", ex.GetType()));
			Object @object = context as Object;
			if (@object != null)
			{
				Debug.LogException(ex, @object);
				return;
			}
			Debug.LogException(ex);
		}

		private int GetRandomColor(int seed)
		{
			return FusionUnityLogger.GetRandomColor(seed, this.MinRandomColor, this.MaxRandomColor, this.ServerColor);
		}

		private static int GetRandomColor(int seed, Color32 min, Color32 max, Color32 svr)
		{
			NetworkRNG networkRNG = new NetworkRNG(seed);
			int num;
			int num2;
			int num3;
			if (seed == -1)
			{
				num = (int)svr.r;
				num2 = (int)svr.g;
				num3 = (int)svr.b;
			}
			else
			{
				num = networkRNG.RangeInclusive((int)min.r, (int)max.r);
				num2 = networkRNG.RangeInclusive((int)min.g, (int)max.g);
				num3 = networkRNG.RangeInclusive((int)min.b, (int)max.b);
			}
			num = Mathf.Clamp(num, 0, 255);
			num2 = Mathf.Clamp(num2, 0, 255);
			num3 = Mathf.Clamp(num3, 0, 255);
			return (num << 16) | (num2 << 8) | num3;
		}

		private static int Color32ToRGB24(Color32 c)
		{
			return ((int)c.r << 16) | ((int)c.g << 8) | (int)c.b;
		}

		private static string Color32ToRGBString(Color32 c)
		{
			return string.Format("#{0:X6}", FusionUnityLogger.Color32ToRGB24(c));
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Initialize()
		{
			if (Fusion.Log.Initialized)
			{
				return;
			}
			FusionUnityLogger fusionUnityLogger = new FusionUnityLogger();
			if (fusionUnityLogger != null)
			{
				Fusion.Log.Init(fusionUnityLogger, LogType.Debug);
			}
		}

		[CompilerGenerated]
		private int <.ctor>b__12_0(object obj)
		{
			NetworkRunner networkRunner = obj as NetworkRunner;
			if (networkRunner != null)
			{
				int hashCodeForLogger = networkRunner.GetHashCodeForLogger();
				return this.GetRandomColor(hashCodeForLogger);
			}
			return 0;
		}

		private StringBuilder _builder = new StringBuilder();

		public bool UseGlobalPrefix;

		public bool UseColorTags;

		public string GlobalPrefixColor;

		public Color32 MinRandomColor;

		public Color32 MaxRandomColor;

		public Color ServerColor;

		[CompilerGenerated]
		private Func<object, int> <GetColor>k__BackingField;
	}
}
