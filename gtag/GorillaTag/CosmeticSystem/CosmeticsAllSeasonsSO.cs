using System;
using System.Collections.Generic;
using GorillaTag.GuidedRefs;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.CosmeticSystem
{
	[CreateAssetMenu(fileName = "CosmeticsAllSeasonsSO", menuName = "- Gorilla Tag/Guided Ref Array", order = 0)]
	public class CosmeticsAllSeasonsSO : ScriptableObject
	{
		public GuidedRefHubIdSO hubId;

		[FormerlySerializedAs("targetArrays")]
		public CosmeticsSeasonSO[] seasons;

		[NonSerialized]
		public List<CosmeticsAllSeasonsSO.IndexesToPart> indexesToParts;

		[NonSerialized]
		public int fieldId;

		[NonSerialized]
		public int resolveCount;

		public struct IndexesToPart
		{
			public int season;

			public int cosmetic;

			public int part;
		}
	}
}
