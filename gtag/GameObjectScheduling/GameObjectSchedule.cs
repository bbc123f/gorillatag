using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameObjectScheduling
{
	[CreateAssetMenu(fileName = "New Game Object Schedule", menuName = "Game Object Scheduling/Game Object Schedule", order = 0)]
	public class GameObjectSchedule : ScriptableObject
	{
		public GameObjectSchedule.GameObjectScheduleNode[] Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		public bool InitialState
		{
			get
			{
				return this.initialState;
			}
		}

		public int GetCurrentNodeIndex(DateTime currentDate, int startFrom = 0)
		{
			if (startFrom >= this.nodes.Length)
			{
				return int.MaxValue;
			}
			for (int i = -1; i < this.nodes.Length - 1; i++)
			{
				if (currentDate < this.nodes[i + 1].DateTime)
				{
					return i;
				}
			}
			return int.MaxValue;
		}

		public void Validate()
		{
			if (this.validated)
			{
				return;
			}
			this._validate();
			this.validated = true;
		}

		private void _validate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				this.nodes[i].Validate();
			}
			List<GameObjectSchedule.GameObjectScheduleNode> list = new List<GameObjectSchedule.GameObjectScheduleNode>(this.nodes);
			list.Sort((GameObjectSchedule.GameObjectScheduleNode e1, GameObjectSchedule.GameObjectScheduleNode e2) => e1.DateTime.CompareTo(e2.DateTime));
			this.nodes = list.ToArray();
		}

		public GameObjectSchedule()
		{
		}

		[SerializeField]
		private bool initialState;

		[SerializeField]
		private GameObjectSchedule.GameObjectScheduleNode[] nodes;

		[SerializeField]
		private SchedulingOptions options;

		private bool validated;

		[Serializable]
		public class GameObjectScheduleNode
		{
			public bool ActiveState
			{
				get
				{
					return this.activeState;
				}
			}

			public DateTime DateTime
			{
				get
				{
					return this.dateTime;
				}
			}

			public void Validate()
			{
				try
				{
					this.dateTime = DateTime.Parse(this.activeDateTime, CultureInfo.InvariantCulture);
				}
				catch
				{
					this.dateTime = DateTime.MinValue;
				}
			}

			public GameObjectScheduleNode()
			{
			}

			[SerializeField]
			private string activeDateTime = "1/1/0001 00:00:00";

			[SerializeField]
			[Tooltip("Check to turn on. Uncheck to turn off.")]
			private bool activeState = true;

			private DateTime dateTime;
		}

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			public <>c()
			{
			}

			internal int <_validate>b__11_0(GameObjectSchedule.GameObjectScheduleNode e1, GameObjectSchedule.GameObjectScheduleNode e2)
			{
				return e1.DateTime.CompareTo(e2.DateTime);
			}

			public static readonly GameObjectSchedule.<>c <>9 = new GameObjectSchedule.<>c();

			public static Comparison<GameObjectSchedule.GameObjectScheduleNode> <>9__11_0;
		}
	}
}
