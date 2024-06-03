using System;
using UnityEngine;

internal abstract class RPCNetworkBase : MonoBehaviour
{
	public abstract void SetClassTarget(IGorillaSerializeable target, GorillaSerializer netHandler);

	protected RPCNetworkBase()
	{
	}
}
