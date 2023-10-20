using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class Bubbler : TransferrableObject
{
	// Token: 0x060007DD RID: 2013 RVA: 0x000316C0 File Offset: 0x0002F8C0
	protected override void Awake()
	{
		base.Awake();
		this.hasParticleSystem = (this.bubbleParticleSystem != null);
		if (this.hasParticleSystem)
		{
			this.bubbleParticleArray = new ParticleSystem.Particle[this.bubbleParticleSystem.main.maxParticles];
			this.bubbleParticleSystem.trigger.SetCollider(0, GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<SphereCollider>());
			this.bubbleParticleSystem.trigger.SetCollider(1, GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<SphereCollider>());
		}
		this.initialTriggerDuration = 0.05f;
		this.triggerStrength = 0.8f;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x00031770 File Offset: 0x0002F970
	public override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasBubblerAudio = (this.bubblerAudio != null && this.bubblerAudio.clip != null);
		this.hasPopBubbleAudio = (this.popBubbleAudio != null && this.popBubbleAudio.clip != null);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x000317DC File Offset: 0x0002F9DC
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.Stop();
		}
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x00031830 File Offset: 0x0002FA30
	public override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
		{
			this.bubbleParticleSystem.Stop();
		}
		if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
		{
			this.bubblerAudio.Stop();
		}
		this.currentParticles.Clear();
		this.particleInfoDict.Clear();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x000318A0 File Offset: 0x0002FAA0
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x000318B0 File Offset: 0x0002FAB0
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && this.myOnlineRig != null && this.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		bool forLeftController = this.currentState == TransferrableObject.PositionState.InLeftHand;
		bool enabled = this.itemState != TransferrableObject.ItemStates.State0;
		Behaviour[] array = this.behavioursToEnableWhenTriggerPressed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = enabled;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.hasParticleSystem && this.bubbleParticleSystem.isPlaying)
			{
				this.bubbleParticleSystem.Stop();
			}
			if (this.hasBubblerAudio && this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.Stop();
			}
		}
		else
		{
			if (this.hasParticleSystem && !this.bubbleParticleSystem.isEmitting)
			{
				this.bubbleParticleSystem.Play();
			}
			if (this.hasBubblerAudio && !this.bubblerAudio.isPlaying)
			{
				this.bubblerAudio.Play();
			}
			if (this.IsMyItem())
			{
				this.initialTriggerPull = Time.time;
				GorillaTagger.Instance.StartVibration(forLeftController, this.triggerStrength, this.initialTriggerDuration);
				if (Time.time > this.initialTriggerPull + this.initialTriggerDuration)
				{
					GorillaTagger.Instance.StartVibration(forLeftController, this.ongoingStrength, Time.deltaTime);
				}
			}
			float z = this.fan.transform.localEulerAngles.z + this.rotationSpeed * Time.fixedDeltaTime;
			this.fan.transform.localEulerAngles = new Vector3(0f, 0f, z);
		}
		if (this.hasParticleSystem && (!this.allBubblesPopped || this.itemState == TransferrableObject.ItemStates.State1))
		{
			int particles = this.bubbleParticleSystem.GetParticles(this.bubbleParticleArray);
			this.allBubblesPopped = (particles <= 0);
			if (!this.allBubblesPopped)
			{
				for (int j = 0; j < particles; j++)
				{
					if (this.currentParticles.Contains(this.bubbleParticleArray[j].randomSeed))
					{
						this.currentParticles.Remove(this.bubbleParticleArray[j].randomSeed);
					}
				}
				foreach (uint key in this.currentParticles)
				{
					if (this.particleInfoDict.TryGetValue(key, out this.outPosition))
					{
						if (this.hasPopBubbleAudio)
						{
							AudioSource.PlayClipAtPoint(this.popBubbleAudio.clip, this.outPosition);
						}
						this.particleInfoDict.Remove(key);
					}
				}
				this.currentParticles.Clear();
				for (int k = 0; k < particles; k++)
				{
					if (this.particleInfoDict.TryGetValue(this.bubbleParticleArray[k].randomSeed, out this.outPosition))
					{
						this.particleInfoDict[this.bubbleParticleArray[k].randomSeed] = this.bubbleParticleArray[k].position;
					}
					else
					{
						this.particleInfoDict.Add(this.bubbleParticleArray[k].randomSeed, this.bubbleParticleArray[k].position);
					}
					this.currentParticles.Add(this.bubbleParticleArray[k].randomSeed);
				}
			}
		}
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x00031C30 File Offset: 0x0002FE30
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00031C3F File Offset: 0x0002FE3F
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x00031C4E File Offset: 0x0002FE4E
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x00031C59 File Offset: 0x0002FE59
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04000971 RID: 2417
	public ParticleSystem bubbleParticleSystem;

	// Token: 0x04000972 RID: 2418
	private ParticleSystem.Particle[] bubbleParticleArray;

	// Token: 0x04000973 RID: 2419
	public AudioSource bubblerAudio;

	// Token: 0x04000974 RID: 2420
	public AudioSource popBubbleAudio;

	// Token: 0x04000975 RID: 2421
	private List<uint> currentParticles = new List<uint>();

	// Token: 0x04000976 RID: 2422
	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	// Token: 0x04000977 RID: 2423
	private Vector3 outPosition;

	// Token: 0x04000978 RID: 2424
	private bool allBubblesPopped;

	// Token: 0x04000979 RID: 2425
	public bool disableActivation;

	// Token: 0x0400097A RID: 2426
	public bool disableDeactivation;

	// Token: 0x0400097B RID: 2427
	public float rotationSpeed = 5f;

	// Token: 0x0400097C RID: 2428
	public GameObject fan;

	// Token: 0x0400097D RID: 2429
	public float ongoingStrength = 0.005f;

	// Token: 0x0400097E RID: 2430
	public float triggerStrength = 0.2f;

	// Token: 0x0400097F RID: 2431
	private float initialTriggerPull;

	// Token: 0x04000980 RID: 2432
	private float initialTriggerDuration;

	// Token: 0x04000981 RID: 2433
	private bool hasBubblerAudio;

	// Token: 0x04000982 RID: 2434
	private bool hasPopBubbleAudio;

	// Token: 0x04000983 RID: 2435
	public Behaviour[] behavioursToEnableWhenTriggerPressed;

	// Token: 0x04000984 RID: 2436
	private bool hasParticleSystem;

	// Token: 0x02000413 RID: 1043
	private enum BubblerState
	{
		// Token: 0x04001CF2 RID: 7410
		None = 1,
		// Token: 0x04001CF3 RID: 7411
		Bubbling
	}
}
