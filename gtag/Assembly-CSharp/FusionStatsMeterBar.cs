using System;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.UI;

public class FusionStatsMeterBar : FusionGraphBase
{
	protected override Color BackColor
	{
		get
		{
			return base.BackColor * new Color(0.5f, 0.5f, 0.5f, 1f);
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		this._max = (double)this.MeterMax;
		if (this.BackImage.sprite == null)
		{
			this.BackImage.sprite = FusionStatsUtilities.MeterSprite;
			this.Bar.sprite = this.BackImage.sprite;
		}
		this.BackImage.type = Image.Type.Simple;
		if (this.Bar.rectTransform.parent != this.BackImage.rectTransform.parent)
		{
			Transform parent = this.Bar.transform.parent;
			this.Bar.rectTransform.SetParent(this.BackImage.rectTransform.parent);
			this.Bar.transform.SetSiblingIndex(this.BackImage.transform.GetSiblingIndex() + 1);
		}
		this.Bar.type = Image.Type.Filled;
		this.Bar.fillMethod = Image.FillMethod.Horizontal;
		this.Bar.fillAmount = 0f;
	}

	public override void Refresh()
	{
		if (this._layoutDirty)
		{
			this.CalculateLayout();
		}
		IStatsBuffer statsBuffer = base.StatsBuffer;
		if (statsBuffer == null || statsBuffer.Count < 1)
		{
			return;
		}
		if (statsBuffer.DefaultVisualization == FusionGraphVisualization.CountHistogram)
		{
			if (statsBuffer.Count > 0)
			{
				int num = 0;
				float num2 = (float)statsBuffer.GetSampleAtIndex(statsBuffer.Count - 1).TickValue;
				float num3 = num2;
				if ((double)num2 > this._lastImportedSampleTickTime)
				{
					int num4 = 0;
					for (int i = statsBuffer.Count - 1; i >= 0; i--)
					{
						int tickValue = statsBuffer.GetSampleAtIndex(i).TickValue;
						if ((double)tickValue <= this._lastImportedSampleTickTime)
						{
							break;
						}
						if ((float)tickValue != num3)
						{
							num3 = (float)tickValue;
							if (num4 > num)
							{
								num = num4;
							}
							num4 = 0;
						}
						num4++;
						this._total += 1.0;
					}
					this._lastImportedSampleTickTime = (double)num2;
				}
				this.SetValue((double)num);
			}
			return;
		}
		if (statsBuffer.Count > 0)
		{
			ISampleData sampleAtIndex = statsBuffer.GetSampleAtIndex(statsBuffer.Count - 1);
			if (sampleAtIndex.TickValue == this._fusionStats.Runner.Simulation.LatestServerState.Tick)
			{
				this.SetValue((double)sampleAtIndex.FloatValue);
				return;
			}
			this.SetValue(0.0);
		}
	}

	public void LateUpdate()
	{
		if (this.DecayTime <= 0f)
		{
			return;
		}
		if (this._currentBarValue <= 0.0)
		{
			return;
		}
		if (Time.time < this._lastPeakSetTime + this.HoldPeakTime)
		{
			return;
		}
		double bar = Math.Max(this._currentBarValue - (double)(Time.deltaTime / this.DecayTime) * this._max, 0.0);
		this.SetBar(bar);
	}

	public void SetValue(double rawvalue)
	{
		Simulation.Statistics.StatSourceInfo statSourceInfo = this.StatSourceInfo;
		double num = rawvalue * statSourceInfo.Multiplier;
		if (this.MeterMax == 0 && num > this._max)
		{
			this._max = num;
		}
		double num2 = Math.Max(Math.Min(num, this._max), 0.0);
		double num3 = Math.Round(num2, statSourceInfo.Decimals);
		double num4 = (this._total > 0.0) ? this._total : num3;
		if (num2 >= this._currentBarValue)
		{
			this._lastPeakSetTime = Time.time;
		}
		if (num4 != this._currentDisplayValue)
		{
			this.ValueLabel.text = ((this._total > 0.0) ? this._total.ToString() : num2.ToString());
			this._currentDisplayValue = num4;
		}
		if (this.DecayTime >= 0f && num2 <= this._currentBarValue)
		{
			return;
		}
		if (num2 != this._currentBarValue)
		{
			this.SetBar(num2);
		}
	}

	private void SetBar(double value)
	{
		FusionStats fusionStats = this._fusionStats;
		this.Bar.fillAmount = (float)(value / this._max);
		this._currentBarValue = value;
		if (value < (double)this.WarnThreshold)
		{
			Color graphColorGood = fusionStats.GraphColorGood;
			if (this.CurrentColor != graphColorGood)
			{
				this.CurrentColor = graphColorGood;
				this.Bar.color = graphColorGood;
				return;
			}
		}
		else if (value < (double)this.ErrorThreshold)
		{
			Color graphColorWarn = fusionStats.GraphColorWarn;
			if (this.CurrentColor != graphColorWarn)
			{
				this.Bar.color = graphColorWarn;
				this.CurrentColor = graphColorWarn;
				return;
			}
		}
		else
		{
			Color graphColorBad = fusionStats.GraphColorBad;
			if (this.CurrentColor != graphColorBad)
			{
				this.Bar.color = graphColorBad;
				this.CurrentColor = graphColorBad;
			}
		}
	}

	public override void CalculateLayout()
	{
		this._layoutDirty = false;
		float num = this.LabelTitle.transform.parent.GetComponent<RectTransform>().rect.height * 0.2f;
		this.LabelTitle.rectTransform.offsetMax = new Vector2(0f, -num);
		this.LabelTitle.rectTransform.offsetMin = new Vector2(10f, num * 1.2f);
		this.ValueLabel.rectTransform.offsetMax = new Vector2(-10f, -num);
		this.ValueLabel.rectTransform.offsetMin = new Vector2(0f, num * 1.2f);
		base.ApplyTitleText();
	}

	public static FusionStatsMeterBar Create(RectTransform parent, FusionStats fusionStats, Simulation.Statistics.StatSourceTypes statSourceType, int statId, float warnThreshold, float alertThreshold)
	{
		Simulation.Statistics.StatSourceInfo description = Simulation.Statistics.GetDescription(statSourceType, statId);
		FusionStatsMeterBar fusionStatsMeterBar = parent.CreateRectTransform(description.LongName, true).gameObject.AddComponent<FusionStatsMeterBar>();
		fusionStatsMeterBar.StatSourceInfo = description;
		fusionStatsMeterBar._fusionStats = fusionStats;
		fusionStatsMeterBar._statSourceType = statSourceType;
		fusionStatsMeterBar._statId = statId;
		fusionStatsMeterBar.GenerateMeter();
		return fusionStatsMeterBar;
	}

	public void GenerateMeter()
	{
		Simulation.Statistics.StatSourceInfo description = Simulation.Statistics.GetDescription(this._statSourceType, this._statId);
		RectTransform rectTransform = base.transform.CreateRectTransform("Back", true);
		this.BackImage = rectTransform.gameObject.AddComponent<Image>();
		this.BackImage.raycastTarget = false;
		this.BackImage.sprite = FusionStatsUtilities.MeterSprite;
		this.BackImage.color = this.BackColor;
		this.BackImage.type = Image.Type.Simple;
		RectTransform rectTransform2 = base.transform.CreateRectTransform("Bar", true);
		this.Bar = rectTransform2.gameObject.AddComponent<Image>();
		this.Bar.raycastTarget = false;
		this.Bar.sprite = this.BackImage.sprite;
		this.Bar.color = this._fusionStats.GraphColorGood;
		this.Bar.type = Image.Type.Filled;
		this.Bar.fillMethod = Image.FillMethod.Horizontal;
		this.Bar.fillAmount = 0f;
		RectTransform rt = base.transform.CreateRectTransform("Label", true).ExpandAnchor(null).SetAnchors(0f, 0.5f, 0f, 1f).SetOffsets(6f, -6f, 6f, -6f);
		this.LabelTitle = rt.AddText(description.LongName, TextAnchor.MiddleLeft, this._fusionStats.FontColor);
		this.LabelTitle.alignByGeometry = false;
		RectTransform rt2 = base.transform.CreateRectTransform("Value", true).ExpandAnchor(null).SetAnchors(0.5f, 1f, 0f, 1f).SetOffsets(6f, -6f, 6f, -6f);
		this.ValueLabel = rt2.AddText("0.0", TextAnchor.MiddleRight, this._fusionStats.FontColor);
		this.ValueLabel.alignByGeometry = false;
	}

	public FusionStatsMeterBar()
	{
	}

	public float HoldPeakTime = 0.1f;

	public float DecayTime = 0.25f;

	[InlineHelp]
	public int MeterMax;

	[InlineHelp]
	[SerializeField]
	private bool _showUITargets;

	[DrawIf("_showUITargets", Hide = true)]
	public Text ValueLabel;

	[DrawIf("_showUITargets", Hide = true)]
	public Image Bar;

	private double _currentDisplayValue;

	private double _currentBarValue;

	private Color CurrentColor;

	private double _lastImportedSampleTickTime;

	private double _max;

	private double _total;

	private float _lastPeakSetTime;
}
