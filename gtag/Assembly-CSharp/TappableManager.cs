using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TappableManager : MonoBehaviourPun
{
	private void Awake()
	{
		if (TappableManager.gManager != null && TappableManager.gManager != this)
		{
			GTDev.LogWarning("Instance of TappableManager already exists. Destroying.", null);
			Object.Destroy(this);
			return;
		}
		if (TappableManager.gManager == null)
		{
			TappableManager.gManager = this;
		}
		if (TappableManager.gRegistry.Count == 0)
		{
			return;
		}
		Tappable[] array = TappableManager.gRegistry.ToArray<Tappable>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == null))
			{
				this.RegisterInstance(array[i]);
			}
		}
		TappableManager.gRegistry.Clear();
	}

	private void RegisterInstance(Tappable t)
	{
		if (t == null)
		{
			GTDev.LogError("Tappable is null.", null);
			return;
		}
		if (!t.useStaticId)
		{
			TappableManager.CalculateId(t, false);
		}
		t.manager = this;
		if (this.idSet.Add(t.tappableId))
		{
			this.tappables.Add(t);
		}
	}

	private void UnregisterInstance(Tappable t)
	{
		if (t == null)
		{
			return;
		}
		if (!this.idSet.Remove(t.tappableId))
		{
			return;
		}
		this.tappables.Remove(t);
		t.manager = null;
	}

	public static void Register(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.RegisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Add(t);
	}

	public static void Unregister(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.UnregisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Remove(t);
	}

	[Conditional("QATESTING")]
	public void DebugTestTap()
	{
		if (this.tappables.Count > 0)
		{
			int index = Random.Range(0, this.tappables.Count);
			Debug.Log("Send TestTap to tappable index: " + index.ToString() + "/" + this.tappables.Count.ToString());
			this.tappables[index].OnTap(10f, Time.time);
			return;
		}
		Debug.Log("TappableManager: tappables array is empty.");
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
		for (int i = 0; i < this.tappables.Count; i++)
		{
			Tappable tappable = this.tappables[i];
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
				t.staticId = string.Format("#ID_{0:X8}", num2);
			}
			t.tappableId = t.staticId.GetStaticHash();
			return;
		}
		t.tappableId = (Application.isPlaying ? num : 0);
	}

	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.tappables != null && this.tappables.Count > 0)
		{
			this.tappables.Clear();
		}
	}

	private static TappableManager gManager;

	[SerializeField]
	private List<Tappable> tappables = new List<Tappable>();

	private HashSet<int> idSet = new HashSet<int>();

	private static HashSet<Tappable> gRegistry = new HashSet<Tappable>();
}
