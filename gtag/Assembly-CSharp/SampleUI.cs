using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A4 RID: 164
public class SampleUI : MonoBehaviour
{
	// Token: 0x06000395 RID: 917 RVA: 0x00015CB4 File Offset: 0x00013EB4
	private void Start()
	{
		DebugUIBuilder.instance.AddLabel("Enable Firebase in your project before running this sample", 1);
		DebugUIBuilder.instance.Show();
		this.inMenu = true;
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00015CD8 File Offset: 0x00013ED8
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.Active) || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.Active))
		{
			if (this.inMenu)
			{
				DebugUIBuilder.instance.Hide();
			}
			else
			{
				DebugUIBuilder.instance.Show();
			}
			this.inMenu = !this.inMenu;
		}
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00015D30 File Offset: 0x00013F30
	private string GetText()
	{
		return this.inputText.GetComponentInChildren<InputField>().text;
	}

	// Token: 0x04000437 RID: 1079
	private RectTransform collectionButton;

	// Token: 0x04000438 RID: 1080
	private RectTransform inputText;

	// Token: 0x04000439 RID: 1081
	private RectTransform valueText;

	// Token: 0x0400043A RID: 1082
	private bool inMenu;
}
