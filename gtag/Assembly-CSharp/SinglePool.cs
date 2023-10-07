using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021A RID: 538
[Serializable]
public class SinglePool
{
	// Token: 0x06000D56 RID: 3414 RVA: 0x0004E020 File Offset: 0x0004C220
	private void PrivAllocPooledObjects()
	{
		int count = this.inactivePool.Count;
		for (int i = count; i < count + this.initAmountToPool; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.objectToPool, this.gameObject.transform, true);
			gameObject.SetActive(false);
			this.inactivePool.Push(gameObject);
			int instanceID = gameObject.GetInstanceID();
			this.pooledObjects.Add(instanceID);
		}
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x0004E08B File Offset: 0x0004C28B
	public void Initialize(GameObject gameObject_)
	{
		this.gameObject = gameObject_;
		this.activePool = new Dictionary<int, GameObject>(this.initAmountToPool);
		this.inactivePool = new Stack<GameObject>(this.initAmountToPool);
		this.pooledObjects = new HashSet<int>();
		this.PrivAllocPooledObjects();
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x0004E0C8 File Offset: 0x0004C2C8
	public GameObject Instantiate()
	{
		if (this.inactivePool.Count == 0)
		{
			Debug.LogWarning("Pool '" + this.objectToPool.name + "'is expanding consider changing initial pool size");
			this.PrivAllocPooledObjects();
		}
		GameObject gameObject = this.inactivePool.Pop();
		int instanceID = gameObject.GetInstanceID();
		gameObject.SetActive(true);
		this.activePool.Add(instanceID, gameObject);
		return gameObject;
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x0004E130 File Offset: 0x0004C330
	public void Destroy(GameObject obj)
	{
		int instanceID = obj.GetInstanceID();
		if (!this.activePool.ContainsKey(instanceID))
		{
			Debug.Log("Failed to destroy Object in pool, It is not contained in the activePool");
			return;
		}
		if (!this.pooledObjects.Contains(instanceID))
		{
			Debug.Log("Failed to destroy Object in pool, It is not contained in the pooledObjects");
			return;
		}
		obj.SetActive(false);
		this.inactivePool.Push(obj);
		this.activePool.Remove(instanceID);
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x0004E196 File Offset: 0x0004C396
	public int PoolGUID()
	{
		return PoolUtils.GameObjHashCode(this.objectToPool);
	}

	// Token: 0x0400107B RID: 4219
	public GameObject objectToPool;

	// Token: 0x0400107C RID: 4220
	public int initAmountToPool = 32;

	// Token: 0x0400107D RID: 4221
	private HashSet<int> pooledObjects;

	// Token: 0x0400107E RID: 4222
	private Stack<GameObject> inactivePool;

	// Token: 0x0400107F RID: 4223
	private Dictionary<int, GameObject> activePool;

	// Token: 0x04001080 RID: 4224
	private GameObject gameObject;
}
