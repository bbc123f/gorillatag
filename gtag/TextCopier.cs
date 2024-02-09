using System;
using UnityEngine;
using UnityEngine.UI;

public class TextCopier : MonoBehaviour
{
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
	}

	private void Update()
	{
		if (this.myText.text != this.textToCopy.text)
		{
			this.myText.text = this.textToCopy.text;
		}
	}

	public Text textToCopy;

	private Text myText;
}
