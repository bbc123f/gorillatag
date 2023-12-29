using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class LegalAgreementBodyText : MonoBehaviour
{
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

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

	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

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

	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	[SerializeField]
	private Text textBox;

	[SerializeField]
	private TextAsset textAsset;

	[SerializeField]
	private RectTransform rectTransform;

	private List<Text> textCollection = new List<Text>();

	private string cachedText;

	private LegalAgreementBodyText.State state;

	private enum State
	{
		Ready,
		Loading,
		Error
	}
}
