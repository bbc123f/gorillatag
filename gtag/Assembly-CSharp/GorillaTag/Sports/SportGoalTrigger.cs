using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000320 RID: 800
	public class SportGoalTrigger : MonoBehaviour
	{
		// Token: 0x06001659 RID: 5721 RVA: 0x0007C4BF File Offset: 0x0007A6BF
		public void BallExitedGoalTrigger(SportBall ball)
		{
			if (this.ballsPendingTriggerExit.Contains(ball))
			{
				this.ballsPendingTriggerExit.Remove(ball);
			}
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x0007C4DC File Offset: 0x0007A6DC
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

		// Token: 0x0600165B RID: 5723 RVA: 0x0007C568 File Offset: 0x0007A768
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

		// Token: 0x0400186B RID: 6251
		[SerializeField]
		private SportScoreboard scoreboard;

		// Token: 0x0400186C RID: 6252
		[SerializeField]
		private int teamScoringOnThisGoal = 1;

		// Token: 0x0400186D RID: 6253
		[SerializeField]
		private float ballTriggerExitDistanceFallback = 3f;

		// Token: 0x0400186E RID: 6254
		private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();
	}
}
