using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class ButterflySwarmManager : MonoBehaviourPun
{
	public float PerchedFlapSpeed
	{
		[CompilerGenerated]
		get
		{
			return this.<PerchedFlapSpeed>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<PerchedFlapSpeed>k__BackingField = value;
		}
	}

	public float PerchedFlapPhase
	{
		[CompilerGenerated]
		get
		{
			return this.<PerchedFlapPhase>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<PerchedFlapPhase>k__BackingField = value;
		}
	}

	public float BeeSpeed
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeSpeed>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeSpeed>k__BackingField = value;
		}
	}

	public float BeeMaxTravelTime
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeMaxTravelTime>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeMaxTravelTime>k__BackingField = value;
		}
	}

	public float BeeAcceleration
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeAcceleration>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeAcceleration>k__BackingField = value;
		}
	}

	public float BeeJitterStrength
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeJitterStrength>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeJitterStrength>k__BackingField = value;
		}
	}

	public float BeeJitterDamping
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeJitterDamping>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeJitterDamping>k__BackingField = value;
		}
	}

	public float BeeMaxJitterRadius
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeMaxJitterRadius>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeMaxJitterRadius>k__BackingField = value;
		}
	}

	public float BeeNearDestinationRadius
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeNearDestinationRadius>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeNearDestinationRadius>k__BackingField = value;
		}
	}

	public float DestRotationAlignmentSpeed
	{
		[CompilerGenerated]
		get
		{
			return this.<DestRotationAlignmentSpeed>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<DestRotationAlignmentSpeed>k__BackingField = value;
		}
	}

	public Vector3 TravellingLocalRotationEuler
	{
		[CompilerGenerated]
		get
		{
			return this.<TravellingLocalRotationEuler>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<TravellingLocalRotationEuler>k__BackingField = value;
		}
	}

	public Quaternion TravellingLocalRotation
	{
		[CompilerGenerated]
		get
		{
			return this.<TravellingLocalRotation>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<TravellingLocalRotation>k__BackingField = value;
		}
	}

	public float AvoidPointRadius
	{
		[CompilerGenerated]
		get
		{
			return this.<AvoidPointRadius>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<AvoidPointRadius>k__BackingField = value;
		}
	}

	public float BeeMinFlowerDuration
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeMinFlowerDuration>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeMinFlowerDuration>k__BackingField = value;
		}
	}

	public float BeeMaxFlowerDuration
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeMaxFlowerDuration>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeMaxFlowerDuration>k__BackingField = value;
		}
	}

	public Color[] BeeColors
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeColors>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeColors>k__BackingField = value;
		}
	}

	private void Awake()
	{
		this.TravellingLocalRotation = Quaternion.Euler(this.TravellingLocalRotationEuler);
		this.butterflies = new List<AnimatedButterfly>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedButterfly animatedButterfly = default(AnimatedButterfly);
			animatedButterfly.InitVisual(this.beePrefab, this);
			if (this.BeeColors.Length != 0)
			{
				animatedButterfly.SetColor(this.BeeColors[i % this.BeeColors.Length]);
			}
			this.butterflies.Add(animatedButterfly);
		}
	}

	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.perchSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				List<GameObject> list = new List<GameObject>();
				this.allPerchZones.Add(list);
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	private void Update()
	{
		for (int i = 0; i < this.butterflies.Count; i++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[i];
			animatedButterfly.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			this.butterflies[i] = animatedButterfly;
		}
	}

	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<List<GameObject>> list = new List<List<GameObject>>(this.allPerchZones.Count);
		for (int i = 0; i < this.allPerchZones.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			list2.AddRange(this.allPerchZones[i]);
			list.Add(list2);
		}
		List<GameObject> list3 = new List<GameObject>(this.loopSizePerBee);
		List<float> list4 = new List<float>(this.loopSizePerBee);
		for (int j = 0; j < this.butterflies.Count; j++)
		{
			AnimatedButterfly animatedButterfly = this.butterflies[j];
			animatedButterfly.SetFlapSpeed(srand.NextFloat(this.minFlapSpeed, this.maxFlapSpeed));
			list3.Clear();
			list4.Clear();
			this.PickPoints(this.loopSizePerBee, list, ref srand, list3);
			for (int k = 0; k < list3.Count; k++)
			{
				list4.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			if (list3.Count == 0)
			{
				this.butterflies.Clear();
				return;
			}
			animatedButterfly.InitRoute(list3, list4, this);
			this.butterflies[j] = animatedButterfly;
		}
	}

	private void PickPoints(int n, List<List<GameObject>> pickBuffer, ref SRand rand, List<GameObject> resultBuffer)
	{
		int num = rand.NextInt(0, pickBuffer.Count);
		int num2 = -1;
		int num3 = n - 2;
		while (resultBuffer.Count < n)
		{
			int num4;
			if (resultBuffer.Count < num3)
			{
				num4 = rand.NextIntWithExclusion(0, pickBuffer.Count, num2);
			}
			else
			{
				num4 = rand.NextIntWithExclusion2(0, pickBuffer.Count, num2, num);
			}
			int num5 = 10;
			while (num4 == num2 || pickBuffer[num4].Count == 0)
			{
				num4 = (num4 + 1) % pickBuffer.Count;
				num5--;
				if (num5 <= 0)
				{
					return;
				}
			}
			num2 = num4;
			List<GameObject> list = pickBuffer[num2];
			while (list.Count == 0)
			{
				num2 = (num2 + 1) % pickBuffer.Count;
				list = pickBuffer[num2];
			}
			resultBuffer.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
	}

	public ButterflySwarmManager()
	{
	}

	[SerializeField]
	private XSceneRef[] perchSections;

	[SerializeField]
	private int loopSizePerBee;

	[SerializeField]
	private int numBees;

	[SerializeField]
	private MeshRenderer beePrefab;

	[SerializeField]
	private float maxFlapSpeed;

	[SerializeField]
	private float minFlapSpeed;

	[CompilerGenerated]
	[SerializeField]
	private float <PerchedFlapSpeed>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <PerchedFlapPhase>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeSpeed>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeMaxTravelTime>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeAcceleration>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeJitterStrength>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	[Tooltip("Should be 0-1; closer to 1 = less damping")]
	private float <BeeJitterDamping>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	[Tooltip("Limits how far the bee can get off course")]
	private float <BeeMaxJitterRadius>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	[Tooltip("Bees stop jittering when close to their destination")]
	private float <BeeNearDestinationRadius>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	[Tooltip(">0 to get butterflies to align to their destination rotation as they land")]
	private float <DestRotationAlignmentSpeed>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	[Tooltip("Model orientation relative to the direction vector while flying")]
	private Vector3 <TravellingLocalRotationEuler>k__BackingField;

	[CompilerGenerated]
	private Quaternion <TravellingLocalRotation>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <AvoidPointRadius>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeMinFlowerDuration>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeMaxFlowerDuration>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private Color[] <BeeColors>k__BackingField;

	private List<AnimatedButterfly> butterflies;

	private List<List<GameObject>> allPerchZones = new List<List<GameObject>>();
}
