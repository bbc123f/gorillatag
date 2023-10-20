using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001A6 RID: 422
public class TextCopier : MonoBehaviour
{
	// Token: 0x06000ADC RID: 2780 RVA: 0x00043413 File Offset: 0x00041613
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00043421 File Offset: 0x00041621
	private void Update()
	{
		if (this.myText.text != this.textToCopy.text)
		{
			this.myText.text = this.textToCopy.text;
		}
	}

	// Token: 0x04000DAD RID: 3501
	public Text textToCopy;

	// Token: 0x04000DAE RID: 3502
	private Text myText;
}
