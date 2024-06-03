using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

public class BeeSwarmManager : MonoBehaviourPun
{
	public BeePerchPoint BeeHive
	{
		[CompilerGenerated]
		get
		{
			return this.<BeeHive>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<BeeHive>k__BackingField = value;
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

	public float GeneralBuzzRange
	{
		[CompilerGenerated]
		get
		{
			return this.<GeneralBuzzRange>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<GeneralBuzzRange>k__BackingField = value;
		}
	}

	private void Awake()
	{
		this.bees = new List<AnimatedBee>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedBee item = default(AnimatedBee);
			item.InitVisual(this.beePrefab, this);
			this.bees.Add(item);
		}
		this.playerCamera = Camera.main.transform;
	}

	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.flowerSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				foreach (BeePerchPoint item in gameObject.GetComponentsInChildren<BeePerchPoint>())
				{
					this.allPerchPoints.Add(item);
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
		Vector3 position = this.playerCamera.transform.position;
		Vector3 position2 = Vector3.zero;
		Vector3 a = Vector3.zero;
		float num = 1f / (float)this.bees.Count;
		float num2 = float.PositiveInfinity;
		float num3 = this.GeneralBuzzRange * this.GeneralBuzzRange;
		int num4 = 0;
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee animatedBee = this.bees[i];
			animatedBee.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			Vector3 position3 = animatedBee.visual.transform.position;
			float sqrMagnitude = (position3 - position).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				position2 = position3;
				num2 = sqrMagnitude;
			}
			if (sqrMagnitude < num3)
			{
				a += position3;
				num4++;
			}
			this.bees[i] = animatedBee;
		}
		this.nearbyBeeBuzz.transform.position = position2;
		if (num4 > 0)
		{
			this.generalBeeBuzz.transform.position = a / (float)num4;
			this.generalBeeBuzz.enabled = true;
			return;
		}
		this.generalBeeBuzz.enabled = false;
	}

	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<BeePerchPoint> pickBuffer = new List<BeePerchPoint>(this.allPerchPoints.Count);
		List<BeePerchPoint> list = new List<BeePerchPoint>(this.loopSizePerBee);
		List<float> list2 = new List<float>(this.loopSizePerBee);
		for (int i = 0; i < this.bees.Count; i++)
		{
			AnimatedBee value = this.bees[i];
			list = new List<BeePerchPoint>(this.loopSizePerBee);
			list2 = new List<float>(this.loopSizePerBee);
			this.PickPoints(this.loopSizePerBee, pickBuffer, this.allPerchPoints, ref srand, list);
			for (int j = 0; j < list.Count; j++)
			{
				list2.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			value.InitRoute(list, list2, this);
			value.InitRouteTimestamps();
			this.bees[i] = value;
		}
	}

	private void PickPoints(int n, List<BeePerchPoint> pickBuffer, List<BeePerchPoint> allPerchPoints, ref SRand rand, List<BeePerchPoint> resultBuffer)
	{
		resultBuffer.Add(this.BeeHive);
		n--;
		int num = 100;
		while (pickBuffer.Count < n && num-- > 0)
		{
			n -= pickBuffer.Count;
			resultBuffer.AddRange(pickBuffer);
			pickBuffer.Clear();
			pickBuffer.AddRange(allPerchPoints);
			rand.Shuffle<BeePerchPoint>(pickBuffer);
		}
		resultBuffer.AddRange(pickBuffer.GetRange(pickBuffer.Count - n, n));
		pickBuffer.RemoveRange(pickBuffer.Count - n, n);
	}

	public static void RegisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Add(obj);
	}

	public static void UnregisterAvoidPoint(GameObject obj)
	{
		BeeSwarmManager.avoidPoints.Remove(obj);
	}

	public BeeSwarmManager()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static BeeSwarmManager()
	{
	}

	[SerializeField]
	private XSceneRef[] flowerSections;

	[SerializeField]
	private int loopSizePerBee;

	[SerializeField]
	private int numBees;

	[SerializeField]
	private MeshRenderer beePrefab;

	[SerializeField]
	private AudioSource nearbyBeeBuzz;

	[SerializeField]
	private AudioSource generalBeeBuzz;

	private GameObject[] flowerSectionsResolved;

	[CompilerGenerated]
	[SerializeField]
	private BeePerchPoint <BeeHive>k__BackingField;

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
	private float <AvoidPointRadius>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeMinFlowerDuration>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <BeeMaxFlowerDuration>k__BackingField;

	[CompilerGenerated]
	[SerializeField]
	private float <GeneralBuzzRange>k__BackingField;

	private List<AnimatedBee> bees;

	private Transform playerCamera;

	private List<BeePerchPoint> allPerchPoints = new List<BeePerchPoint>();

	public static readonly List<GameObject> avoidPoints = new List<GameObject>();
}
