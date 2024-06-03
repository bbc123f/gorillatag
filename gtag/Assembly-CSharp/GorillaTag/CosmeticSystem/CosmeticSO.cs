using System;
using GorillaTag.GuidedRefs;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	[CreateAssetMenu(fileName = "UntitledSeason_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
	public class CosmeticSO : ScriptableObject
	{
		public CosmeticSO()
		{
		}

		public string displayName;

		public string cosmeticId;

		public Sprite icon;

		public GuidedRefTargetIdSO[] targets;
	}
}
