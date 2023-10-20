using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000195 RID: 405
[Obsolete("replaced with ThrowableSetDressing.cs")]
public class MagicIngredient : TransferrableObject
{
	// Token: 0x06000A6B RID: 2667 RVA: 0x00040F9C File Offset: 0x0003F19C
	protected override void Awake()
	{
		this.item = this.worldShareableInstance;
		this.grabPtInitParent = this.anchor.transform.parent;
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00040FC0 File Offset: 0x0003F1C0
	private void ReParent()
	{
		Transform transform = this.anchor.transform;
		base.gameObject.transform.parent = transform;
		transform.parent = this.grabPtInitParent;
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00040FF6 File Offset: 0x0003F1F6
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

	// Token: 0x04000D28 RID: 3368
	[FormerlySerializedAs("IngredientType")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04000D29 RID: 3369
	public Transform rootParent;

	// Token: 0x04000D2A RID: 3370
	private WorldShareableItem item;

	// Token: 0x04000D2B RID: 3371
	private Transform grabPtInitParent;
}
