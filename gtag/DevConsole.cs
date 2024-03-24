using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour, IDebugObject
{
	public static DevConsole instance
	{
		get
		{
			if (DevConsole._instance == null)
			{
				DevConsole._instance = Object.FindObjectOfType<DevConsole>();
			}
			return DevConsole._instance;
		}
	}

	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	public DevConsole()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static DevConsole()
	{
	}

	private static DevConsole _instance;

	[SerializeField]
	private AudioClip errorSound;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private float maxHeight;

	public static readonly string[] tracebackScrubbing = new string[] { "ExitGames.Client.Photon", "Photon.Realtime.LoadBalancingClient", "Photon.Pun.PhotonHandler" };

	private const int kLogEntriesCapacityIncrementAmount = 1024;

	[SerializeReference]
	[SerializeField]
	private readonly List<DevConsole.LogEntry> _logEntries = new List<DevConsole.LogEntry>(1024);

	public int targetLogIndex = -1;

	public int currentLogIndex;

	public bool isMuted;

	public float currentZoomLevel = 1f;

	public List<GameObject> disableWhileActive;

	public List<GameObject> enableWhileActive;

	public int expandAmount = 20;

	public int expandedMessageIndex = -1;

	public bool canExpand = true;

	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	public float lineStartHeight;

	public float textStartHeight;

	public float lineStartTextWidth;

	public double textScale = 0.5;

	public List<DevConsoleInstance> instances;

	[Serializable]
	public class LogEntry
	{
		public string Message
		{
			get
			{
				if (this.repeatCount > 1)
				{
					return string.Format("({0}) {1}", this.repeatCount, this._Message);
				}
				return this._Message;
			}
		}

		public LogEntry(string message, LogType type, string trace)
		{
			this._Message = message;
			this.Type = type;
			this.Trace = trace;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = trace.Split("\n".ToCharArray(), StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string line = array[i];
				if (!DevConsole.tracebackScrubbing.Any((string scrubString) => line.Contains(scrubString)))
				{
					stringBuilder.AppendLine(line);
				}
			}
			this.Trace = stringBuilder.ToString();
			DevConsole.LogEntry.TotalIndex++;
			this.index = DevConsole.LogEntry.TotalIndex;
		}

		private static int TotalIndex;

		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		public readonly string Trace;

		public bool forwarded;

		public int repeatCount = 1;

		public bool filtered;

		public int index;

		[CompilerGenerated]
		private sealed class <>c__DisplayClass10_0
		{
			public <>c__DisplayClass10_0()
			{
			}

			internal bool <.ctor>b__0(string scrubString)
			{
				return this.line.Contains(scrubString);
			}

			public string line;
		}
	}

	[Serializable]
	public class DisplayedLogLine
	{
		public Type data
		{
			[CompilerGenerated]
			get
			{
				return this.<data>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<data>k__BackingField = value;
			}
		}

		public DisplayedLogLine(GameObject obj)
		{
			this.lineText = obj.GetComponentInChildren<Text>();
			this.buttons = obj.GetComponentsInChildren<GorillaDevButton>();
			this.transform = obj.GetComponent<RectTransform>();
			this.backdrop = obj.GetComponentInChildren<SpriteRenderer>();
			foreach (GorillaDevButton gorillaDevButton in this.buttons)
			{
				if (gorillaDevButton.Type == DevButtonType.LineExpand)
				{
					this.maximizeButton = gorillaDevButton;
				}
				if (gorillaDevButton.Type == DevButtonType.LineForward)
				{
					this.forwardButton = gorillaDevButton;
				}
			}
		}

		public GorillaDevButton[] buttons;

		public Text lineText;

		public RectTransform transform;

		public int targetMessage;

		public GorillaDevButton maximizeButton;

		public GorillaDevButton forwardButton;

		public SpriteRenderer backdrop;

		private bool expanded;

		public DevInspector inspector;

		[CompilerGenerated]
		private Type <data>k__BackingField;
	}

	[Serializable]
	public class MessagePayload
	{
		public static List<DevConsole.MessagePayload> GeneratePayloads(string username, List<DevConsole.LogEntry> entries)
		{
			List<DevConsole.MessagePayload> list = new List<DevConsole.MessagePayload>();
			List<DevConsole.MessagePayload.Block> list2 = new List<DevConsole.MessagePayload.Block>();
			entries.Sort((DevConsole.LogEntry e1, DevConsole.LogEntry e2) => e1.index.CompareTo(e2.index));
			string text = "";
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block("User `" + username + "` Forwarded some errors"));
			foreach (DevConsole.LogEntry logEntry in entries)
			{
				string[] array = logEntry.Trace.Split("\n".ToCharArray());
				string text2 = "";
				foreach (string text3 in array)
				{
					text2 = text2 + "    " + text3 + "\n";
				}
				string text4 = string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
				if (text.Length + text4.Length > 3000)
				{
					text += "```";
					list2.Add(new DevConsole.MessagePayload.Block(text));
					list.Add(new DevConsole.MessagePayload
					{
						blocks = list2.ToArray()
					});
					list2 = new List<DevConsole.MessagePayload.Block>();
					text = "```";
				}
				text += string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
			}
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block(text));
			list.Add(new DevConsole.MessagePayload
			{
				blocks = list2.ToArray()
			});
			return list;
		}

		public MessagePayload()
		{
		}

		public DevConsole.MessagePayload.Block[] blocks;

		[Serializable]
		public class Block
		{
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			public string type;

			public DevConsole.MessagePayload.TextBlock text;
		}

		[Serializable]
		public class TextBlock
		{
			public TextBlock()
			{
			}

			public string type;

			public string text;
		}

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

			internal int <GeneratePayloads>b__3_0(DevConsole.LogEntry e1, DevConsole.LogEntry e2)
			{
				return e1.index.CompareTo(e2.index);
			}

			public static readonly DevConsole.MessagePayload.<>c <>9 = new DevConsole.MessagePayload.<>c();

			public static Comparison<DevConsole.LogEntry> <>9__3_0;
		}
	}
}
