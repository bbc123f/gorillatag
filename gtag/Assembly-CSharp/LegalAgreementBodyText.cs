using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D6 RID: 470
public class LegalAgreementBodyText : MonoBehaviour
{
	// Token: 0x06000C30 RID: 3120 RVA: 0x0004A2F0 File Offset: 0x000484F0
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0004A304 File Offset: 0x00048504
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

	// Token: 0x06000C32 RID: 3122 RVA: 0x0004A394 File Offset: 0x00048594
	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0004A3F0 File Offset: 0x000485F0
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

	// Token: 0x06000C34 RID: 3124 RVA: 0x0004A443 File Offset: 0x00048643
	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x0004A461 File Offset: 0x00048661
	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000C36 RID: 3126 RVA: 0x0004A474 File Offset: 0x00048674
	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	// Token: 0x04000F83 RID: 3971
	[SerializeField]
	private Text textBox;

	// Token: 0x04000F84 RID: 3972
	[SerializeField]
	private TextAsset textAsset;

	// Token: 0x04000F85 RID: 3973
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x04000F86 RID: 3974
	private List<Text> textCollection = new List<Text>();

	// Token: 0x04000F87 RID: 3975
	private string cachedText;

	// Token: 0x04000F88 RID: 3976
	private LegalAgreementBodyText.State state;

	// Token: 0x02000459 RID: 1113
	private enum State
	{
		// Token: 0x04001E19 RID: 7705
		Ready,
		// Token: 0x04001E1A RID: 7706
		Loading,
		// Token: 0x04001E1B RID: 7707
		Error
	}
}
