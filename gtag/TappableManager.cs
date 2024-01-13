using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(PhotonView))]
public class TappableManager : MonoBehaviourPun
{
	private static TappableManager gManager;

	[SerializeField]
	private List<Tappable> tappables = new List<Tappable>();

	private HashSet<int> idSet = new HashSet<int>();

	private static HashSet<Tappable> gRegistry = new HashSet<Tappable>();

	private void Awake()
	{
		if (gManager == null)
		{
			gManager = this;
		}
		if (gRegistry.Count != 0)
		{
			Tappable[] array = gRegistry.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				RegisterInstance(array[i]);
			}
			gRegistry.Clear();
		}
	}

	private void RegisterInstance(Tappable t)
	{
		if (t == null)
		{
			GTDev.LogError("Tappable is null.");
			return;
		}
		if (!t.useStaticId)
		{
			CalculateId(t);
		}
		t.manager = this;
		if (idSet.Add(t.tappableId))
		{
			tappables.Add(t);
		}
	}

	private void UnregisterInstance(Tappable t)
	{
		if (!(t == null) && idSet.Remove(t.tappableId))
		{
			tappables.Remove(t);
			t.manager = null;
		}
	}

	public static void Register(Tappable t)
	{
		if (gManager != null)
		{
			gManager.RegisterInstance(t);
		}
		else
		{
			gRegistry.Add(t);
		}
	}

	public static void Unregister(Tappable t)
	{
		if (gManager != null)
		{
			gManager.UnregisterInstance(t);
		}
		else
		{
			gRegistry.Remove(t);
		}
	}

	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SendOnTapRPC");
		if (key == 0)
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		for (int i = 0; i < tappables.Count; i++)
		{
			Tappable tappable = tappables[i];
			if (tappable.tappableId == key)
			{
				tappable.OnTapLocal(tapStrength, Time.time);
			}
		}
	}

	public static void CalculateId(Tappable t, bool force = false)
	{
		if (t == null)
		{
			return;
		}
		Transform transform = t.transform;
		int staticHash = TransformUtils.GetScenePath(transform).GetStaticHash();
		int staticHash2 = t.GetType().Name.GetStaticHash();
		int num = StaticHash.Combine(staticHash, staticHash2);
		if (t.useStaticId)
		{
			if (string.IsNullOrEmpty(t.staticId) || force)
			{
				Vector3 position = transform.position;
				int i = StaticHash.Combine(position.x, position.y, position.z);
				int instanceID = t.GetInstanceID();
				int num2 = StaticHash.Combine(num, i, instanceID);
				t.staticId = $"#ID_{num2:X8}";
			}
			t.tappableId = t.staticId.GetStaticHash();
		}
		else
		{
			t.tappableId = (Application.isPlaying ? num : 0);
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (tappables != null && tappables.Count > 0)
		{
			tappables.Clear();
		}
	}
}
