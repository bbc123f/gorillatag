using System;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class AugmentedObject : MonoBehaviour
{
	// Token: 0x060003BF RID: 959 RVA: 0x0001765C File Offset: 0x0001585C
	private void Start()
	{
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x000176C4 File Offset: 0x000158C4
	private void Update()
	{
		if (this.controllerHand != OVRInput.Controller.None && OVRInput.GetUp(OVRInput.Button.One, this.controllerHand))
		{
			this.ToggleShadowType();
		}
		if (this.shadow)
		{
			if (this.groundShadow)
			{
				this.shadow.transform.position = new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				return;
			}
			this.shadow.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00017752 File Offset: 0x00015952
	public void Grab(OVRInput.Controller grabHand)
	{
		this.controllerHand = grabHand;
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x0001775B File Offset: 0x0001595B
	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00017764 File Offset: 0x00015964
	private void ToggleShadowType()
	{
		this.groundShadow = !this.groundShadow;
	}

	// Token: 0x0400045F RID: 1119
	public OVRInput.Controller controllerHand;

	// Token: 0x04000460 RID: 1120
	public Transform shadow;

	// Token: 0x04000461 RID: 1121
	private bool groundShadow;
}
