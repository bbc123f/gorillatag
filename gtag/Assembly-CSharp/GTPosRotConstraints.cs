using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class GTPosRotConstraints : MonoBehaviour
{
	// Token: 0x060001DA RID: 474 RVA: 0x0000D3CA File Offset: 0x0000B5CA
	protected void OnEnable()
	{
		GTPosRotConstraintManager.Register(this);
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000D3D2 File Offset: 0x0000B5D2
	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	// Token: 0x040002A0 RID: 672
	public GorillaPosRotConstraint[] constraints;
}
