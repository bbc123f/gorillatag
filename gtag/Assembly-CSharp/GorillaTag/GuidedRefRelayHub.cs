using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200030C RID: 780
	public class GuidedRefRelayHub : MonoBehaviour, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x060015A1 RID: 5537 RVA: 0x00077A6D File Offset: 0x00075C6D
		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x00077A78 File Offset: 0x00075C78
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

		// Token: 0x060015A3 RID: 5539 RVA: 0x00077B10 File Offset: 0x00075D10
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

		// Token: 0x060015A4 RID: 5540 RVA: 0x00077BC4 File Offset: 0x00075DC4
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

		// Token: 0x060015A5 RID: 5541 RVA: 0x00077C34 File Offset: 0x00075E34
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

		// Token: 0x060015A6 RID: 5542 RVA: 0x00077D40 File Offset: 0x00075F40
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

		// Token: 0x060015A7 RID: 5543 RVA: 0x00077DFC File Offset: 0x00075FFC
		private void RegisterReceiverField(GuidedRefRelayHub.ReceiverFieldInfo receiverFieldInfo, GuidedRefTargetIdSO targetId)
		{
			GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[base.GetInstanceID()].Add(receiverFieldInfo.receiver.GetInstanceID());
			GuidedRefRelayHub.GlobalGetOrAddHubsListByRefInstId(receiverFieldInfo.receiver.GetInstanceID()).Add(this);
			GuidedRefRelayHub.RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetId);
			orAddRelayInfoByTargetId.fieldInfos.Add(receiverFieldInfo);
			GuidedRefRelayHub.ResolveReferences(orAddRelayInfoByTargetId);
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00077E58 File Offset: 0x00076058
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

		// Token: 0x060015A9 RID: 5545 RVA: 0x00077F48 File Offset: 0x00076148
		public static int RegisterReceiverFieldWithParentHub(IGuidedRefReceiver receiver, string fieldIdName, GuidedRefTargetIdSO targetId, GuidedRefRelayHubIdSO hubId = null)
		{
			int num = Shader.PropertyToID(fieldIdName);
			GuidedRefRelayHub.RegisterReceiverFieldWithParentHub(receiver, num, targetId, hubId);
			return num;
		}

		// Token: 0x060015AA RID: 5546 RVA: 0x00077F68 File Offset: 0x00076168
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

		// Token: 0x060015AB RID: 5547 RVA: 0x00078090 File Offset: 0x00076290
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

		// Token: 0x060015AC RID: 5548 RVA: 0x000780EC File Offset: 0x000762EC
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

		// Token: 0x060015AD RID: 5549 RVA: 0x0007811C File Offset: 0x0007631C
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

		// Token: 0x060015AE RID: 5550 RVA: 0x0007817C File Offset: 0x0007637C
		public static bool ResolveReference<T>(int theirFieldIdCandidate, IGuidedRefTarget theirTargetCandidate, int ourFieldId, ref T ourObj) where T : Object
		{
			if (ourFieldId == theirFieldIdCandidate && ourObj == null)
			{
				ourObj = (theirTargetCandidate.GuidedRefTargetObject as T);
				return ourObj != null;
			}
			return false;
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x0007820D File Offset: 0x0007640D
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x00078215 File Offset: 0x00076415
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040017BF RID: 6079
		[SerializeField]
		public bool isRootInstance;

		// Token: 0x040017C0 RID: 6080
		public GuidedRefRelayHubIdSO hubId;

		// Token: 0x040017C1 RID: 6081
		[NonSerialized]
		public static GuidedRefRelayHub rootInstance;

		// Token: 0x040017C2 RID: 6082
		[NonSerialized]
		public static bool hasRootInstance;

		// Token: 0x040017C3 RID: 6083
		private static bool isAppQuitting;

		// Token: 0x040017C4 RID: 6084
		[DebugReadout]
		private readonly Dictionary<GuidedRefTargetIdSO, GuidedRefRelayHub.RelayInfo> lookupRelayInfoByTargetId = new Dictionary<GuidedRefTargetIdSO, GuidedRefRelayHub.RelayInfo>(256);

		// Token: 0x040017C5 RID: 6085
		private static readonly Dictionary<int, List<GuidedRefRelayHub>> globalLookupHubsByInstanceID = new Dictionary<int, List<GuidedRefRelayHub>>(256);

		// Token: 0x040017C6 RID: 6086
		private static readonly Dictionary<int, List<int>> globalLookupRefInstIDsByHubInstId = new Dictionary<int, List<int>>(256);

		// Token: 0x040017C7 RID: 6087
		private static readonly List<GuidedRefRelayHub> hubsTransientList = new List<GuidedRefRelayHub>(32);

		// Token: 0x040017C8 RID: 6088
		private const string kUnsuppliedCallerName = "UNSUPPLIED_CALLER_NAME";

		// Token: 0x020004FE RID: 1278
		public class RelayInfo
		{
			// Token: 0x040020D4 RID: 8404
			public IGuidedRefTarget target;

			// Token: 0x040020D5 RID: 8405
			public List<GuidedRefRelayHub.ReceiverFieldInfo> fieldInfos;
		}

		// Token: 0x020004FF RID: 1279
		public struct ReceiverFieldInfo
		{
			// Token: 0x040020D6 RID: 8406
			public IGuidedRefReceiver receiver;

			// Token: 0x040020D7 RID: 8407
			public int fieldId;
		}
	}
}
