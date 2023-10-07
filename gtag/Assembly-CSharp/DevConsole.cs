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
	// (get) Token: 0x06000179 RID: 377 RVA: 0x0000BEFF File Offset: 0x0000A0FF
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
	// (get) Token: 0x0600017A RID: 378 RVA: 0x0000BF1D File Offset: 0x0000A11D
	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000BF2C File Offset: 0x0000A12C
	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0000BF8C File Offset: 0x0000A18C
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

	// Token: 0x02000396 RID: 918
	[Serializable]
	public class LogEntry
	{
		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06001AC9 RID: 6857 RVA: 0x00094775 File Offset: 0x00092975
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

		// Token: 0x06001ACA RID: 6858 RVA: 0x000947A4 File Offset: 0x000929A4
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

		// Token: 0x04001B2C RID: 6956
		private static int TotalIndex;

		// Token: 0x04001B2D RID: 6957
		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		// Token: 0x04001B2E RID: 6958
		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		// Token: 0x04001B2F RID: 6959
		public readonly string Trace;

		// Token: 0x04001B30 RID: 6960
		public bool forwarded;

		// Token: 0x04001B31 RID: 6961
		public int repeatCount = 1;

		// Token: 0x04001B32 RID: 6962
		public bool filtered;

		// Token: 0x04001B33 RID: 6963
		public int index;
	}

	// Token: 0x02000397 RID: 919
	[Serializable]
	public class DisplayedLogLine
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06001ACB RID: 6859 RVA: 0x00094850 File Offset: 0x00092A50
		// (set) Token: 0x06001ACC RID: 6860 RVA: 0x00094858 File Offset: 0x00092A58
		public Type data { get; set; }

		// Token: 0x06001ACD RID: 6861 RVA: 0x00094864 File Offset: 0x00092A64
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

		// Token: 0x04001B34 RID: 6964
		public GorillaDevButton[] buttons;

		// Token: 0x04001B35 RID: 6965
		public Text lineText;

		// Token: 0x04001B36 RID: 6966
		public RectTransform transform;

		// Token: 0x04001B37 RID: 6967
		public int targetMessage;

		// Token: 0x04001B38 RID: 6968
		public GorillaDevButton maximizeButton;

		// Token: 0x04001B39 RID: 6969
		public GorillaDevButton forwardButton;

		// Token: 0x04001B3A RID: 6970
		public SpriteRenderer backdrop;

		// Token: 0x04001B3B RID: 6971
		private bool expanded;

		// Token: 0x04001B3C RID: 6972
		public DevInspector inspector;
	}

	// Token: 0x02000398 RID: 920
	[Serializable]
	public class MessagePayload
	{
		// Token: 0x06001ACE RID: 6862 RVA: 0x000948E0 File Offset: 0x00092AE0
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

		// Token: 0x04001B3E RID: 6974
		public DevConsole.MessagePayload.Block[] blocks;

		// Token: 0x0200054B RID: 1355
		[Serializable]
		public class Block
		{
			// Token: 0x06001FBD RID: 8125 RVA: 0x000A3241 File Offset: 0x000A1441
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			// Token: 0x0400221F RID: 8735
			public string type;

			// Token: 0x04002220 RID: 8736
			public DevConsole.MessagePayload.TextBlock text;
		}

		// Token: 0x0200054C RID: 1356
		[Serializable]
		public class TextBlock
		{
			// Token: 0x04002221 RID: 8737
			public string type;

			// Token: 0x04002222 RID: 8738
			public string text;
		}
	}
}
