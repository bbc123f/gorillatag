using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x06000AC7 RID: 2759 RVA: 0x00042464 File Offset: 0x00040664
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			this.testList.Add(i);
		}
		for (int j = 0; j < 10; j++)
		{
			this.testListArray[j] = 0;
		}
		for (int k = 0; k < this.testList.Count; k++)
		{
			this.testListArray[k] = this.testList[k];
		}
		this.RandomizeList(ref this.testList);
		for (int l = 0; l < 10; l++)
		{
			this.testListArray[l] = 0;
		}
		for (int m = 0; m < this.testList.Count; m++)
		{
			this.testListArray[m] = this.testList[m];
		}
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x0004251C File Offset: 0x0004071C
	public void RandomizeList(ref List<int> listToRandomize)
	{
		this.randomIterator = 0;
		while (this.randomIterator < listToRandomize.Count)
		{
			this.tempRandIndex = Random.Range(this.randomIterator, listToRandomize.Count);
			this.tempRandValue = listToRandomize[this.randomIterator];
			listToRandomize[this.randomIterator] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandValue;
			this.randomIterator++;
		}
	}

	// Token: 0x04000D8D RID: 3469
	public List<int> testList = new List<int>();

	// Token: 0x04000D8E RID: 3470
	public int[] testListArray = new int[10];

	// Token: 0x04000D8F RID: 3471
	public int randomIterator;

	// Token: 0x04000D90 RID: 3472
	public int tempRandIndex;

	// Token: 0x04000D91 RID: 3473
	public int tempRandValue;
}
