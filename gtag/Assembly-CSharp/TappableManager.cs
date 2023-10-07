using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019D RID: 413
[RequireComponent(typeof(PhotonView))]
public class TappableManager : MonoBehaviourPun
{
	// Token: 0x06000AAD RID: 2733 RVA: 0x00041EC8 File Offset: 0x000400C8
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

	// Token: 0x06000AAE RID: 2734 RVA: 0x00041F58 File Offset: 0x00040158
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

	// Token: 0x06000AAF RID: 2735 RVA: 0x00041FAF File Offset: 0x000401AF
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

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00041FE3 File Offset: 0x000401E3
	public static void Register(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.RegisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Add(t);
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0004200A File Offset: 0x0004020A
	public static void Unregister(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.UnregisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Remove(t);
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x00042034 File Offset: 0x00040234
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

	// Token: 0x06000AB3 RID: 2739 RVA: 0x000420B8 File Offset: 0x000402B8
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

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00042120 File Offset: 0x00040320
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

	// Token: 0x06000AB5 RID: 2741 RVA: 0x000421E6 File Offset: 0x000403E6
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

	// Token: 0x04000D7F RID: 3455
	private static TappableManager gManager;

	// Token: 0x04000D80 RID: 3456
	[SerializeField]
	private List<Tappable> tappables = new List<Tappable>();

	// Token: 0x04000D81 RID: 3457
	private HashSet<int> idSet = new HashSet<int>();

	// Token: 0x04000D82 RID: 3458
	private static HashSet<Tappable> gRegistry = new HashSet<Tappable>();
}
