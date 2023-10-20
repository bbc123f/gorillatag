using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002EC RID: 748
	public class TrainCar : TrainCarBase
	{
		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x00072D98 File Offset: 0x00070F98
		public float DistanceBehindParentScaled
		{
			get
			{
				return this.scale * this._distanceBehindParent;
			}
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x00072DA7 File Offset: 0x00070FA7
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x00072DAF File Offset: 0x00070FAF
		public override void UpdatePosition()
		{
			base.Distance = this._parentLocomotive.Distance - this.DistanceBehindParentScaled;
			base.UpdateCarPosition();
			base.RotateCarWheels();
		}

		// Token: 0x040016F0 RID: 5872
		[SerializeField]
		private TrainCarBase _parentLocomotive;

		// Token: 0x040016F1 RID: 5873
		[SerializeField]
		protected float _distanceBehindParent = 0.1f;
	}
}
