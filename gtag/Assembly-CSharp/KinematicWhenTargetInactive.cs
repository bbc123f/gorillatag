using System;
using UnityEngine;

// Token: 0x0200012E RID: 302
public class KinematicWhenTargetInactive : MonoBehaviour
{
	// Token: 0x060007E7 RID: 2023 RVA: 0x00031E64 File Offset: 0x00030064
	private void LateUpdate()
	{
		if (!this.target.activeSelf)
		{
			foreach (Rigidbody rigidbody in this.rigidBodies)
			{
				if (!rigidbody.isKinematic)
				{
					rigidbody.isKinematic = true;
				}
			}
			return;
		}
		foreach (Rigidbody rigidbody2 in this.rigidBodies)
		{
			if (rigidbody2.isKinematic)
			{
				rigidbody2.isKinematic = false;
			}
		}
	}

	// Token: 0x04000985 RID: 2437
	public Rigidbody[] rigidBodies;

	// Token: 0x04000986 RID: 2438
	public GameObject target;
}
