using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EA RID: 746
	public class TrainCar : TrainCarBase
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x0600143B RID: 5179 RVA: 0x000728CC File Offset: 0x00070ACC
		public float DistanceBehindParentScaled
		{
			get
			{
				return this.scale * this._distanceBehindParent;
			}
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x000728DB File Offset: 0x00070ADB
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x000728E3 File Offset: 0x00070AE3
		public override void UpdatePosition()
		{
			base.Distance = this._parentLocomotive.Distance - this.DistanceBehindParentScaled;
			base.UpdateCarPosition();
			base.RotateCarWheels();
		}

		// Token: 0x040016E3 RID: 5859
		[SerializeField]
		private TrainCarBase _parentLocomotive;

		// Token: 0x040016E4 RID: 5860
		[SerializeField]
		protected float _distanceBehindParent = 0.1f;
	}
}
