using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200009B RID: 155
public class ButtonDownListener : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000358 RID: 856 RVA: 0x00013EF4 File Offset: 0x000120F4
	// (remove) Token: 0x06000359 RID: 857 RVA: 0x00013F2C File Offset: 0x0001212C
	public event Action onButtonDown;

	// Token: 0x0600035A RID: 858 RVA: 0x00013F61 File Offset: 0x00012161
	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onButtonDown != null)
		{
			this.onButtonDown();
		}
	}
}
