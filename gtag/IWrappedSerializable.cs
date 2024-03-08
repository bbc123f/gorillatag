using System;
using Fusion;

internal interface IWrappedSerializable : INetworkStruct
{
	void OnSerializeRead(object newData);

	object OnSerializeWrite();
}
