using System;
using UnityEngine;

namespace GorillaTag
{
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour
	{
		protected void Awake()
		{
			this.index = StaticLodManager.Register(this);
		}

		protected void OnEnable()
		{
			StaticLodManager.SetEnabled(this.index, true);
		}

		protected void OnDisable()
		{
			StaticLodManager.SetEnabled(this.index, false);
		}

		private void OnDestroy()
		{
			StaticLodManager.Unregister(this.index);
		}

		private int index;

		public float collisionEnableDistance = 3f;

		public float uiFadeDistanceMin = 1f;

		public float uiFadeDistanceMax = 10f;
	}
}
