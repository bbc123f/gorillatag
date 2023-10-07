using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021B RID: 539
public class ObjectPools : MonoBehaviour
{
	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000D5C RID: 3420 RVA: 0x0004E1B3 File Offset: 0x0004C3B3
	// (set) Token: 0x06000D5D RID: 3421 RVA: 0x0004E1BB File Offset: 0x0004C3BB
	public bool initialized { get; private set; }

	// Token: 0x06000D5E RID: 3422 RVA: 0x0004E1C4 File Offset: 0x0004C3C4
	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x0004E1CC File Offset: 0x0004C3CC
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

	// Token: 0x06000D60 RID: 3424 RVA: 0x0004E2F8 File Offset: 0x0004C4F8
	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x0004E308 File Offset: 0x0004C508
	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int hash = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(hash);
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x0004E323 File Offset: 0x0004C523
	public GameObject Instantiate(GameObject obj)
	{
		return this.GetPoolByObjectType(obj).Instantiate();
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0004E331 File Offset: 0x0004C531
	public GameObject Instantiate(int hash)
	{
		return this.GetPoolByHash(hash).Instantiate();
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x0004E33F File Offset: 0x0004C53F
	public GameObject Instantiate(GameObject obj, Vector3 position)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0004E354 File Offset: 0x0004C554
	public GameObject Instantiate(int hash, Vector3 position)
	{
		GameObject gameObject = this.Instantiate(hash);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x0004E369 File Offset: 0x0004C569
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x0004E37F File Offset: 0x0004C57F
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x0004E3AC File Offset: 0x0004C5AC
	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0004E3BB File Offset: 0x0004C5BB
	private void OnApplicationQuit()
	{
		this.isQuitting = true;
		Debug.Log("Application Quitting");
	}

	// Token: 0x04001081 RID: 4225
	public static ObjectPools instance;

	// Token: 0x04001083 RID: 4227
	[SerializeField]
	private List<SinglePool> pools;

	// Token: 0x04001084 RID: 4228
	private Dictionary<int, SinglePool> lookUp;

	// Token: 0x04001085 RID: 4229
	public bool isQuitting;
}
