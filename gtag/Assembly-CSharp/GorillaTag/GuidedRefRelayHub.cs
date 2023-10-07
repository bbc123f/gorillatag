using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200030A RID: 778
	public class GuidedRefRelayHub : MonoBehaviour, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06001598 RID: 5528 RVA: 0x00077585 File Offset: 0x00075785
		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x00077590 File Offset: 0x00075790
		protected void OnDestroy()
		{
			if (GuidedRefRelayHub.isAppQuitting)
			{
				return;
			}
			if (this.isRootInstance)
			{
				GuidedRefRelayHub.hasRootInstance = false;
				GuidedRefRelayHub.rootInstance = null;
			}
			foreach (int key in GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[base.GetInstanceID()])
			{
				GuidedRefRelayHub.globalLookupHubsByInstanceID[key].Remove(this);
			}
			GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId.Remove(base.GetInstanceID());
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x00077628 File Offset: 0x00075828
		public void GuidedRefInitialize()
		{
			if (this.isRootInstance)
			{
				if (GuidedRefRelayHub.hasRootInstance)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"GuidedRefRelayHub: Attempted to assign global instance when one was already assigned:\n- This path: ",
						base.transform.GetPath(),
						"\n- Global instance: ",
						GuidedRefRelayHub.rootInstance.transform.GetPath(),
						"\n"
					}), this);
					Object.Destroy(this);
					return;
				}
				GuidedRefRelayHub.hasRootInstance = true;
				GuidedRefRelayHub.rootInstance = this;
			}
			GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[base.GetInstanceID()] = new List<int>(2);
			Application.quitting += delegate()
			{
				GuidedRefRelayHub.isAppQuitting = true;
			};
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x000776DC File Offset: 0x000758DC
		private void RegisterTarget(IGuidedRefTarget target)
		{
			GuidedRefRelayHub.RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(target.GuidedRefTargetId);
			if (orAddRelayInfoByTargetId == null)
			{
				return;
			}
			if (orAddRelayInfoByTargetId.target != null && orAddRelayInfoByTargetId.target != target)
			{
				Debug.LogError("GuidedRefRelayHub: Multiple targets registering with the same Relay. Maybe look at the HubID you are using.");
				return;
			}
			int instanceID = target.GetInstanceID();
			GuidedRefRelayHub.GlobalGetOrAddHubsListByRefInstId(instanceID).Add(this);
			GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[base.GetInstanceID()].Add(instanceID);
			orAddRelayInfoByTargetId.target = target;
			GuidedRefRelayHub.ResolveReferences(orAddRelayInfoByTargetId);
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x0007774C File Offset: 0x0007594C
		public static void RegisterTargetWithParentRelays(IGuidedRefTarget target, GuidedRefRelayHubIdSO[] hubIds = null, Component debugCaller = null)
		{
			if (target == null)
			{
				string str = (debugCaller == null) ? "UNSUPPLIED_CALLER_NAME" : debugCaller.name;
				Debug.LogError("GuidedRefRelayHub: Cannot register null target from \"" + str + "\".", debugCaller);
				return;
			}
			if (target.GuidedRefTargetId == null)
			{
				string str2 = (debugCaller == null) ? "UNSUPPLIED_CALLER_NAME" : debugCaller.GetComponentPath(int.MaxValue);
				Debug.LogError("GuidedRefRelayHub: Cannot register target with null targetId from \"" + str2 + "\".", debugCaller);
				return;
			}
			GuidedRefRelayHub.hubsTransientList.Clear();
			target.transform.GetComponentsInParent<GuidedRefRelayHub>(true, GuidedRefRelayHub.hubsTransientList);
			if (GuidedRefRelayHub.hasRootInstance)
			{
				GuidedRefRelayHub.hubsTransientList.Add(GuidedRefRelayHub.rootInstance);
			}
			foreach (GuidedRefRelayHub guidedRefRelayHub in GuidedRefRelayHub.hubsTransientList)
			{
				if (hubIds == null || hubIds.Length == 0 || Array.IndexOf<GuidedRefRelayHubIdSO>(hubIds, guidedRefRelayHub.hubId) != -1)
				{
					guidedRefRelayHub.RegisterTarget(target);
				}
			}
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x00077858 File Offset: 0x00075A58
		public static void UnregisterTarget(IGuidedRefTarget target)
		{
			if (target == null)
			{
				Debug.LogError("GuidedRefRelayHub: Cannot unregister null target.");
				return;
			}
			int instanceID = target.GetInstanceID();
			List<GuidedRefRelayHub> list;
			if (!GuidedRefRelayHub.globalLookupHubsByInstanceID.TryGetValue(instanceID, out list))
			{
				Debug.LogError("Tried to unregister target before it was registered.", target.transform);
				return;
			}
			foreach (GuidedRefRelayHub guidedRefRelayHub in list)
			{
				GuidedRefRelayHub.RelayInfo relayInfo;
				if (guidedRefRelayHub.lookupRelayInfoByTargetId.TryGetValue(target.GuidedRefTargetId, out relayInfo))
				{
					relayInfo.target = null;
				}
				GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[guidedRefRelayHub.GetInstanceID()].Remove(instanceID);
			}
			GuidedRefRelayHub.globalLookupHubsByInstanceID.Remove(instanceID);
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x00077914 File Offset: 0x00075B14
		private void RegisterReceiverField(GuidedRefRelayHub.ReceiverFieldInfo receiverFieldInfo, GuidedRefTargetIdSO targetId)
		{
			GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[base.GetInstanceID()].Add(receiverFieldInfo.receiver.GetInstanceID());
			GuidedRefRelayHub.GlobalGetOrAddHubsListByRefInstId(receiverFieldInfo.receiver.GetInstanceID()).Add(this);
			GuidedRefRelayHub.RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetId);
			orAddRelayInfoByTargetId.fieldInfos.Add(receiverFieldInfo);
			GuidedRefRelayHub.ResolveReferences(orAddRelayInfoByTargetId);
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x00077970 File Offset: 0x00075B70
		private static void RegisterReceiverFieldWithParentHub(IGuidedRefReceiver receiver, int fieldId, GuidedRefTargetIdSO targetId, GuidedRefRelayHubIdSO hubId = null)
		{
			if (receiver == null)
			{
				Debug.LogError("GuidedRefRelayHub: Cannot register null receiver.");
				return;
			}
			GuidedRefRelayHub.hubsTransientList.Clear();
			receiver.transform.GetComponentsInParent<GuidedRefRelayHub>(true, GuidedRefRelayHub.hubsTransientList);
			if (GuidedRefRelayHub.hasRootInstance)
			{
				GuidedRefRelayHub.hubsTransientList.Add(GuidedRefRelayHub.rootInstance);
			}
			GuidedRefRelayHub.ReceiverFieldInfo receiverFieldInfo = new GuidedRefRelayHub.ReceiverFieldInfo
			{
				receiver = receiver,
				fieldId = fieldId
			};
			bool flag = false;
			foreach (GuidedRefRelayHub guidedRefRelayHub in GuidedRefRelayHub.hubsTransientList)
			{
				if (!(hubId != null) || !(guidedRefRelayHub.hubId != hubId))
				{
					flag = true;
					guidedRefRelayHub.RegisterReceiverField(receiverFieldInfo, targetId);
					break;
				}
			}
			if (!flag)
			{
				Debug.LogError("Could not find matching GuidedRefRelayHub to register with for receiver at: " + receiver.transform.GetPath(), receiver.transform);
			}
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x00077A60 File Offset: 0x00075C60
		public static int RegisterReceiverFieldWithParentHub(IGuidedRefReceiver receiver, string fieldIdName, GuidedRefTargetIdSO targetId, GuidedRefRelayHubIdSO hubId = null)
		{
			int num = Shader.PropertyToID(fieldIdName);
			GuidedRefRelayHub.RegisterReceiverFieldWithParentHub(receiver, num, targetId, hubId);
			return num;
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x00077A80 File Offset: 0x00075C80
		public static void UnregisterReceiver(IGuidedRefReceiver receiver)
		{
			if (receiver == null)
			{
				Debug.LogError("GuidedRefRelayHub: Cannot unregister null receiver.");
				return;
			}
			int instanceID = receiver.GetInstanceID();
			List<GuidedRefRelayHub> list;
			if (!GuidedRefRelayHub.globalLookupHubsByInstanceID.TryGetValue(instanceID, out list))
			{
				Debug.LogError("Tried to unregister a receiver before it was registered.");
				return;
			}
			Predicate<GuidedRefRelayHub.ReceiverFieldInfo> <>9__0;
			foreach (GuidedRefRelayHub guidedRefRelayHub in list)
			{
				foreach (GuidedRefRelayHub.RelayInfo relayInfo in guidedRefRelayHub.lookupRelayInfoByTargetId.Values)
				{
					List<GuidedRefRelayHub.ReceiverFieldInfo> fieldInfos = relayInfo.fieldInfos;
					Predicate<GuidedRefRelayHub.ReceiverFieldInfo> match;
					if ((match = <>9__0) == null)
					{
						match = (<>9__0 = ((GuidedRefRelayHub.ReceiverFieldInfo fieldInfo) => fieldInfo.receiver == receiver));
					}
					fieldInfos.RemoveAll(match);
				}
				GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[guidedRefRelayHub.GetInstanceID()].Remove(instanceID);
			}
			GuidedRefRelayHub.globalLookupHubsByInstanceID.Remove(instanceID);
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x00077BA8 File Offset: 0x00075DA8
		private GuidedRefRelayHub.RelayInfo GetOrAddRelayInfoByTargetId(GuidedRefTargetIdSO targetId)
		{
			if (targetId == null)
			{
				Debug.LogError("GetOrAddRelayInfoByTargetId cannot register null target id");
				return null;
			}
			GuidedRefRelayHub.RelayInfo relayInfo;
			if (!this.lookupRelayInfoByTargetId.TryGetValue(targetId, out relayInfo))
			{
				relayInfo = new GuidedRefRelayHub.RelayInfo
				{
					target = null,
					fieldInfos = new List<GuidedRefRelayHub.ReceiverFieldInfo>(1)
				};
				this.lookupRelayInfoByTargetId[targetId] = relayInfo;
			}
			return relayInfo;
		}

		// Token: 0x060015A3 RID: 5539 RVA: 0x00077C04 File Offset: 0x00075E04
		public static List<GuidedRefRelayHub> GlobalGetOrAddHubsListByRefInstId(int instanceId)
		{
			List<GuidedRefRelayHub> list;
			if (!GuidedRefRelayHub.globalLookupHubsByInstanceID.TryGetValue(instanceId, out list))
			{
				list = new List<GuidedRefRelayHub>(1);
				GuidedRefRelayHub.globalLookupHubsByInstanceID[instanceId] = list;
			}
			return list;
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x00077C34 File Offset: 0x00075E34
		private static void ResolveReferences(GuidedRefRelayHub.RelayInfo relayInfo)
		{
			if (relayInfo.target == null)
			{
				return;
			}
			for (int i = relayInfo.fieldInfos.Count - 1; i >= 0; i--)
			{
				GuidedRefRelayHub.ReceiverFieldInfo receiverFieldInfo = relayInfo.fieldInfos[i];
				if (receiverFieldInfo.receiver.GuidRefResolveReference(receiverFieldInfo.fieldId, relayInfo.target))
				{
					relayInfo.fieldInfos.RemoveAt(i);
				}
			}
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x00077C94 File Offset: 0x00075E94
		public static bool ResolveReference<T>(int theirFieldIdCandidate, IGuidedRefTarget theirTargetCandidate, int ourFieldId, ref T ourObj) where T : Object
		{
			if (ourFieldId == theirFieldIdCandidate && ourObj == null)
			{
				ourObj = (theirTargetCandidate.GuidedRefTargetObject as T);
				return ourObj != null;
			}
			return false;
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00077D25 File Offset: 0x00075F25
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060015A9 RID: 5545 RVA: 0x00077D2D File Offset: 0x00075F2D
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040017B2 RID: 6066
		[SerializeField]
		public bool isRootInstance;

		// Token: 0x040017B3 RID: 6067
		public GuidedRefRelayHubIdSO hubId;

		// Token: 0x040017B4 RID: 6068
		[NonSerialized]
		public static GuidedRefRelayHub rootInstance;

		// Token: 0x040017B5 RID: 6069
		[NonSerialized]
		public static bool hasRootInstance;

		// Token: 0x040017B6 RID: 6070
		private static bool isAppQuitting;

		// Token: 0x040017B7 RID: 6071
		[DebugReadout]
		private readonly Dictionary<GuidedRefTargetIdSO, GuidedRefRelayHub.RelayInfo> lookupRelayInfoByTargetId = new Dictionary<GuidedRefTargetIdSO, GuidedRefRelayHub.RelayInfo>(256);

		// Token: 0x040017B8 RID: 6072
		private static readonly Dictionary<int, List<GuidedRefRelayHub>> globalLookupHubsByInstanceID = new Dictionary<int, List<GuidedRefRelayHub>>(256);

		// Token: 0x040017B9 RID: 6073
		private static readonly Dictionary<int, List<int>> globalLookupRefInstIDsByHubInstId = new Dictionary<int, List<int>>(256);

		// Token: 0x040017BA RID: 6074
		private static readonly List<GuidedRefRelayHub> hubsTransientList = new List<GuidedRefRelayHub>(32);

		// Token: 0x040017BB RID: 6075
		private const string kUnsuppliedCallerName = "UNSUPPLIED_CALLER_NAME";

		// Token: 0x020004FC RID: 1276
		public class RelayInfo
		{
			// Token: 0x040020C7 RID: 8391
			public IGuidedRefTarget target;

			// Token: 0x040020C8 RID: 8392
			public List<GuidedRefRelayHub.ReceiverFieldInfo> fieldInfos;
		}

		// Token: 0x020004FD RID: 1277
		public struct ReceiverFieldInfo
		{
			// Token: 0x040020C9 RID: 8393
			public IGuidedRefReceiver receiver;

			// Token: 0x040020CA RID: 8394
			public int fieldId;
		}
	}
}
