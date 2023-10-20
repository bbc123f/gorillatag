using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200009B RID: 155
public class ButtonDownListener : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000358 RID: 856 RVA: 0x00013CD0 File Offset: 0x00011ED0
	// (remove) Token: 0x06000359 RID: 857 RVA: 0x00013D08 File Offset: 0x00011F08
	public event Action onButtonDown;

	// Token: 0x0600035A RID: 858 RVA: 0x00013D3D File Offset: 0x00011F3D
	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onButtonDown != null)
		{
			this.onButtonDown();
		}
	}
}
