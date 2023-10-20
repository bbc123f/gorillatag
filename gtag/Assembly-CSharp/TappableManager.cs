using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019E RID: 414
[RequireComponent(typeof(PhotonView))]
public class TappableManager : MonoBehaviourPun
{
	// Token: 0x06000AB2 RID: 2738 RVA: 0x00042000 File Offset: 0x00040200
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

	// Token: 0x06000AB3 RID: 2739 RVA: 0x00042090 File Offset: 0x00040290
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

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000420E7 File Offset: 0x000402E7
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

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0004211B File Offset: 0x0004031B
	public static void Register(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.RegisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Add(t);
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00042142 File Offset: 0x00040342
	public static void Unregister(Tappable t)
	{
		if (TappableManager.gManager != null)
		{
			TappableManager.gManager.UnregisterInstance(t);
			return;
		}
		TappableManager.gRegistry.Remove(t);
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x0004216C File Offset: 0x0004036C
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

	// Token: 0x06000AB8 RID: 2744 RVA: 0x000421F0 File Offset: 0x000403F0
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

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00042258 File Offset: 0x00040458
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

	// Token: 0x06000ABA RID: 2746 RVA: 0x0004231E File Offset: 0x0004051E
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

	// Token: 0x04000D83 RID: 3459
	private static TappableManager gManager;

	// Token: 0x04000D84 RID: 3460
	[SerializeField]
	private List<Tappable> tappables = new List<Tappable>();

	// Token: 0x04000D85 RID: 3461
	private HashSet<int> idSet = new HashSet<int>();

	// Token: 0x04000D86 RID: 3462
	private static HashSet<Tappable> gRegistry = new HashSet<Tappable>();
}
