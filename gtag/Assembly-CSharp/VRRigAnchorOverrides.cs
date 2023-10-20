using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class VRRigAnchorOverrides : MonoBehaviour
{
	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060007A0 RID: 1952 RVA: 0x00030A3B File Offset: 0x0002EC3B
	public Transform NameDefaultAnchor
	{
		get
		{
			return this.nameDefaultAnchor;
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060007A1 RID: 1953 RVA: 0x00030A43 File Offset: 0x0002EC43
	public Transform NameTransform
	{
		get
		{
			return this.nameTransform;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060007A2 RID: 1954 RVA: 0x00030A4B File Offset: 0x0002EC4B
	public Transform HuntDefaultAnchor
	{
		get
		{
			return this.huntComputerDefaultAnchor;
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060007A3 RID: 1955 RVA: 0x00030A53 File Offset: 0x0002EC53
	public Transform HuntComputer
	{
		get
		{
			return this.huntComputer;
		}
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x00030A5C File Offset: 0x0002EC5C
	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			this.overrideAnchors[i] = null;
		}
		this.huntDefaultTransform = this.huntComputer;
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00030A8A File Offset: 0x0002EC8A
	private void OnEnable()
	{
		this.nameTransform.parent = this.nameDefaultAnchor.parent;
		this.huntComputer = this.huntDefaultTransform;
		this.huntComputer.parent = this.huntComputerDefaultAnchor.parent;
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00030AC4 File Offset: 0x0002ECC4
	private int MapPositionToIndex(TransferrableObject.PositionState pos)
	{
		int num = (int)pos;
		int num2 = 0;
		while ((num >>= 1) != 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x00030AE4 File Offset: 0x0002ECE4
	public void OverrideAnchor(TransferrableObject.PositionState pos, Transform anchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (this.overrideAnchors[num])
		{
			foreach (object obj in this.overrideAnchors[num])
			{
				((Transform)obj).parent = null;
			}
		}
		this.overrideAnchors[num] = anchor;
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00030B60 File Offset: 0x0002ED60
	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = this.MapPositionToIndex(pos);
		Transform transform = this.overrideAnchors[num];
		if (transform != null)
		{
			return transform;
		}
		return fallback;
	}

	// Token: 0x0400093E RID: 2366
	[SerializeField]
	protected Transform nameDefaultAnchor;

	// Token: 0x0400093F RID: 2367
	[SerializeField]
	protected Transform nameTransform;

	// Token: 0x04000940 RID: 2368
	[SerializeField]
	protected Transform huntComputer;

	// Token: 0x04000941 RID: 2369
	[SerializeField]
	protected Transform huntComputerDefaultAnchor;

	// Token: 0x04000942 RID: 2370
	private Transform huntDefaultTransform;

	// Token: 0x04000943 RID: 2371
	private readonly Transform[] overrideAnchors = new Transform[8];

	// Token: 0x04000944 RID: 2372
	private GameObject nameLastObjectToAttach;
}
