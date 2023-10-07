using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000355 RID: 853
	[ExecuteInEditMode]
	public class LatexFormula : MonoBehaviour
	{
		// Token: 0x0400199A RID: 6554
		public static readonly string BaseUrl = "http://tex.s2cms.ru/svg/f(x) ";

		// Token: 0x0400199B RID: 6555
		private int m_hash = LatexFormula.BaseUrl.GetHashCode();

		// Token: 0x0400199C RID: 6556
		[SerializeField]
		private string m_formula = "";

		// Token: 0x0400199D RID: 6557
		private Texture m_texture;
	}
}
