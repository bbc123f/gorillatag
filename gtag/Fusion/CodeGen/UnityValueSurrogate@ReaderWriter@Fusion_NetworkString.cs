using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_16>, ReaderWriter@Fusion_NetworkString>
	{
		public override NetworkString<_16> DataProperty
		{
			get
			{
				return this.Data;
			}
			set
			{
				this.Data = value;
			}
		}

		public NetworkString<_16> Data;
	}
}
