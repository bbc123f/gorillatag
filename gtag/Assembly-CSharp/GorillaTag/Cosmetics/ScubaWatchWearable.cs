﻿using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000332 RID: 818
	[ExecuteAlways]
	public class ScubaWatchWearable : MonoBehaviour
	{
		// Token: 0x060016B7 RID: 5815 RVA: 0x0007E6F0 File Offset: 0x0007C8F0
		protected void Update()
		{
			Player instance = Player.Instance;
			if (this.onLeftHand)
			{
				if (instance.LeftHandWaterVolume != null)
				{
					this.currentDepth = Mathf.Max(-instance.LeftHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastLeftHandPosition), 0f);
				}
				else
				{
					this.currentDepth = 0f;
				}
			}
			else if (instance.RightHandWaterVolume != null)
			{
				this.currentDepth = Mathf.Max(-instance.RightHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastRightHandPosition), 0f);
			}
			else
			{
				this.currentDepth = 0f;
			}
			float t = (this.currentDepth - this.depthRange.x) / (this.depthRange.y - this.depthRange.x);
			float angle = Mathf.Lerp(this.dialRotationRange.x, this.dialRotationRange.y, t);
			this.dialNeedle.localRotation = this.initialDialRotation * Quaternion.AngleAxis(angle, this.dialRotationAxis);
		}

		// Token: 0x040018DE RID: 6366
		public bool onLeftHand;

		// Token: 0x040018DF RID: 6367
		[Tooltip("The transform that will be rotated to indicate the current depth.")]
		public Transform dialNeedle;

		// Token: 0x040018E0 RID: 6368
		[Tooltip("If your rotation is not zeroed out then click the Auto button to use the current rotation as 0.")]
		public Quaternion initialDialRotation;

		// Token: 0x040018E1 RID: 6369
		[Tooltip("The range of depth values that the dial will rotate between.")]
		public Vector2 depthRange = new Vector2(0f, 20f);

		// Token: 0x040018E2 RID: 6370
		[Tooltip("The range of rotation values that the dial will rotate between.")]
		public Vector2 dialRotationRange = new Vector2(0f, 360f);

		// Token: 0x040018E3 RID: 6371
		[Tooltip("The axis that the dial will rotate around.")]
		public Vector3 dialRotationAxis = Vector3.right;

		// Token: 0x040018E4 RID: 6372
		[Tooltip("The current depth of the player.")]
		[DebugOption]
		private float currentDepth;
	}
}