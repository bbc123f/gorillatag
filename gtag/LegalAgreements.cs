using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
}
