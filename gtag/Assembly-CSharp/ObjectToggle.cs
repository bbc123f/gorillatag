using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class ObjectToggle : MonoBehaviour
{
	// Token: 0x06000BF5 RID: 3061 RVA: 0x00049CA9 File Offset: 0x00047EA9
	public void Toggle(bool initialState = true)
	{
		if (this._toggled == null)
		{
			if (initialState)
			{
				this.Enable();
				return;
			}
			this.Disable();
			return;
		}
		else
		{
			if (this._toggled.Value)
			{
				this.Disable();
				return;
			}
			this.Enable();
			return;
		}
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00049CE4 File Offset: 0x00047EE4
	public void Enable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(true);
				}
				else if (!gameObject.activeInHierarchy)
				{
					gameObject.SetActive(true);
				}
			}
		}
		this._toggled = new bool?(true);
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00049D54 File Offset: 0x00047F54
	public void Disable()
	{
		if (this.objectsToToggle == null)
		{
			return;
		}
		for (int i = 0; i < this.objectsToToggle.Count; i++)
		{
			GameObject gameObject = this.objectsToToggle[i];
			if (!(gameObject == null))
			{
				if (this._ignoreHierarchyState)
				{
					gameObject.SetActive(false);
				}
				else if (gameObject.activeInHierarchy)
				{
					gameObject.SetActive(false);
				}
			}
		}
		this._toggled = new bool?(false);
	}

	// Token: 0x04000F6E RID: 3950
	public List<GameObject> objectsToToggle = new List<GameObject>();

	// Token: 0x04000F6F RID: 3951
	[SerializeField]
	private bool _ignoreHierarchyState;

	// Token: 0x04000F70 RID: 3952
	[NonSerialized]
	private bool? _toggled;
}
