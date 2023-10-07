using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class ObjectToggle : MonoBehaviour
{
	// Token: 0x06000BEF RID: 3055 RVA: 0x00049A41 File Offset: 0x00047C41
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

	// Token: 0x06000BF0 RID: 3056 RVA: 0x00049A7C File Offset: 0x00047C7C
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

	// Token: 0x06000BF1 RID: 3057 RVA: 0x00049AEC File Offset: 0x00047CEC
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

	// Token: 0x04000F6A RID: 3946
	public List<GameObject> objectsToToggle = new List<GameObject>();

	// Token: 0x04000F6B RID: 3947
	[SerializeField]
	private bool _ignoreHierarchyState;

	// Token: 0x04000F6C RID: 3948
	[NonSerialized]
	private bool? _toggled;
}
