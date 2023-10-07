using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x06000AC2 RID: 2754 RVA: 0x0004232C File Offset: 0x0004052C
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

	// Token: 0x06000AC3 RID: 2755 RVA: 0x000423E4 File Offset: 0x000405E4
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

	// Token: 0x04000D89 RID: 3465
	public List<int> testList = new List<int>();

	// Token: 0x04000D8A RID: 3466
	public int[] testListArray = new int[10];

	// Token: 0x04000D8B RID: 3467
	public int randomIterator;

	// Token: 0x04000D8C RID: 3468
	public int tempRandIndex;

	// Token: 0x04000D8D RID: 3469
	public int tempRandValue;
}
