using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	public class GuidedRefRelayHub : MonoBehaviour, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

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

		private void RegisterTarget(IGuidedRefTarget target)
		{
			GuidedRefRelayHub.RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(target.GuidedRefTargetId);
			if (orAddRelayInfoByTargetId == null)
			{
				return;
			}
			if (orAddRelayInfoByTargetId.target != null && orAddRelayInfoByTargetId.target != target)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"GuidedRefRelayHub: Multiple targets registering with the same Relay. Maybe look at the HubID you are using.hub=",
					base.name,
					", target1=",
					orAddRelayInfoByTargetId.target.transform.name,
					", target2=",
					target.transform.name
				}), this);
				return;
			}
			int instanceID = target.GetInstanceID();
			GuidedRefRelayHub.GlobalGetOrAddHubsListByRefInstId(instanceID).Add(this);
			GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[base.GetInstanceID()].Add(instanceID);
			orAddRelayInfoByTargetId.target = target;
			GuidedRefRelayHub.ResolveReferences(orAddRelayInfoByTargetId);
		}

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

		private void RegisterReceiverField(GuidedRefRelayHub.ReceiverFieldInfo receiverFieldInfo, GuidedRefTargetIdSO targetId)
		{
			GuidedRefRelayHub.globalLookupRefInstIDsByHubInstId[base.GetInstanceID()].Add(receiverFieldInfo.receiver.GetInstanceID());
			GuidedRefRelayHub.GlobalGetOrAddHubsListByRefInstId(receiverFieldInfo.receiver.GetInstanceID()).Add(this);
			GuidedRefRelayHub.RelayInfo orAddRelayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetId);
			orAddRelayInfoByTargetId.fieldInfos.Add(receiverFieldInfo);
			GuidedRefRelayHub.ResolveReferences(orAddRelayInfoByTargetId);
		}

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

		public static int RegisterReceiverFieldWithParentHub(IGuidedRefReceiver receiver, string fieldIdName, GuidedRefTargetIdSO targetId, GuidedRefRelayHubIdSO hubId = null)
		{
			int num = Shader.PropertyToID(fieldIdName);
			GuidedRefRelayHub.RegisterReceiverFieldWithParentHub(receiver, num, targetId, hubId);
			return num;
		}

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

		public static bool ResolveReference<T>(int theirFieldIdCandidate, IGuidedRefTarget theirTargetCandidate, int ourFieldId, ref T ourObj) where T : Object
		{
			if (ourFieldId == theirFieldIdCandidate && ourObj == null)
			{
				ourObj = (theirTargetCandidate.GuidedRefTargetObject as T);
				return ourObj != null;
			}
			return false;
		}

		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		[SerializeField]
		public bool isRootInstance;

		public GuidedRefRelayHubIdSO hubId;

		[OnEnterPlay_SetNull]
		[NonSerialized]
		public static GuidedRefRelayHub rootInstance;

		[OnEnterPlay_Set(false)]
		[NonSerialized]
		public static bool hasRootInstance;

		private static bool isAppQuitting;

		[DebugReadout]
		private readonly Dictionary<GuidedRefTargetIdSO, GuidedRefRelayHub.RelayInfo> lookupRelayInfoByTargetId = new Dictionary<GuidedRefTargetIdSO, GuidedRefRelayHub.RelayInfo>(256);

		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, List<GuidedRefRelayHub>> globalLookupHubsByInstanceID = new Dictionary<int, List<GuidedRefRelayHub>>(256);

		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, List<int>> globalLookupRefInstIDsByHubInstId = new Dictionary<int, List<int>>(256);

		[OnEnterPlay_Clear]
		private static readonly List<GuidedRefRelayHub> hubsTransientList = new List<GuidedRefRelayHub>(32);

		private const string kUnsuppliedCallerName = "UNSUPPLIED_CALLER_NAME";

		public class RelayInfo
		{
			public IGuidedRefTarget target;

			public List<GuidedRefRelayHub.ReceiverFieldInfo> fieldInfos;
		}

		public struct ReceiverFieldInfo
		{
			public IGuidedRefReceiver receiver;

			public int fieldId;
		}
	}
}
