using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class ObjectPools : MonoBehaviour
{
	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000D62 RID: 3426 RVA: 0x0004E413 File Offset: 0x0004C613
	// (set) Token: 0x06000D63 RID: 3427 RVA: 0x0004E41B File Offset: 0x0004C61B
	public bool initialized { get; private set; }

	// Token: 0x06000D64 RID: 3428 RVA: 0x0004E424 File Offset: 0x0004C624
	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0004E42C File Offset: 0x0004C62C
	protected void Start()
	{
		this.lookUp = new Dictionary<int, SinglePool>();
		foreach (SinglePool singlePool in this.pools)
		{
			singlePool.Initialize(base.gameObject);
			int num = singlePool.PoolGUID();
			if (this.lookUp.ContainsKey(num))
			{
				using (List<SinglePool>.Enumerator enumerator2 = this.pools.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SinglePool singlePool2 = enumerator2.Current;
						if (singlePool2.PoolGUID() == num)
						{
							Debug.LogError("Pools contain more then one instance of the same object\n" + string.Format("First object in question is {0} tag: {1}\n", singlePool2.objectToPool, singlePool2.objectToPool.tag) + string.Format("Second object is {0} tag: {1}", singlePool.objectToPool, singlePool.objectToPool.tag));
							break;
						}
					}
					continue;
				}
			}
			this.lookUp.Add(singlePool.PoolGUID(), singlePool);
		}
		this.initialized = true;
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x0004E558 File Offset: 0x0004C758
	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0004E568 File Offset: 0x0004C768
	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int hash = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(hash);
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x0004E583 File Offset: 0x0004C783
	public GameObject Instantiate(GameObject obj)
	{
		return this.GetPoolByObjectType(obj).Instantiate();
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0004E591 File Offset: 0x0004C791
	public GameObject Instantiate(int hash)
	{
		return this.GetPoolByHash(hash).Instantiate();
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x0004E59F File Offset: 0x0004C79F
	public GameObject Instantiate(GameObject obj, Vector3 position)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x0004E5B4 File Offset: 0x0004C7B4
	public GameObject Instantiate(int hash, Vector3 position)
	{
		GameObject gameObject = this.Instantiate(hash);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0004E5C9 File Offset: 0x0004C7C9
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x0004E5DF File Offset: 0x0004C7DF
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x0004E60C File Offset: 0x0004C80C
	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x0004E61B File Offset: 0x0004C81B
	private void OnApplicationQuit()
	{
		this.isQuitting = true;
		Debug.Log("Application Quitting");
	}

	// Token: 0x04001086 RID: 4230
	public static ObjectPools instance;

	// Token: 0x04001088 RID: 4232
	[SerializeField]
	private List<SinglePool> pools;

	// Token: 0x04001089 RID: 4233
	private Dictionary<int, SinglePool> lookUp;

	// Token: 0x0400108A RID: 4234
	public bool isQuitting;
}
