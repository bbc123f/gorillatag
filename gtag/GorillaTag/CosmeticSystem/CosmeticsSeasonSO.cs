using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	[CreateAssetMenu(fileName = "UntitledSeason_CosmeticsSeasonSO", menuName = "- Gorilla Tag/CosmeticsSeasonSO", order = 0)]
	public class CosmeticsSeasonSO : ScriptableObject
	{
		public CosmeticSO[] cosmetics;
	}
}
