using System.Linq;
using System.Threading.Tasks;
using GorillaNetworking;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class LegalAgreements : MonoBehaviour
{
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

	private void Awake()
	{
	}

	private async void Start()
	{
		cam = Camera.main;
		originalColor = cam.backgroundColor;
		canvasGroup.alpha = 0f;
		cam.backgroundColor = Color.black;
		progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		cullingMask = cam.cullingMask;
		while (!PlayFabClientAPI.IsClientLoggedIn())
		{
			await Task.Yield();
		}
		bool versionMismatch = false;
		JsonObject agreementResults = await GetAcceptedAgreements(legalAgreementScreens);
		LegalAgreementTextAsset[] array = legalAgreementScreens;
		foreach (LegalAgreementTextAsset screen in array)
		{
			string latestVersion2 = await GetTitleDataAsync(screen.latestVersionKey);
			latestVersion2 = latestVersion2.Substring(1, latestVersion2.Length - 2);
			object value = string.Empty;
			bool flag = agreementResults?.TryGetValue(screen.playFabKey, out value) ?? false;
			if (!testAgreement && flag && latestVersion2 == value.ToString())
			{
				continue;
			}
			if (!versionMismatch)
			{
				cam.cullingMask = LayerMask.GetMask("UI");
				cam.backgroundColor = camBackgroundColor;
				await FadeBackgroundColor(camBackgroundColor, 1f);
				versionMismatch = true;
				GorillaTagger.Instance.overrideNotInFocus = true;
				UIParent.SetActive(value: true);
			}
			if (!(await UpdateText(screen, latestVersion2)))
			{
				Object.Destroy(acknowledgementPrompt);
				await FadeGroup(canvasGroup, 1f, 1f);
				while (true)
				{
					await Task.Yield();
				}
			}
			base.transform.parent.eulerAngles = new Vector3(0f, cam.transform.rotation.y, 0f);
			base.transform.parent.position = cam.transform.position;
			await FadeGroup(canvasGroup, 1f, 1f);
			await WaitForAcknowledgement();
			await FadeGroup(canvasGroup, 0f, 1f);
			agreementResults[screen.playFabKey] = latestVersion2;
		}
		if (versionMismatch)
		{
			await FadeBackgroundColor(Color.black, 1f);
			cam.cullingMask = cullingMask;
			cam.backgroundColor = originalColor;
			GorillaTagger.Instance.overrideNotInFocus = false;
		}
		await SubmitAcceptedAgreements(agreementResults);
		Object.Destroy(base.transform.parent.gameObject);
	}

	private void Update()
	{
		float num = 0f;
		leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		leftHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var value);
		rightHandDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var value2);
		num = Mathf.Clamp(value.y + value2.y, -1f, 1f);
		scrollView.verticalNormalizedPosition += num * (scrollSpeed / body.Height) * Time.deltaTime;
	}

	private async Task WaitForAcknowledgement()
	{
		float progress = 0f;
		progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		while (progress < 1f)
		{
			leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
			leftHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var value);
			leftHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var value2);
			rightHandDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var value3);
			rightHandDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out var value4);
			bool flag = value || value2 || value3 || value4;
			progress = ((!(testFaceButtonPress || flag)) ? 0f : (progress + Time.deltaTime / holdTime));
			progressImage.rectTransform.localScale = new Vector3(Mathf.Clamp01(progress), 1f, 1f);
			await Task.Yield();
		}
		progressImage.rectTransform.localScale = new Vector3(0f, 1f, 1f);
	}

	private async Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		scrollView.verticalNormalizedPosition = 1f;
		title.text = asset.title;
		body.ClearText();
		bool num = await body.UpdateTextFromPlayFabTitleData(asset.playFabKey, version);
		if (!num)
		{
			body.SetText(asset.errorMessage + "\n\nPlease restart the game and try again.");
		}
		return num;
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
		cam.backgroundColor = Color.black;
		float t = 0f;
		Color startColor = cam.backgroundColor;
		while (t < 1f)
		{
			t += Time.deltaTime / time;
			cam.backgroundColor = Color.Lerp(startColor, targetColor, t);
			await Task.Yield();
		}
		cam.backgroundColor = targetColor;
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
		string[] value = agreements.Select((LegalAgreementTextAsset x) => x.playFabKey).ToArray();
		PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
		{
			Entity = new EntityKey
			{
				Id = PlayFabSettings.staticPlayer.EntityId,
				Type = PlayFabSettings.staticPlayer.EntityType
			},
			FunctionName = "GetAcceptedAgreements",
			FunctionParameter = string.Join(",", value),
			GeneratePlayStreamEvent = false
		}, delegate(ExecuteFunctionResult result)
		{
			state = 1;
			returnValue = result.FunctionResult as JsonObject;
		}, delegate(PlayFabError error)
		{
			Debug.LogError(error.ErrorMessage);
			state = -1;
		});
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
			GeneratePlayStreamEvent = false
		}, delegate
		{
			state = 1;
		}, delegate
		{
			state = -1;
		});
		while (state == 0)
		{
			await Task.Yield();
		}
	}
}
