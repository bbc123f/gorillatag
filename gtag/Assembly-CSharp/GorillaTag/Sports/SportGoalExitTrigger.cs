using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x0200031F RID: 799
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x06001657 RID: 5719 RVA: 0x0007C480 File Offset: 0x0007A680
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x0400186A RID: 6250
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
