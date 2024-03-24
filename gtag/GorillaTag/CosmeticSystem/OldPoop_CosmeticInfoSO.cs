using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.CosmeticSystem
{
	[CreateAssetMenu(fileName = "CosmeticInfo", menuName = "Gorilla Tag/Cosmetic Info", order = 0)]
	public class OldPoop_CosmeticInfoSO : ScriptableObject
	{
		public OldPoop_CosmeticInfoSO()
		{
		}

		[Tooltip("If set to false this cosmetic will be completely ignored by the cosmetic system.")]
		public bool includeInGame = true;

		[FormerlySerializedAs("season")]
		public OldPoop_SeasonInfoSO oldPoopSeason;

		public CosmeticsController.CosmeticItem cosmeticItem;

		public GameObject wardrobePrefab;

		public GameObject functionalPrefab;

		public GameObject holdableAnchorPrefab;

		public GameObject firstPersonAndMirrorPrefab;

		private const string logPrefix = "OldPoop_CosmeticInfoSO: ";
	}
}
