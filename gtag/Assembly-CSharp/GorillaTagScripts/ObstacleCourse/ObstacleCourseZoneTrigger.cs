using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	public class ObstacleCourseZoneTrigger : MonoBehaviour
	{
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter
		{
			[CompilerGenerated]
			add
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnPlayerTriggerEnter;
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent value2 = (ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent)Delegate.Combine(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent>(ref this.OnPlayerTriggerEnter, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
			[CompilerGenerated]
			remove
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnPlayerTriggerEnter;
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent value2 = (ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent)Delegate.Remove(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent>(ref this.OnPlayerTriggerEnter, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
		}

		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit
		{
			[CompilerGenerated]
			add
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnPlayerTriggerExit;
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent value2 = (ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent)Delegate.Combine(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent>(ref this.OnPlayerTriggerExit, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
			[CompilerGenerated]
			remove
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent = this.OnPlayerTriggerExit;
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent obstacleCourseTriggerEvent2;
				do
				{
					obstacleCourseTriggerEvent2 = obstacleCourseTriggerEvent;
					ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent value2 = (ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent)Delegate.Remove(obstacleCourseTriggerEvent2, value);
					obstacleCourseTriggerEvent = Interlocked.CompareExchange<ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent>(ref this.OnPlayerTriggerExit, value2, obstacleCourseTriggerEvent2);
				}
				while (obstacleCourseTriggerEvent != obstacleCourseTriggerEvent2);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerExit = this.OnPlayerTriggerExit;
				if (onPlayerTriggerExit == null)
				{
					return;
				}
				onPlayerTriggerExit(other);
			}
		}

		public ObstacleCourseZoneTrigger()
		{
		}

		public LayerMask bodyLayer;

		[CompilerGenerated]
		private ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		[CompilerGenerated]
		private ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit;

		public delegate void ObstacleCourseTriggerEvent(Collider collider);
	}
}
