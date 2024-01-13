using System.Collections.Generic;
using UnityEngine;

namespace GorillaLocomotion.Climbing;

public class GorillaVelocityTracker : MonoBehaviour
{
	public class VelocityDataPoint
	{
		public Vector3 delta;

		public float time = -1f;
	}

	[SerializeField]
	private int maxDataPoints = 20;

	[SerializeField]
	private Transform relativeTo;

	private int currentDataPointIndex;

	private VelocityDataPoint[] localSpaceData;

	private VelocityDataPoint[] worldSpaceData;

	private Transform trans;

	private Vector3 lastWorldSpacePos;

	private Vector3 lastLocalSpacePos;

	private bool isRelativeTo;

	private int lastTickedFrame = -1;

	public void ResetState()
	{
		trans = base.transform;
		localSpaceData = new VelocityDataPoint[maxDataPoints];
		PopulateArray(localSpaceData);
		worldSpaceData = new VelocityDataPoint[maxDataPoints];
		PopulateArray(worldSpaceData);
		isRelativeTo = relativeTo != null;
		lastLocalSpacePos = GetPosition(worldSpace: false);
		lastWorldSpacePos = GetPosition(worldSpace: true);
		void PopulateArray(VelocityDataPoint[] array)
		{
			for (int i = 0; i < maxDataPoints; i++)
			{
				array[i] = new VelocityDataPoint();
			}
		}
	}

	private void Awake()
	{
		ResetState();
	}

	private void OnDisable()
	{
		ResetState();
	}

	private Vector3 GetPosition(bool worldSpace)
	{
		if (worldSpace)
		{
			return trans.position;
		}
		if (isRelativeTo)
		{
			return relativeTo.InverseTransformPoint(trans.position);
		}
		return trans.localPosition;
	}

	private void Update()
	{
		Tick();
	}

	public void Tick()
	{
		if (Time.frameCount > lastTickedFrame)
		{
			Vector3 position = GetPosition(worldSpace: false);
			Vector3 position2 = GetPosition(worldSpace: true);
			VelocityDataPoint velocityDataPoint = localSpaceData[currentDataPointIndex];
			velocityDataPoint.delta = (position - lastLocalSpacePos) / Time.deltaTime;
			velocityDataPoint.time = Time.time;
			localSpaceData[currentDataPointIndex] = velocityDataPoint;
			VelocityDataPoint velocityDataPoint2 = worldSpaceData[currentDataPointIndex];
			velocityDataPoint2.delta = (position2 - lastWorldSpacePos) / Time.deltaTime;
			velocityDataPoint2.time = Time.time;
			worldSpaceData[currentDataPointIndex] = velocityDataPoint2;
			lastLocalSpacePos = position;
			lastWorldSpacePos = position2;
			currentDataPointIndex++;
			if (currentDataPointIndex >= maxDataPoints)
			{
				currentDataPointIndex = 0;
			}
			lastTickedFrame = Time.frameCount;
		}
	}

	private void AddToQueue(ref List<VelocityDataPoint> dataPoints, VelocityDataPoint newData)
	{
		dataPoints.Add(newData);
		if (dataPoints.Count >= maxDataPoints)
		{
			dataPoints.RemoveAt(0);
		}
	}

	public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
	{
		float num = maxTimeFromPast / 2f;
		VelocityDataPoint[] array = ((!worldSpace) ? localSpaceData : worldSpaceData);
		if (array.Length <= 1)
		{
			return Vector3.zero;
		}
		Vector3 total = Vector3.zero;
		float totalMag = 0f;
		int added = 0;
		float num2 = Time.time - maxTimeFromPast;
		float num3 = Time.time - num;
		int i = 0;
		int num4 = currentDataPointIndex;
		for (; i < maxDataPoints; i++)
		{
			VelocityDataPoint velocityDataPoint = array[num4];
			if (doMagnitudeCheck && added > 1 && velocityDataPoint.time >= num3)
			{
				if (velocityDataPoint.delta.magnitude >= totalMag / (float)added)
				{
					AddPoint(velocityDataPoint);
				}
			}
			else if (velocityDataPoint.time >= num2)
			{
				AddPoint(velocityDataPoint);
			}
			num4++;
			if (num4 >= maxDataPoints)
			{
				num4 = 0;
			}
		}
		if (added > 0)
		{
			return total / added;
		}
		return Vector3.zero;
		void AddPoint(VelocityDataPoint point)
		{
			total += point.delta;
			totalMag += point.delta.magnitude;
			added++;
		}
	}

	public Vector3 GetLatestVelocity(bool worldSpace = false)
	{
		VelocityDataPoint[] array = ((!worldSpace) ? localSpaceData : worldSpaceData);
		return array[currentDataPointIndex].delta;
	}

	public float GetAverageSpeedChangeMagnitudeInDirection(Vector3 dir, bool worldSpace = false, float maxTimeFromPast = 0.05f)
	{
		VelocityDataPoint[] array = ((!worldSpace) ? localSpaceData : worldSpaceData);
		if (array.Length <= 1)
		{
			return 0f;
		}
		float num = 0f;
		int num2 = 0;
		float num3 = Time.time - maxTimeFromPast;
		bool flag = false;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].time >= num3)
			{
				if (!flag)
				{
					vector = array[i].delta;
					flag = true;
				}
				else
				{
					num += Mathf.Abs(Vector3.Dot(array[i].delta - vector, dir));
					num2++;
				}
			}
		}
		if (num2 <= 0)
		{
			return 0f;
		}
		return num / (float)num2;
	}
}
