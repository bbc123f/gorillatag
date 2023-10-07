using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020001D8 RID: 472
public class LegalAgreements : MonoBehaviour
{
	// Token: 0x06000C39 RID: 3129 RVA: 0x0004A4AF File Offset: 0x000486AF
	private void Awake()
	{
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0004A4B4 File Offset: 0x000486B4
	private void Start()
	{
		LegalAgreements.<Start>d__20 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<LegalAgreements.<Start>d__20>(ref <Start>d__);
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0004A4EC File Offset: 0x000486EC
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

	// Token: 0x06000C3C RID: 3132 RVA: 0x0004A5A0 File Offset: 0x000487A0
	private Task WaitForAcknowledgement()
	{
		LegalAgreements.<WaitForAcknowledgement>d__22 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<LegalAgreements.<WaitForAcknowledgement>d__22>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x0004A5E4 File Offset: 0x000487E4
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

	// Token: 0x06000C3E RID: 3134 RVA: 0x0004A638 File Offset: 0x00048838
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

	// Token: 0x06000C3F RID: 3135 RVA: 0x0004A68C File Offset: 0x0004888C
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

	// Token: 0x06000C40 RID: 3136 RVA: 0x0004A6E0 File Offset: 0x000488E0
	private Task<string> GetTitleDataAsync(string key)
	{
		LegalAgreements.<GetTitleDataAsync>d__26 <GetTitleDataAsync>d__;
		<GetTitleDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetTitleDataAsync>d__.key = key;
		<GetTitleDataAsync>d__.<>1__state = -1;
		<GetTitleDataAsync>d__.<>t__builder.Start<LegalAgreements.<GetTitleDataAsync>d__26>(ref <GetTitleDataAsync>d__);
		return <GetTitleDataAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x0004A724 File Offset: 0x00048924
	private Task<JsonObject> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		LegalAgreements.<GetAcceptedAgreements>d__27 <GetAcceptedAgreements>d__;
		<GetAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder<JsonObject>.Create();
		<GetAcceptedAgreements>d__.agreements = agreements;
		<GetAcceptedAgreements>d__.<>1__state = -1;
		<GetAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<GetAcceptedAgreements>d__27>(ref <GetAcceptedAgreements>d__);
		return <GetAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x0004A768 File Offset: 0x00048968
	private Task SubmitAcceptedAgreements(JsonObject agreements)
	{
		LegalAgreements.<SubmitAcceptedAgreements>d__28 <SubmitAcceptedAgreements>d__;
		<SubmitAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SubmitAcceptedAgreements>d__.agreements = agreements;
		<SubmitAcceptedAgreements>d__.<>1__state = -1;
		<SubmitAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<SubmitAcceptedAgreements>d__28>(ref <SubmitAcceptedAgreements>d__);
		return <SubmitAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x04000F8C RID: 3980
	[SerializeField]
	private ScrollRect scrollView;

	// Token: 0x04000F8D RID: 3981
	[SerializeField]
	private float scrollSpeed = 0.2f;

	// Token: 0x04000F8E RID: 3982
	[SerializeField]
	private float holdTime = 1f;

	// Token: 0x04000F8F RID: 3983
	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	// Token: 0x04000F90 RID: 3984
	[SerializeField]
	private Text title;

	// Token: 0x04000F91 RID: 3985
	[SerializeField]
	private Text acknowledgementPrompt;

	// Token: 0x04000F92 RID: 3986
	[SerializeField]
	private LegalAgreementBodyText body;

	// Token: 0x04000F93 RID: 3987
	[SerializeField]
	private Image progressImage;

	// Token: 0x04000F94 RID: 3988
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x04000F95 RID: 3989
	[SerializeField]
	private Camera cam;

	// Token: 0x04000F96 RID: 3990
	[SerializeField]
	public bool testAgreement;

	// Token: 0x04000F97 RID: 3991
	[SerializeField]
	public bool testSubmitResult;

	// Token: 0x04000F98 RID: 3992
	[SerializeField]
	public bool testFaceButtonPress;

	// Token: 0x04000F99 RID: 3993
	[SerializeField]
	private int cullingMask;

	// Token: 0x04000F9A RID: 3994
	[SerializeField]
	private GameObject UIParent;

	// Token: 0x04000F9B RID: 3995
	private InputDevice leftHandDevice;

	// Token: 0x04000F9C RID: 3996
	private InputDevice rightHandDevice;

	// Token: 0x04000F9D RID: 3997
	private Color camBackgroundColor = Color.black;

	// Token: 0x04000F9E RID: 3998
	private Color originalColor;
}
