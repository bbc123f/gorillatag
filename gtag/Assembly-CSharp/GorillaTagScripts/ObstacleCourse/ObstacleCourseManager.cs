using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;

namespace GorillaTagScripts.ObstacleCourse
{
	public class ObstacleCourseManager : MonoBehaviourPunCallbacks, IPunObservable, ITickSystemTick
	{
		public static ObstacleCourseManager Instance
		{
			[CompilerGenerated]
			get
			{
				return ObstacleCourseManager.<Instance>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				ObstacleCourseManager.<Instance>k__BackingField = value;
			}
		}

		public bool TickRunning
		{
			[CompilerGenerated]
			get
			{
				return this.<TickRunning>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.<TickRunning>k__BackingField = value;
			}
		}

		private void Awake()
		{
			ObstacleCourseManager.Instance = this;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			TickSystem<object>.AddCallbackTarget(this);
		}

		public override void OnDisable()
		{
			base.OnEnable();
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		public void Tick()
		{
			foreach (ObstacleCourse obstacleCourse in this.allObstaclesCourses)
			{
				obstacleCourse.InvokeUpdate();
			}
		}

		private void OnDestroy()
		{
			this.allObstaclesCourses.Clear();
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(this.allObstaclesCourses.Count);
				for (int i = 0; i < this.allObstaclesCourses.Count; i++)
				{
					stream.SendNext(this.allObstaclesCourses[i].winnerActorNumber);
					stream.SendNext(this.allObstaclesCourses[i].currentState);
				}
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int j = 0; j < num; j++)
			{
				int winnerActorNumber = (int)stream.ReceiveNext();
				ObstacleCourse.RaceState raceState = (ObstacleCourse.RaceState)stream.ReceiveNext();
				if (this.allObstaclesCourses[j].currentState != raceState)
				{
					this.allObstaclesCourses[j].Deserialize(winnerActorNumber, raceState);
				}
			}
		}

		public ObstacleCourseManager()
		{
		}

		[CompilerGenerated]
		private static ObstacleCourseManager <Instance>k__BackingField;

		public List<ObstacleCourse> allObstaclesCourses = new List<ObstacleCourse>();

		[CompilerGenerated]
		private bool <TickRunning>k__BackingField;
	}
}
