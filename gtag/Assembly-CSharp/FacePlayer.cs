using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class FacePlayer : MonoBehaviour
{
	// Token: 0x06000810 RID: 2064 RVA: 0x00032DC8 File Offset: 0x00030FC8
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
