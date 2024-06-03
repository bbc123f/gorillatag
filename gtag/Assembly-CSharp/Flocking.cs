using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Flocking : MonoBehaviour
{
	public FlockingManager.FishArea FishArea
	{
		[CompilerGenerated]
		get
		{
			return this.<FishArea>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<FishArea>k__BackingField = value;
		}
	}

	private void Awake()
	{
		this.manager = base.GetComponentInParent<FlockingManager>();
	}

	private void Start()
	{
		this.speed = Random.Range(this.minSpeed, this.maxSpeed);
		this.fishState = Flocking.FishState.patrol;
	}

	private void OnDisable()
	{
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<SlingshotProjectile, bool>)Delegate.Remove(flockingManager.onFoodDetected, new UnityAction<SlingshotProjectile, bool>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction)Delegate.Remove(flockingManager2.onFoodDestroyed, new UnityAction(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.UnregisterFlocking(this);
	}

	public void InvokeUpdate()
	{
		this.AvoidPlayerHands();
		this.MaybeTurn();
		switch (this.fishState)
		{
		case Flocking.FishState.flock:
			this.Flock(this.FishArea.nextWaypoint);
			this.SwitchState(Flocking.FishState.patrol);
			break;
		case Flocking.FishState.patrol:
			if (Random.Range(0, 10) < 2)
			{
				this.SwitchState(Flocking.FishState.flock);
			}
			break;
		case Flocking.FishState.followFood:
			if (this.isTurning)
			{
				return;
			}
			if (this.isRealFood)
			{
				if ((double)Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFoodStopDistance)
				{
					this.FollowFood();
				}
				else
				{
					this.followingFood = false;
					this.Flock(this.projectileGameObject.transform.position);
					this.feedingTimeStarted += Time.deltaTime;
					if (this.feedingTimeStarted > this.eatFoodDuration)
					{
						this.SwitchState(Flocking.FishState.patrol);
					}
				}
			}
			else if (Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFakeFoodStopDistance)
			{
				this.FollowFood();
			}
			else
			{
				this.followingFood = false;
				this.SwitchState(Flocking.FishState.patrol);
			}
			break;
		}
		if (!this.followingFood)
		{
			base.transform.Translate(0f, 0f, this.speed * Time.deltaTime);
		}
		this.pos = base.transform.position;
		this.rot = base.transform.rotation;
	}

	private void MaybeTurn()
	{
		if (!this.manager.IsInside(base.transform.position, this.FishArea))
		{
			this.Turn(this.FishArea.colliderCenter);
			if (Vector3.Angle(this.FishArea.colliderCenter - base.transform.position, Vector3.forward) > 5f)
			{
				this.isTurning = true;
				return;
			}
		}
		else
		{
			this.isTurning = false;
		}
	}

	private void Turn(Vector3 towardPoint)
	{
		this.isTurning = true;
		Quaternion to = Quaternion.LookRotation(towardPoint - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.rotationSpeed * Time.deltaTime);
	}

	private void SwitchState(Flocking.FishState state)
	{
		this.fishState = state;
	}

	private void Flock(Vector3 nextGoal)
	{
		Vector3 a = Vector3.zero;
		Vector3 vector = Vector3.zero;
		float num = 1f;
		int num2 = 0;
		foreach (Flocking flocking in this.FishArea.fishList)
		{
			if (flocking.gameObject != base.gameObject)
			{
				float num3 = Vector3.Distance(flocking.transform.position, base.transform.position);
				if (num3 <= this.maxNeighbourDistance)
				{
					a += flocking.transform.position;
					num2++;
					if (num3 < this.flockingAvoidanceDistance)
					{
						vector += base.transform.position - flocking.transform.position;
					}
					num += flocking.speed;
				}
			}
		}
		if (num2 > 0)
		{
			this.fishState = Flocking.FishState.flock;
			a = a / (float)num2 + (nextGoal - base.transform.position);
			this.speed = num / (float)num2;
			this.speed = Mathf.Clamp(this.speed, this.minSpeed, this.maxSpeed);
			Vector3 vector2 = a + vector - base.transform.position;
			if (vector2 != Vector3.zero)
			{
				Quaternion to = Quaternion.LookRotation(vector2);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.rotationSpeed * Time.deltaTime);
			}
		}
	}

	private void HandleOnFoodDetected(SlingshotProjectile projectile, bool _isRealFood)
	{
		this.SwitchState(Flocking.FishState.followFood);
		this.feedingTimeStarted = 0f;
		this.projectileGameObject = projectile.gameObject;
		this.isRealFood = _isRealFood;
	}

	private void HandleOnFoodDestroyed()
	{
		this.SwitchState(Flocking.FishState.patrol);
		this.projectileGameObject = null;
		this.followingFood = false;
	}

	private void FollowFood()
	{
		this.followingFood = true;
		Quaternion to = Quaternion.LookRotation(this.projectileGameObject.transform.position - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, this.rotationSpeed * Time.deltaTime);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.projectileGameObject.transform.position, this.speed * this.followFoodSpeedMult * Time.deltaTime);
	}

	private void AvoidPlayerHands()
	{
		foreach (GameObject gameObject in FlockingManager.avoidPoints)
		{
			Vector3 position = gameObject.transform.position;
			if ((base.transform.position - position).IsShorterThan(this.avointPointRadius))
			{
				this.isAvoindingHand = true;
				Vector3 randomPointInsideCollider = this.manager.GetRandomPointInsideCollider(this.FishArea);
				this.Turn(randomPointInsideCollider);
				this.speed = this.avoidHandSpeed;
			}
			this.isAvoindingHand = false;
		}
	}

	internal void SetSyncPosRot(Vector3 syncPos, Quaternion syncRot)
	{
		if (syncRot.IsValid())
		{
			this.rot = syncRot;
		}
		if (syncPos.IsValid())
		{
			this.pos = this.manager.RestrictPointToArea(syncPos, this.FishArea);
		}
	}

	private void OnEnable()
	{
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<SlingshotProjectile, bool>)Delegate.Combine(flockingManager.onFoodDetected, new UnityAction<SlingshotProjectile, bool>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction)Delegate.Combine(flockingManager2.onFoodDestroyed, new UnityAction(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.RegisterFlocking(this);
	}

	public Flocking()
	{
	}

	[Tooltip("Speed is randomly generated from min and max speed")]
	public float minSpeed = 2f;

	public float maxSpeed = 4f;

	public float rotationSpeed = 360f;

	[Tooltip("Maximum distance to the neighbours to form a flocking group")]
	public float maxNeighbourDistance = 4f;

	public float eatFoodDuration = 10f;

	[Tooltip("How fast should it follow the food? This value multiplies by the current speed")]
	public float followFoodSpeedMult = 3f;

	[Tooltip("How fast should it run away from players hand?")]
	public float avoidHandSpeed = 1.2f;

	[FormerlySerializedAs("avoidanceDistance")]
	[Tooltip("When flocking they will avoid each other if the distance between them is less than this value")]
	public float flockingAvoidanceDistance = 2f;

	[Tooltip("Follow the fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFood")]
	public double FollowFoodStopDistance = 0.20000000298023224;

	[Tooltip("Follow any fake fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFakeFood")]
	public float FollowFakeFoodStopDistance = 2f;

	private float speed;

	private Vector3 averageHeading;

	private Vector3 averagePosition;

	private float feedingTimeStarted;

	private GameObject projectileGameObject;

	private bool followingFood;

	private FlockingManager manager;

	private GameObjectManagerWithId _fishSceneGameObjectsManager;

	private UnityEvent<string, Transform> sendIdEvent;

	private Flocking.FishState fishState;

	[HideInInspector]
	public Vector3 pos;

	[HideInInspector]
	public Quaternion rot;

	private float velocity;

	private bool isTurning;

	private bool isRealFood;

	public float avointPointRadius = 0.5f;

	private float cacheSpeed;

	private bool isAvoindingHand;

	[CompilerGenerated]
	private FlockingManager.FishArea <FishArea>k__BackingField;

	public enum FishState
	{
		flock,
		patrol,
		followFood
	}
}
