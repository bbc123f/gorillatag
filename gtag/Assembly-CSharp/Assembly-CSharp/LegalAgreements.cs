using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
