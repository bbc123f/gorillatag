using System;
using System.Collections;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000190 RID: 400
public class HauntedObject : MonoBehaviour
{
	// Token: 0x06000A44 RID: 2628 RVA: 0x0003FEE8 File Offset: 0x0003E0E8
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

	// Token: 0x06000A45 RID: 2629 RVA: 0x0003FFA4 File Offset: 0x0003E1A4
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

	// Token: 0x06000A46 RID: 2630 RVA: 0x0004002F File Offset: 0x0003E22F
	private void Start()
	{
		this.initialPos = base.transform.position;
		this.passedTime = 0f;
		this.lightPassedTime = 0f;
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x00040058 File Offset: 0x0003E258
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

	// Token: 0x06000A48 RID: 2632 RVA: 0x00040122 File Offset: 0x0003E322
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

	// Token: 0x06000A49 RID: 2633 RVA: 0x00040131 File Offset: 0x0003E331
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

	// Token: 0x04000CDE RID: 3294
	[Tooltip("If this box is checked, then object will rattle when hunted")]
	public bool rattle;

	// Token: 0x04000CDF RID: 3295
	public float speed = 60f;

	// Token: 0x04000CE0 RID: 3296
	public float amount = 0.01f;

	// Token: 0x04000CE1 RID: 3297
	public float duration = 1f;

	// Token: 0x04000CE2 RID: 3298
	[FormerlySerializedAs("FBX")]
	public GameObject FBXprefab;

	// Token: 0x04000CE3 RID: 3299
	[Tooltip("Use to turn off a game object like candle flames when hunted")]
	public GameObject TurnOffLight;

	// Token: 0x04000CE4 RID: 3300
	public float TurnOffDuration = 2f;

	// Token: 0x04000CE5 RID: 3301
	private Vector3 initialPos;

	// Token: 0x04000CE6 RID: 3302
	private float passedTime;

	// Token: 0x04000CE7 RID: 3303
	private float lightPassedTime;

	// Token: 0x04000CE8 RID: 3304
	private GameObject lurkerGhost;

	// Token: 0x04000CE9 RID: 3305
	private GameObject wanderingGhost;

	// Token: 0x04000CEA RID: 3306
	private Animator[] animators;

	// Token: 0x04000CEB RID: 3307
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000CEC RID: 3308
	[FormerlySerializedAs("rattlingSound")]
	public AudioClip hauntedSound;
}
