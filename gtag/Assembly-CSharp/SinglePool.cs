using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021B RID: 539
[Serializable]
public class SinglePool
{
	// Token: 0x06000D5C RID: 3420 RVA: 0x0004E280 File Offset: 0x0004C480
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

	// Token: 0x06000D5D RID: 3421 RVA: 0x0004E2EB File Offset: 0x0004C4EB
	public void Initialize(GameObject gameObject_)
	{
		this.gameObject = gameObject_;
		this.activePool = new Dictionary<int, GameObject>(this.initAmountToPool);
		this.inactivePool = new Stack<GameObject>(this.initAmountToPool);
		this.pooledObjects = new HashSet<int>();
		this.PrivAllocPooledObjects();
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x0004E328 File Offset: 0x0004C528
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

	// Token: 0x06000D5F RID: 3423 RVA: 0x0004E390 File Offset: 0x0004C590
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

	// Token: 0x06000D60 RID: 3424 RVA: 0x0004E3F6 File Offset: 0x0004C5F6
	public int PoolGUID()
	{
		return PoolUtils.GameObjHashCode(this.objectToPool);
	}

	// Token: 0x04001080 RID: 4224
	public GameObject objectToPool;

	// Token: 0x04001081 RID: 4225
	public int initAmountToPool = 32;

	// Token: 0x04001082 RID: 4226
	private HashSet<int> pooledObjects;

	// Token: 0x04001083 RID: 4227
	private Stack<GameObject> inactivePool;

	// Token: 0x04001084 RID: 4228
	private Dictionary<int, GameObject> activePool;

	// Token: 0x04001085 RID: 4229
	private GameObject gameObject;
}
