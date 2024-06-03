using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class DebugHudStats : MonoBehaviour
{
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

	private void Awake()
	{
		if (DebugHudStats._instance != null && DebugHudStats._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DebugHudStats._instance = this;
		}
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		if (DebugHudStats._instance == this)
		{
			DebugHudStats._instance = null;
		}
	}

	private void Update()
	{
		bool flag = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		if (flag != this.buttonDown)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				if (!this.text.gameObject.activeInHierarchy)
				{
					this.text.gameObject.SetActive(true);
					this.showLog = false;
				}
				else if (!this.showLog)
				{
					this.showLog = true;
				}
				else
				{
					this.text.gameObject.SetActive(false);
				}
			}
		}
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		this.builder.Clear();
		this.builder.Append("v: ");
		this.builder.Append(GorillaComputer.instance.version);
		this.builder.Append(":");
		this.builder.Append(GorillaComputer.instance.buildCode);
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		if (num < 89)
		{
			this.lowFps++;
		}
		else
		{
			this.lowFps = 0;
		}
		this.fpsWarning.gameObject.SetActive(this.lowFps > 5);
		num = Mathf.Min(num, 90);
		this.builder.Append((num < 89) ? " - <color=\"red\">" : " - <color=\"white\">");
		this.builder.Append(num);
		this.builder.AppendLine(" fps</color>");
		if (GorillaComputer.instance != null)
		{
			this.builder.AppendLine(GorillaComputer.instance.GetServerTime().ToString());
		}
		else
		{
			this.builder.AppendLine("Server Time Unavailable");
		}
		float magnitude = Player.Instance.currentVelocity.magnitude;
		this.builder.Append(Mathf.RoundToInt(magnitude));
		this.builder.AppendLine(" m/s");
		if (this.showLog)
		{
			this.builder.AppendLine();
			for (int i = 0; i < this.logMessages.Count; i++)
			{
				this.builder.AppendLine(this.logMessages[i]);
			}
		}
		this.text.text = this.builder.ToString();
		this.updateTimer = 0f;
	}

	private void OnEnable()
	{
		Application.logMessageReceived += this.LogMessageReceived;
	}

	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		this.logMessages.Add(this.getColorStringFromLogType(type) + condition + "</color>");
		if (this.logMessages.Count > 6)
		{
			this.logMessages.RemoveAt(0);
		}
	}

	private string getColorStringFromLogType(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			return "<color=\"red\">";
		case LogType.Warning:
			return "<color=\"yellow\">";
		}
		return "<color=\"white\">";
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= this.LogMessageReceived;
	}

	public DebugHudStats()
	{
	}

	private static DebugHudStats _instance;

	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private TMP_Text fpsWarning;

	[SerializeField]
	private float delayUpdateRate = 0.25f;

	private float updateTimer;

	public float sessionAnytrackingLost;

	public float last30SecondsTrackingLost;

	private float firstAwake;

	private bool wasTrackingLost;

	private bool leftHandTracked;

	private bool rightHandTracked;

	private StringBuilder builder;

	private List<string> logMessages = new List<string>();

	private bool buttonDown;

	private bool showLog;

	private int lowFps;
}
