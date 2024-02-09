using System;
using System.Collections.Generic;
using UnityEngine;

public class CompositeTriggerEvents : MonoBehaviour
{
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerEnter;

	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerExit;

	private void Awake()
	{
		if (this.individualTriggerColliders.Count > 32)
		{
			Debug.LogError("The max number of triggers was exceeded in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
		}
		for (int i = 0; i < this.individualTriggerColliders.Count; i++)
		{
			TriggerEventNotifier triggerEventNotifier = this.individualTriggerColliders[i].gameObject.AddComponent<TriggerEventNotifier>();
			triggerEventNotifier.maskIndex = i;
			triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
			triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
			this.triggerEventNotifiers.Add(triggerEventNotifier);
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < this.triggerEventNotifiers.Count; i++)
		{
			if (this.triggerEventNotifiers[i] != null)
			{
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
			}
		}
	}

	public void TriggerEnterReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexTrue(num, notifier.maskIndex);
			this.overlapMask[other] = num;
			return;
		}
		int num2 = this.SetMaskIndexTrue(0, notifier.maskIndex);
		this.overlapMask.Add(other, num2);
		this.CompositeTriggerEnter(other);
	}

	public void TriggerExitReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexFalse(num, notifier.maskIndex);
			if (num == 0)
			{
				this.overlapMask.Remove(other);
				this.CompositeTriggerExit(other);
				return;
			}
			this.overlapMask[other] = num;
		}
	}

	public void CompositeTriggerEnterReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	public void CompositeTriggerExitReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
		if (compositeTriggerExit == null)
		{
			return;
		}
		compositeTriggerExit(other);
	}

	private bool TestMaskIndex(int mask, int index)
	{
		return (mask & (1 << index)) != 0;
	}

	private int SetMaskIndexTrue(int mask, int index)
	{
		return mask | (1 << index);
	}

	private int SetMaskIndexFalse(int mask, int index)
	{
		return mask & ~(1 << index);
	}

	private string MaskToString(int mask)
	{
		string text = "";
		for (int i = 31; i >= 0; i--)
		{
			text += (this.TestMaskIndex(mask, i) ? "1" : "0");
		}
		return text;
	}

	[SerializeField]
	private List<Collider> individualTriggerColliders = new List<Collider>();

	private List<TriggerEventNotifier> triggerEventNotifiers = new List<TriggerEventNotifier>();

	private Dictionary<Collider, int> overlapMask = new Dictionary<Collider, int>();

	public delegate void TriggerEvent(Collider collider);
}
