using System;
using UnityEngine;

namespace GorillaTagScripts
{
	public class RandomBucketThrowable : MonoBehaviour
	{
		public GameObject projectilePrefab;

		[Range(0f, 100f)]
		public float weightedChance = 1f;
	}
}
