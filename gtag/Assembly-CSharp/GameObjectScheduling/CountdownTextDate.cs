using System;
using UnityEngine;

namespace GameObjectScheduling
{
	[CreateAssetMenu(fileName = "New CountdownText Date", menuName = "Game Object Scheduling/CountdownText Date", order = 1)]
	public class CountdownTextDate : ScriptableObject
	{
		public CountdownTextDate()
		{
		}

		public string CountdownTo = "1/1/0001 00:00:00";

		public string FormatString = "";

		public int DaysThreshold = 365;
	}
}
