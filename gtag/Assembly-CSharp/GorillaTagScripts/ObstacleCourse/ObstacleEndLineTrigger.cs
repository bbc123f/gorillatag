using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	public class ObstacleEndLineTrigger : MonoBehaviour
	{
		public event ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter
		{
			[CompilerGenerated]
			add
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnPlayerTriggerEnter;
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					ObstacleEndLineTrigger.ObstacleCourseTriggerEvent value2 = (ObstacleEndLineTrigger.ObstacleCourseTriggerEvent)Delegate.Combine(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<ObstacleEndLineTrigger.ObstacleCourseTriggerEvent>(ref this.OnPlayerTriggerEnter, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
			[CompilerGenerated]
			remove
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnPlayerTriggerEnter;
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					ObstacleEndLineTrigger.ObstacleCourseTriggerEvent value2 = (ObstacleEndLineTrigger.ObstacleCourseTriggerEvent)Delegate.Remove(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<ObstacleEndLineTrigger.ObstacleCourseTriggerEvent>(ref this.OnPlayerTriggerEnter, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			VRRig vrrig;
			if (other.attachedRigidbody.gameObject.TryGetComponent<VRRig>(out vrrig))
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(vrrig);
			}
		}

		public ObstacleEndLineTrigger()
		{
		}

		[CompilerGenerated]
		private ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
