using System;

[Serializable]
public class ScenePerformanceData
{
	public ScenePerformanceData(string mapName, int msHigh, int medianMS, int averageMS, int fpsLow, int medianFPS, int averageFPS)
	{
		this.mapName = mapName;
		this.msHigh = msHigh;
		this.medianMS = medianMS;
		this.averageMS = averageMS;
		this.fpsLow = fpsLow;
		this.medianFPS = medianFPS;
		this.averageFPS = averageFPS;
	}

	public string mapName;

	public int msHigh;

	public int medianMS;

	public int averageMS;

	public int fpsLow;

	public int medianFPS;

	public int averageFPS;
}
