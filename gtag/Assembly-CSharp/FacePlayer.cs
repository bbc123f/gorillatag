using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class FacePlayer : MonoBehaviour
{
	// Token: 0x06000811 RID: 2065 RVA: 0x00032C08 File Offset: 0x00030E08
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
