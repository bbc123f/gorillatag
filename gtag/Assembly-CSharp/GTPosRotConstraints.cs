using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class GTPosRotConstraints : MonoBehaviour
{
	// Token: 0x060001DA RID: 474 RVA: 0x0000D412 File Offset: 0x0000B612
	protected void OnEnable()
	{
		GTPosRotConstraintManager.Register(this);
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000D41A File Offset: 0x0000B61A
	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	// Token: 0x040002A0 RID: 672
	public GorillaPosRotConstraint[] constraints;
}
