using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

public class Bubbler : TransferrableObject
{
	protected override void Awake()
	{
		base.Awake();
		this.hasParticleSystem = this.bubbleParticleSystem != null;
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

	public override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasBubblerAudio = this.bubblerAudio != null && this.bubblerAudio.clip != null;
		this.hasPopBubbleAudio = this.popBubbleAudio != null && this.popBubbleAudio.clip != null;
		this.hasFan = this.fan != null;
	}

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

	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this._worksInWater && Player.Instance.InWater)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && this.myOnlineRig != null && this.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		bool flag2 = this.itemState != TransferrableObject.ItemStates.State0;
		Behaviour[] array = this.behavioursToEnableWhenTriggerPressed;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = flag2;
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
				GorillaTagger.Instance.StartVibration(flag, this.triggerStrength, this.initialTriggerDuration);
				if (Time.time > this.initialTriggerPull + this.initialTriggerDuration)
				{
					GorillaTagger.Instance.StartVibration(flag, this.ongoingStrength, Time.deltaTime);
				}
			}
			if (this.hasFan)
			{
				float num = this.fan.transform.localEulerAngles.z + this.rotationSpeed * Time.fixedDeltaTime;
				this.fan.transform.localEulerAngles = new Vector3(0f, 0f, num);
			}
		}
		if (this.hasParticleSystem && (!this.allBubblesPopped || this.itemState == TransferrableObject.ItemStates.State1))
		{
			int particles = this.bubbleParticleSystem.GetParticles(this.bubbleParticleArray);
			this.allBubblesPopped = particles <= 0;
			if (!this.allBubblesPopped)
			{
				for (int j = 0; j < particles; j++)
				{
					if (this.currentParticles.Contains(this.bubbleParticleArray[j].randomSeed))
					{
						this.currentParticles.Remove(this.bubbleParticleArray[j].randomSeed);
					}
				}
				foreach (uint num2 in this.currentParticles)
				{
					if (this.particleInfoDict.TryGetValue(num2, out this.outPosition))
					{
						if (this.hasPopBubbleAudio)
						{
							AudioSource.PlayClipAtPoint(this.popBubbleAudio.clip, this.outPosition);
						}
						this.particleInfoDict.Remove(num2);
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

	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	[SerializeField]
	private bool _worksInWater = true;

	public ParticleSystem bubbleParticleSystem;

	private ParticleSystem.Particle[] bubbleParticleArray;

	public AudioSource bubblerAudio;

	public AudioSource popBubbleAudio;

	private List<uint> currentParticles = new List<uint>();

	private Dictionary<uint, Vector3> particleInfoDict = new Dictionary<uint, Vector3>();

	private Vector3 outPosition;

	private bool allBubblesPopped;

	public bool disableActivation;

	public bool disableDeactivation;

	public float rotationSpeed = 5f;

	public GameObject fan;

	public float ongoingStrength = 0.005f;

	public float triggerStrength = 0.2f;

	private float initialTriggerPull;

	private float initialTriggerDuration;

	private bool hasBubblerAudio;

	private bool hasPopBubbleAudio;

	public Behaviour[] behavioursToEnableWhenTriggerPressed;

	private bool hasParticleSystem;

	private bool hasFan;

	private enum BubblerState
	{
		None = 1,
		Bubbling
	}
}
