using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class TrainCar : TrainCarBase
	{
		public float DistanceBehindParentScaled
		{
			get
			{
				return this.scale * this._distanceBehindParent;
			}
		}

		protected override void Awake()
		{
			base.Awake();
		}

		public override void UpdatePosition()
		{
			base.Distance = this._parentLocomotive.Distance - this.DistanceBehindParentScaled;
			base.UpdateCarPosition();
			base.RotateCarWheels();
		}

		[SerializeField]
		private TrainCarBase _parentLocomotive;

		[SerializeField]
		protected float _distanceBehindParent = 0.1f;
	}
}
