using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;
using UnityEngine.Scripting;

[NetworkBehaviourWeaved(3862)]
public class FusionPlayerProperties : NetworkBehaviour
{
	[Networked(OnChanged = "AttributesChanged")]
	[Capacity(10)]
	[NetworkedWeaved(0, 3862)]
	private NetworkDictionary<PlayerRef, FusionPlayerProperties.PlayerInfo> netPlayerAttributes
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FusionPlayerProperties.netPlayerAttributes. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkDictionary<PlayerRef, FusionPlayerProperties.PlayerInfo>(this.Ptr + 0, 17, ReaderWriter@Fusion_PlayerRef.GetInstance(), ReaderWriter@FusionPlayerProperties__PlayerInfo.GetInstance());
		}
	}

	public FusionPlayerProperties.PlayerInfo PlayerProperties
	{
		get
		{
			return this.netPlayerAttributes[this.Runner.LocalPlayer];
		}
	}

	[Preserve]
	public static void AttributesChanged(Changed<FusionPlayerProperties> changed)
	{
		changed.Behaviour.OnAttributesChanged();
	}

	private void OnAttributesChanged()
	{
		FusionPlayerProperties.PlayerAttributeOnChanged playerAttributeOnChanged = this.playerAttributeOnChanged;
		if (playerAttributeOnChanged == null)
		{
			return;
		}
		playerAttributeOnChanged();
	}

	[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true, TickAligned = true)]
	public unsafe void RPC_UpdatePlayerAttributes(FusionPlayerProperties.PlayerInfo newInfo, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (this.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = this.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void FusionPlayerProperties::RPC_UpdatePlayerAttributes(FusionPlayerProperties/PlayerInfo,Fusion.RpcInfo)", this.Object, 7);
				}
				else
				{
					if (this.Runner.HasAnyActiveConnections())
					{
						int num = 8;
						num += 896;
						SimulationMessage* ptr = SimulationMessage.Allocate(this.Runner.Simulation, num);
						byte* data = SimulationMessage.GetData(ptr);
						int num2 = RpcHeader.Write(RpcHeader.Create(this.Object.Id, this.ObjectIndex, 1), data);
						*(FusionPlayerProperties.PlayerInfo*)(data + num2) = newInfo;
						num2 += 896;
						ptr->Offset = num2 * 8;
						this.Runner.SendRpc(ptr);
					}
					if ((localAuthorityMask & 7) != 0)
					{
						info = RpcInfo.FromLocal(this.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_12;
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		Debug.Log("Update Player attributes triggered");
		PlayerRef source = info.Source;
		if (this.netPlayerAttributes.ContainsKey(source))
		{
			Debug.Log("Current nickname is " + this.netPlayerAttributes[source].NickName.ToString());
			Debug.Log("Sent nickname is " + newInfo.NickName.ToString());
			if (this.netPlayerAttributes[source].Equals(newInfo))
			{
				Debug.Log("Info is already correct for this user. Shouldnt have received an RPC in this case.");
				return;
			}
		}
		this.netPlayerAttributes.Set(source, newInfo);
	}

	public override void Spawned()
	{
		Debug.Log("Player props SPAWNED!");
		if (this.Runner.Mode == SimulationModes.Client)
		{
			Debug.Log("SET Player Properties manager!");
		}
	}

	public string GetDisplayName(PlayerRef player)
	{
		return this.netPlayerAttributes[player].NickName.Value;
	}

	public string GetLocalDisplayName()
	{
		return this.netPlayerAttributes[this.Runner.LocalPlayer].NickName.Value;
	}

	public bool GetProperty(PlayerRef player, string propertyName, out string propertyValue)
	{
		NetworkString<_32> networkString;
		if (this.netPlayerAttributes[player].properties.TryGet(propertyName, out networkString))
		{
			propertyValue = networkString.Value;
			return true;
		}
		propertyValue = null;
		return false;
	}

	public bool PlayerHasEntry(PlayerRef player)
	{
		return this.netPlayerAttributes.ContainsKey(player);
	}

	public void RemovePlayerEntry(PlayerRef player)
	{
		if (this.Object.HasStateAuthority)
		{
			string value = this.netPlayerAttributes[player].NickName.Value;
			this.netPlayerAttributes.Remove(player);
			Debug.Log("Removed " + value + "player properties as they just left.");
		}
	}

	public FusionPlayerProperties()
	{
	}

	public override void CopyBackingFieldsToState(bool A_1)
	{
		NetworkBehaviourUtils.InitializeNetworkDictionary<SerializableDictionary<PlayerRef, FusionPlayerProperties.PlayerInfo>, PlayerRef, FusionPlayerProperties.PlayerInfo>(this.netPlayerAttributes, this._netPlayerAttributes, "netPlayerAttributes");
	}

	public override void CopyStateToBackingFields()
	{
		NetworkBehaviourUtils.CopyFromNetworkDictionary<SerializableDictionary<PlayerRef, FusionPlayerProperties.PlayerInfo>, PlayerRef, FusionPlayerProperties.PlayerInfo>(this.netPlayerAttributes, ref this._netPlayerAttributes);
	}

	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	protected unsafe static void RPC_UpdatePlayerAttributes@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = RpcHeader.ReadSize(data) + 3 & -4;
		FusionPlayerProperties.PlayerInfo playerInfo = *(FusionPlayerProperties.PlayerInfo*)(data + num);
		num += 896;
		FusionPlayerProperties.PlayerInfo newInfo = playerInfo;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
		behaviour.InvokeRpc = true;
		((FusionPlayerProperties)behaviour).RPC_UpdatePlayerAttributes(newInfo, info);
	}

	public FusionPlayerProperties.PlayerAttributeOnChanged playerAttributeOnChanged;

	static Changed<FusionPlayerProperties> $IL2CPP_CHANGED;

	static ChangedDelegate<FusionPlayerProperties> $IL2CPP_CHANGED_DELEGATE;

	static NetworkBehaviourCallbacks<FusionPlayerProperties> $IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[DefaultForProperty("netPlayerAttributes", 0, 3862)]
	private SerializableDictionary<PlayerRef, FusionPlayerProperties.PlayerInfo> _netPlayerAttributes;

	[NetworkStructWeaved(224)]
	[StructLayout(LayoutKind.Explicit, Size = 896)]
	public struct PlayerInfo : INetworkStruct
	{
		[Networked]
		public unsafe NetworkString<_16> NickName
		{
			readonly get
			{
				return *(NetworkString<_16>*)Native.ReferenceToPointer<FixedStorage@17>(ref this._NickName);
			}
			set
			{
				*(NetworkString<_16>*)Native.ReferenceToPointer<FixedStorage@17>(ref this._NickName) = value;
			}
		}

		[Networked]
		public unsafe NetworkDictionary<NetworkString<_32>, NetworkString<_32>> properties
		{
			get
			{
				return new NetworkDictionary<NetworkString<_32>, NetworkString<_32>>((int*)Native.ReferenceToPointer<FixedStorage@207>(ref this._properties), 3, ReaderWriter@Fusion_NetworkString.GetInstance(), ReaderWriter@Fusion_NetworkString.GetInstance());
			}
		}

		[FixedBufferProperty(typeof(NetworkString<_16>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@17 _NickName;

		[FixedBufferProperty(typeof(NetworkDictionary<NetworkString<_32>, NetworkString<_32>>), typeof(UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString), 3, order = -2147483647)]
		[SerializeField]
		[FieldOffset(68)]
		private FixedStorage@207 _properties;
	}

	public delegate void PlayerAttributeOnChanged();
}
