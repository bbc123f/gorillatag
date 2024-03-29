﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
	private void Start()
	{
		DebugUIBuilder.instance.AddLabel("Select Sample Scene", 0);
		int sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
		for (int i = 0; i < sceneCountInBuildSettings; i++)
		{
			string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
			int sceneIndex = i;
			DebugUIBuilder.instance.AddButton(Path.GetFileNameWithoutExtension(scenePathByBuildIndex), delegate
			{
				this.LoadScene(sceneIndex);
			}, -1, 0, false);
		}
		DebugUIBuilder.instance.Show();
	}

	private void LoadScene(int idx)
	{
		DebugUIBuilder.instance.Hide();
		Debug.Log("Load scene: " + idx.ToString());
		SceneManager.LoadScene(idx);
	}

	public StartMenu()
	{
	}

	public OVROverlay overlay;

	public OVROverlay text;

	public OVRCameraRig vrRig;

	[CompilerGenerated]
	private sealed class <>c__DisplayClass3_0
	{
		public <>c__DisplayClass3_0()
		{
		}

		internal void <Start>b__0()
		{
			this.<>4__this.LoadScene(this.sceneIndex);
		}

		public int sceneIndex;

		public StartMenu <>4__this;
	}
}
