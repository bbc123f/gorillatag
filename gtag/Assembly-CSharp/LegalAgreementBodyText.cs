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
			int num = this.<>1__state;
			LegalAgreementBodyText legalAgreementBodyText = this.<>4__this;
			bool result;
			try
			{
				if (num != 0)
				{
					string name = this.key + "_" + this.version;
					legalAgreementBodyText.state = LegalAgreementBodyText.State.Loading;
					PlayFabTitleDataCache.Instance.GetTitleData(name, new Action<string>(legalAgreementBodyText.OnTitleDataReceived), new Action<PlayFabError>(legalAgreementBodyText.OnPlayFabError));
					goto IL_B8;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_B1:
				awaiter.GetResult();
				IL_B8:
				if (legalAgreementBodyText.state != LegalAgreementBodyText.State.Loading)
				{
					if (legalAgreementBodyText.cachedText != null)
					{
						legalAgreementBodyText.SetText(legalAgreementBodyText.cachedText.Substring(1, legalAgreementBodyText.cachedText.Length - 2));
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10>(ref awaiter, ref this);
						return;
					}
					goto IL_B1;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result);
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
