using System;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.UI;

public class FusionStatsObjectIds : Fusion.Behaviour, IFusionStatsView
{
	private void Awake()
	{
		this._fusionStats = base.GetComponentInParent<FusionStats>();
	}

	void IFusionStatsView.Initialize()
	{
	}

	public static RectTransform Create(RectTransform parent, FusionStats fusionStats)
	{
		RectTransform rectTransform = parent.CreateRectTransform("Object Ids Panel", false).ExpandTopAnchor(new float?((float)6));
		FusionStatsObjectIds fusionStatsObjectIds = rectTransform.gameObject.AddComponent<FusionStatsObjectIds>();
		fusionStatsObjectIds._fusionStats = fusionStats;
		fusionStatsObjectIds.Generate();
		return rectTransform;
	}

	public void Generate()
	{
		Color fontColor = this._fusionStats.FontColor;
		RectTransform rectTransform = base.transform.CreateRectTransform("IDs Layout", false).ExpandAnchor(null).AddCircleSprite(this._fusionStats.ObjDataBackColor);
		RectTransform rectTransform2 = rectTransform.CreateRectTransform("Object Id Panel", true).ExpandTopAnchor(null).SetAnchors(0f, 0.4f, 0f, 1f);
		rectTransform2.CreateRectTransform("Object Id Label", false).SetAnchors(0f, 1f, 0.7f, 1f).SetOffsets(6f, -6f, 0f, -4f)
			.AddText("OBJECT ID", TextAnchor.MiddleCenter, fontColor)
			.resizeTextMaxSize = 18;
		RectTransform rectTransform3 = rectTransform2.CreateRectTransform("Object Id Value", false).SetAnchors(0f, 1f, 0f, 0.7f).SetOffsets(6f, -6f, 4f, 0f);
		this._objectIdLabel = rectTransform3.AddText("00", TextAnchor.MiddleCenter, fontColor);
		this.AddAuthorityPanel(rectTransform, "Input", ref this._inputValueText, ref this._inputAuthBackImage).SetAnchors(0.4f, 0.7f, 0f, 1f);
		this.AddAuthorityPanel(rectTransform, "State", ref this._stateValueText, ref this._stateAuthBackImage).SetAnchors(0.7f, 1f, 0f, 1f);
	}

	private RectTransform AddAuthorityPanel(RectTransform parent, string label, ref Text valueText, ref Image backImage)
	{
		Color fontColor = this._fusionStats.FontColor;
		RectTransform rectTransform = parent.CreateRectTransform(label + " Id Panel", true).ExpandTopAnchor(null).SetAnchors(0.5f, 1f, 0f, 1f)
			.AddCircleSprite(FusionStatsObjectIds._noneAuthColor, out backImage);
		rectTransform.CreateRectTransform(label + " Label", false).SetAnchors(0f, 1f, 0.7f, 1f).SetOffsets(6f, -6f, 0f, -4f)
			.AddText(label, TextAnchor.MiddleCenter, fontColor)
			.resizeTextMaxSize = 18;
		RectTransform rectTransform2 = rectTransform.CreateRectTransform(label + " Value", false).SetAnchors(0f, 1f, 0f, 0.7f).SetOffsets(6f, -6f, 4f, 0f);
		valueText = rectTransform2.AddText("P0", TextAnchor.MiddleCenter, fontColor);
		return rectTransform;
	}

	void IFusionStatsView.CalculateLayout()
	{
	}

	void IFusionStatsView.Refresh()
	{
		if (this._fusionStats == null)
		{
			return;
		}
		NetworkObject @object = this._fusionStats.Object;
		if (@object == null)
		{
			return;
		}
		if (@object.IsValid)
		{
			bool hasInputAuthority = @object.HasInputAuthority;
			if (this._previousHasInputAuth != hasInputAuthority)
			{
				this._inputAuthBackImage.color = (hasInputAuthority ? FusionStatsObjectIds._inputAuthColor : FusionStatsObjectIds._noneAuthColor);
				this._previousHasInputAuth = hasInputAuthority;
			}
			bool flag = @object.HasStateAuthority || @object.Runner.IsServer;
			if (this._previousHasStateAuth != flag)
			{
				this._stateAuthBackImage.color = (flag ? FusionStatsObjectIds._stateAuthColor : FusionStatsObjectIds._noneAuthColor);
				this._previousHasStateAuth = flag;
			}
			PlayerRef inputAuthority = @object.InputAuthority;
			if (this._previousInputAuthValue != inputAuthority)
			{
				this._inputValueText.text = ((inputAuthority == -1) ? "-" : ("P" + inputAuthority.PlayerId.ToString()));
				this._previousInputAuthValue = inputAuthority;
			}
			PlayerRef stateAuthority = @object.StateAuthority;
			if (this._previousStateAuthValue != stateAuthority)
			{
				this._stateValueText.text = ((stateAuthority == -1) ? "-" : ("P" + stateAuthority.PlayerId.ToString()));
				this._previousStateAuthValue = stateAuthority;
			}
		}
		uint raw = @object.Id.Raw;
		if (raw != this._previousObjectIdValue)
		{
			this._objectIdLabel.text = raw.ToString();
			this._previousObjectIdValue = raw;
		}
	}

	bool IFusionStatsView.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	Transform IFusionStatsView.get_transform()
	{
		return base.transform;
	}

	protected const int PAD = 10;

	protected const int MARGIN = 6;

	[SerializeField]
	[HideInInspector]
	private Text _inputValueText;

	[SerializeField]
	[HideInInspector]
	private Text _stateValueText;

	[SerializeField]
	[HideInInspector]
	private Text _objectIdLabel;

	[SerializeField]
	[HideInInspector]
	private Image _stateAuthBackImage;

	[SerializeField]
	[HideInInspector]
	private Image _inputAuthBackImage;

	private FusionStats _fusionStats;

	private static Color _noneAuthColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);

	private static Color _inputAuthColor = new Color(0.1f, 0.6f, 0.1f, 1f);

	private static Color _stateAuthColor = new Color(0.8f, 0.4f, 0f, 1f);

	private const float LABEL_DIVIDING_POINT = 0.7f;

	private const float TEXT_PAD = 4f;

	private const float TEXT_PAD_HORIZ = 6f;

	private const int MAX_TAG_FONT_SIZE = 18;

	private bool _previousHasInputAuth;

	private bool _previousHasStateAuth;

	private int _previousInputAuthValue = -2;

	private int _previousStateAuthValue = -2;

	private uint _previousObjectIdValue;
}
