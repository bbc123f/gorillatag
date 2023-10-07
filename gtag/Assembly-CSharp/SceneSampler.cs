using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000B9 RID: 185
public class SceneSampler : MonoBehaviour
{
	// Token: 0x06000417 RID: 1047 RVA: 0x0001B147 File Offset: 0x00019347
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0001B154 File Offset: 0x00019354
	private void Update()
	{
		bool active = OVRInput.GetActiveController() == OVRInput.Controller.Touch || OVRInput.GetActiveController() == OVRInput.Controller.LTouch || OVRInput.GetActiveController() == OVRInput.Controller.RTouch;
		this.displayText.SetActive(active);
		if (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			this.currentSceneIndex++;
			if (this.currentSceneIndex >= SceneManager.sceneCountInBuildSettings)
			{
				this.currentSceneIndex = 0;
			}
			SceneManager.LoadScene(this.currentSceneIndex);
		}
		Vector3 vector = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + Vector3.up * 0.09f;
		this.displayText.transform.position = vector;
		this.displayText.transform.rotation = Quaternion.LookRotation(vector - Camera.main.transform.position);
	}

	// Token: 0x040004C5 RID: 1221
	private int currentSceneIndex;

	// Token: 0x040004C6 RID: 1222
	public GameObject displayText;
}
