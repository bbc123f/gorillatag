using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	public class RendererCullerByTriggers : MonoBehaviour
	{
		protected void OnEnable()
		{
			this.camWasTouching = false;
			Renderer[] array = this.renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}

		protected void LateUpdate()
		{
			Vector3 position = Camera.main.transform.position;
			bool flag = false;
			Collider[] array = this.colliders;
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i].ClosestPoint(position) - position).sqrMagnitude < 0.010000001f)
				{
					flag = true;
					break;
				}
			}
			if (this.camWasTouching == flag)
			{
				return;
			}
			this.camWasTouching = flag;
			Renderer[] array2 = this.renderers;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = flag;
			}
		}

		[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
		public Renderer[] renderers;

		public Collider[] colliders;

		private bool camWasTouching;

		private const float cameraRadiusSq = 0.010000001f;
	}
}
