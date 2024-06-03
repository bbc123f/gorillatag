using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	public class TappableBell : Tappable
	{
		public event TappableBell.ObstacleCourseTriggerEvent OnTapped
		{
			[CompilerGenerated]
			add
			{
				TappableBell.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnTapped;
				TappableBell.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					TappableBell.ObstacleCourseTriggerEvent value2 = (TappableBell.ObstacleCourseTriggerEvent)Delegate.Combine(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<TappableBell.ObstacleCourseTriggerEvent>(ref this.OnTapped, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
			[CompilerGenerated]
			remove
			{
				TappableBell.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnTapped;
				TappableBell.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					TappableBell.ObstacleCourseTriggerEvent value2 = (TappableBell.ObstacleCourseTriggerEvent)Delegate.Remove(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<TappableBell.ObstacleCourseTriggerEvent>(ref this.OnTapped, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
		}

		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfo info)
		{
			if (!PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				return;
			}
			if (!this.rpcCooldown.CheckCallTime(Time.time))
			{
				return;
			}
			this.winnerRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (this.winnerRig != null)
			{
				TappableBell.ObstacleCourseTriggerEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(this.winnerRig);
			}
		}

		public TappableBell()
		{
		}

		private VRRig winnerRig;

		[CompilerGenerated]
		private TappableBell.ObstacleCourseTriggerEvent OnTapped;

		public CallLimiter rpcCooldown;

		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
