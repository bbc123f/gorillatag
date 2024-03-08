using System;
using UnityEngine;

namespace Fusion.StatsInternal
{
	public interface IFusionStatsView
	{
		void Initialize();

		void CalculateLayout();

		void Refresh();

		bool isActiveAndEnabled { get; }

		Transform transform { get; }
	}
}
