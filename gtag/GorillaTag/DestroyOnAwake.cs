using System;
using UnityEngine;

namespace GorillaTag
{
	public class DestroyOnAwake : MonoBehaviour
	{
		protected void Awake()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
