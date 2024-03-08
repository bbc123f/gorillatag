using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion;
using Fusion.StatsInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FusionGraph : FusionGraphBase
{
	private static Shader Shader
	{
		get
		{
			return Resources.Load<Shader>("FusionGraphShader");
		}
	}

	public FusionGraph.Layouts Layout
	{
		get
		{
			return this._layout;
		}
		set
		{
			this._layout = value;
			this.CalculateLayout();
		}
	}

	public FusionGraph.ShowGraphOptions ShowGraph
	{
		get
		{
			return this._showGraph;
		}
		set
		{
			this._showGraph = value;
			this.CalculateLayout();
			this._layoutDirty = true;
		}
	}

	public bool AlwaysExpandGraph
	{
		get
		{
			return this._alwaysExpandGraph;
		}
		set
		{
			this._alwaysExpandGraph = value;
			this.CalculateLayout();
			this._layoutDirty = true;
		}
	}

	protected override bool TryConnect()
	{
		if (base.TryConnect())
		{
			FusionGraphVisualization visualizationFlags = this._statsBuffer.VisualizationFlags;
			this.DropdownLookup.Clear();
			this._viewDropdown.ClearOptions();
			for (int i = 0; i < 16; i++)
			{
				if ((visualizationFlags & (FusionGraphVisualization)(1 << i)) != FusionGraphVisualization.Auto)
				{
					this.DropdownLookup.Add(1 << i);
					this._viewDropdown.options.Add(new Dropdown.OptionData(FusionStatsUtilities.CachedTelemetryNames[i + 1]));
					if (((1 << i) & (int)this._statsBuffer.DefaultVisualization) != 0)
					{
						this._viewDropdown.value = i - 1;
					}
				}
			}
			this.SetPerText();
			return true;
		}
		return false;
	}

	public FusionGraphVisualization GraphVisualization
	{
		set
		{
			this._graphVisualization = value;
			this.Reset();
		}
	}

	private void Reset()
	{
		this._values = null;
		this._histogram = null;
		this._intensity = null;
		this._min = 0f;
		this._max = 0f;
		this.ResetGraphShader();
	}

	public void Clear()
	{
		if (this._values != null && this._values.Length != 0)
		{
			Array.Clear(this._values, 0, this._values.Length);
			Array.Clear(this._histogram, 0, this._histogram.Length);
			for (int i = 0; i < this._intensity.Length; i++)
			{
				this._intensity[i] = -2f;
			}
			this._min = 0f;
			this._max = 0f;
			this._histoHighestUsedBucketIndex = 0;
			this._histoAvg = 0.0;
			this._histoAvgSampleCount = 0;
		}
	}

	public override void Initialize()
	{
		Dropdown viewDropdown = this._viewDropdown;
		if (viewDropdown != null)
		{
			viewDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownChanged));
		}
		Button avgBttn = this._avgBttn;
		if (avgBttn == null)
		{
			return;
		}
		avgBttn.onClick.AddListener(new UnityAction(this.CyclePer));
	}

	public void OnDropdownChanged(int value)
	{
		this.GraphVisualization = (FusionGraphVisualization)this.DropdownLookup[value];
		this.SetPerText();
	}

	[BehaviourButtonAction("ResetShader", null, null)]
	private void ResetShaderButton()
	{
		this._intensity = new float[200];
		this._values = new float[200];
		for (int i = 0; i < this._values.Length; i++)
		{
			this._values[i] = (float)i / (float)this._values.Length;
			this._intensity[i] = (float)i / 200f;
		}
		this.GraphImg.material.SetFloat("_ZeroCenter", 0.3f);
		this.GraphImg.material.SetFloatArray("_Data", this._values);
		this.GraphImg.material.SetFloatArray("_Intensity", this._intensity);
		this.GraphImg.material.SetInt("_Count", this._values.Length);
	}

	private void ResetGraphShader()
	{
		if (this.GraphImg)
		{
			FusionGraph.ShaderType shaderType = ((base.LocateParentFusionStats() != null) ? ((this._fusionStats.CanvasType == FusionStats.StatCanvasTypes.GameObject) ? FusionGraph.ShaderType.GameObject : FusionGraph.ShaderType.Overlay) : FusionGraph.ShaderType.None);
			this.GraphImg.material = new Material(FusionGraph.Shader);
			this.GraphImg.material.SetColor("_GoodColor", this._fusionStats.GraphColorGood);
			this.GraphImg.material.SetColor("_WarnColor", this._fusionStats.GraphColorWarn);
			this.GraphImg.material.SetColor("_BadColor", this._fusionStats.GraphColorBad);
			this.GraphImg.material.SetColor("_FlagColor", this._fusionStats.GraphColorFlag);
			this.GraphImg.material.SetInt("_ZWrite", (shaderType == FusionGraph.ShaderType.GameObject) ? 1 : 0);
		}
	}

	public override void CyclePer()
	{
		if (this._graphVisualization != FusionGraphVisualization.CountHistogram && this._graphVisualization != FusionGraphVisualization.ValueHistogram)
		{
			base.CyclePer();
			this.SetPerText();
		}
	}

	private void SetPerText()
	{
		if (this.LabelPer == null)
		{
			RectTransform rectTransform = this.LabelAvg.rectTransform.parent.CreateRectTransform("Per", false).SetAnchors(0.3f, 0.7f, 0f, 0.125f).SetOffsets(6f, -6f, 6f, 0f);
			this.LabelPer = rectTransform.AddText("per sample", TextAnchor.LowerCenter, this._fusionStats.FontColor);
		}
		this.LabelPer.text = (((this._graphVisualization == FusionGraphVisualization.ValueHistogram) | (this._graphVisualization == FusionGraphVisualization.CountHistogram)) ? "avg per Sample" : ((this.CurrentPer == Simulation.Statistics.StatsPer.Second) ? "avg per Second" : ((this.CurrentPer == Simulation.Statistics.StatsPer.Tick) ? "avg per Tick" : "avg per Sample")));
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
		FusionGraphVisualization fusionGraphVisualization = ((this._graphVisualization == FusionGraphVisualization.Auto) ? this._statsBuffer.DefaultVisualization : this._graphVisualization);
		if (this._values == null)
		{
			int num = ((fusionGraphVisualization == FusionGraphVisualization.ContinuousTick) ? statsBuffer.Capacity : ((fusionGraphVisualization == FusionGraphVisualization.ValueHistogram) ? (this.StatSourceInfo.HistoBucketCount + 3) : 128));
			this._values = new float[num];
			this._histogram = new float[num];
			this._intensity = new float[num];
		}
		switch (fusionGraphVisualization)
		{
		case FusionGraphVisualization.ContinuousTick:
			this.UpdateContinuousTick(ref statsBuffer);
			return;
		case FusionGraphVisualization.IntermittentTick:
			this.UpdateIntermittentTick(ref statsBuffer);
			return;
		case FusionGraphVisualization.ContinuousTick | FusionGraphVisualization.IntermittentTick:
			break;
		case FusionGraphVisualization.IntermittentTime:
			this.UpdateIntermittentTime(ref statsBuffer);
			return;
		default:
			if (fusionGraphVisualization != FusionGraphVisualization.ValueHistogram)
			{
				return;
			}
			this.UpdateTickValueHistogram(ref statsBuffer);
			break;
		}
	}

	private void UpdateContinuousTick(ref IStatsBuffer data)
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < data.Count; i++)
		{
			float num5 = (float)(this.StatSourceInfo.Multiplier * (double)data.GetSampleAtIndex(i).FloatValue);
			num = Math.Min(num5, num);
			num2 = Math.Max(num5, num2);
			if (i >= this._values.Length)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					base.name,
					" Out of range ",
					i.ToString(),
					" ",
					this._values.Length.ToString(),
					" ",
					data.Count.ToString()
				}));
			}
			num4 = (this._values[i] = num5);
			num3 += num5;
		}
		num3 /= (float)data.Count;
		this.ApplyScaling(ref num, ref num2);
		this.UpdateUiText(num, num2, num3, num4);
	}

	private void UpdateIntermittentTick(ref IStatsBuffer data)
	{
		if (this._cachedValues == null)
		{
			this._cachedValues = new ValueTuple<int, float>[128];
		}
		int num = this._fusionStats.Runner.Simulation.LatestServerState.Tick;
		float num2 = float.MaxValue;
		float num3 = float.MinValue;
		float num4 = 0f;
		float num5 = 0f;
		int num6 = num - 128 + 1;
		int num7 = (num % 128 + 1) % 128;
		int num8 = this._lastCachedTick;
		for (int i = 0; i < data.Count; i++)
		{
			ISampleData sampleAtIndex = data.GetSampleAtIndex(i);
			int tickValue = sampleAtIndex.TickValue;
			if (tickValue < num6)
			{
				num8 = tickValue;
			}
			else if (tickValue <= this._lastCachedTick)
			{
				num8 = tickValue;
			}
			else
			{
				for (int j = num8 + 1; j < tickValue; j++)
				{
					this._cachedValues[j % 128] = new ValueTuple<int, float>(j, 0f);
				}
				this._lastCachedTick = tickValue;
				this._cachedValues[tickValue % 128] = new ValueTuple<int, float>(tickValue, (float)(this.StatSourceInfo.Multiplier * (double)sampleAtIndex.FloatValue));
				num8 = tickValue;
			}
		}
		for (int k = 0; k < 128; k++)
		{
			ValueTuple<int, float> valueTuple = this._cachedValues[(k + num7) % 128];
			float num9 = valueTuple.Item2;
			if (valueTuple.Item1 < num6)
			{
				valueTuple.Item1 = num6 + k;
				num9 = (valueTuple.Item2 = 0f);
			}
			num2 = Math.Min(num9, num2);
			num3 = Math.Max(num9, num3);
			num5 = (this._values[k] = num9);
			num4 += num9;
		}
		float intermittentAverageInfo = this.GetIntermittentAverageInfo(ref data, num4);
		this.ApplyScaling(ref num2, ref num3);
		this.UpdateUiText(num2, num3, intermittentAverageInfo, num5);
	}

	private void UpdateIntermittentTime(ref IStatsBuffer data)
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < data.Count; i++)
		{
			float num5 = (float)(this.StatSourceInfo.Multiplier * (double)data.GetSampleAtIndex(i).FloatValue);
			num = Math.Min(num5, num);
			num2 = Math.Max(num5, num2);
			num4 = (this._values[i] = num5);
			num3 += num5;
		}
		float intermittentAverageInfo = this.GetIntermittentAverageInfo(ref data, num3);
		this.ApplyScaling(ref num, ref num2);
		this.UpdateUiText(num, num2, intermittentAverageInfo, num4);
	}

	private void ApplyScaling(ref float min, ref float max)
	{
		if (min > 0f)
		{
			min = 0f;
		}
		if (max > this._max)
		{
			this._max = max;
		}
		if (min < this._min)
		{
			this._min = min;
		}
		float num = this._max - this._min;
		int i = 0;
		int num2 = this._values.Length;
		while (i < num2)
		{
			float num3 = this._values[i];
			float num4 = ((num3 < 0f) ? (-1f) : ((num3 >= this.ErrorThreshold) ? 1f : ((num3 >= this.WarnThreshold) ? Mathf.Lerp(0.5f, 1f, (num3 - this.WarnThreshold) / (this.ErrorThreshold - this.WarnThreshold)) : 0f)));
			this._intensity[i] = num4;
			this._values[i] = Mathf.Clamp01((num3 - this._min) / num);
			i++;
		}
	}

	private void UpdateUiText(float min, float max, float avg, float last)
	{
		int decimals = this.StatSourceInfo.Decimals;
		if (this.LabelMin)
		{
			this.LabelMin.text = Math.Round((double)min, decimals).ToString();
		}
		if (this.LabelMax)
		{
			this.LabelMax.text = Math.Round((double)max, decimals).ToString();
		}
		if (this.LabelAvg)
		{
			this.LabelAvg.text = Math.Round((double)avg, decimals).ToString();
		}
		if (this.LabelLast)
		{
			this.LabelLast.text = Math.Round((double)last, decimals).ToString();
		}
		if (this.GraphImg && this.GraphImg.enabled)
		{
			this.GraphImg.material.SetFloatArray("_Data", this._values);
			this.GraphImg.material.SetFloatArray("_Intensity", this._intensity);
			this.GraphImg.material.SetFloat("_Count", (float)this._values.Length);
			this.GraphImg.material.SetFloat("_Height", this.Height);
			this.GraphImg.material.SetFloat("_ZeroCenter", (min < 0f) ? (min / (min - max)) : 0f);
		}
		this._min = Mathf.Lerp(this._min, 0f, Time.deltaTime);
		this._max = Mathf.Lerp(this._max, 1f, Time.deltaTime);
	}

	private float GetIntermittentAverageInfo(ref IStatsBuffer data, float sum)
	{
		Simulation.Statistics.StatsPer currentPer = this.CurrentPer;
		if (currentPer == Simulation.Statistics.StatsPer.Tick)
		{
			int tickValue = data.GetSampleAtIndex(0).TickValue;
			float num = (float)this._fusionStats.Runner.Simulation.LatestServerState.Tick;
			return sum / (num - (float)tickValue);
		}
		if (currentPer == Simulation.Statistics.StatsPer.Second)
		{
			float timeValue = data.GetSampleAtIndex(0).TimeValue;
			float num2 = (float)this._fusionStats.Runner.Simulation.LatestServerState.Time;
			return sum / (num2 - timeValue);
		}
		return sum / (float)this._values.Length;
	}

	private void UpdateTickValueHistogram(ref IStatsBuffer data)
	{
		int histoBucketCount = this.StatSourceInfo.HistoBucketCount;
		double histogMaxValue = this.StatSourceInfo.HistogMaxValue;
		if (this._histoStepInverse == 0.0)
		{
			this._histoStepInverse = (double)histoBucketCount / this.StatSourceInfo.HistogMaxValue;
		}
		int tickValue = data.GetSampleAtIndex(data.Count - 1).TickValue;
		SimulationSnapshot latestServerState = this._fusionStats.Runner.Simulation.LatestServerState;
		bool flag = tickValue > 0;
		double num;
		if (flag)
		{
			num = (double)latestServerState.Tick;
			double num2 = (double)tickValue;
			if (num2 < num)
			{
				int num3 = Math.Max((int)num2, (int)this._lastCachedTickTime);
				int num4 = (int)num - num3;
				float num5 = this._histogram[0] + (float)num4;
				this._histogram[0] = num5;
				if (num5 > this._max)
				{
					this._max = num5;
				}
			}
		}
		else
		{
			num = latestServerState.Time;
		}
		Simulation.Statistics.StatSourceInfo statSourceInfo = this.StatSourceInfo;
		double multiplier = statSourceInfo.Multiplier;
		for (int i = data.Count - 1; i >= 0; i--)
		{
			float floatValue = data.GetSampleAtIndex(i).FloatValue;
			ISampleData sampleAtIndex = data.GetSampleAtIndex(i);
			if ((double)(flag ? ((float)sampleAtIndex.TickValue) : sampleAtIndex.TimeValue) <= this._lastCachedTickTime)
			{
				break;
			}
			double num6 = (double)sampleAtIndex.FloatValue * multiplier;
			int num7;
			if (num6 == 0.0)
			{
				num7 = 0;
			}
			else if (num6 == histogMaxValue)
			{
				num7 = histoBucketCount;
			}
			else if (num6 > histogMaxValue)
			{
				num7 = histoBucketCount + 1;
			}
			else
			{
				num7 = (int)(num6 * this._histoStepInverse) + 1;
			}
			double num8 = this._histoAvg * (double)this._histoAvgSampleCount + num6;
			int num9 = this._histoAvgSampleCount + 1;
			this._histoAvgSampleCount = num9;
			this._histoAvg = num8 / (double)num9;
			float num10 = this._histogram[num7] + 1f;
			if (num10 > this._max)
			{
				this._max = num10;
			}
			this._histogram[num7] = num10;
			if (num7 > this._histoHighestUsedBucketIndex)
			{
				this._histoHighestUsedBucketIndex = num7;
			}
		}
		int num11 = 0;
		float num12 = 0f;
		float num13 = (this._max - this._min) * 1.1f;
		int j = 0;
		int num14 = this._histogram.Length;
		while (j < num14)
		{
			float num15 = this._histogram[j];
			this._intensity[j] = 0f;
			if (j != 0 && num15 > num12)
			{
				num12 = num15;
				num11 = j;
			}
			this._values[j] = Mathf.Clamp01((this._histogram[j] - this._min) / num13);
			j++;
		}
		this._intensity[num11] = 2f;
		this._lastCachedTickTime = num;
		if (this.GraphImg && this.GraphImg.enabled)
		{
			this.GraphImg.material.SetFloatArray("_Data", this._values);
			this.GraphImg.material.SetFloatArray("_Intensity", this._intensity);
			this.GraphImg.material.SetFloat("_Count", (float)(this._histoHighestUsedBucketIndex + 1));
			this.GraphImg.material.SetFloat("_Height", this.Height);
		}
		this._min = 0f;
		int decimals = statSourceInfo.Decimals;
		this.LabelMax.text = string.Format("<color=yellow>{0}</color>", Math.Ceiling((double)(num11 + 1) / this._histoStepInverse));
		this.LabelAvg.text = Math.Round(this._histoAvg, decimals).ToString();
		this.LabelMin.text = Math.Floor((double)this._min).ToString();
		this.LabelLast.text = Math.Round((double)(this._histoHighestUsedBucketIndex + 1) / this._histoStepInverse, decimals).ToString();
	}

	public static FusionGraph Create(FusionStats iFusionStats, Simulation.Statistics.StatSourceTypes statSourceType, int statId, RectTransform parentRT)
	{
		Simulation.Statistics.StatSourceInfo description = Simulation.Statistics.GetDescription(statSourceType, statId);
		RectTransform rectTransform = parentRT.CreateRectTransform(description.LongName, false);
		FusionGraph fusionGraph = rectTransform.gameObject.AddComponent<FusionGraph>();
		fusionGraph._fusionStats = iFusionStats;
		fusionGraph.Generate(statSourceType, statId, rectTransform);
		return fusionGraph;
	}

	public void Generate(Simulation.Statistics.StatSourceTypes type, int statId, RectTransform root)
	{
		this._statSourceType = type;
		base.GetComponent<RectTransform>();
		this._statId = statId;
		root.anchorMin = new Vector2(0.5f, 0.5f);
		root.anchorMax = new Vector2(0.5f, 0.5f);
		root.anchoredPosition3D = default(Vector3);
		RectTransform rectTransform = root.CreateRectTransform("Background", false).ExpandAnchor(null);
		this.BackImage = rectTransform.gameObject.AddComponent<Image>();
		this.BackImage.color = this.BackColor;
		this.BackImage.raycastTarget = false;
		RectTransform rectTransform2 = rectTransform.CreateRectTransform("Graph", false).SetAnchors(0f, 1f, 0.2f, 0.8f).SetOffsets(0f, 0f, 0f, 0f);
		this.GraphImg = rectTransform2.gameObject.AddComponent<Image>();
		this.GraphImg.raycastTarget = false;
		this.ResetGraphShader();
		Color fontColor = this._fusionStats.FontColor;
		Color color = this._fusionStats.FontColor * new Color(1f, 1f, 1f, 0.5f);
		RectTransform rectTransform3 = root.CreateRectTransform("Title", false).ExpandAnchor(null).SetOffsets(10f, -10f, 0f, -2f);
		rectTransform3.anchoredPosition = new Vector2(0f, 0f);
		this.LabelTitle = rectTransform3.AddText(base.name, TextAnchor.UpperRight, fontColor);
		this.LabelTitle.resizeTextMaxSize = 24;
		this.LabelTitle.raycastTarget = true;
		RectTransform rectTransform4 = root.CreateRectTransform("Max", false).SetAnchors(0f, 0.3f, 0.85f, 1f).SetOffsets(6f, 0f, 0f, -2f);
		this.LabelMax = rectTransform4.AddText("-", TextAnchor.UpperLeft, color);
		RectTransform rectTransform5 = root.CreateRectTransform("Min", false).SetAnchors(0f, 0.3f, 0f, 0.15f).SetOffsets(6f, 0f, 0f, -2f);
		this.LabelMin = rectTransform5.AddText("-", TextAnchor.LowerLeft, color);
		RectTransform rectTransform6 = root.CreateRectTransform("Avg", false).SetOffsets(0f, 0f, 0f, 0f);
		rectTransform6.anchoredPosition = new Vector2(0f, 0f);
		this.LabelAvg = rectTransform6.AddText("-", TextAnchor.LowerCenter, fontColor);
		this.LabelAvg.raycastTarget = true;
		this._avgBttn = rectTransform6.gameObject.AddComponent<Button>();
		RectTransform rectTransform7 = root.CreateRectTransform("Per", false).SetAnchors(0.3f, 0.7f, 0f, 0.125f).SetOffsets(6f, -6f, 6f, 0f);
		this.LabelPer = rectTransform7.AddText("avg per Sample", TextAnchor.LowerCenter, fontColor);
		RectTransform rectTransform8 = root.CreateRectTransform("Last", false).SetAnchors(0.7f, 1f, 0f, 0.15f).SetOffsets(10f, -10f, 0f, -2f);
		this.LabelLast = rectTransform8.AddText("-", TextAnchor.LowerRight, color);
		this._viewDropdown = rectTransform3.CreateDropdown(10f, fontColor);
		this._layoutDirty = true;
	}

	[BehaviourButtonAction("Update Layout", null, null)]
	public override void CalculateLayout()
	{
		try
		{
			if (base.gameObject == null)
			{
				return;
			}
		}
		catch
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this._layoutDirty = false;
		RectTransform component = base.GetComponent<RectTransform>();
		if (this._statsBuffer == null)
		{
			this.TryConnect();
		}
		base.ApplyTitleText();
		bool flag = this.StatSourceInfo.InvalidReason == null;
		this.LabelMin.gameObject.SetActive(flag);
		this.LabelMax.gameObject.SetActive(flag);
		this.LabelAvg.gameObject.SetActive(flag);
		this.LabelPer.gameObject.SetActive(flag);
		if (!flag)
		{
			this.LabelTitle.rectTransform.ExpandAnchor(new float?((float)10));
			this.LabelTitle.alignment = TextAnchor.MiddleCenter;
			this.LabelTitle.raycastTarget = false;
			this._viewDropdown.gameObject.SetActive(false);
			return;
		}
		this.GraphImg.material.SetInt("_ZWrite", (this._fusionStats.CanvasType == FusionStats.StatCanvasTypes.GameObject) ? 1 : 0);
		bool flag2 = this._fusionStats.CanvasType == FusionStats.StatCanvasTypes.GameObject;
		bool noGraphShader = this._fusionStats.NoGraphShader;
		float height = component.rect.height;
		float width = component.rect.width;
		FusionGraph.Layouts layouts;
		if (this._layout != FusionGraph.Layouts.Auto)
		{
			layouts = this._layout;
		}
		else if (this._fusionStats.DefaultLayout != FusionGraph.Layouts.Auto)
		{
			layouts = this._fusionStats.DefaultLayout;
		}
		else if (height < 52f)
		{
			layouts = FusionGraph.Layouts.CompactAuto;
		}
		else if (width < 200f)
		{
			layouts = FusionGraph.Layouts.CenteredAuto;
		}
		else
		{
			layouts = FusionGraph.Layouts.FullAuto;
		}
		bool flag3 = noGraphShader || layouts == FusionGraph.Layouts.CompactNoGraph || layouts == FusionGraph.Layouts.CenteredNoGraph || (this._fusionStats.NoTextOverlap && layouts == FusionGraph.Layouts.CompactAuto);
		bool flag4 = this._fusionStats.NoTextOverlap || layouts == FusionGraph.Layouts.FullNoOverlap || layouts == FusionGraph.Layouts.CenteredNoOverlap;
		bool flag5 = !flag3 && (this.ShowGraph == FusionGraph.ShowGraphOptions.Always || (this.ShowGraph == FusionGraph.ShowGraphOptions.OverlayOnly && flag2));
		bool flag6 = !flag4 && (this._alwaysExpandGraph || !flag5 || layouts == FusionGraph.Layouts.CompactAuto || (!flag4 && height < 112f));
		bool flag7 = height < 18f;
		RectTransform rectTransform = this.GraphImg.rectTransform;
		if (rectTransform)
		{
			rectTransform.gameObject.SetActive(flag5);
			if (flag6)
			{
				rectTransform.SetAnchors(0f, 1f, 0f, 1f);
			}
			else
			{
				rectTransform.SetAnchors(0f, 1f, 0.25f, 0.8f);
			}
		}
		bool flag8 = layouts == FusionGraph.Layouts.FullAuto || layouts == FusionGraph.Layouts.FullNoOverlap;
		RectTransform rectTransform2 = this.LabelTitle.rectTransform;
		RectTransform rectTransform3 = this.LabelAvg.rectTransform;
		if (this.LabelPer == null)
		{
			RectTransform rectTransform4 = rectTransform3.parent.CreateRectTransform("Per", false).SetAnchors(0.3f, 0.7f, 0f, 0.125f).SetOffsets(6f, -6f, 6f, 0f);
			this.LabelPer = rectTransform4.AddText("per sample", TextAnchor.LowerCenter, this._fusionStats.FontColor);
		}
		RectTransform rectTransform5 = this.LabelPer.rectTransform;
		switch (layouts)
		{
		case FusionGraph.Layouts.FullAuto:
		case FusionGraph.Layouts.FullNoOverlap:
			rectTransform2.anchorMin = new Vector2(flag8 ? 0.3f : 0f, flag6 ? 0.5f : 0.8f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.offsetMin = new Vector2(6f, 0f);
			rectTransform2.offsetMax = new Vector2(-6f, -6f);
			this.LabelTitle.alignment = (flag8 ? TextAnchor.UpperRight : TextAnchor.UpperCenter);
			rectTransform3.anchorMin = new Vector2(flag8 ? 0.3f : 0f, flag6 ? 0.15f : 0.1f);
			rectTransform3.anchorMax = new Vector2(flag8 ? 0.7f : 1f, flag6 ? 0.5f : 0.25f);
			rectTransform3.SetOffsets(0f, 0f, 0f, 0f);
			this.LabelAvg.alignment = TextAnchor.LowerCenter;
			rectTransform5.SetAnchors(0.3f, 0.7f, 0f, flag6 ? 0.2f : 0.1f);
			this.LabelPer.alignment = TextAnchor.LowerCenter;
			break;
		case FusionGraph.Layouts.CenteredAuto:
		case FusionGraph.Layouts.CenteredNoGraph:
		case FusionGraph.Layouts.CenteredNoOverlap:
			rectTransform2.anchorMin = new Vector2(0f, flag6 ? 0.5f : 0.8f);
			rectTransform2.anchorMax = new Vector2(1f, 1f);
			rectTransform2.offsetMin = new Vector2(6f, 0f);
			rectTransform2.offsetMax = new Vector2(-6f, -6f);
			this.LabelTitle.alignment = TextAnchor.UpperCenter;
			rectTransform3.anchorMin = new Vector2(0f, flag6 ? 0.15f : 0.1f);
			rectTransform3.anchorMax = new Vector2(1f, flag6 ? 0.5f : 0.25f);
			rectTransform3.SetOffsets(6f, -6f, 0f, 0f);
			rectTransform5.SetAnchors(0f, 1f, 0f, flag6 ? 0.2f : 0.1f);
			this.LabelPer.alignment = TextAnchor.LowerCenter;
			this.LabelAvg.alignment = TextAnchor.LowerCenter;
			break;
		case FusionGraph.Layouts.CompactAuto:
		case FusionGraph.Layouts.CompactNoGraph:
			rectTransform2.anchorMin = new Vector2(0.05f, 0f);
			rectTransform2.anchorMax = new Vector2(0.5f, 1f);
			if (flag7)
			{
				rectTransform2.SetOffsets(0f, 0f, 0f, 0f);
				rectTransform3.SetOffsets(6f, 0f, 0f, 0f);
			}
			else
			{
				rectTransform2.SetOffsets(0f, 0f, 6f, -6f);
				rectTransform3.SetOffsets(6f, 0f, 6f, -6f);
			}
			this.LabelTitle.alignment = TextAnchor.MiddleLeft;
			rectTransform3.SetAnchors(0.5f, 0.95f, 0f, 1f);
			rectTransform5.SetAnchors(0.5f, 0.95f, 0f, 0.15f).SetOffsets(6f, -12f, 6f, 0f);
			this.LabelPer.alignment = TextAnchor.LowerRight;
			this.LabelAvg.alignment = TextAnchor.MiddleRight;
			break;
		}
		this.LabelMin.enabled = flag8;
		this.LabelMax.enabled = flag8;
		this.LabelLast.enabled = flag8;
	}

	private const int GRPH_TOP_PAD = 36;

	private const int GRPH_BTM_PAD = 36;

	private const int HIDE_XTRAS_WDTH = 200;

	private const int INTERMITTENT_DATA_ARRAYSIZE = 128;

	private const int EXPAND_GRPH_THRESH = 112;

	private const int COMPACT_THRESH = 52;

	[SerializeField]
	[HideInInspector]
	public float Height = 50f;

	[InlineHelp]
	[SerializeField]
	[Header("Graph Layout")]
	private FusionGraph.Layouts _layout;

	[InlineHelp]
	[SerializeField]
	private FusionGraph.ShowGraphOptions _showGraph = FusionGraph.ShowGraphOptions.Always;

	[InlineHelp]
	public float Padding = 5f;

	[InlineHelp]
	[SerializeField]
	private bool _alwaysExpandGraph;

	[InlineHelp]
	[SerializeField]
	private bool _showUITargets;

	[DrawIf("_showUITargets", Hide = true)]
	public Image GraphImg;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelMin;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelMax;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelAvg;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelLast;

	[DrawIf("_showUITargets", Hide = true)]
	public Text LabelPer;

	[DrawIf("_showUITargets", Hide = true)]
	public Dropdown _viewDropdown;

	[DrawIf("_showUITargets", Hide = true)]
	public Button _avgBttn;

	private float _min;

	private float _max;

	private float[] _values;

	private float[] _intensity;

	private float[] _histogram;

	private List<int> DropdownLookup = new List<int>();

	[InlineHelp]
	private FusionGraphVisualization _graphVisualization;

	private FusionGraph.ShaderType _currentShader;

	[TupleElementNames(new string[] { "tick", "value" })]
	private ValueTuple<int, float>[] _cachedValues;

	private double _lastCachedTickTime;

	private int _lastCachedTick;

	private int _histoHighestUsedBucketIndex;

	private int _histoAvgSampleCount;

	private double _histoStepInverse;

	private double _histoAvg;

	public enum Layouts
	{
		Auto,
		FullAuto,
		FullNoOverlap,
		CenteredAuto,
		CenteredNoGraph,
		CenteredNoOverlap,
		CompactAuto,
		CompactNoGraph
	}

	public enum ShowGraphOptions
	{
		Never,
		OverlayOnly,
		Always
	}

	private enum ShaderType
	{
		None,
		Overlay,
		GameObject
	}
}
