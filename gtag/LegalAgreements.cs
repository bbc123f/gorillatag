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

	private async void Start()
	{
		this.cam = Camera.main;
		this.originalColor = this.cam.backgroundColor;
		this.canvasGroup.alpha = 0f;
		this.cam.backgroundColor = Color.black;
		this.progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		this.cullingMask = this.cam.cullingMask;
		while (!PlayFabClientAPI.IsClientLoggedIn())
		{
			await Task.Yield();
		}
		bool versionMismatch = false;
		JsonObject agreementResults = await this.GetAcceptedAgreements(this.legalAgreementScreens);
		foreach (LegalAgreementTextAsset screen in this.legalAgreementScreens)
		{
			string latestVersion = await this.GetTitleDataAsync(screen.latestVersionKey);
			latestVersion = latestVersion.Substring(1, latestVersion.Length - 2);
			object empty = string.Empty;
			bool flag = agreementResults != null && agreementResults.TryGetValue(screen.playFabKey, out empty);
			if (this.testAgreement || !flag || !(latestVersion == empty.ToString()))
			{
				if (!versionMismatch)
				{
					this.cam.cullingMask = LayerMask.GetMask(new string[] { "UI" });
					this.cam.backgroundColor = this.camBackgroundColor;
					await this.FadeBackgroundColor(this.camBackgroundColor, 1f);
					versionMismatch = true;
					GorillaTagger.Instance.overrideNotInFocus = true;
					this.UIParent.SetActive(true);
				}
				TaskAwaiter<bool> taskAwaiter = this.UpdateText(screen, latestVersion).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					Object.Destroy(this.acknowledgementPrompt);
					await this.FadeGroup(this.canvasGroup, 1f, 1f);
					for (;;)
					{
						await Task.Yield();
					}
				}
				else
				{
					base.transform.parent.eulerAngles = new Vector3(0f, this.cam.transform.rotation.y, 0f);
					base.transform.parent.position = this.cam.transform.position;
					await this.FadeGroup(this.canvasGroup, 1f, 1f);
					await this.WaitForAcknowledgement();
					await this.FadeGroup(this.canvasGroup, 0f, 1f);
					agreementResults[screen.playFabKey] = latestVersion;
					latestVersion = null;
					screen = null;
				}
			}
		}
		LegalAgreementTextAsset[] array = null;
		if (versionMismatch)
		{
			await this.FadeBackgroundColor(Color.black, 1f);
			this.cam.cullingMask = this.cullingMask;
			this.cam.backgroundColor = this.originalColor;
			GorillaTagger.Instance.overrideNotInFocus = false;
		}
		await this.SubmitAcceptedAgreements(agreementResults);
		Object.Destroy(base.transform.parent.gameObject);
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

	private async Task WaitForAcknowledgement()
	{
		float progress = 0f;
		this.progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		while (progress < 1f)
		{
			this.leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			this.rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
			bool state;
			this.leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out state);
			bool state2;
			this.leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out state2);
			bool state3;
			this.rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out state3);
			bool state4;
			this.rightHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out state4);
			state = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
			state2 = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
			state3 = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
			state4 = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
			bool flag = state || state2 || state3 || state4;
			if (this.testFaceButtonPress || flag)
			{
				progress += Time.deltaTime / this.holdTime;
			}
			else
			{
				progress = 0f;
			}
			this.progressImage.rectTransform.localScale = new Vector3(Mathf.Clamp01(progress), 1f, 1f);
			await Task.Yield();
		}
		this.progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
	}

	private async Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		this.scrollView.verticalNormalizedPosition = 1f;
		this.title.text = asset.title;
		this.body.ClearText();
		bool flag = await this.body.UpdateTextFromPlayFabTitleData(asset.playFabKey, version);
		if (!flag)
		{
			this.body.SetText(asset.errorMessage + "\n\nPlease restart the game and try again.");
		}
		return flag;
	}

	private async Task FadeGroup(CanvasGroup canvasGroup, float finalAlpha, float time)
	{
		float t = 0f;
		float startAlpha = canvasGroup.alpha;
		while (t < 1f)
		{
			t += Time.deltaTime / time;
			canvasGroup.alpha = Mathf.Lerp(startAlpha, finalAlpha, t);
			await Task.Yield();
		}
		canvasGroup.alpha = finalAlpha;
	}

	private async Task FadeBackgroundColor(Color targetColor, float time)
	{
		this.cam.backgroundColor = Color.black;
		float t = 0f;
		Color startColor = this.cam.backgroundColor;
		while (t < 1f)
		{
			t += Time.deltaTime / time;
			this.cam.backgroundColor = Color.Lerp(startColor, targetColor, t);
			await Task.Yield();
		}
		this.cam.backgroundColor = targetColor;
	}

	private async Task<string> GetTitleDataAsync(string key)
	{
		int state = 0;
		string result = null;
		PlayFabTitleDataCache.Instance.GetTitleData(key, delegate(string res)
		{
			result = res;
			state = 1;
		}, delegate(PlayFabError err)
		{
			result = null;
			state = -1;
			Debug.LogError(err.ErrorMessage);
		});
		while (state == 0)
		{
			await Task.Yield();
		}
		return (state == 1) ? result : null;
	}

	private async Task<JsonObject> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		int state = 0;
		JsonObject returnValue = null;
		string[] array = agreements.Select((LegalAgreementTextAsset x) => x.playFabKey).ToArray<string>();
		PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
		{
			Entity = new EntityKey
			{
				Id = PlayFabSettings.staticPlayer.EntityId,
				Type = PlayFabSettings.staticPlayer.EntityType
			},
			FunctionName = "GetAcceptedAgreements",
			FunctionParameter = string.Join(",", array),
			GeneratePlayStreamEvent = new bool?(false)
		}, delegate(ExecuteFunctionResult result)
		{
			state = 1;
			returnValue = result.FunctionResult as JsonObject;
		}, delegate(PlayFabError error)
		{
			Debug.LogError(error.ErrorMessage);
			state = -1;
		}, null, null);
		while (state == 0)
		{
			await Task.Yield();
		}
		return returnValue;
	}

	private async Task SubmitAcceptedAgreements(JsonObject agreements)
	{
		int state = 0;
		PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
		{
			Entity = new EntityKey
			{
				Id = PlayFabSettings.staticPlayer.EntityId,
				Type = PlayFabSettings.staticPlayer.EntityType
			},
			FunctionName = "SubmitAcceptedAgreements",
			FunctionParameter = agreements.ToString(),
			GeneratePlayStreamEvent = new bool?(false)
		}, delegate(ExecuteFunctionResult result)
		{
			state = 1;
		}, delegate(PlayFabError error)
		{
			state = -1;
		}, null, null);
		while (state == 0)
		{
			await Task.Yield();
		}
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
			this.returnValue = result.FunctionResult as JsonObject;
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
			int num2;
			int num = num2;
			LegalAgreements legalAgreements = this;
			try
			{
				if (num != 0)
				{
					legalAgreements.cam.backgroundColor = Color.black;
					t = 0f;
					startColor = legalAgreements.cam.backgroundColor;
					goto IL_DA;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_D3:
				yieldAwaiter.GetResult();
				IL_DA:
				if (t >= 1f)
				{
					legalAgreements.cam.backgroundColor = targetColor;
				}
				else
				{
					t += Time.deltaTime / time;
					legalAgreements.cam.backgroundColor = Color.Lerp(startColor, targetColor, t);
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<FadeBackgroundColor>d__25>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_D3;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
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
			int num2;
			int num = num2;
			try
			{
				if (num != 0)
				{
					t = 0f;
					startAlpha = canvasGroup.alpha;
					goto IL_C3;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_BC:
				yieldAwaiter.GetResult();
				IL_C3:
				if (t >= 1f)
				{
					canvasGroup.alpha = finalAlpha;
				}
				else
				{
					t += Time.deltaTime / time;
					canvasGroup.alpha = Mathf.Lerp(startAlpha, finalAlpha, t);
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<FadeGroup>d__24>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_BC;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
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
			int num2;
			int num = num2;
			JsonObject returnValue;
			try
			{
				if (num != 0)
				{
					CS$<>8__locals1 = new LegalAgreements.<>c__DisplayClass27_0();
					CS$<>8__locals1.state = 0;
					CS$<>8__locals1.returnValue = null;
					string[] array = agreements.Select((LegalAgreementTextAsset x) => x.playFabKey).ToArray<string>();
					PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
					{
						Entity = new EntityKey
						{
							Id = PlayFabSettings.staticPlayer.EntityId,
							Type = PlayFabSettings.staticPlayer.EntityType
						},
						FunctionName = "GetAcceptedAgreements",
						FunctionParameter = string.Join(",", array),
						GeneratePlayStreamEvent = new bool?(false)
					}, delegate(ExecuteFunctionResult result)
					{
						CS$<>8__locals1.state = 1;
						CS$<>8__locals1.returnValue = result.FunctionResult as JsonObject;
					}, delegate(PlayFabError error)
					{
						Debug.LogError(error.ErrorMessage);
						CS$<>8__locals1.state = -1;
					}, null, null);
					goto IL_13E;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_137:
				yieldAwaiter.GetResult();
				IL_13E:
				if (CS$<>8__locals1.state != 0)
				{
					returnValue = CS$<>8__locals1.returnValue;
				}
				else
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<GetAcceptedAgreements>d__27>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_137;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				CS$<>8__locals1 = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			CS$<>8__locals1 = null;
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
			int num2;
			int num = num2;
			string text;
			try
			{
				if (num != 0)
				{
					CS$<>8__locals1 = new LegalAgreements.<>c__DisplayClass26_0();
					CS$<>8__locals1.state = 0;
					CS$<>8__locals1.result = null;
					PlayFabTitleDataCache.Instance.GetTitleData(key, delegate(string res)
					{
						CS$<>8__locals1.result = res;
						CS$<>8__locals1.state = 1;
					}, delegate(PlayFabError err)
					{
						CS$<>8__locals1.result = null;
						CS$<>8__locals1.state = -1;
						Debug.LogError(err.ErrorMessage);
					});
					goto IL_C1;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_BA:
				yieldAwaiter.GetResult();
				IL_C1:
				if (CS$<>8__locals1.state != 0)
				{
					text = ((CS$<>8__locals1.state == 1) ? CS$<>8__locals1.result : null);
				}
				else
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<GetTitleDataAsync>d__26>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_BA;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				CS$<>8__locals1 = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			CS$<>8__locals1 = null;
			this.<>t__builder.SetResult(text);
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
			int num2;
			int num = num2;
			LegalAgreements legalAgreements = this;
			try
			{
				YieldAwaitable.YieldAwaiter yieldAwaiter;
				TaskAwaiter<JsonObject> taskAwaiter3;
				TaskAwaiter<string> taskAwaiter5;
				TaskAwaiter taskAwaiter7;
				TaskAwaiter<bool> taskAwaiter9;
				switch (num)
				{
				case 0:
				{
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num2 = -1;
					break;
				}
				case 1:
				{
					TaskAwaiter<JsonObject> taskAwaiter4;
					taskAwaiter3 = taskAwaiter4;
					taskAwaiter4 = default(TaskAwaiter<JsonObject>);
					num2 = -1;
					goto IL_17F;
				}
				case 2:
				{
					TaskAwaiter<string> taskAwaiter6;
					taskAwaiter5 = taskAwaiter6;
					taskAwaiter6 = default(TaskAwaiter<string>);
					num2 = -1;
					goto IL_21D;
				}
				case 3:
				{
					TaskAwaiter taskAwaiter8;
					taskAwaiter7 = taskAwaiter8;
					taskAwaiter8 = default(TaskAwaiter);
					num2 = -1;
					goto IL_33A;
				}
				case 4:
					taskAwaiter9 = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num2 = -1;
					goto IL_3C2;
				case 5:
				{
					TaskAwaiter taskAwaiter8;
					taskAwaiter7 = taskAwaiter8;
					taskAwaiter8 = default(TaskAwaiter);
					num2 = -1;
					goto IL_440;
				}
				case 6:
				{
					YieldAwaitable.YieldAwaiter yieldAwaiter2;
					yieldAwaiter = yieldAwaiter2;
					yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
					num2 = -1;
					goto IL_49D;
				}
				case 7:
				{
					TaskAwaiter taskAwaiter8;
					taskAwaiter7 = taskAwaiter8;
					taskAwaiter8 = default(TaskAwaiter);
					num2 = -1;
					goto IL_561;
				}
				case 8:
				{
					TaskAwaiter taskAwaiter8;
					taskAwaiter7 = taskAwaiter8;
					taskAwaiter8 = default(TaskAwaiter);
					num2 = -1;
					goto IL_5BF;
				}
				case 9:
				{
					TaskAwaiter taskAwaiter8;
					taskAwaiter7 = taskAwaiter8;
					taskAwaiter8 = default(TaskAwaiter);
					num2 = -1;
					goto IL_62E;
				}
				case 10:
				{
					TaskAwaiter taskAwaiter8;
					taskAwaiter7 = taskAwaiter8;
					taskAwaiter8 = default(TaskAwaiter);
					num2 = -1;
					goto IL_6F4;
				}
				case 11:
				{
					TaskAwaiter taskAwaiter8;
					taskAwaiter7 = taskAwaiter8;
					taskAwaiter8 = default(TaskAwaiter);
					num2 = -1;
					goto IL_783;
				}
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
				yieldAwaiter.GetResult();
				IL_114:
				if (PlayFabClientAPI.IsClientLoggedIn())
				{
					versionMismatch = false;
					taskAwaiter3 = legalAgreements.GetAcceptedAgreements(legalAgreements.legalAgreementScreens).GetAwaiter();
					if (!taskAwaiter3.IsCompleted)
					{
						num2 = 1;
						TaskAwaiter<JsonObject> taskAwaiter4 = taskAwaiter3;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<JsonObject>, LegalAgreements.<Start>d__20>(ref taskAwaiter3, ref this);
						return;
					}
				}
				else
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<Start>d__20>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_10D;
				}
				IL_17F:
				JsonObject result = taskAwaiter3.GetResult();
				agreementResults = result;
				array = legalAgreements.legalAgreementScreens;
				i = 0;
				goto IL_66D;
				IL_21D:
				string result2 = taskAwaiter5.GetResult();
				latestVersion = result2;
				latestVersion = latestVersion.Substring(1, latestVersion.Length - 2);
				object empty = string.Empty;
				bool flag = agreementResults != null && agreementResults.TryGetValue(screen.playFabKey, out empty);
				if (!legalAgreements.testAgreement && flag && latestVersion == empty.ToString())
				{
					goto IL_65F;
				}
				if (versionMismatch)
				{
					goto IL_35F;
				}
				legalAgreements.cam.cullingMask = LayerMask.GetMask(new string[] { "UI" });
				legalAgreements.cam.backgroundColor = legalAgreements.camBackgroundColor;
				taskAwaiter7 = legalAgreements.FadeBackgroundColor(legalAgreements.camBackgroundColor, 1f).GetAwaiter();
				if (!taskAwaiter7.IsCompleted)
				{
					num2 = 3;
					TaskAwaiter taskAwaiter8 = taskAwaiter7;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref taskAwaiter7, ref this);
					return;
				}
				IL_33A:
				taskAwaiter7.GetResult();
				versionMismatch = true;
				GorillaTagger.Instance.overrideNotInFocus = true;
				legalAgreements.UIParent.SetActive(true);
				IL_35F:
				taskAwaiter9 = legalAgreements.UpdateText(screen, latestVersion).GetAwaiter();
				if (!taskAwaiter9.IsCompleted)
				{
					num2 = 4;
					taskAwaiter2 = taskAwaiter9;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, LegalAgreements.<Start>d__20>(ref taskAwaiter9, ref this);
					return;
				}
				IL_3C2:
				if (!taskAwaiter9.GetResult())
				{
					Object.Destroy(legalAgreements.acknowledgementPrompt);
					taskAwaiter7 = legalAgreements.FadeGroup(legalAgreements.canvasGroup, 1f, 1f).GetAwaiter();
					if (!taskAwaiter7.IsCompleted)
					{
						num2 = 5;
						TaskAwaiter taskAwaiter8 = taskAwaiter7;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref taskAwaiter7, ref this);
						return;
					}
				}
				else
				{
					legalAgreements.transform.parent.eulerAngles = new Vector3(0f, legalAgreements.cam.transform.rotation.y, 0f);
					legalAgreements.transform.parent.position = legalAgreements.cam.transform.position;
					taskAwaiter7 = legalAgreements.FadeGroup(legalAgreements.canvasGroup, 1f, 1f).GetAwaiter();
					if (!taskAwaiter7.IsCompleted)
					{
						num2 = 7;
						TaskAwaiter taskAwaiter8 = taskAwaiter7;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref taskAwaiter7, ref this);
						return;
					}
					goto IL_561;
				}
				IL_440:
				taskAwaiter7.GetResult();
				IL_447:
				yieldAwaiter = Task.Yield().GetAwaiter();
				if (!yieldAwaiter.IsCompleted)
				{
					num2 = 6;
					YieldAwaitable.YieldAwaiter yieldAwaiter2 = yieldAwaiter;
					this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<Start>d__20>(ref yieldAwaiter, ref this);
					return;
				}
				IL_49D:
				yieldAwaiter.GetResult();
				goto IL_447;
				IL_561:
				taskAwaiter7.GetResult();
				taskAwaiter7 = legalAgreements.WaitForAcknowledgement().GetAwaiter();
				if (!taskAwaiter7.IsCompleted)
				{
					num2 = 8;
					TaskAwaiter taskAwaiter8 = taskAwaiter7;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref taskAwaiter7, ref this);
					return;
				}
				IL_5BF:
				taskAwaiter7.GetResult();
				taskAwaiter7 = legalAgreements.FadeGroup(legalAgreements.canvasGroup, 0f, 1f).GetAwaiter();
				if (!taskAwaiter7.IsCompleted)
				{
					num2 = 9;
					TaskAwaiter taskAwaiter8 = taskAwaiter7;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref taskAwaiter7, ref this);
					return;
				}
				IL_62E:
				taskAwaiter7.GetResult();
				agreementResults[screen.playFabKey] = latestVersion;
				latestVersion = null;
				screen = null;
				IL_65F:
				i++;
				IL_66D:
				if (i >= array.Length)
				{
					array = null;
					if (!versionMismatch)
					{
						goto IL_728;
					}
					taskAwaiter7 = legalAgreements.FadeBackgroundColor(Color.black, 1f).GetAwaiter();
					if (!taskAwaiter7.IsCompleted)
					{
						num2 = 10;
						TaskAwaiter taskAwaiter8 = taskAwaiter7;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref taskAwaiter7, ref this);
						return;
					}
				}
				else
				{
					screen = array[i];
					taskAwaiter5 = legalAgreements.GetTitleDataAsync(screen.latestVersionKey).GetAwaiter();
					if (!taskAwaiter5.IsCompleted)
					{
						num2 = 2;
						TaskAwaiter<string> taskAwaiter6 = taskAwaiter5;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<string>, LegalAgreements.<Start>d__20>(ref taskAwaiter5, ref this);
						return;
					}
					goto IL_21D;
				}
				IL_6F4:
				taskAwaiter7.GetResult();
				legalAgreements.cam.cullingMask = legalAgreements.cullingMask;
				legalAgreements.cam.backgroundColor = legalAgreements.originalColor;
				GorillaTagger.Instance.overrideNotInFocus = false;
				IL_728:
				taskAwaiter7 = legalAgreements.SubmitAcceptedAgreements(agreementResults).GetAwaiter();
				if (!taskAwaiter7.IsCompleted)
				{
					num2 = 11;
					TaskAwaiter taskAwaiter8 = taskAwaiter7;
					this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, LegalAgreements.<Start>d__20>(ref taskAwaiter7, ref this);
					return;
				}
				IL_783:
				taskAwaiter7.GetResult();
				Object.Destroy(legalAgreements.transform.parent.gameObject);
			}
			catch (Exception ex)
			{
				num2 = -2;
				agreementResults = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			agreementResults = null;
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
			int num2;
			int num = num2;
			try
			{
				if (num != 0)
				{
					CS$<>8__locals1 = new LegalAgreements.<>c__DisplayClass28_0();
					CS$<>8__locals1.state = 0;
					PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
					{
						Entity = new EntityKey
						{
							Id = PlayFabSettings.staticPlayer.EntityId,
							Type = PlayFabSettings.staticPlayer.EntityType
						},
						FunctionName = "SubmitAcceptedAgreements",
						FunctionParameter = agreements.ToString(),
						GeneratePlayStreamEvent = new bool?(false)
					}, delegate(ExecuteFunctionResult result)
					{
						CS$<>8__locals1.state = 1;
					}, delegate(PlayFabError error)
					{
						CS$<>8__locals1.state = -1;
					}, null, null);
					goto IL_101;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_FA:
				yieldAwaiter.GetResult();
				IL_101:
				if (CS$<>8__locals1.state == 0)
				{
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<SubmitAcceptedAgreements>d__28>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_FA;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				CS$<>8__locals1 = null;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
			CS$<>8__locals1 = null;
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
			int num2;
			int num = num2;
			LegalAgreements legalAgreements = this;
			bool flag;
			try
			{
				TaskAwaiter<bool> taskAwaiter;
				if (num != 0)
				{
					legalAgreements.scrollView.verticalNormalizedPosition = 1f;
					legalAgreements.title.text = asset.title;
					legalAgreements.body.ClearText();
					taskAwaiter = legalAgreements.body.UpdateTextFromPlayFabTitleData(asset.playFabKey, version).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						num2 = 0;
						TaskAwaiter<bool> taskAwaiter2 = taskAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, LegalAgreements.<UpdateText>d__23>(ref taskAwaiter, ref this);
						return;
					}
				}
				else
				{
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
					num2 = -1;
				}
				bool result = taskAwaiter.GetResult();
				if (!result)
				{
					legalAgreements.body.SetText(asset.errorMessage + "\n\nPlease restart the game and try again.");
				}
				flag = result;
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
			int num2;
			int num = num2;
			LegalAgreements legalAgreements = this;
			try
			{
				if (num != 0)
				{
					progress = 0f;
					legalAgreements.progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
					goto IL_1A5;
				}
				YieldAwaitable.YieldAwaiter yieldAwaiter2;
				YieldAwaitable.YieldAwaiter yieldAwaiter = yieldAwaiter2;
				yieldAwaiter2 = default(YieldAwaitable.YieldAwaiter);
				num2 = -1;
				IL_19E:
				yieldAwaiter.GetResult();
				IL_1A5:
				if (progress >= 1f)
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
						progress += Time.deltaTime / legalAgreements.holdTime;
					}
					else
					{
						progress = 0f;
					}
					legalAgreements.progressImage.rectTransform.localScale = new Vector3(Mathf.Clamp01(progress), 1f, 1f);
					yieldAwaiter = Task.Yield().GetAwaiter();
					if (!yieldAwaiter.IsCompleted)
					{
						num2 = 0;
						yieldAwaiter2 = yieldAwaiter;
						this.<>t__builder.AwaitUnsafeOnCompleted<YieldAwaitable.YieldAwaiter, LegalAgreements.<WaitForAcknowledgement>d__22>(ref yieldAwaiter, ref this);
						return;
					}
					goto IL_19E;
				}
			}
			catch (Exception ex)
			{
				num2 = -2;
				this.<>t__builder.SetException(ex);
				return;
			}
			num2 = -2;
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
