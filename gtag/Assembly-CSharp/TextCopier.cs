using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001A5 RID: 421
public class TextCopier : MonoBehaviour
{
	// Token: 0x06000AD7 RID: 2775 RVA: 0x000432DB File Offset: 0x000414DB
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x000432E9 File Offset: 0x000414E9
	private void Update()
	{
		if (this.myText.text != this.textToCopy.text)
		{
			this.myText.text = this.textToCopy.text;
		}
	}

	// Token: 0x04000DA9 RID: 3497
	public Text textToCopy;

	// Token: 0x04000DAA RID: 3498
	private Text myText;
}
