using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDownListener : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public event Action onButtonDown
	{
		[CompilerGenerated]
		add
		{
			Action action = this.onButtonDown;
			Action action2;
			do
			{
				action2 = action;
				Action action3 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.onButtonDown, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action action = this.onButtonDown;
			Action action2;
			do
			{
				action2 = action;
				Action action3 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action>(ref this.onButtonDown, action3, action2);
			}
			while (action != action2);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onButtonDown != null)
		{
			this.onButtonDown();
		}
	}

	public ButtonDownListener()
	{
	}

	[CompilerGenerated]
	private Action onButtonDown;
}
