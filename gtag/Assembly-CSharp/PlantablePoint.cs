using System;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class PlantablePoint : MonoBehaviour
{
	// Token: 0x060001F4 RID: 500 RVA: 0x0000DEA6 File Offset: 0x0000C0A6
	private void OnTriggerEnter(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.planted = true;
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000DED2 File Offset: 0x0000C0D2
	public void OnTriggerExit(Collider other)
	{
		if ((this.floorMask & 1 << other.gameObject.layer) != 0)
		{
			this.plantableObject.planted = false;
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000DEFE File Offset: 0x0000C0FE
	private void Start()
	{
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000DF00 File Offset: 0x0000C100
	private void Update()
	{
	}

	// Token: 0x040002BE RID: 702
	public bool shouldBeSet;

	// Token: 0x040002BF RID: 703
	public LayerMask floorMask;

	// Token: 0x040002C0 RID: 704
	public PlantableObject plantableObject;
}
