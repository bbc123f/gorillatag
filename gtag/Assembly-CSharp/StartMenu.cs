using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000C9 RID: 201
public class StartMenu : MonoBehaviour
{
	// Token: 0x0600047D RID: 1149 RVA: 0x0001CD6C File Offset: 0x0001AF6C
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

	// Token: 0x0600047E RID: 1150 RVA: 0x0001CDE1 File Offset: 0x0001AFE1
	private void LoadScene(int idx)
	{
		DebugUIBuilder.instance.Hide();
		Debug.Log("Load scene: " + idx.ToString());
		SceneManager.LoadScene(idx);
	}

	// Token: 0x04000521 RID: 1313
	public OVROverlay overlay;

	// Token: 0x04000522 RID: 1314
	public OVROverlay text;

	// Token: 0x04000523 RID: 1315
	public OVRCameraRig vrRig;
}
