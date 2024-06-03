using System;
using UnityEngine;

namespace GorillaTag
{
	[Serializable]
	public struct SturdyAssetRef<T> where T : Object
	{
		public T obj
		{
			get
			{
				return this._obj;
			}
			set
			{
				this._obj = value;
			}
		}

		public static implicit operator T(SturdyAssetRef<T> refObject)
		{
			return refObject.obj;
		}

		public static implicit operator SturdyAssetRef<T>(T obj)
		{
			return new SturdyAssetRef<T>
			{
				obj = obj
			};
		}

		[SerializeField]
		[HideInInspector]
		private T _obj;
	}
}
