using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02000321 RID: 801
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x06001660 RID: 5728 RVA: 0x0007C968 File Offset: 0x0007AB68
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x04001877 RID: 6263
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
