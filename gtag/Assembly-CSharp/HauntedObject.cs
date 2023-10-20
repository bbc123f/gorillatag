using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000191 RID: 401
public class HauntedObject : MonoBehaviour
{
	// Token: 0x06000A49 RID: 2633 RVA: 0x00040018 File Offset: 0x0003E218
	private void Awake()
	{
		this.lurkerGhost = GameObject.FindGameObjectWithTag("LurkerGhost");
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.wanderingGhost = GameObject.FindGameObjectWithTag("WanderingGhost");
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Combine(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		this.animators = base.transform.GetComponentsInChildren<Animator>();
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x000400D4 File Offset: 0x0003E2D4
	private void OnDestroy()
	{
		LurkerGhost lurkerGhost;
		if (this.lurkerGhost != null && this.lurkerGhost.TryGetComponent<LurkerGhost>(out lurkerGhost))
		{
			LurkerGhost lurkerGhost2 = lurkerGhost;
			lurkerGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(lurkerGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
		WanderingGhost wanderingGhost;
		if (this.wanderingGhost != null && this.wanderingGhost.TryGetComponent<WanderingGhost>(out wanderingGhost))
		{
			WanderingGhost wanderingGhost2 = wanderingGhost;
			wanderingGhost2.TriggerHauntedObjects = (UnityAction<GameObject>)Delegate.Remove(wanderingGhost2.TriggerHauntedObjects, new UnityAction<GameObject>(this.TriggerEffects));
		}
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0004015F File Offset: 0x0003E35F
	private void Start()
	{
		this.initialPos = base.transform.position;
		this.passedTime = 0f;
		this.lightPassedTime = 0f;
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00040188 File Offset: 0x0003E388
	private void TriggerEffects(GameObject go)
	{
		if (base.gameObject != go)
		{
			return;
		}
		if (this.rattle)
		{
			base.StartCoroutine("Shake");
		}
		if (this.audioSource && this.hauntedSound)
		{
			this.audioSource.PlayOneShot(this.hauntedSound);
		}
		if (this.FBXprefab)
		{
			ObjectPools.instance.Instantiate(this.FBXprefab, base.transform.position);
		}
		if (this.TurnOffLight != null)
		{
			base.StartCoroutine("TurnOff");
		}
		foreach (Animator animator in this.animators)
		{
			if (animator)
			{
				animator.SetTrigger("Haunted");
			}
		}
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x00040252 File Offset: 0x0003E452
	private IEnumerator Shake()
	{
		while (this.passedTime < this.duration)
		{
			this.passedTime += Time.deltaTime;
			base.transform.position = new Vector3(this.initialPos.x + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.y + Mathf.Sin(Time.time * this.speed) * this.amount, this.initialPos.z);
			yield return null;
		}
		this.passedTime = 0f;
		yield break;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x00040261 File Offset: 0x0003E461
	private IEnumerator TurnOff()
	{
		this.TurnOffLight.gameObject.SetActive(false);
		while (this.lightPassedTime < this.TurnOffDuration)
		{
			this.lightPassedTime += Time.deltaTime;
			yield return null;
		}
		this.TurnOffLight.SetActive(true);
		this.lightPassedTime = 0f;
		yield break;
	}

	// Token: 0x04000CE2 RID: 3298
	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	// Token: 0x04000CE3 RID: 3299
	public float speed = 60f;

	// Token: 0x04000CE4 RID: 3300
	public float amount = 0.01f;

	// Token: 0x04000CE5 RID: 3301
	public float duration = 1f;

	// Token: 0x04000CE6 RID: 3302
	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	// Token: 0x04000CE7 RID: 3303
	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	// Token: 0x04000CE8 RID: 3304
	public float TurnOffDuration = 2f;

	// Token: 0x04000CE9 RID: 3305
	private Vector3 initialPos;

	// Token: 0x04000CEA RID: 3306
	private float passedTime;

	// Token: 0x04000CEB RID: 3307
	private float lightPassedTime;

	// Token: 0x04000CEC RID: 3308
	private GameObject lurkerGhost;

	// Token: 0x04000CED RID: 3309
	private GameObject wanderingGhost;

	// Token: 0x04000CEE RID: 3310
	private Animator[] animators;

	// Token: 0x04000CEF RID: 3311
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000CF0 RID: 3312
	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;
}
