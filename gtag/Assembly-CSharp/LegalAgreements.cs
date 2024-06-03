using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

public class LegalAgreements : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		LegalAgreements.<Start>d__20 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<LegalAgreements.<Start>d__20>(ref <Start>d__);
	}

	private void Update()
	{
		this.leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		this.rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		Vector2 axis;
		this.leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis);
		Vector2 axis2;
		this.rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out axis2);
		axis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
		axis2 = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
		float num = Mathf.Clamp(axis.y + axis2.y, -1f, 1f);
		this.scrollView.verticalNormalizedPosition += num * (this.scrollSpeed / this.body.Height) * Time.deltaTime;
	}

	private Task WaitForAcknowledgement()
	{
		LegalAgreements.<WaitForAcknowledgement>d__22 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<LegalAgreements.<WaitForAcknowledgement>d__22>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	private Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		LegalAgreements.<UpdateText>d__23 <UpdateText>d__;
		<UpdateText>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateText>d__.<>4__this = this;
		<UpdateText>d__.asset = asset;
		<UpdateText>d__.version = version;
		<UpdateText>d__.<>1__state = -1;
		<UpdateText>d__.<>t__builder.Start<LegalAgreements.<UpdateText>d__23>(ref <UpdateText>d__);
		return <UpdateText>d__.<>t__builder.Task;
	}

	private Task FadeGroup(CanvasGroup canvasGroup, float finalAlpha, float time)
	{
		LegalAgreements.<FadeGroup>d__24 <FadeGroup>d__;
		<FadeGroup>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<FadeGroup>d__.canvasGroup = canvasGroup;
		<FadeGroup>d__.finalAlpha = finalAlpha;
		<FadeGroup>d__.time = time;
		<FadeGroup>d__.<>1__state = -1;
		<FadeGroup>d__.<>t__builder.Start<LegalAgreements.<FadeGroup>d__24>(ref <FadeGroup>d__);
		return <FadeGroup>d__.<>t__builder.Task;
	}

	private Task FadeBackgroundColor(Color targetColor, float time)
	{
		LegalAgreements.<FadeBackgroundColor>d__25 <FadeBackgroundColor>d__;
		<FadeBackgroundColor>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<FadeBackgroundColor>d__.<>4__this = this;
		<FadeBackgroundColor>d__.targetColor = targetColor;
		<FadeBackgroundColor>d__.time = time;
		<FadeBackgroundColor>d__.<>1__state = -1;
		<FadeBackgroundColor>d__.<>t__builder.Start<LegalAgreements.<FadeBackgroundColor>d__25>(ref <FadeBackgroundColor>d__);
		return <FadeBackgroundColor>d__.<>t__builder.Task;
	}

	private Task<string> GetTitleDataAsync(string key)
	{
		LegalAgreements.<GetTitleDataAsync>d__26 <GetTitleDataAsync>d__;
		<GetTitleDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetTitleDataAsync>d__.key = key;
		<GetTitleDataAsync>d__.<>1__state = -1;
		<GetTitleDataAsync>d__.<>t__builder.Start<LegalAgreements.<GetTitleDataAsync>d__26>(ref <GetTitleDataAsync>d__);
		return <GetTitleDataAsync>d__.<>t__builder.Task;
	}

	private Task<JsonObject> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		LegalAgreements.<GetAcceptedAgreements>d__27 <GetAcceptedAgreements>d__;
		<GetAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder<JsonObject>.Create();
		<GetAcceptedAgreements>d__.agreements = agreements;
		<GetAcceptedAgreements>d__.<>1__state = -1;
		<GetAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<GetAcceptedAgreements>d__27>(ref <GetAcceptedAgreements>d__);
		return <GetAcceptedAgreements>d__.<>t__builder.Task;
	}

	private Task SubmitAcceptedAgreements(JsonObject agreements)
	{
		LegalAgreements.<SubmitAcceptedAgreements>d__28 <SubmitAcceptedAgreements>d__;
		<SubmitAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SubmitAcceptedAgreements>d__.agreements = agreements;
		<SubmitAcceptedAgreements>d__.<>1__state = -1;
		<SubmitAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<SubmitAcceptedAgreements>d__28>(ref <SubmitAcceptedAgreements>d__);
		return <SubmitAcceptedAgreements>d__.<>t__builder.Task;
	}

	public LegalAgreements()
	{
	}

	[SerializeField]
	private ScrollRect scrollView;

	[SerializeField]
	private float scrollSpeed = 0.2f;

	[SerializeField]
	private float holdTime = 1f;

	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	[SerializeField]
	private Text title;

	[SerializeField]
	private Text acknowledgementPrompt;

	[SerializeField]
	private LegalAgreementBodyText body;

	[SerializeField]
	private Image progressImage;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Camera cam;

	[SerializeField]
	public bool testAgreement;

	[SerializeField]
	public bool testSubmitResult;

	[SerializeField]
	public bool testFaceButtonPress;

	[SerializeField]
	private int cullingMask;

	[SerializeField]
	private GameObject UIParent;

	private InputDevice leftHandDevice;

	private InputDevice rightHandDevice;

	private Color camBackgroundColor = Color.black;

	private Color originalColor;

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal string <GetAcceptedAgreements>b__27_0(LegalAgreementTextAsset x)
		{
			return x.playFabKey;
		}

		public static readonly LegalAgreements.<>c <>9 = new LegalAgreements.<>c();

		public static Func<LegalAgreementTextAsset, string> <>9__27_0;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass26_0
	{
		public <>c__DisplayClass26_0()
		{
		}

		internal void <GetTitleDataAsync>b__0(string res)
		{
			this.result = res;
			this.state = 1;
		}

		internal void <GetTitleDataAsync>b__1(PlayFabError err)
		{
			this.result = null;
			this.state = -1;
			Debug.LogError(err.ErrorMessage);
		}

		public string result;

		public int state;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass27_0
	{
		public <>c__DisplayClass27_0()
		{
		}

		internal void <GetAcceptedAgreements>b__1(ExecuteFunctionResult result)
		{
			this.state = 1;
			this.returnValue = (result.FunctionResult as JsonObject);
		}

		internal void <GetAcceptedAgreements>b__2(PlayFabError error)
		{
			Debug.LogError(error.ErrorMessage);
			this.state = -1;
		}

		public int state;

		public JsonObject returnValue;
	}

	[CompilerGenerated]
	private sealed class <>c__DisplayClass28_0
	{
		public <>c__DisplayClass28_0()
		{
		}

		internal void <SubmitAcceptedAgreements>b__0(ExecuteFunctionResult result)
		{
			this.state = 1;
		}

		internal void <SubmitAcceptedAgreements>b__1(PlayFabError error)
		{
			this.state = -1;
		}

		public int state;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <FadeBackgroundColor>d__25 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			LegalAgreements legalAgreements = this.<>4__this;
			try
			{
				if (num != 0)
				{
					legalAgreements.cam.backgroundColor = Color.black;
					this.<t>5__2 = 0f;
					this.<startColor>5__3 = legalAgreements.cam.backgroundColor;
					goto IL_DA;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_D3:
				awaiter.GetResult();
				IL_DA:
				if (this.<t>5__2 >= 1f)
				{
					legalAgreements.cam.backgroundColor = this.targetColor;
				}
				else
				{
					this.<t>5__2 += Time.deltaTime / this.time;
					legalAgreements.cam.backgroundColor = Color.Lerp(this.<startColor>5__3, this.targetColor, this.<t>5__2);
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<FadeBackgroundColor>d__25>(ref awaiter, ref this);
						return;
					}
					goto IL_D3;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public LegalAgreements <>4__this;

		public float time;

		public Color targetColor;

		private float <t>5__2;

		private Color <startColor>5__3;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <FadeGroup>d__24 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			try
			{
				if (num != 0)
				{
					this.<t>5__2 = 0f;
					this.<startAlpha>5__3 = this.canvasGroup.alpha;
					goto IL_C3;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_BC:
				awaiter.GetResult();
				IL_C3:
				if (this.<t>5__2 >= 1f)
				{
					this.canvasGroup.alpha = this.finalAlpha;
				}
				else
				{
					this.<t>5__2 += Time.deltaTime / this.time;
					this.canvasGroup.alpha = Mathf.Lerp(this.<startAlpha>5__3, this.finalAlpha, this.<t>5__2);
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<FadeGroup>d__24>(ref awaiter, ref this);
						return;
					}
					goto IL_BC;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public CanvasGroup canvasGroup;

		public float time;

		public float finalAlpha;

		private float <t>5__2;

		private float <startAlpha>5__3;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <GetAcceptedAgreements>d__27 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			JsonObject returnValue;
			try
			{
				if (num != 0)
				{
					this.<>8__1 = new LegalAgreements.<>c__DisplayClass27_0();
					this.<>8__1.state = 0;
					this.<>8__1.returnValue = null;
					string[] value = this.agreements.Select(new Func<LegalAgreementTextAsset, string>(LegalAgreements.<>c.<>9.<GetAcceptedAgreements>b__27_0)).ToArray<string>();
					PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
					{
						Entity = new EntityKey
						{
							Id = PlayFabSettings.staticPlayer.EntityId,
							Type = PlayFabSettings.staticPlayer.EntityType
						},
						FunctionName = "GetAcceptedAgreements",
						FunctionParameter = string.Join(",", value),
						GeneratePlayStreamEvent = new bool?(false)
					}, new Action<ExecuteFunctionResult>(this.<>8__1.<GetAcceptedAgreements>b__1), new Action<PlayFabError>(this.<>8__1.<GetAcceptedAgreements>b__2), null, null);
					goto IL_13E;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_137:
				awaiter.GetResult();
				IL_13E:
				if (this.<>8__1.state != 0)
				{
					returnValue = this.<>8__1.returnValue;
				}
				else
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<GetAcceptedAgreements>d__27>(ref awaiter, ref this);
						return;
					}
					goto IL_137;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>8__1 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>8__1 = null;
			this.<>t__builder.SetResult(returnValue);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<JsonObject> <>t__builder;

		public LegalAgreementTextAsset[] agreements;

		private LegalAgreements.<>c__DisplayClass27_0 <>8__1;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <GetTitleDataAsync>d__26 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			string result;
			try
			{
				if (num != 0)
				{
					this.<>8__1 = new LegalAgreements.<>c__DisplayClass26_0();
					this.<>8__1.state = 0;
					this.<>8__1.result = null;
					PlayFabTitleDataCache.Instance.GetTitleData(this.key, new Action<string>(this.<>8__1.<GetTitleDataAsync>b__0), new Action<PlayFabError>(this.<>8__1.<GetTitleDataAsync>b__1));
					goto IL_C1;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_BA:
				awaiter.GetResult();
				IL_C1:
				if (this.<>8__1.state != 0)
				{
					result = ((this.<>8__1.state == 1) ? this.<>8__1.result : null);
				}
				else
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<GetTitleDataAsync>d__26>(ref awaiter, ref this);
						return;
					}
					goto IL_BA;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>8__1 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>8__1 = null;
			this.<>t__builder.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<string> <>t__builder;

		public string key;

		private LegalAgreements.<>c__DisplayClass26_0 <>8__1;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <Start>d__20 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			LegalAgreements legalAgreements = this.<>4__this;
			try
			{
				YieldAwaitable.YieldAwaiter awaiter;
				TaskAwaiter<JsonObject> awaiter2;
				TaskAwaiter<string> awaiter3;
				TaskAwaiter awaiter4;
				TaskAwaiter<bool> awaiter5;
				switch (num)
				{
				case 0:
					awaiter = this.<>u__1;
					this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
					this.<>1__state = -1;
					break;
				case 1:
					awaiter2 = this.<>u__2;
					this.<>u__2 = default(TaskAwaiter<JsonObject>);
					this.<>1__state = -1;
					goto IL_17F;
				case 2:
					awaiter3 = this.<>u__3;
					this.<>u__3 = default(TaskAwaiter<string>);
					this.<>1__state = -1;
					goto IL_21D;
				case 3:
					awaiter4 = this.<>u__4;
					this.<>u__4 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_33A;
				case 4:
					awaiter5 = this.<>u__5;
					this.<>u__5 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
					goto IL_3C2;
				case 5:
					awaiter4 = this.<>u__4;
					this.<>u__4 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_440;
				case 6:
					awaiter = this.<>u__1;
					this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
					this.<>1__state = -1;
					goto IL_49D;
				case 7:
					awaiter4 = this.<>u__4;
					this.<>u__4 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_561;
				case 8:
					awaiter4 = this.<>u__4;
					this.<>u__4 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_5BF;
				case 9:
					awaiter4 = this.<>u__4;
					this.<>u__4 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_62E;
				case 10:
					awaiter4 = this.<>u__4;
					this.<>u__4 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_6F4;
				case 11:
					awaiter4 = this.<>u__4;
					this.<>u__4 = default(TaskAwaiter);
					this.<>1__state = -1;
					goto IL_783;
				default:
					legalAgreements.cam = Camera.main;
					legalAgreements.originalColor = legalAgreements.cam.backgroundColor;
					legalAgreements.canvasGroup.alpha = 0f;
					legalAgreements.cam.backgroundColor = Color.black;
					legalAgreements.progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
					legalAgreements.cullingMask = legalAgreements.cam.cullingMask;
					goto IL_114;
				}
				IL_10D:
				awaiter.GetResult();
				IL_114:
				if (PlayFabClientAPI.IsClientLoggedIn())
				{
					this.<versionMismatch>5__2 = false;
					awaiter2 = legalAgreements.GetAcceptedAgreements(legalAgreements.legalAgreementScreens).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						this.<>1__state = 1;
						this.<>u__2 = awaiter2;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<JsonObject>, LegalAgreements.<Start>d__20>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<Start>d__20>(ref awaiter, ref this);
						return;
					}
					goto IL_10D;
				}
				IL_17F:
				JsonObject result = awaiter2.GetResult();
				this.<agreementResults>5__3 = result;
				this.<>7__wrap3 = legalAgreements.legalAgreementScreens;
				this.<>7__wrap4 = 0;
				goto IL_66D;
				IL_21D:
				string result2 = awaiter3.GetResult();
				this.<latestVersion>5__7 = result2;
				this.<latestVersion>5__7 = this.<latestVersion>5__7.Substring(1, this.<latestVersion>5__7.Length - 2);
				object empty = string.Empty;
				bool flag = this.<agreementResults>5__3 != null && this.<agreementResults>5__3.TryGetValue(this.<screen>5__6.playFabKey, out empty);
				if (!legalAgreements.testAgreement && flag && this.<latestVersion>5__7 == empty.ToString())
				{
					goto IL_65F;
				}
				if (this.<versionMismatch>5__2)
				{
					goto IL_35F;
				}
				legalAgreements.cam.cullingMask = LayerMask.GetMask(new string[]
				{
					"UI"
				});
				legalAgreements.cam.backgroundColor = legalAgreements.camBackgroundColor;
				awaiter4 = legalAgreements.FadeBackgroundColor(legalAgreements.camBackgroundColor, 1f).GetAwaiter();
				if (!awaiter4.IsCompleted)
				{
					this.<>1__state = 3;
					this.<>u__4 = awaiter4;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref awaiter4, ref this);
					return;
				}
				IL_33A:
				awaiter4.GetResult();
				this.<versionMismatch>5__2 = true;
				GorillaTagger.Instance.overrideNotInFocus = true;
				legalAgreements.UIParent.SetActive(true);
				IL_35F:
				awaiter5 = legalAgreements.UpdateText(this.<screen>5__6, this.<latestVersion>5__7).GetAwaiter();
				if (!awaiter5.IsCompleted)
				{
					this.<>1__state = 4;
					this.<>u__5 = awaiter5;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, LegalAgreements.<Start>d__20>(ref awaiter5, ref this);
					return;
				}
				IL_3C2:
				if (!awaiter5.GetResult())
				{
					Object.Destroy(legalAgreements.acknowledgementPrompt);
					awaiter4 = legalAgreements.FadeGroup(legalAgreements.canvasGroup, 1f, 1f).GetAwaiter();
					if (!awaiter4.IsCompleted)
					{
						this.<>1__state = 5;
						this.<>u__4 = awaiter4;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref awaiter4, ref this);
						return;
					}
				}
				else
				{
					legalAgreements.transform.parent.eulerAngles = new Vector3(0f, legalAgreements.cam.transform.rotation.y, 0f);
					legalAgreements.transform.parent.position = legalAgreements.cam.transform.position;
					awaiter4 = legalAgreements.FadeGroup(legalAgreements.canvasGroup, 1f, 1f).GetAwaiter();
					if (!awaiter4.IsCompleted)
					{
						this.<>1__state = 7;
						this.<>u__4 = awaiter4;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref awaiter4, ref this);
						return;
					}
					goto IL_561;
				}
				IL_440:
				awaiter4.GetResult();
				IL_447:
				awaiter = Task.Yield().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					this.<>1__state = 6;
					this.<>u__1 = awaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<Start>d__20>(ref awaiter, ref this);
					return;
				}
				IL_49D:
				awaiter.GetResult();
				goto IL_447;
				IL_561:
				awaiter4.GetResult();
				awaiter4 = legalAgreements.WaitForAcknowledgement().GetAwaiter();
				if (!awaiter4.IsCompleted)
				{
					this.<>1__state = 8;
					this.<>u__4 = awaiter4;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref awaiter4, ref this);
					return;
				}
				IL_5BF:
				awaiter4.GetResult();
				awaiter4 = legalAgreements.FadeGroup(legalAgreements.canvasGroup, 0f, 1f).GetAwaiter();
				if (!awaiter4.IsCompleted)
				{
					this.<>1__state = 9;
					this.<>u__4 = awaiter4;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref awaiter4, ref this);
					return;
				}
				IL_62E:
				awaiter4.GetResult();
				this.<agreementResults>5__3[this.<screen>5__6.playFabKey] = this.<latestVersion>5__7;
				this.<latestVersion>5__7 = null;
				this.<screen>5__6 = null;
				IL_65F:
				this.<>7__wrap4++;
				IL_66D:
				if (this.<>7__wrap4 >= this.<>7__wrap3.Length)
				{
					this.<>7__wrap3 = null;
					if (!this.<versionMismatch>5__2)
					{
						goto IL_728;
					}
					awaiter4 = legalAgreements.FadeBackgroundColor(Color.black, 1f).GetAwaiter();
					if (!awaiter4.IsCompleted)
					{
						this.<>1__state = 10;
						this.<>u__4 = awaiter4;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref awaiter4, ref this);
						return;
					}
				}
				else
				{
					this.<screen>5__6 = this.<>7__wrap3[this.<>7__wrap4];
					awaiter3 = legalAgreements.GetTitleDataAsync(this.<screen>5__6.latestVersionKey).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						this.<>1__state = 2;
						this.<>u__3 = awaiter3;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<string>, LegalAgreements.<Start>d__20>(ref awaiter3, ref this);
						return;
					}
					goto IL_21D;
				}
				IL_6F4:
				awaiter4.GetResult();
				legalAgreements.cam.cullingMask = legalAgreements.cullingMask;
				legalAgreements.cam.backgroundColor = legalAgreements.originalColor;
				GorillaTagger.Instance.overrideNotInFocus = false;
				IL_728:
				awaiter4 = legalAgreements.SubmitAcceptedAgreements(this.<agreementResults>5__3).GetAwaiter();
				if (!awaiter4.IsCompleted)
				{
					this.<>1__state = 11;
					this.<>u__4 = awaiter4;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref awaiter4, ref this);
					return;
				}
				IL_783:
				awaiter4.GetResult();
				Object.Destroy(legalAgreements.transform.parent.gameObject);
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<agreementResults>5__3 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<agreementResults>5__3 = null;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncVoidMethodBuilder <>t__builder;

		public LegalAgreements <>4__this;

		private bool <versionMismatch>5__2;

		private JsonObject <agreementResults>5__3;

		private YieldAwaitable.YieldAwaiter <>u__1;

		private TaskAwaiter<JsonObject> <>u__2;

		private LegalAgreementTextAsset[] <>7__wrap3;

		private int <>7__wrap4;

		private LegalAgreementTextAsset <screen>5__6;

		private string <latestVersion>5__7;

		private TaskAwaiter<string> <>u__3;

		private TaskAwaiter <>u__4;

		private TaskAwaiter<bool> <>u__5;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <SubmitAcceptedAgreements>d__28 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			try
			{
				if (num != 0)
				{
					this.<>8__1 = new LegalAgreements.<>c__DisplayClass28_0();
					this.<>8__1.state = 0;
					PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
					{
						Entity = new EntityKey
						{
							Id = PlayFabSettings.staticPlayer.EntityId,
							Type = PlayFabSettings.staticPlayer.EntityType
						},
						FunctionName = "SubmitAcceptedAgreements",
						FunctionParameter = this.agreements.ToString(),
						GeneratePlayStreamEvent = new bool?(false)
					}, new Action<ExecuteFunctionResult>(this.<>8__1.<SubmitAcceptedAgreements>b__0), new Action<PlayFabError>(this.<>8__1.<SubmitAcceptedAgreements>b__1), null, null);
					goto IL_101;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_FA:
				awaiter.GetResult();
				IL_101:
				if (this.<>8__1.state == 0)
				{
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<SubmitAcceptedAgreements>d__28>(ref awaiter, ref this);
						return;
					}
					goto IL_FA;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>8__1 = null;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>8__1 = null;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public JsonObject agreements;

		private LegalAgreements.<>c__DisplayClass28_0 <>8__1;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <UpdateText>d__23 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			LegalAgreements legalAgreements = this.<>4__this;
			bool result2;
			try
			{
				TaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					legalAgreements.scrollView.verticalNormalizedPosition = 1f;
					legalAgreements.title.text = this.asset.title;
					legalAgreements.body.ClearText();
					awaiter = legalAgreements.body.UpdateTextFromPlayFabTitleData(this.asset.playFabKey, this.version).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, LegalAgreements.<UpdateText>d__23>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = this.<>u__1;
					this.<>u__1 = default(TaskAwaiter<bool>);
					this.<>1__state = -1;
				}
				bool result = awaiter.GetResult();
				if (!result)
				{
					legalAgreements.body.SetText(this.asset.errorMessage + "\n\nPlease restart the game and try again.");
				}
				result2 = result;
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder<bool> <>t__builder;

		public LegalAgreements <>4__this;

		public LegalAgreementTextAsset asset;

		public string version;

		private TaskAwaiter<bool> <>u__1;
	}

	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <WaitForAcknowledgement>d__22 : IAsyncStateMachine
	{
		void IAsyncStateMachine.MoveNext()
		{
			int num = this.<>1__state;
			LegalAgreements legalAgreements = this.<>4__this;
			try
			{
				if (num != 0)
				{
					this.<progress>5__2 = 0f;
					legalAgreements.progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
					goto IL_1A5;
				}
				YieldAwaitable.YieldAwaiter awaiter = this.<>u__1;
				this.<>u__1 = default(YieldAwaitable.YieldAwaiter);
				this.<>1__state = -1;
				IL_19E:
				awaiter.GetResult();
				IL_1A5:
				if (this.<progress>5__2 >= 1f)
				{
					legalAgreements.progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
				}
				else
				{
					legalAgreements.leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
					legalAgreements.rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
					bool state;
					legalAgreements.leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out state);
					bool state2;
					legalAgreements.leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out state2);
					bool state3;
					legalAgreements.rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out state3);
					bool state4;
					legalAgreements.rightHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out state4);
					state = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
					state2 = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
					state3 = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
					state4 = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
					bool flag = state || state2 || state3 || state4;
					if (legalAgreements.testFaceButtonPress || flag)
					{
						this.<progress>5__2 += Time.deltaTime / legalAgreements.holdTime;
					}
					else
					{
						this.<progress>5__2 = 0f;
					}
					legalAgreements.progressImage.rectTransform.localScale = new Vector3(Mathf.Clamp01(this.<progress>5__2), 1f, 1f);
					awaiter = Task.Yield().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						this.<>1__state = 0;
						this.<>u__1 = awaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<WaitForAcknowledgement>d__22>(ref awaiter, ref this);
						return;
					}
					goto IL_19E;
				}
			}
			catch (Exception exception)
			{
				this.<>1__state = -2;
				this.<>t__builder.SetException(exception);
				return;
			}
			this.<>1__state = -2;
			this.<>t__builder.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.<>t__builder.SetStateMachine(stateMachine);
		}

		public int <>1__state;

		public AsyncTaskMethodBuilder <>t__builder;

		public LegalAgreements <>4__this;

		private float <progress>5__2;

		private YieldAwaitable.YieldAwaiter <>u__1;
	}
}
