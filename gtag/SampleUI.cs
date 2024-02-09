using System;
using UnityEngine;
using UnityEngine.UI;

public class SampleUI : MonoBehaviour
{
	private void Start()
	{
		DebugUIBuilder.instance.AddLabel("Enable Firebase in your project before running this sample", 1);
		DebugUIBuilder.instance.Show();
		this.inMenu = true;
	}

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

	private string GetText()
	{
		return this.inputText.GetComponentInChildren<InputField>().text;
	}

	private RectTransform collectionButton;

	private RectTransform inputText;

	private RectTransform valueText;

	private bool inMenu;
}
