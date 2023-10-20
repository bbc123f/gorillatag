using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000322 RID: 802
	public class SportGoalTrigger : MonoBehaviour
	{
		// Token: 0x06001662 RID: 5730 RVA: 0x0007C9A7 File Offset: 0x0007ABA7
		public void BallExitedGoalTrigger(SportBall ball)
		{
			if (this.ballsPendingTriggerExit.Contains(ball))
			{
				this.ballsPendingTriggerExit.Remove(ball);
			}
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x0007C9C4 File Offset: 0x0007ABC4
		private void PruneBallsPendingTriggerExitByDistance()
		{
			foreach (SportBall sportBall in this.ballsPendingTriggerExit)
			{
				if ((sportBall.transform.position - base.transform.position).sqrMagnitude > this.ballTriggerExitDistanceFallback * this.ballTriggerExitDistanceFallback)
				{
					this.ballsPendingTriggerExit.Remove(sportBall);
				}
			}
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x0007CA50 File Offset: 0x0007AC50
		private void OnTriggerEnter(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.scoreboard != null)
			{
				this.PruneBallsPendingTriggerExitByDistance();
				if (!this.ballsPendingTriggerExit.Contains(componentInParent))
				{
					this.scoreboard.TeamScored(this.teamScoringOnThisGoal);
					this.ballsPendingTriggerExit.Add(componentInParent);
				}
			}
		}

		// Token: 0x04001878 RID: 6264
		[SerializeField]
		private SportScoreboard scoreboard;

		// Token: 0x04001879 RID: 6265
		[SerializeField]
		private int teamScoringOnThisGoal = 1;

		// Token: 0x0400187A RID: 6266
		[SerializeField]
		private float ballTriggerExitDistanceFallback = 3f;

		// Token: 0x0400187B RID: 6267
		private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();
	}
}
