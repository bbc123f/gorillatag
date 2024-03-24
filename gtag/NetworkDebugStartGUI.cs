using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkDebugStart))]
[AddComponentMenu("Fusion/Network Debug Start GUI")]
[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class NetworkDebugStartGUI : Fusion.Behaviour
{
	protected virtual void OnValidate()
	{
		this.ValidateClientCount();
	}

	protected void ValidateClientCount()
	{
		if (this._clientCount == null)
		{
			this._clientCount = "1";
			return;
		}
		this._clientCount = Regex.Replace(this._clientCount, "[^0-9]", "");
	}

	protected int GetClientCount()
	{
		int num;
		try
		{
			num = Convert.ToInt32(this._clientCount);
		}
		catch
		{
			num = 0;
		}
		return num;
	}

	protected virtual void Awake()
	{
		this._nicifiedStageNames = NetworkDebugStartGUI.ConvertEnumToNicifiedNameLookup<NetworkDebugStart.Stage>("Fusion Status: ", null);
		this._networkDebugStart = this.EnsureNetworkDebugStartExists();
		this._clientCount = this._networkDebugStart.AutoClients.ToString();
		this.ValidateClientCount();
	}

	protected virtual void Start()
	{
		this._isMultiplePeerMode = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
	}

	protected NetworkDebugStart EnsureNetworkDebugStartExists()
	{
		if (this._networkDebugStart && this._networkDebugStart.gameObject == base.gameObject)
		{
			return this._networkDebugStart;
		}
		NetworkDebugStart networkDebugStart;
		if (base.TryGetBehaviour<NetworkDebugStart>(out networkDebugStart))
		{
			this._networkDebugStart = networkDebugStart;
			return networkDebugStart;
		}
		this._networkDebugStart = base.AddBehaviour<NetworkDebugStart>();
		return this._networkDebugStart;
	}

	private void Update()
	{
		NetworkDebugStart networkDebugStart = this.EnsureNetworkDebugStartExists();
		if (networkDebugStart.StartMode != NetworkDebugStart.StartModes.UserInterface)
		{
			return;
		}
		if (networkDebugStart.CurrentStage != NetworkDebugStart.Stage.Disconnected)
		{
			return;
		}
		if (this.EnableHotkeys)
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				this._networkDebugStart.StartSinglePlayer();
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartHostWithClients(this._networkDebugStart);
				}
				else
				{
					this._networkDebugStart.StartHost();
				}
			}
			if (Input.GetKeyDown(KeyCode.S))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartServerWithClients(this._networkDebugStart);
				}
				else
				{
					this._networkDebugStart.StartServer();
				}
			}
			if (Input.GetKeyDown(KeyCode.C))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartMultipleClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartClient();
				}
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartMultipleAutoClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartAutoClient();
				}
			}
			if (Input.GetKeyDown(KeyCode.P))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartMultipleSharedClients(networkDebugStart);
					return;
				}
				networkDebugStart.StartSharedClient();
			}
		}
	}

	protected virtual void OnGUI()
	{
		NetworkDebugStart networkDebugStart = this.EnsureNetworkDebugStartExists();
		if (networkDebugStart.StartMode != NetworkDebugStart.StartModes.UserInterface)
		{
			return;
		}
		NetworkDebugStart.Stage currentStage = networkDebugStart.CurrentStage;
		if (networkDebugStart.AutoHideGUI && currentStage == NetworkDebugStart.Stage.AllConnected)
		{
			return;
		}
		GUISkin skin = GUI.skin;
		float num;
		float num2;
		int num3;
		int num4;
		float num5;
		GUI.skin = FusionScalableIMGUI.GetScaledSkin(this.BaseSkin, out num, out num2, out num3, out num4, out num5);
		GUILayout.BeginArea(new Rect(num5, (float)num4, num2, (float)Screen.height));
		GUILayout.BeginVertical(GUI.skin.window, Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Height(num) });
		string text;
		GUILayout.Label(this._nicifiedStageNames.TryGetValue(networkDebugStart.CurrentStage, out text) ? text : "Unrecognized Stage", new GUIStyle(GUI.skin.label)
		{
			fontSize = (int)((float)GUI.skin.label.fontSize * 0.8f),
			alignment = TextAnchor.UpperLeft
		}, Array.Empty<GUILayoutOption>());
		if (!networkDebugStart.AutoHideGUI && networkDebugStart.CurrentStage == NetworkDebugStart.Stage.AllConnected && GUILayout.Button("X", new GUILayoutOption[]
		{
			GUILayout.ExpandHeight(true),
			GUILayout.Width(num)
		}))
		{
			networkDebugStart.AutoHideGUI = true;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUI.skin.window, Array.Empty<GUILayoutOption>());
		if (currentStage == NetworkDebugStart.Stage.Disconnected)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Room:", new GUILayoutOption[]
			{
				GUILayout.Height(num),
				GUILayout.Width(num2 * 0.33f)
			});
			networkDebugStart.DefaultRoomName = GUILayout.TextField(networkDebugStart.DefaultRoomName, 25, new GUILayoutOption[] { GUILayout.Height(num) });
			GUILayout.EndHorizontal();
			if (GUILayout.Button(this.EnableHotkeys ? "Start Single Player (I)" : "Start Single Player", new GUILayoutOption[] { GUILayout.Height(num) }))
			{
				networkDebugStart.StartSinglePlayer();
			}
			if (GUILayout.Button(this.EnableHotkeys ? "Start Shared Client (P)" : "Start Shared Client", new GUILayoutOption[] { GUILayout.Height(num) }))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartMultipleSharedClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartSharedClient();
				}
			}
			if (GUILayout.Button(this.EnableHotkeys ? "Start Server (S)" : "Start Server", new GUILayoutOption[] { GUILayout.Height(num) }))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartServerWithClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartServer();
				}
			}
			if (GUILayout.Button(this.EnableHotkeys ? "Start Host (H)" : "Start Host", new GUILayoutOption[] { GUILayout.Height(num) }))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartHostWithClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartHost();
				}
			}
			if (GUILayout.Button(this.EnableHotkeys ? "Start Client (C)" : "Start Client", new GUILayoutOption[] { GUILayout.Height(num) }))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartMultipleClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartClient();
				}
			}
			if (GUILayout.Button(this.EnableHotkeys ? "Start Auto Host Or Client (A)" : "Start Auto Host Or Client", new GUILayoutOption[] { GUILayout.Height(num) }))
			{
				if (this._isMultiplePeerMode)
				{
					this.StartMultipleAutoClients(networkDebugStart);
				}
				else
				{
					networkDebugStart.StartAutoClient();
				}
			}
			if (this._isMultiplePeerMode)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label("Client Count:", new GUILayoutOption[] { GUILayout.Height(num) });
				GUILayout.Label("", new GUILayoutOption[] { GUILayout.Width(4f) });
				string text2 = GUILayout.TextField(this._clientCount, 10, new GUILayoutOption[]
				{
					GUILayout.Width(num2 * 0.25f),
					GUILayout.Height(num)
				});
				if (this._clientCount != text2)
				{
					this._clientCount = text2;
					this.ValidateClientCount();
				}
				GUILayout.EndHorizontal();
			}
		}
		else if (GUILayout.Button("Shutdown", new GUILayoutOption[] { GUILayout.Height(num) }))
		{
			this._networkDebugStart.ShutdownAll();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUI.skin = skin;
	}

	private void StartHostWithClients(NetworkDebugStart nds)
	{
		int num;
		try
		{
			num = Convert.ToInt32(this._clientCount);
		}
		catch
		{
			num = 0;
		}
		nds.StartHostPlusClients(num);
	}

	private void StartServerWithClients(NetworkDebugStart nds)
	{
		int num;
		try
		{
			num = Convert.ToInt32(this._clientCount);
		}
		catch
		{
			num = 0;
		}
		nds.StartServerPlusClients(num);
	}

	private void StartMultipleClients(NetworkDebugStart nds)
	{
		int num;
		try
		{
			num = Convert.ToInt32(this._clientCount);
		}
		catch
		{
			num = 0;
		}
		nds.StartMultipleClients(num);
	}

	private void StartMultipleAutoClients(NetworkDebugStart nds)
	{
		int num;
		int.TryParse(this._clientCount, out num);
		nds.StartMultipleAutoClients(num);
	}

	private void StartMultipleSharedClients(NetworkDebugStart nds)
	{
		int num;
		try
		{
			num = Convert.ToInt32(this._clientCount);
		}
		catch
		{
			num = 0;
		}
		nds.StartMultipleSharedClients(num);
	}

	public static Dictionary<T, string> ConvertEnumToNicifiedNameLookup<T>(string prefix = null, Dictionary<T, string> nonalloc = null) where T : Enum
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (nonalloc == null)
		{
			nonalloc = new Dictionary<T, string>();
		}
		else
		{
			nonalloc.Clear();
		}
		string[] names = Enum.GetNames(typeof(T));
		Array values = Enum.GetValues(typeof(T));
		int i = 0;
		int num = names.Length;
		while (i < num)
		{
			stringBuilder.Clear();
			if (prefix != null)
			{
				stringBuilder.Append(prefix);
			}
			string text = names[i];
			for (int j = 0; j < text.Length; j++)
			{
				if (char.IsUpper(text[j]) && j != 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(text[j]);
			}
			nonalloc.Add((T)((object)values.GetValue(i)), stringBuilder.ToString());
			i++;
		}
		return nonalloc;
	}

	public NetworkDebugStartGUI()
	{
	}

	[InlineHelp]
	public bool EnableHotkeys;

	[InlineHelp]
	public GUISkin BaseSkin;

	private NetworkDebugStart _networkDebugStart;

	private string _clientCount;

	private bool _isMultiplePeerMode;

	private Dictionary<NetworkDebugStart.Stage, string> _nicifiedStageNames;
}
