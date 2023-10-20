using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000048 RID: 72
public class DevConsole : MonoBehaviour, IDebugObject
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000179 RID: 377 RVA: 0x0000BF47 File Offset: 0x0000A147
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

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600017A RID: 378 RVA: 0x0000BF65 File Offset: 0x0000A165
	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000BF74 File Offset: 0x0000A174
	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000BFD4 File Offset: 0x0000A1D4
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400021C RID: 540
	private static DevConsole _instance;

	// Token: 0x0400021D RID: 541
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x0400021E RID: 542
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400021F RID: 543
	[SerializeField]
	private float maxHeight;

	// Token: 0x04000220 RID: 544
	public static readonly string[] tracebackScrubbing = new string[]
	{
		"ExitGames.Client.Photon",
		"Photon.Realtime.LoadBalancingClient",
		"Photon.Pun.PhotonHandler"
	};

	// Token: 0x04000221 RID: 545
	private const int kLogEntriesCapacityIncrementAmount = 1024;

	// Token: 0x04000222 RID: 546
	[SerializeReference]
	[SerializeField]
	private readonly List<DevConsole.LogEntry> _logEntries = new List<DevConsole.LogEntry>(1024);

	// Token: 0x04000223 RID: 547
	public int targetLogIndex = -1;

	// Token: 0x04000224 RID: 548
	public int currentLogIndex;

	// Token: 0x04000225 RID: 549
	public bool isMuted;

	// Token: 0x04000226 RID: 550
	public float currentZoomLevel = 1f;

	// Token: 0x04000227 RID: 551
	public List<GameObject> disableWhileActive;

	// Token: 0x04000228 RID: 552
	public List<GameObject> enableWhileActive;

	// Token: 0x04000229 RID: 553
	public int expandAmount = 20;

	// Token: 0x0400022A RID: 554
	public int expandedMessageIndex = -1;

	// Token: 0x0400022B RID: 555
	public bool canExpand = true;

	// Token: 0x0400022C RID: 556
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x0400022D RID: 557
	public float lineStartHeight;

	// Token: 0x0400022E RID: 558
	public float textStartHeight;

	// Token: 0x0400022F RID: 559
	public float lineStartTextWidth;

	// Token: 0x04000230 RID: 560
	public double textScale = 0.5;

	// Token: 0x04000231 RID: 561
	public List<DevConsoleInstance> instances;

	// Token: 0x02000398 RID: 920
	[Serializable]
	public class LogEntry
	{
		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06001AD2 RID: 6866 RVA: 0x00094C5D File Offset: 0x00092E5D
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

		// Token: 0x06001AD3 RID: 6867 RVA: 0x00094C8C File Offset: 0x00092E8C
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

		// Token: 0x04001B39 RID: 6969
		private static int TotalIndex;

		// Token: 0x04001B3A RID: 6970
		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		// Token: 0x04001B3B RID: 6971
		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		// Token: 0x04001B3C RID: 6972
		public readonly string Trace;

		// Token: 0x04001B3D RID: 6973
		public bool forwarded;

		// Token: 0x04001B3E RID: 6974
		public int repeatCount = 1;

		// Token: 0x04001B3F RID: 6975
		public bool filtered;

		// Token: 0x04001B40 RID: 6976
		public int index;
	}

	// Token: 0x02000399 RID: 921
	[Serializable]
	public class DisplayedLogLine
	{
		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06001AD4 RID: 6868 RVA: 0x00094D38 File Offset: 0x00092F38
		// (set) Token: 0x06001AD5 RID: 6869 RVA: 0x00094D40 File Offset: 0x00092F40
		public Type data { get; set; }

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00094D4C File Offset: 0x00092F4C
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

		// Token: 0x04001B41 RID: 6977
		public GorillaDevButton[] buttons;

		// Token: 0x04001B42 RID: 6978
		public Text lineText;

		// Token: 0x04001B43 RID: 6979
		public RectTransform transform;

		// Token: 0x04001B44 RID: 6980
		public int targetMessage;

		// Token: 0x04001B45 RID: 6981
		public GorillaDevButton maximizeButton;

		// Token: 0x04001B46 RID: 6982
		public GorillaDevButton forwardButton;

		// Token: 0x04001B47 RID: 6983
		public SpriteRenderer backdrop;

		// Token: 0x04001B48 RID: 6984
		private bool expanded;

		// Token: 0x04001B49 RID: 6985
		public DevInspector inspector;
	}

	// Token: 0x0200039A RID: 922
	[Serializable]
	public class MessagePayload
	{
		// Token: 0x06001AD7 RID: 6871 RVA: 0x00094DC8 File Offset: 0x00092FC8
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
				foreach (string str in array)
				{
					text2 = text2 + "    " + str + "\n";
				}
				string text3 = string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
				if (text.Length + text3.Length > 3000)
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

		// Token: 0x04001B4B RID: 6987
		public DevConsole.MessagePayload.Block[] blocks;

		// Token: 0x0200054D RID: 1357
		[Serializable]
		public class Block
		{
			// Token: 0x06001FC6 RID: 8134 RVA: 0x000A354D File Offset: 0x000A174D
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			// Token: 0x0400222C RID: 8748
			public string type;

			// Token: 0x0400222D RID: 8749
			public DevConsole.MessagePayload.TextBlock text;
		}

		// Token: 0x0200054E RID: 1358
		[Serializable]
		public class TextBlock
		{
			// Token: 0x0400222E RID: 8750
			public string type;

			// Token: 0x0400222F RID: 8751
			public string text;
		}
	}
}
