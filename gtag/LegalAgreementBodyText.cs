using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
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

	public async Task<bool> UpdateTextFromPlayFabTitleData(string key, string version)
	{
		string text = key + "_" + version;
		this.state = LegalAgreementBodyText.State.Loading;
		PlayFabTitleDataCache.Instance.GetTitleData(text, new Action<string>(this.OnTitleDataReceived), new Action<PlayFabError>(this.OnPlayFabError));
		while (this.state == LegalAgreementBodyText.State.Loading)
		{
			await Task.Yield();
		}
		bool flag;
		if (this.cachedText != null)
		{
			this.SetText(this.cachedText.Substring(1, this.cachedText.Length - 2));
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
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

	public LegalAgreementBodyText()
	{
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

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <UpdateTextFromPlayFabTitleData>d__10 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num2;
			int num = num2;
			LegalAgreementBodyText legalAgreementBodyText = this;
			bool flag;
			try
			{
				if (num != 0)
				{
					string text = key + "_" + version;
					legalAgreementBodyText.state = LegalAgreementBodyText.State.Loading;
					PlayFabTitleDataCache.Instance.GetTitleData(text, new Action<string>(legalAgreementBodyText.OnTitleDataReceived), new Action<PlayFabError>(legalAgreementBodyText.OnPlayFabError));
					goto IL_B8;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_B1:
				yieldAwaiter.GetResult();
				IL_B8:
				if (legalAgreementBodyText.state != LegalAgreementBodyText.State.Loading)
				{
					if (legalAgreementBodyText.cachedText != null)
					{
						legalAgreementBodyText.SetText(legalAgreementBodyText.cachedText.Substring(1, legalAgreementBodyText.cachedText.Length - 2));
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_B1;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			this.<>t__builder.SetResult(flag);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<bool> <>t__builder;

		public string key;

		public string version;

		public LegalAgreementBodyText <>4__this;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}
}
