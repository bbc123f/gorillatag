using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02000357 RID: 855
	[ExecuteInEditMode]
	public class LatexFormula : MonoBehaviour
	{
		// Token: 0x040019A7 RID: 6567
		public static readonly string BaseUrl = "http://tex.s2cms.ru/svg/f(x) ";

		// Token: 0x040019A8 RID: 6568
		private int m_hash = LatexFormula.BaseUrl.GetHashCode();

		// Token: 0x040019A9 RID: 6569
		[SerializeField]
		private string m_formula = "";

		// Token: 0x040019AA RID: 6570
		private Texture m_texture;
	}
}
