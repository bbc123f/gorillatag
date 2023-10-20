using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// Token: 0x020000F1 RID: 241
public class CalibrationCube : MonoBehaviour
{
	// Token: 0x060005A0 RID: 1440 RVA: 0x0002309B File Offset: 0x0002129B
	private void Awake()
	{
		this.calibratedLength = this.baseLength;
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x000230AC File Offset: 0x000212AC
	private void Start()
	{
		try
		{
			this.OnCollisionExit(null);
		}
		catch
		{
		}
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x000230D8 File Offset: 0x000212D8
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x000230DA File Offset: 0x000212DA
	private void OnTriggerExit(Collider other)
	{
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x000230DC File Offset: 0x000212DC
	public void RecalibrateSize(bool pressed)
	{
		this.lastCalibratedLength = this.calibratedLength;
		this.calibratedLength = (this.rightController.transform.position - this.leftController.transform.position).magnitude;
		this.calibratedLength = ((this.calibratedLength > this.maxLength) ? this.maxLength : ((this.calibratedLength < this.minLength) ? this.minLength : this.calibratedLength));
		float d = this.calibratedLength / this.lastCalibratedLength;
		Vector3 localScale = this.playerBody.transform.localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Clear();
		this.playerBody.transform.localScale = new Vector3(1f, 1f, 1f);
		this.playerBody.GetComponentInChildren<TransformReset>().ResetTransforms();
		this.playerBody.transform.localScale = d * localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Build();
		this.playerBody.GetComponentInChildren<VRRig>().SetHeadBodyOffset();
		GorillaPlaySpace.Instance.bodyColliderOffset *= d;
		GorillaPlaySpace.Instance.bodyCollider.gameObject.transform.localScale *= d;
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00023236 File Offset: 0x00021436
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x00023238 File Offset: 0x00021438
	private void OnCollisionExit(Collision collision)
	{
		try
		{
			bool flag = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyName name = assemblies[i].GetName();
				if (!this.calibrationPresetsTest3[0].Contains(name.Name))
				{
					flag = true;
				}
			}
			if (!flag || Application.platform == RuntimePlatform.Android)
			{
				GorillaComputer.instance.includeUpdatedServerSynchTest = 0;
			}
		}
		catch
		{
		}
	}

	// Token: 0x0400068A RID: 1674
	public PrimaryButtonWatcher watcher;

	// Token: 0x0400068B RID: 1675
	public GameObject rightController;

	// Token: 0x0400068C RID: 1676
	public GameObject leftController;

	// Token: 0x0400068D RID: 1677
	public GameObject playerBody;

	// Token: 0x0400068E RID: 1678
	private float calibratedLength;

	// Token: 0x0400068F RID: 1679
	private float lastCalibratedLength;

	// Token: 0x04000690 RID: 1680
	public float minLength = 1f;

	// Token: 0x04000691 RID: 1681
	public float maxLength = 2.5f;

	// Token: 0x04000692 RID: 1682
	public float baseLength = 1.61f;

	// Token: 0x04000693 RID: 1683
	public string[] calibrationPresets;

	// Token: 0x04000694 RID: 1684
	public string[] calibrationPresetsTest;

	// Token: 0x04000695 RID: 1685
	public string[] calibrationPresetsTest2;

	// Token: 0x04000696 RID: 1686
	public string[] calibrationPresetsTest3;

	// Token: 0x04000697 RID: 1687
	public string[] calibrationPresetsTest4;

	// Token: 0x04000698 RID: 1688
	public string outputstring;

	// Token: 0x04000699 RID: 1689
	private List<string> stringList = new List<string>();
}
