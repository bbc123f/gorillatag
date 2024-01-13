using UnityEngine;
using UnityEngine.UI;

public class TextCopier : MonoBehaviour
{
	public Text textToCopy;

	private Text myText;

	private void Start()
	{
		myText = GetComponent<Text>();
	}

	private void Update()
	{
		if (myText.text != textToCopy.text)
		{
			myText.text = textToCopy.text;
		}
	}
}
