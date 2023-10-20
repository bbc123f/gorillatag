using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D7 RID: 471
public class LegalAgreementBodyText : MonoBehaviour
{
	// Token: 0x06000C36 RID: 3126 RVA: 0x0004A558 File Offset: 0x00048758
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x0004A56C File Offset: 0x0004876C
	public void SetText(string text)
	{
		text = Regex.Unescape(text);
		string[] array = text.Split(new string[]
		{
			Environment.NewLine,
			"\\r\\n",
			"\n"
		}, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			Text text2;
			if (i >= this.textCollection.Count)
			{
				text2 = Object.Instantiate<Text>(this.textBox, base.transform);
				this.textCollection.Add(text2);
			}
			else
			{
				text2 = this.textCollection[i];
			}
			text2.text = array[i];
		}
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0004A5FC File Offset: 0x000487FC
	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x0004A658 File Offset: 0x00048858
	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version)
	{
		LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0004A6AB File Offset: 0x000488AB
	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0004A6C9 File Offset: 0x000488C9
	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000C3C RID: 3132 RVA: 0x0004A6DC File Offset: 0x000488DC
	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	// Token: 0x04000F87 RID: 3975
	[SerializeField]
	private Text textBox;

	// Token: 0x04000F88 RID: 3976
	[SerializeField]
	private TextAsset textAsset;

	// Token: 0x04000F89 RID: 3977
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x04000F8A RID: 3978
	private List<Text> textCollection = new List<Text>();

	// Token: 0x04000F8B RID: 3979
	private string cachedText;

	// Token: 0x04000F8C RID: 3980
	private LegalAgreementBodyText.State state;

	// Token: 0x0200045B RID: 1115
	private enum State
	{
		// Token: 0x04001E26 RID: 7718
		Ready,
		// Token: 0x04001E27 RID: 7719
		Loading,
		// Token: 0x04001E28 RID: 7720
		Error
	}
}
