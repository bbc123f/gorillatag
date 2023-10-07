using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000194 RID: 404
[Obsolete("replaced with ThrowableSetDressing.cs")]
public class MagicIngredient : TransferrableObject
{
	// Token: 0x06000A66 RID: 2662 RVA: 0x00040E64 File Offset: 0x0003F064
	protected override void Awake()
	{
		this.item = this.worldShareableInstance;
		this.grabPtInitParent = this.anchor.transform.parent;
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00040E88 File Offset: 0x0003F088
	private void ReParent()
	{
		Transform transform = this.anchor.transform;
		base.gameObject.transform.parent = transform;
		transform.parent = this.grabPtInitParent;
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00040EBE File Offset: 0x0003F0BE
	public void Disable()
	{
		this.DropItem();
		base.OnDisable();
		if (this.item)
		{
			this.item.OnDisable();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000D24 RID: 3364
	[FormerlySerializedAs("IngredientType")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04000D25 RID: 3365
	public Transform rootParent;

	// Token: 0x04000D26 RID: 3366
	private WorldShareableItem item;

	// Token: 0x04000D27 RID: 3367
	private Transform grabPtInitParent;
}
