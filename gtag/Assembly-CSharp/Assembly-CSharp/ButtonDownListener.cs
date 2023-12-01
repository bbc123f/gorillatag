using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDownListener : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public event Action onButtonDown;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onButtonDown != null)
		{
			this.onButtonDown();
		}
	}
}
