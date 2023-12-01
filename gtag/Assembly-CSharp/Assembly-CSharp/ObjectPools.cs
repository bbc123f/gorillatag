using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPools : MonoBehaviour
{
	public bool initialized { get; private set; }

	protected void Awake()
	{
		ObjectPools.instance = this;
	}

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

	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int hash = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(hash);
	}

	public GameObject Instantiate(GameObject obj)
	{
		return this.GetPoolByObjectType(obj).Instantiate();
	}

	public GameObject Instantiate(int hash)
	{
		return this.GetPoolByHash(hash).Instantiate();
	}

	public GameObject Instantiate(GameObject obj, Vector3 position)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.position = position;
		return gameObject;
	}

	public GameObject Instantiate(int hash, Vector3 position)
	{
		GameObject gameObject = this.Instantiate(hash);
		gameObject.transform.position = position;
		return gameObject;
	}

	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale)
	{
		GameObject gameObject = this.Instantiate(obj);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	private void OnApplicationQuit()
	{
		this.isQuitting = true;
		Debug.Log("Application Quitting");
	}

	public static ObjectPools instance;

	[SerializeField]
	private List<SinglePool> pools;

	private Dictionary<int, SinglePool> lookUp;

	public bool isQuitting;
}
