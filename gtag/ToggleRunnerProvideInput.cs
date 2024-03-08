using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Prototyping/Toggle Runner Provide Input")]
[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class ToggleRunnerProvideInput : Fusion.Behaviour
{
	public void Awake()
	{
		if (NetworkProjectConfig.Global.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
		{
			Debug.LogWarning("ToggleRunnerProvideInput only works in Multi-Peer mode. Destroying.");
			Object.Destroy(this);
			return;
		}
		if (ToggleRunnerProvideInput._instance)
		{
			Object.Destroy(this);
		}
		ToggleRunnerProvideInput._instance = this;
	}

	public void Update()
	{
		if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftMeta)) && Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				this.ToggleAll(-1);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				this.ToggleAll(0);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				this.ToggleAll(1);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				this.ToggleAll(2);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				this.ToggleAll(3);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				this.ToggleAll(4);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				this.ToggleAll(5);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				this.ToggleAll(6);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				this.ToggleAll(7);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				this.ToggleAll(8);
			}
		}
	}

	private void ToggleAll(int runnerIndex)
	{
		List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
		int num = 0;
		while (instancesEnumerator.MoveNext())
		{
			NetworkRunner networkRunner = instancesEnumerator.Current;
			if (!(networkRunner == null) && networkRunner.IsRunning)
			{
				bool flag = runnerIndex == -1 || num == runnerIndex;
				networkRunner.ProvideInput = flag;
				num++;
			}
		}
	}

	private static ToggleRunnerProvideInput _instance;
}
