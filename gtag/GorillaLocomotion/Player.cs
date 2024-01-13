using System;
using System.Collections.Generic;
using AA;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion;

public class Player : MonoBehaviour
{
	[Serializable]
	public struct MaterialData
	{
		public string matName;

		public bool overrideAudio;

		public AudioClip audio;

		public bool overrideSlidePercent;

		public float slidePercent;
	}

	[Serializable]
	public struct LiquidProperties
	{
		[Range(0f, 2f)]
		[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
		public float resistance;

		[Range(0f, 3f)]
		[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
		public float buoyancy;

		[Range(0f, 3f)]
		[Tooltip("0: no damping. 1: Damping tuned for regular water. 2: greater damping than regular water")]
		public float dampingFactor;

		[Range(0f, 1f)]
		public float surfaceJumpFactor;
	}

	public enum LiquidType
	{
		Water
	}

	private static Player _instance;

	public static bool hasInstance;

	public SphereCollider headCollider;

	public CapsuleCollider bodyCollider;

	private float bodyInitialRadius;

	private float bodyInitialHeight;

	private RaycastHit bodyHitInfo;

	private RaycastHit lastHitInfoHand;

	public Transform leftHandFollower;

	public Transform rightHandFollower;

	public Transform rightControllerTransform;

	public Transform leftControllerTransform;

	public GorillaVelocityTracker rightHandCenterVelocityTracker;

	public GorillaVelocityTracker leftHandCenterVelocityTracker;

	public GorillaVelocityTracker rightInteractPointVelocityTracker;

	public GorillaVelocityTracker leftInteractPointVelocityTracker;

	public PlayerAudioManager audioManager;

	private Vector3 lastLeftHandPosition;

	private Vector3 lastRightHandPosition;

	public Vector3 lastHeadPosition;

	private Vector3 lastRigidbodyPosition;

	private Rigidbody playerRigidBody;

	public int velocityHistorySize;

	public float maxArmLength = 1f;

	public float unStickDistance = 1f;

	public float velocityLimit;

	public float slideVelocityLimit;

	public float maxJumpSpeed;

	public float jumpMultiplier;

	public float minimumRaycastDistance = 0.05f;

	public float defaultSlideFactor = 0.03f;

	public float slidingMinimum = 0.9f;

	public float defaultPrecision = 0.995f;

	public float teleportThresholdNoVel = 1f;

	public float frictionConstant = 1f;

	public float slideControl = 0.00425f;

	public float stickDepth = 0.01f;

	private Vector3[] velocityHistory;

	private Vector3[] slideAverageHistory;

	private int velocityIndex;

	public Vector3 currentVelocity;

	private Vector3 denormalizedVelocityAverage;

	private Vector3 lastPosition;

	public Vector3 rightHandOffset;

	public Vector3 leftHandOffset;

	public Quaternion rightHandRotOffset = Quaternion.identity;

	public Quaternion leftHandRotOffset = Quaternion.identity;

	public Vector3 bodyOffset;

	public LayerMask locomotionEnabledLayers;

	public LayerMask waterLayer;

	public bool wasLeftHandTouching;

	public bool wasRightHandTouching;

	public bool wasHeadTouching;

	public int currentMaterialIndex;

	public bool leftHandSlide;

	public Vector3 leftHandSlideNormal;

	public bool rightHandSlide;

	public Vector3 rightHandSlideNormal;

	public Vector3 headSlideNormal;

	public float rightHandSlipPercentage;

	public float leftHandSlipPercentage;

	public float headSlipPercentage;

	public bool wasLeftHandSlide;

	public bool wasRightHandSlide;

	public Vector3 rightHandHitPoint;

	public Vector3 leftHandHitPoint;

	public float scale = 1f;

	public bool debugMovement;

	public bool disableMovement;

	[NonSerialized]
	public bool inOverlay;

	[NonSerialized]
	public bool isUserPresent;

	public GameObject turnParent;

	public int leftHandMaterialTouchIndex;

	public GorillaSurfaceOverride leftHandSurfaceOverride;

	public int rightHandMaterialTouchIndex;

	public GorillaSurfaceOverride rightHandSurfaceOverride;

	public GorillaSurfaceOverride currentOverride;

	public MaterialDatasSO materialDatasSO;

	private bool leftHandColliding;

	private bool rightHandColliding;

	private bool headColliding;

	private float degreesTurnedThisFrame;

	private Vector3 finalPosition;

	private Vector3 rigidBodyMovement;

	private Vector3 leftHandPushDisplacement;

	private Vector3 rightHandPushDisplacement;

	private RaycastHit hitInfo;

	private RaycastHit iterativeHitInfo;

	private RaycastHit collisionsInnerHit;

	private float slipPercentage;

	private Vector3 bodyOffsetVector;

	private Vector3 distanceTraveled;

	private Vector3 movementToProjectedAboveCollisionPlane;

	private MeshCollider meshCollider;

	private Mesh collidedMesh;

	private MaterialData foundMatData;

	private string findMatName;

	private int vertex1;

	private int vertex2;

	private int vertex3;

	private List<int> trianglesList = new List<int>(1000000);

	private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

	private int[] sharedMeshTris;

	private float lastRealTime;

	private float calcDeltaTime;

	private float tempRealTime;

	private Vector3 junkNormal;

	private Vector3 slideAverage;

	private Vector3 slideAverageNormal;

	private Vector3 tempVector3;

	private RaycastHit tempHitInfo;

	private RaycastHit junkHit;

	private Vector3 firstPosition;

	private RaycastHit tempIterativeHit;

	private bool collisionsReturnBool;

	private float overlapRadiusFunction;

	private float maxSphereSize1;

	private float maxSphereSize2;

	private Collider[] overlapColliders = new Collider[10];

	private int overlapAttempts;

	private int touchPoints;

	private float averageSlipPercentage;

	private Vector3 surfaceDirection;

	public float iceThreshold = 0.9f;

	private float bodyMaxRadius;

	public float bodyLerp = 0.17f;

	private bool areBothTouching;

	private float slideFactor;

	[DebugOption]
	public bool didAJump;

	private Renderer slideRenderer;

	private RaycastHit[] rayCastNonAllocColliders;

	private Vector3[] crazyCheckVectors;

	private RaycastHit emptyHit;

	private int bufferCount;

	private Vector3 lastOpenHeadPosition;

	private List<Material> tempMaterialArray = new List<Material>(16);

	private int disableGripFrameIdx = -1;

	[Header("Swimming")]
	public PlayerSwimmingParameters swimmingParams;

	public WaterParameters waterParams;

	public List<LiquidProperties> liquidPropertiesList = new List<LiquidProperties>(16);

	public bool debugDrawSwimming;

	[Header("Slam/Hit effects")]
	public GameObject wizardStaffSlamEffects;

	public GameObject geodeHitEffects;

	private WaterVolume leftHandWaterVolume;

	private WaterVolume rightHandWaterVolume;

	private WaterVolume.SurfaceQuery leftHandWaterSurface;

	private WaterVolume.SurfaceQuery rightHandWaterSurface;

	private Vector3 swimmingVelocity = Vector3.zero;

	private WaterVolume.SurfaceQuery waterSurfaceForHead;

	private bool bodyInWater;

	private bool headInWater;

	private float buoyancyExtension;

	private float lastWaterSurfaceJumpTimeLeft = -1f;

	private float lastWaterSurfaceJumpTimeRight = -1f;

	private float waterSurfaceJumpCooldown = 0.1f;

	private float leftHandNonDiveHapticsAmount;

	private float rightHandNonDiveHapticsAmount;

	private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

	private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

	private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

	private Platform currentPlatform;

	private Platform lastPlatformTouched;

	private Vector3 currentFrameTouchPos;

	private Vector3 lastFrameTouchPosLocal;

	private Vector3 lastFrameTouchPosWorld;

	private Vector3 refMovement = Vector3.zero;

	private Vector3 platformTouchOffset;

	private Vector3 debugLastRightHandPosition;

	private Vector3 debugPlatformDeltaPosition;

	private const float climbingMaxThrowSpeed = 5.5f;

	private const float climbHelperSmoothSnapSpeed = 12f;

	[SerializeField]
	private float velocityThrowClimbingMultiplier = 1f;

	[SerializeField]
	private float climbingMaxThrowMagnitude = 10f;

	[NonSerialized]
	public bool isClimbing;

	private GorillaClimbable currentClimbable;

	private GorillaHandClimber currentClimber;

	private Vector3 climbHelperTargetPos = Vector3.zero;

	private Transform climbHelper;

	private GorillaRopeSwing currentSwing;

	private GorillaZipline currentZipline;

	[SerializeField]
	private ConnectedControllerHandler controllerState;

	public int sizeLayerMask;

	public bool InReportMenu;

	public static Player Instance => _instance;

	public bool turnedThisFrame => degreesTurnedThisFrame != 0f;

	public List<MaterialData> materialData => materialDatasSO.datas;

	public List<WaterVolume> HeadOverlappingWaterVolumes => headOverlappingWaterVolumes;

	public bool InWater => bodyInWater;

	public bool HeadInWater => headInWater;

	public WaterVolume CurrentWaterVolume
	{
		get
		{
			if (bodyOverlappingWaterVolumes.Count <= 0)
			{
				return null;
			}
			return bodyOverlappingWaterVolumes[0];
		}
	}

	public WaterVolume.SurfaceQuery WaterSurfaceForHead => waterSurfaceForHead;

	public WaterVolume LeftHandWaterVolume => leftHandWaterVolume;

	public WaterVolume RightHandWaterVolume => rightHandWaterVolume;

	public WaterVolume.SurfaceQuery LeftHandWaterSurface => leftHandWaterSurface;

	public WaterVolume.SurfaceQuery RightHandWaterSurface => rightHandWaterSurface;

	public Vector3 LastLeftHandPosition => lastLeftHandPosition;

	public Vector3 LastRightHandPosition => lastRightHandPosition;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
			hasInstance = true;
		}
		InitializeValues();
		playerRigidBody.maxAngularVelocity = 0f;
		bodyOffsetVector = new Vector3(0f, (0f - bodyCollider.height) / 2f, 0f);
		bodyInitialHeight = bodyCollider.height;
		bodyInitialRadius = bodyCollider.radius;
		rayCastNonAllocColliders = new RaycastHit[5];
		crazyCheckVectors = new Vector3[7];
		emptyHit = default(RaycastHit);
		crazyCheckVectors[0] = Vector3.up;
		crazyCheckVectors[1] = Vector3.down;
		crazyCheckVectors[2] = Vector3.left;
		crazyCheckVectors[3] = Vector3.right;
		crazyCheckVectors[4] = Vector3.forward;
		crazyCheckVectors[5] = Vector3.back;
		crazyCheckVectors[6] = Vector3.zero;
		if (controllerState == null)
		{
			controllerState = GetComponent<ConnectedControllerHandler>();
		}
	}

	protected void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
			hasInstance = false;
		}
		if ((bool)climbHelper)
		{
			UnityEngine.Object.Destroy(climbHelper.gameObject);
		}
	}

	public void InitializeValues()
	{
		Physics.SyncTransforms();
		playerRigidBody = GetComponent<Rigidbody>();
		velocityHistory = new Vector3[velocityHistorySize];
		slideAverageHistory = new Vector3[velocityHistorySize];
		for (int i = 0; i < velocityHistory.Length; i++)
		{
			velocityHistory[i] = Vector3.zero;
			slideAverageHistory[i] = Vector3.zero;
		}
		leftHandFollower.transform.position = leftControllerTransform.position;
		rightHandFollower.transform.position = rightControllerTransform.position;
		lastLeftHandPosition = leftHandFollower.transform.position;
		lastRightHandPosition = rightHandFollower.transform.position;
		lastHeadPosition = headCollider.transform.position;
		wasLeftHandTouching = false;
		wasRightHandTouching = false;
		velocityIndex = 0;
		denormalizedVelocityAverage = Vector3.zero;
		slideAverage = Vector3.zero;
		lastPosition = base.transform.position;
		lastRealTime = Time.realtimeSinceStartup;
		lastOpenHeadPosition = headCollider.transform.position;
		bodyCollider.transform.position = PositionWithOffset(headCollider.transform, bodyOffset) + bodyOffsetVector;
		bodyCollider.transform.eulerAngles = new Vector3(0f, headCollider.transform.eulerAngles.y, 0f);
	}

	public void FixedUpdate()
	{
		AntiTeleportTechnology();
		if (scale != 1f)
		{
			playerRigidBody.AddForce(-Physics.gravity * (1f - scale), ForceMode.Acceleration);
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		bodyInWater = false;
		Vector3 lhs = swimmingVelocity;
		swimmingVelocity = Vector3.MoveTowards(swimmingVelocity, Vector3.zero, swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
		leftHandNonDiveHapticsAmount = 0f;
		rightHandNonDiveHapticsAmount = 0f;
		if (bodyOverlappingWaterVolumes.Count <= 0)
		{
			return;
		}
		WaterVolume waterVolume = null;
		float num = float.MinValue;
		Vector3 vector = headCollider.transform.position + Vector3.down * swimmingParams.floatingWaterLevelBelowHead;
		activeWaterCurrents.Clear();
		for (int i = 0; i < bodyOverlappingWaterVolumes.Count; i++)
		{
			if (bodyOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out var result))
			{
				float num2 = Vector3.Dot(result.surfacePoint - vector, result.surfaceNormal);
				if (num2 > num)
				{
					num = num2;
					waterVolume = bodyOverlappingWaterVolumes[i];
					waterSurfaceForHead = result;
				}
				WaterCurrent current = bodyOverlappingWaterVolumes[i].Current;
				if (current != null && num2 > 0f && !activeWaterCurrents.Contains(current))
				{
					activeWaterCurrents.Add(current);
				}
			}
		}
		if (!(waterVolume != null))
		{
			return;
		}
		Vector3 velocity = playerRigidBody.velocity;
		float magnitude = velocity.magnitude;
		bool flag = headInWater;
		headInWater = headCollider.transform.position.y < waterSurfaceForHead.surfacePoint.y && headCollider.transform.position.y > waterSurfaceForHead.surfacePoint.y - waterSurfaceForHead.maxDepth;
		if (headInWater && !flag)
		{
			audioManager.SetMixerSnapshot(audioManager.underwaterSnapshot);
		}
		else if (!headInWater && flag)
		{
			audioManager.UnsetMixerSnapshot();
		}
		bodyInWater = vector.y < waterSurfaceForHead.surfacePoint.y && vector.y > waterSurfaceForHead.surfacePoint.y - waterSurfaceForHead.maxDepth;
		if (!bodyInWater)
		{
			return;
		}
		LiquidProperties liquidProperties = liquidPropertiesList[(int)waterVolume.LiquidType];
		if (waterVolume != null)
		{
			float num4;
			if (swimmingParams.extendBouyancyFromSpeed)
			{
				float time = Mathf.Clamp(Vector3.Dot(velocity, waterSurfaceForHead.surfaceNormal), swimmingParams.speedToBouyancyExtensionMinMax.x, swimmingParams.speedToBouyancyExtensionMinMax.y);
				float b = swimmingParams.speedToBouyancyExtension.Evaluate(time);
				buoyancyExtension = Mathf.Max(buoyancyExtension, b);
				float num3 = Mathf.InverseLerp(0f, swimmingParams.buoyancyFadeDist + buoyancyExtension, num + buoyancyExtension);
				buoyancyExtension = Spring.DamperDecayExact(buoyancyExtension, swimmingParams.buoyancyExtensionDecayHalflife, fixedDeltaTime);
				num4 = num3;
			}
			else
			{
				num4 = Mathf.InverseLerp(0f, swimmingParams.buoyancyFadeDist, num);
			}
			Vector3 vector2 = Physics.gravity * scale;
			Vector3 force = liquidProperties.buoyancy * -vector2 * num4;
			playerRigidBody.AddForce(force, ForceMode.Acceleration);
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		for (int j = 0; j < activeWaterCurrents.Count; j++)
		{
			if (activeWaterCurrents[j].GetCurrentAtPoint(startingVelocity: velocity + zero, worldPoint: bodyCollider.transform.position, dt: fixedDeltaTime, currentVelocity: out var vector3, velocityChange: out var velocityChange))
			{
				zero2 += vector3;
				zero += velocityChange;
			}
		}
		if (!(magnitude > Mathf.Epsilon))
		{
			return;
		}
		float num5 = 0.01f;
		Vector3 vector4 = velocity / magnitude;
		Vector3 right = leftHandFollower.right;
		Vector3 dir = -rightHandFollower.right;
		Vector3 forward = leftHandFollower.forward;
		Vector3 forward2 = rightHandFollower.forward;
		Vector3 vector5 = vector4;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		if (swimmingParams.applyDiveSteering && !disableMovement)
		{
			float value = Vector3.Dot(velocity - zero2, vector4);
			float time2 = Mathf.Clamp(value, swimmingParams.swimSpeedToRedirectAmountMinMax.x, swimmingParams.swimSpeedToRedirectAmountMinMax.y);
			float b2 = swimmingParams.swimSpeedToRedirectAmount.Evaluate(time2);
			time2 = Mathf.Clamp(value, swimmingParams.swimSpeedToMaxRedirectAngleMinMax.x, swimmingParams.swimSpeedToMaxRedirectAngleMinMax.y);
			float num9 = swimmingParams.swimSpeedToMaxRedirectAngle.Evaluate(time2);
			float value2 = Mathf.Acos(Vector3.Dot(vector4, forward)) / (float)Math.PI * -2f + 1f;
			float value3 = Mathf.Acos(Vector3.Dot(vector4, forward2)) / (float)Math.PI * -2f + 1f;
			float num10 = Mathf.Clamp(value2, swimmingParams.palmFacingToRedirectAmountMinMax.x, swimmingParams.palmFacingToRedirectAmountMinMax.y);
			float num11 = Mathf.Clamp(value3, swimmingParams.palmFacingToRedirectAmountMinMax.x, swimmingParams.palmFacingToRedirectAmountMinMax.y);
			float a = ((!float.IsNaN(num10)) ? swimmingParams.palmFacingToRedirectAmount.Evaluate(num10) : 0f);
			float a2 = ((!float.IsNaN(num11)) ? swimmingParams.palmFacingToRedirectAmount.Evaluate(num11) : 0f);
			Vector3 vector6 = Vector3.ProjectOnPlane(vector4, right);
			Vector3 vector7 = Vector3.ProjectOnPlane(vector4, right);
			float num12 = Mathf.Min(vector6.magnitude, 1f);
			float num13 = Mathf.Min(vector7.magnitude, 1f);
			float magnitude2 = leftHandCenterVelocityTracker.GetAverageVelocity(worldSpace: false, swimmingParams.diveVelocityAveragingWindow).magnitude;
			float magnitude3 = rightHandCenterVelocityTracker.GetAverageVelocity(worldSpace: false, swimmingParams.diveVelocityAveragingWindow).magnitude;
			float time3 = Mathf.Clamp(magnitude2, swimmingParams.handSpeedToRedirectAmountMinMax.x, swimmingParams.handSpeedToRedirectAmountMinMax.y);
			float time4 = Mathf.Clamp(magnitude3, swimmingParams.handSpeedToRedirectAmountMinMax.x, swimmingParams.handSpeedToRedirectAmountMinMax.y);
			float a3 = swimmingParams.handSpeedToRedirectAmount.Evaluate(time3);
			float a4 = swimmingParams.handSpeedToRedirectAmount.Evaluate(time4);
			float averageSpeedChangeMagnitudeInDirection = leftHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, worldSpace: false, swimmingParams.diveVelocityAveragingWindow);
			float averageSpeedChangeMagnitudeInDirection2 = rightHandCenterVelocityTracker.GetAverageSpeedChangeMagnitudeInDirection(dir, worldSpace: false, swimmingParams.diveVelocityAveragingWindow);
			float time5 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection, swimmingParams.handAccelToRedirectAmountMinMax.x, swimmingParams.handAccelToRedirectAmountMinMax.y);
			float time6 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection2, swimmingParams.handAccelToRedirectAmountMinMax.x, swimmingParams.handAccelToRedirectAmountMinMax.y);
			float b3 = swimmingParams.handAccelToRedirectAmount.Evaluate(time5);
			float b4 = swimmingParams.handAccelToRedirectAmount.Evaluate(time6);
			num6 = Mathf.Min(a, Mathf.Min(a3, b3));
			float num14 = ((Vector3.Dot(vector4, forward) > 0f) ? (Mathf.Min(num6, b2) * num12) : 0f);
			num7 = Mathf.Min(a2, Mathf.Min(a4, b4));
			float num15 = ((Vector3.Dot(vector4, forward2) > 0f) ? (Mathf.Min(num7, b2) * num13) : 0f);
			if (swimmingParams.reduceDiveSteeringBelowVelocityPlane)
			{
				Vector3 rhs = ((!(Vector3.Dot(headCollider.transform.up, vector4) > 0.95f)) ? Vector3.Cross(Vector3.Cross(vector4, headCollider.transform.up), vector4).normalized : (-headCollider.transform.forward));
				Vector3 position = headCollider.transform.position;
				Vector3 lhs2 = position - leftHandFollower.position;
				Vector3 lhs3 = position - rightHandFollower.position;
				float reduceDiveSteeringBelowPlaneFadeStartDist = swimmingParams.reduceDiveSteeringBelowPlaneFadeStartDist;
				float reduceDiveSteeringBelowPlaneFadeEndDist = swimmingParams.reduceDiveSteeringBelowPlaneFadeEndDist;
				float f = Vector3.Dot(lhs2, Vector3.up);
				float f2 = Vector3.Dot(lhs3, Vector3.up);
				float f3 = Vector3.Dot(lhs2, rhs);
				float f4 = Vector3.Dot(lhs3, rhs);
				float num16 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f), Mathf.Abs(f3)));
				float num17 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(f2), Mathf.Abs(f4)));
				num14 *= num16;
				num15 *= num17;
			}
			float num18 = num15 + num14;
			Vector3 zero3 = Vector3.zero;
			if (swimmingParams.applyDiveSteering && num18 > num5)
			{
				zero3 = ((num14 * vector6 + num15 * vector7) / num18).normalized;
				zero3 = Vector3.Lerp(vector4, zero3, num18);
				vector5 = Vector3.RotateTowards(vector4, zero3, (float)Math.PI / 180f * num9 * fixedDeltaTime, 0f);
			}
			else
			{
				vector5 = vector4;
			}
			num8 = Mathf.Clamp01((num6 + num7) * 0.5f);
		}
		float num19 = Mathf.Clamp(Vector3.Dot(lhs, vector4), 0f, magnitude);
		float num20 = magnitude - num19;
		if (swimmingParams.applyDiveSwimVelocityConversion && !disableMovement && num8 > num5 && num19 < swimmingParams.diveMaxSwimVelocityConversion)
		{
			float num21 = Mathf.Min(swimmingParams.diveSwimVelocityConversionRate * fixedDeltaTime, num20) * num8;
			num19 += num21;
			num20 -= num21;
		}
		float halflife = swimmingParams.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
		float halflife2 = swimmingParams.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
		float num22 = Spring.DamperDecayExact(num19, halflife, fixedDeltaTime);
		float num23 = Spring.DamperDecayExact(num20, halflife2, fixedDeltaTime);
		if (swimmingParams.applyDiveDampingMultiplier && !disableMovement)
		{
			float t = Mathf.Lerp(1f, swimmingParams.diveDampingMultiplier, num8);
			num22 = Mathf.Lerp(num19, num22, t);
			num23 = Mathf.Lerp(num20, num23, t);
			float time7 = Mathf.Clamp((1f - num6) * (num19 + num20), swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num5, swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num5);
			float time8 = Mathf.Clamp((1f - num7) * (num19 + num20), swimmingParams.nonDiveDampingHapticsAmountMinMax.x + num5, swimmingParams.nonDiveDampingHapticsAmountMinMax.y - num5);
			leftHandNonDiveHapticsAmount = swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time7);
			rightHandNonDiveHapticsAmount = swimmingParams.nonDiveDampingHapticsAmount.Evaluate(time8);
		}
		swimmingVelocity = num22 * vector5 + zero;
		playerRigidBody.velocity = swimmingVelocity + num23 * vector5;
	}

	private void BodyCollider()
	{
		if (MaxSphereSizeForNoOverlap(bodyInitialRadius * scale, PositionWithOffset(headCollider.transform, bodyOffset), ignoreOneWay: false, out bodyMaxRadius))
		{
			if (scale > 0f)
			{
				bodyCollider.radius = bodyMaxRadius / scale;
			}
			if (Physics.SphereCast(PositionWithOffset(headCollider.transform, bodyOffset), bodyMaxRadius, Vector3.down, out bodyHitInfo, bodyInitialHeight * scale - bodyMaxRadius, locomotionEnabledLayers))
			{
				bodyCollider.height = (bodyHitInfo.distance + bodyMaxRadius) / scale;
			}
			else
			{
				bodyCollider.height = bodyInitialHeight;
			}
			if (!bodyCollider.gameObject.activeSelf)
			{
				bodyCollider.gameObject.SetActive(value: true);
			}
		}
		else
		{
			bodyCollider.gameObject.SetActive(value: false);
		}
		bodyCollider.height = Mathf.Lerp(bodyCollider.height, bodyInitialHeight, bodyLerp);
		bodyCollider.radius = Mathf.Lerp(bodyCollider.radius, bodyInitialRadius, bodyLerp);
		bodyOffsetVector = Vector3.down * bodyCollider.height / 2f;
		bodyCollider.transform.position = PositionWithOffset(headCollider.transform, bodyOffset) + bodyOffsetVector * scale;
		bodyCollider.transform.eulerAngles = new Vector3(0f, headCollider.transform.eulerAngles.y, 0f);
	}

	private Vector3 GetCurrentHandPosition(Transform handTransform, Vector3 handOffset)
	{
		if (inOverlay)
		{
			return headCollider.transform.position + headCollider.transform.up * -0.5f * scale;
		}
		if ((PositionWithOffset(handTransform, handOffset) - headCollider.transform.position).magnitude < maxArmLength * scale)
		{
			return PositionWithOffset(handTransform, handOffset);
		}
		return headCollider.transform.position + (PositionWithOffset(handTransform, handOffset) - headCollider.transform.position).normalized * maxArmLength * scale;
	}

	private Vector3 GetLastLeftHandPosition()
	{
		return lastLeftHandPosition + MovingSurfaceMovement();
	}

	private Vector3 GetLastRightHandPosition()
	{
		return lastRightHandPosition + MovingSurfaceMovement();
	}

	private Vector3 GetCurrentLeftHandPosition()
	{
		if (inOverlay)
		{
			return headCollider.transform.position + headCollider.transform.up * -0.5f * scale;
		}
		if ((PositionWithOffset(leftControllerTransform, leftHandOffset) - headCollider.transform.position).magnitude < maxArmLength * scale)
		{
			return PositionWithOffset(leftControllerTransform, leftHandOffset);
		}
		return headCollider.transform.position + (PositionWithOffset(leftControllerTransform, leftHandOffset) - headCollider.transform.position).normalized * maxArmLength * scale;
	}

	private Vector3 GetCurrentRightHandPosition()
	{
		if (inOverlay)
		{
			return headCollider.transform.position + headCollider.transform.up * -0.5f * scale;
		}
		if ((PositionWithOffset(rightControllerTransform, rightHandOffset) - headCollider.transform.position).magnitude < maxArmLength * scale)
		{
			return PositionWithOffset(rightControllerTransform, rightHandOffset);
		}
		return headCollider.transform.position + (PositionWithOffset(rightControllerTransform, rightHandOffset) - headCollider.transform.position).normalized * maxArmLength * scale;
	}

	private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
	{
		return transformToModify.position + transformToModify.rotation * offsetVector * scale;
	}

	private void LateUpdate()
	{
		float time = Time.time;
		Vector3 position = headCollider.transform.position;
		turnParent.transform.localScale = Vector3.one * scale;
		playerRigidBody.MovePosition(playerRigidBody.position + position - headCollider.transform.position);
		Camera.main.nearClipPlane = ((scale > 0.5f) ? 0.01f : 0.002f);
		Camera.main.farClipPlane = ((scale > 0.5f) ? 500f : 15f);
		debugLastRightHandPosition = lastRightHandPosition;
		debugPlatformDeltaPosition = MovingSurfaceMovement();
		rigidBodyMovement = Vector3.zero;
		leftHandPushDisplacement = Vector3.zero;
		rightHandPushDisplacement = Vector3.zero;
		currentFrameTouchPos = ComputeWorldHitPoint(lastHitInfoHand, lastFrameTouchPosLocal);
		if (debugMovement)
		{
			tempRealTime = Time.time;
			calcDeltaTime = Time.deltaTime;
			lastRealTime = tempRealTime;
		}
		else
		{
			tempRealTime = Time.realtimeSinceStartup;
			calcDeltaTime = tempRealTime - lastRealTime;
			lastRealTime = tempRealTime;
			if (calcDeltaTime > 0.1f)
			{
				calcDeltaTime = 0.05f;
			}
		}
		if (lastPlatformTouched != null)
		{
			refMovement = currentFrameTouchPos - lastFrameTouchPosWorld;
		}
		if (!didAJump && (wasLeftHandTouching || wasRightHandTouching))
		{
			base.transform.position = base.transform.position + 4.9f * Vector3.down * calcDeltaTime * calcDeltaTime * scale;
			if (Vector3.Dot(denormalizedVelocityAverage, slideAverageNormal) <= 0f && Vector3.Dot(Vector3.down, slideAverageNormal) <= 0f)
			{
				base.transform.position = base.transform.position - Vector3.Project(Mathf.Min(stickDepth * scale, Vector3.Project(denormalizedVelocityAverage, slideAverageNormal).magnitude * calcDeltaTime) * slideAverageNormal, Vector3.down);
			}
		}
		if (!didAJump && (wasLeftHandSlide || wasRightHandSlide))
		{
			base.transform.position = base.transform.position + slideAverage * calcDeltaTime;
			slideAverage += 9.8f * Vector3.down * calcDeltaTime;
		}
		FirstHandIteration(leftControllerTransform, leftHandOffset, GetLastLeftHandPosition(), wasLeftHandSlide, wasLeftHandTouching, out leftHandPushDisplacement, ref leftHandSlipPercentage, ref leftHandSlide, ref leftHandSlideNormal, ref leftHandColliding, ref leftHandMaterialTouchIndex, ref leftHandSurfaceOverride);
		leftHandColliding = leftHandColliding && controllerState.LeftValid;
		leftHandSlide = leftHandSlide && controllerState.LeftValid;
		FirstHandIteration(rightControllerTransform, rightHandOffset, GetLastRightHandPosition(), wasRightHandSlide, wasRightHandTouching, out rightHandPushDisplacement, ref rightHandSlipPercentage, ref rightHandSlide, ref rightHandSlideNormal, ref rightHandColliding, ref rightHandMaterialTouchIndex, ref rightHandSurfaceOverride);
		rightHandColliding = rightHandColliding && controllerState.RightValid;
		rightHandSlide = rightHandSlide && controllerState.RightValid;
		touchPoints = 0;
		rigidBodyMovement = Vector3.zero;
		if (leftHandColliding || wasLeftHandTouching)
		{
			rigidBodyMovement += leftHandPushDisplacement;
			touchPoints++;
		}
		if (rightHandColliding || wasRightHandTouching)
		{
			rigidBodyMovement += rightHandPushDisplacement;
			touchPoints++;
		}
		if (touchPoints != 0)
		{
			rigidBodyMovement /= (float)touchPoints;
		}
		if (!MaxSphereSizeForNoOverlap(headCollider.radius * 0.9f * scale, lastHeadPosition, ignoreOneWay: true, out maxSphereSize1) && !CrazyCheck2(headCollider.radius * 0.9f * 0.75f * scale, lastHeadPosition))
		{
			lastHeadPosition = lastOpenHeadPosition;
		}
		if (IterativeCollisionSphereCast(lastHeadPosition, headCollider.radius * 0.9f * scale, headCollider.transform.position + rigidBodyMovement - lastHeadPosition, out finalPosition, singleHand: false, out slipPercentage, out junkHit, fullSlide: true))
		{
			rigidBodyMovement = finalPosition - headCollider.transform.position;
		}
		if (!MaxSphereSizeForNoOverlap(headCollider.radius * 0.9f * scale, lastHeadPosition + rigidBodyMovement, ignoreOneWay: true, out maxSphereSize1) || !CrazyCheck2(headCollider.radius * 0.9f * 0.75f * scale, lastHeadPosition + rigidBodyMovement))
		{
			lastHeadPosition = lastOpenHeadPosition;
			rigidBodyMovement = lastHeadPosition - headCollider.transform.position;
		}
		else if (headCollider.radius * 0.9f * 0.825f * scale < maxSphereSize1)
		{
			lastOpenHeadPosition = headCollider.transform.position + rigidBodyMovement;
		}
		if (rigidBodyMovement != Vector3.zero)
		{
			base.transform.position += rigidBodyMovement;
		}
		lastHeadPosition = headCollider.transform.position;
		areBothTouching = (!leftHandColliding && !wasLeftHandTouching) || (!rightHandColliding && !wasRightHandTouching);
		Vector3 vector = FinalHandPosition(leftControllerTransform, leftHandOffset, GetLastLeftHandPosition(), areBothTouching, leftHandColliding, out leftHandColliding, leftHandSlide, out leftHandSlide, leftHandMaterialTouchIndex, out leftHandMaterialTouchIndex, leftHandSurfaceOverride, out leftHandSurfaceOverride);
		leftHandColliding = leftHandColliding && controllerState.LeftValid;
		leftHandSlide = leftHandSlide && controllerState.LeftValid;
		Vector3 vector2 = FinalHandPosition(rightControllerTransform, rightHandOffset, GetLastRightHandPosition(), areBothTouching, rightHandColliding, out rightHandColliding, rightHandSlide, out rightHandSlide, rightHandMaterialTouchIndex, out rightHandMaterialTouchIndex, rightHandSurfaceOverride, out rightHandSurfaceOverride);
		rightHandColliding = rightHandColliding && controllerState.RightValid;
		rightHandSlide = rightHandSlide && controllerState.RightValid;
		StoreVelocities();
		didAJump = false;
		if (OverrideSlipToMax())
		{
			didAJump = true;
		}
		else if (rightHandSlide || leftHandSlide)
		{
			slideAverageNormal = Vector3.zero;
			touchPoints = 0;
			averageSlipPercentage = 0f;
			if (leftHandSlide)
			{
				slideAverageNormal += leftHandSlideNormal.normalized;
				averageSlipPercentage += leftHandSlipPercentage;
				touchPoints++;
			}
			if (rightHandSlide)
			{
				slideAverageNormal += rightHandSlideNormal.normalized;
				averageSlipPercentage += rightHandSlipPercentage;
				touchPoints++;
			}
			slideAverageNormal = slideAverageNormal.normalized;
			averageSlipPercentage /= touchPoints;
			if (touchPoints == 1)
			{
				surfaceDirection = (rightHandSlide ? Vector3.ProjectOnPlane(rightControllerTransform.forward, rightHandSlideNormal) : Vector3.ProjectOnPlane(leftControllerTransform.forward, leftHandSlideNormal));
				if (Vector3.Dot(slideAverage, surfaceDirection) > 0f)
				{
					slideAverage = Vector3.Project(slideAverage, Vector3.Slerp(slideAverage, surfaceDirection.normalized * slideAverage.magnitude, slideControl));
				}
				else
				{
					slideAverage = Vector3.Project(slideAverage, Vector3.Slerp(slideAverage, -surfaceDirection.normalized * slideAverage.magnitude, slideControl));
				}
			}
			if (!wasLeftHandSlide && !wasRightHandSlide)
			{
				slideAverage = ((Vector3.Dot(playerRigidBody.velocity, slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(playerRigidBody.velocity, slideAverageNormal) : playerRigidBody.velocity);
			}
			else
			{
				slideAverage = ((Vector3.Dot(slideAverage, slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(slideAverage, slideAverageNormal) : slideAverage);
			}
			slideAverage = slideAverage.normalized * Mathf.Min(slideAverage.magnitude, Mathf.Max(0.5f, denormalizedVelocityAverage.magnitude * 2f));
			playerRigidBody.velocity = Vector3.zero;
		}
		else if (leftHandColliding || rightHandColliding)
		{
			if (!turnedThisFrame)
			{
				playerRigidBody.velocity = Vector3.zero;
			}
			else
			{
				playerRigidBody.velocity = playerRigidBody.velocity.normalized * Mathf.Min(2f, playerRigidBody.velocity.magnitude);
			}
		}
		else if (wasLeftHandSlide || wasRightHandSlide)
		{
			playerRigidBody.velocity = ((Vector3.Dot(slideAverage, slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(slideAverage, slideAverageNormal) : slideAverage);
		}
		if ((rightHandColliding || leftHandColliding) && !disableMovement && !turnedThisFrame && !didAJump)
		{
			if (rightHandSlide || leftHandSlide)
			{
				if (Vector3.Project(denormalizedVelocityAverage, slideAverageNormal).magnitude > slideVelocityLimit && Vector3.Dot(denormalizedVelocityAverage, slideAverageNormal) > 0f && Vector3.Project(denormalizedVelocityAverage, slideAverageNormal).magnitude > Vector3.Project(slideAverage, slideAverageNormal).magnitude)
				{
					leftHandSlide = false;
					rightHandSlide = false;
					didAJump = true;
					playerRigidBody.velocity = Mathf.Min(maxJumpSpeed * ExtraVelMaxMultiplier(), jumpMultiplier * ExtraVelMultiplier() * Vector3.Project(denormalizedVelocityAverage, slideAverageNormal).magnitude) * slideAverageNormal.normalized + Vector3.ProjectOnPlane(slideAverage, slideAverageNormal);
				}
			}
			else if (denormalizedVelocityAverage.magnitude > velocityLimit * scale)
			{
				float num = ((InWater && CurrentWaterVolume != null) ? liquidPropertiesList[(int)CurrentWaterVolume.LiquidType].surfaceJumpFactor : 1f);
				Vector3 vector3 = Mathf.Min(maxJumpSpeed * ExtraVelMaxMultiplier(), jumpMultiplier * ExtraVelMultiplier() * num * denormalizedVelocityAverage.magnitude) * denormalizedVelocityAverage.normalized;
				didAJump = true;
				playerRigidBody.velocity = vector3;
				if (InWater)
				{
					swimmingVelocity += vector3 * swimmingParams.underwaterJumpsAsSwimVelocityFactor;
				}
			}
		}
		if (!controllerState.LeftValid || (leftHandColliding && (GetCurrentLeftHandPosition() - GetLastLeftHandPosition()).magnitude > unStickDistance * scale && !Physics.Raycast(headCollider.transform.position, (GetCurrentLeftHandPosition() - headCollider.transform.position).normalized, out hitInfo, (GetCurrentLeftHandPosition() - headCollider.transform.position).magnitude, locomotionEnabledLayers.value)))
		{
			vector = GetCurrentLeftHandPosition();
			leftHandColliding = false;
		}
		if (!controllerState.RightValid || (rightHandColliding && (GetCurrentRightHandPosition() - GetLastRightHandPosition()).magnitude > unStickDistance * scale && !Physics.Raycast(headCollider.transform.position, (GetCurrentRightHandPosition() - headCollider.transform.position).normalized, out hitInfo, (GetCurrentRightHandPosition() - headCollider.transform.position).magnitude, locomotionEnabledLayers.value)))
		{
			vector2 = GetCurrentRightHandPosition();
			rightHandColliding = false;
		}
		if (currentPlatform == null)
		{
			playerRigidBody.velocity += refMovement / calcDeltaTime;
			refMovement = Vector3.zero;
		}
		Vector3 zero = Vector3.zero;
		float a = 0f;
		if (GetSwimmingVelocityForHand(lastLeftHandPosition, vector, leftControllerTransform.right, calcDeltaTime, ref leftHandWaterVolume, ref leftHandWaterSurface, out var swimmingVelocityChange))
		{
			a = Mathf.InverseLerp(0f, 0.2f, swimmingVelocityChange.magnitude) * swimmingParams.swimmingHapticsStrength;
			zero += swimmingVelocityChange;
		}
		float a2 = 0f;
		if (GetSwimmingVelocityForHand(lastRightHandPosition, vector2, -rightControllerTransform.right, calcDeltaTime, ref rightHandWaterVolume, ref rightHandWaterSurface, out var swimmingVelocityChange2))
		{
			a2 = Mathf.InverseLerp(0f, 0.15f, swimmingVelocityChange2.magnitude) * swimmingParams.swimmingHapticsStrength;
			zero += swimmingVelocityChange2;
		}
		Vector3 zero2 = Vector3.zero;
		if (swimmingParams.allowWaterSurfaceJumps && time - lastWaterSurfaceJumpTimeLeft > waterSurfaceJumpCooldown && CheckWaterSurfaceJump(lastLeftHandPosition, vector, leftControllerTransform.right, leftHandCenterVelocityTracker.GetAverageVelocity(worldSpace: false, 0.1f), swimmingParams, leftHandWaterVolume, leftHandWaterSurface, out var jumpVelocity))
		{
			if (time - lastWaterSurfaceJumpTimeRight > waterSurfaceJumpCooldown)
			{
				zero2 += jumpVelocity;
			}
			lastWaterSurfaceJumpTimeLeft = Time.time;
			GorillaTagger.Instance.StartVibration(forLeftController: true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
		}
		if (swimmingParams.allowWaterSurfaceJumps && time - lastWaterSurfaceJumpTimeRight > waterSurfaceJumpCooldown && CheckWaterSurfaceJump(lastRightHandPosition, vector2, -rightControllerTransform.right, rightHandCenterVelocityTracker.GetAverageVelocity(worldSpace: false, 0.1f), swimmingParams, rightHandWaterVolume, rightHandWaterSurface, out var jumpVelocity2))
		{
			if (time - lastWaterSurfaceJumpTimeLeft > waterSurfaceJumpCooldown)
			{
				zero2 += jumpVelocity2;
			}
			lastWaterSurfaceJumpTimeRight = Time.time;
			GorillaTagger.Instance.StartVibration(forLeftController: false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
		}
		zero2 = Vector3.ClampMagnitude(zero2, swimmingParams.waterSurfaceJumpMaxSpeed);
		float num2 = Mathf.Max(a, leftHandNonDiveHapticsAmount);
		if (num2 > 0.001f && time - lastWaterSurfaceJumpTimeLeft > GorillaTagger.Instance.tapHapticDuration)
		{
			GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num2, calcDeltaTime);
		}
		float num3 = Mathf.Max(a2, rightHandNonDiveHapticsAmount);
		if (num3 > 0.001f && time - lastWaterSurfaceJumpTimeRight > GorillaTagger.Instance.tapHapticDuration)
		{
			GorillaTagger.Instance.DoVibration(XRNode.RightHand, num3, calcDeltaTime);
		}
		if (!disableMovement)
		{
			swimmingVelocity += zero;
			playerRigidBody.velocity += zero + zero2;
		}
		else
		{
			swimmingVelocity = Vector3.zero;
		}
		if (isClimbing && (inOverlay || climbHelper == null || currentClimbable == null || !currentClimbable.isActiveAndEnabled))
		{
			EndClimbing(currentClimber, startingNewClimb: false);
		}
		Vector3 vector4 = Vector3.zero;
		if (isClimbing)
		{
			vector4 = currentClimber.transform.position - climbHelper.position;
			if (vector4.magnitude > 1f)
			{
				EndClimbing(currentClimber, startingNewClimb: false);
			}
		}
		if (isClimbing)
		{
			playerRigidBody.velocity = Vector3.zero;
			climbHelper.localPosition = Vector3.Lerp(climbHelper.localPosition, climbHelperTargetPos, Time.deltaTime * 12f);
			playerRigidBody.MovePosition(playerRigidBody.position - vector4);
			if ((bool)currentSwing)
			{
				currentSwing.lastGrabTime = Time.time;
			}
		}
		leftHandFollower.position = vector;
		rightHandFollower.position = vector2;
		leftHandFollower.rotation = leftControllerTransform.rotation * leftHandRotOffset;
		rightHandFollower.rotation = rightControllerTransform.rotation * rightHandRotOffset;
		wasLeftHandTouching = leftHandColliding;
		wasRightHandTouching = rightHandColliding;
		wasLeftHandSlide = leftHandSlide;
		wasRightHandSlide = rightHandSlide;
		degreesTurnedThisFrame = 0f;
		lastPlatformTouched = currentPlatform;
		currentPlatform = null;
		lastLeftHandPosition = vector;
		lastRightHandPosition = vector2;
		lastFrameTouchPosLocal = ComputeLocalHitPoint(lastHitInfoHand);
		lastFrameTouchPosWorld = lastHitInfoHand.point;
		lastRigidbodyPosition = playerRigidBody.transform.position;
		BodyCollider();
	}

	private Vector3 FirstHandIteration(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, bool wasHandSlide, bool wasHandTouching, out Vector3 pushDisplacement, ref float handSlipPercentage, ref bool handSlide, ref Vector3 slideNormal, ref bool handColliding, ref int materialTouchIndex, ref GorillaSurfaceOverride touchedOverride)
	{
		Vector3 currentHandPosition = GetCurrentHandPosition(handTransform, handOffset);
		Vector3 vector = currentHandPosition;
		distanceTraveled = currentHandPosition - lastHandPosition;
		if (!didAJump && wasHandSlide && Vector3.Dot(slideNormal, Vector3.up) > 0f)
		{
			distanceTraveled += Vector3.Project(-slideAverageNormal * stickDepth * scale, Vector3.down);
		}
		if (IterativeCollisionSphereCast(lastHandPosition, minimumRaycastDistance * scale, distanceTraveled, out finalPosition, singleHand: true, out slipPercentage, out tempHitInfo, fullSlide: false) && !InReportMenu)
		{
			vector = ((!wasHandTouching || !(slipPercentage <= defaultSlideFactor)) ? finalPosition : lastHandPosition);
			pushDisplacement = vector - currentHandPosition;
			handSlipPercentage = slipPercentage;
			handSlide = slipPercentage > iceThreshold;
			slideNormal = tempHitInfo.normal;
			handColliding = true;
			materialTouchIndex = currentMaterialIndex;
			touchedOverride = currentOverride;
			lastHitInfoHand = tempHitInfo;
		}
		else
		{
			pushDisplacement = Vector3.zero;
			handSlipPercentage = 0f;
			handSlide = false;
			slideNormal = Vector3.up;
			handColliding = false;
			materialTouchIndex = 0;
			touchedOverride = null;
		}
		return vector;
	}

	private Vector3 FinalHandPosition(Transform handTransform, Vector3 handOffset, Vector3 lastHandPosition, bool bothTouching, bool isHandTouching, out bool handColliding, bool isHandSlide, out bool handSlide, int currentMaterialTouchIndex, out int materialTouchIndex, GorillaSurfaceOverride currentSurface, out GorillaSurfaceOverride touchedOverride)
	{
		handColliding = isHandTouching;
		handSlide = isHandSlide;
		materialTouchIndex = currentMaterialTouchIndex;
		touchedOverride = currentSurface;
		distanceTraveled = GetCurrentHandPosition(handTransform, handOffset) - lastHandPosition;
		if (IterativeCollisionSphereCast(lastHandPosition, minimumRaycastDistance * scale, distanceTraveled, out finalPosition, bothTouching, out slipPercentage, out junkHit, fullSlide: false))
		{
			handColliding = true;
			handSlide = slipPercentage > iceThreshold;
			materialTouchIndex = currentMaterialIndex;
			touchedOverride = currentOverride;
			lastHitInfoHand = junkHit;
			return finalPosition;
		}
		return GetCurrentHandPosition(handTransform, handOffset);
	}

	private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, bool fullSlide)
	{
		slipPercentage = defaultSlideFactor;
		if (CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out tempIterativeHit))
		{
			firstPosition = endPosition;
			iterativeHitInfo = tempIterativeHit;
			slideFactor = GetSlidePercentage(iterativeHitInfo);
			slipPercentage = ((slideFactor != defaultSlideFactor) ? slideFactor : ((!singleHand) ? defaultSlideFactor : 0.001f));
			if (fullSlide || OverrideSlipToMax())
			{
				slipPercentage = 1f;
			}
			movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - firstPosition, iterativeHitInfo.normal) * slipPercentage;
			if (CollisionsSphereCast(firstPosition, sphereRadius, movementToProjectedAboveCollisionPlane, out endPosition, out tempIterativeHit))
			{
				iterativeHitInfo = tempIterativeHit;
				return true;
			}
			if (CollisionsSphereCast(movementToProjectedAboveCollisionPlane + firstPosition, sphereRadius, startPosition + movementVector - (movementToProjectedAboveCollisionPlane + firstPosition), out endPosition, out tempIterativeHit))
			{
				iterativeHitInfo = tempIterativeHit;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}
		iterativeHitInfo = tempIterativeHit;
		endPosition = Vector3.zero;
		return false;
	}

	private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
	{
		MaxSphereSizeForNoOverlap(sphereRadius, startPosition, ignoreOneWay: false, out maxSphereSize1);
		ClearRaycasthitBuffer(ref rayCastNonAllocColliders);
		bufferCount = Physics.SphereCastNonAlloc(startPosition, maxSphereSize1, movementVector.normalized, rayCastNonAllocColliders, movementVector.magnitude, locomotionEnabledLayers.value);
		if (bufferCount > 0)
		{
			tempHitInfo = rayCastNonAllocColliders[0];
			for (int i = 0; i < bufferCount; i++)
			{
				if (rayCastNonAllocColliders[i].distance < tempHitInfo.distance)
				{
					tempHitInfo = rayCastNonAllocColliders[i];
				}
			}
			collisionsHitInfo = tempHitInfo;
			finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
			ClearRaycasthitBuffer(ref rayCastNonAllocColliders);
			bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
			if (bufferCount > 0)
			{
				tempHitInfo = rayCastNonAllocColliders[0];
				for (int j = 0; j < bufferCount; j++)
				{
					if (rayCastNonAllocColliders[j].distance < tempHitInfo.distance)
					{
						tempHitInfo = rayCastNonAllocColliders[j];
					}
				}
				finalPosition = startPosition + movementVector.normalized * tempHitInfo.distance;
			}
			MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, ignoreOneWay: false, out maxSphereSize2);
			ClearRaycasthitBuffer(ref rayCastNonAllocColliders);
			bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(maxSphereSize1, maxSphereSize2), (finalPosition - startPosition).normalized, rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, locomotionEnabledLayers.value);
			if (bufferCount > 0)
			{
				tempHitInfo = rayCastNonAllocColliders[0];
				for (int k = 0; k < bufferCount; k++)
				{
					if (rayCastNonAllocColliders[k].collider != null && rayCastNonAllocColliders[k].distance < tempHitInfo.distance)
					{
						tempHitInfo = rayCastNonAllocColliders[k];
					}
				}
				finalPosition = startPosition + tempHitInfo.distance * (finalPosition - startPosition).normalized;
				collisionsHitInfo = tempHitInfo;
			}
			ClearRaycasthitBuffer(ref rayCastNonAllocColliders);
			bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, locomotionEnabledLayers.value);
			if (bufferCount > 0)
			{
				tempHitInfo = rayCastNonAllocColliders[0];
				for (int l = 0; l < bufferCount; l++)
				{
					if (rayCastNonAllocColliders[l].distance < tempHitInfo.distance)
					{
						tempHitInfo = rayCastNonAllocColliders[l];
					}
				}
				collisionsHitInfo = tempHitInfo;
				finalPosition = startPosition;
			}
			return true;
		}
		ClearRaycasthitBuffer(ref rayCastNonAllocColliders);
		bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, rayCastNonAllocColliders, movementVector.magnitude, locomotionEnabledLayers.value);
		if (bufferCount > 0)
		{
			tempHitInfo = rayCastNonAllocColliders[0];
			for (int m = 0; m < bufferCount; m++)
			{
				if (rayCastNonAllocColliders[m].collider != null && rayCastNonAllocColliders[m].distance < tempHitInfo.distance)
				{
					tempHitInfo = rayCastNonAllocColliders[m];
				}
			}
			collisionsHitInfo = tempHitInfo;
			finalPosition = startPosition;
			return true;
		}
		finalPosition = startPosition + movementVector;
		collisionsHitInfo = default(RaycastHit);
		return false;
	}

	public bool IsHandTouching(bool forLeftHand)
	{
		if (forLeftHand)
		{
			return wasLeftHandTouching;
		}
		return wasRightHandTouching;
	}

	public bool IsHandSliding(bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (!wasLeftHandSlide)
			{
				return leftHandSlide;
			}
			return true;
		}
		if (!wasRightHandSlide)
		{
			return rightHandSlide;
		}
		return true;
	}

	public float GetSlidePercentage(RaycastHit raycastHit)
	{
		currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
		Platform component = raycastHit.collider.gameObject.GetComponent<Platform>();
		if (component != null)
		{
			currentPlatform = component;
		}
		if (currentOverride != null)
		{
			currentMaterialIndex = currentOverride.overrideIndex;
			if (!materialData[currentMaterialIndex].overrideSlidePercent)
			{
				return defaultSlideFactor;
			}
			return materialData[currentMaterialIndex].slidePercent;
		}
		meshCollider = raycastHit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null || meshCollider.convex)
		{
			return defaultSlideFactor;
		}
		collidedMesh = meshCollider.sharedMesh;
		if (!meshTrianglesDict.TryGetValue(collidedMesh, out sharedMeshTris))
		{
			sharedMeshTris = collidedMesh.triangles;
			meshTrianglesDict.Add(collidedMesh, (int[])sharedMeshTris.Clone());
		}
		vertex1 = sharedMeshTris[raycastHit.triangleIndex * 3];
		vertex2 = sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
		vertex3 = sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
		slideRenderer = raycastHit.collider.GetComponent<Renderer>();
		slideRenderer.GetSharedMaterials(tempMaterialArray);
		if (tempMaterialArray.Count > 1)
		{
			for (int i = 0; i < tempMaterialArray.Count; i++)
			{
				collidedMesh.GetTriangles(trianglesList, i);
				for (int j = 0; j < trianglesList.Count; j += 3)
				{
					if (trianglesList[j] == vertex1 && trianglesList[j + 1] == vertex2 && trianglesList[j + 2] == vertex3)
					{
						findMatName = tempMaterialArray[i].name;
						foundMatData = materialData.Find((MaterialData matData) => matData.matName == findMatName);
						currentMaterialIndex = materialData.FindIndex((MaterialData matData) => matData.matName == findMatName);
						if (currentMaterialIndex == -1)
						{
							currentMaterialIndex = 0;
						}
						if (!foundMatData.overrideSlidePercent)
						{
							return defaultSlideFactor;
						}
						return foundMatData.slidePercent;
					}
				}
			}
			currentMaterialIndex = 0;
			return defaultSlideFactor;
		}
		findMatName = tempMaterialArray[0].name;
		foundMatData = materialData.Find((MaterialData matData) => matData.matName == findMatName);
		currentMaterialIndex = materialData.FindIndex((MaterialData matData) => matData.matName == findMatName);
		if (currentMaterialIndex == -1)
		{
			currentMaterialIndex = 0;
		}
		if (!foundMatData.overrideSlidePercent)
		{
			return defaultSlideFactor;
		}
		return foundMatData.slidePercent;
	}

	public void Turn(float degrees)
	{
		turnParent.transform.RotateAround(headCollider.transform.position, base.transform.up, degrees);
		degreesTurnedThisFrame = degrees;
		denormalizedVelocityAverage = Vector3.zero;
		for (int i = 0; i < velocityHistory.Length; i++)
		{
			velocityHistory[i] = Quaternion.Euler(0f, degrees, 0f) * velocityHistory[i];
			denormalizedVelocityAverage += velocityHistory[i];
		}
	}

	public void BeginClimbing(GorillaClimbable climbable, GorillaHandClimber hand, GorillaClimbableRef climbableRef = null)
	{
		if (currentClimber != null)
		{
			EndClimbing(currentClimber, startingNewClimb: true);
		}
		try
		{
			climbable.onBeforeClimb?.Invoke(hand, climbableRef);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		climbable.TryGetComponent<Rigidbody>(out var _);
		VerifyClimbHelper();
		climbHelper.SetParent(climbable.transform);
		climbHelper.position = hand.transform.position;
		Vector3 localPosition = climbHelper.localPosition;
		if (climbable.snapX)
		{
			SnapAxis(ref localPosition.x, climbable.maxDistanceSnap);
		}
		if (climbable.snapY)
		{
			SnapAxis(ref localPosition.y, climbable.maxDistanceSnap);
		}
		if (climbable.snapZ)
		{
			SnapAxis(ref localPosition.z, climbable.maxDistanceSnap);
		}
		climbHelperTargetPos = localPosition;
		climbable.isBeingClimbed = true;
		hand.isClimbing = true;
		currentClimbable = climbable;
		currentClimber = hand;
		isClimbing = true;
		GorillaZipline component3;
		if (climbable.TryGetComponent<GorillaRopeSegment>(out var component2) && (bool)component2.swing)
		{
			currentSwing = component2.swing;
			currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, climbHelperTargetPos, currentVelocity);
		}
		else if ((bool)climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out component3))
		{
			currentZipline = component3;
		}
		playerRigidBody.useGravity = false;
		GorillaTagger.Instance.StartVibration(currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
		if ((bool)climbable.clip)
		{
			GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);
		}
		static void SnapAxis(ref float val, float maxDist)
		{
			if (val > maxDist)
			{
				val = maxDist;
			}
			else if (val < 0f - maxDist)
			{
				val = 0f - maxDist;
			}
		}
	}

	private void VerifyClimbHelper()
	{
		if (climbHelper == null || climbHelper.gameObject == null)
		{
			climbHelper = new GameObject("Climb Helper").transform;
		}
	}

	public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
	{
		if (hand != currentClimber)
		{
			return;
		}
		if (!startingNewClimb)
		{
			playerRigidBody.useGravity = true;
		}
		Rigidbody component = null;
		if ((bool)currentClimbable)
		{
			currentClimbable.TryGetComponent<Rigidbody>(out component);
			currentClimbable.isBeingClimbed = false;
		}
		Vector3 force = Vector3.zero;
		if ((bool)currentClimber)
		{
			currentClimber.isClimbing = false;
			if (doDontReclimb)
			{
				currentClimber.dontReclimbLast = currentClimbable;
			}
			else
			{
				currentClimber.dontReclimbLast = null;
			}
			currentClimber.queuedToBecomeValidToGrabAgain = true;
			currentClimber.lastAutoReleasePos = currentClimber.handRoot.localPosition;
			if (!startingNewClimb && (bool)currentClimbable)
			{
				GorillaVelocityTracker gorillaVelocityTracker = ((currentClimber.xrNode != XRNode.LeftHand) ? rightInteractPointVelocityTracker : leftInteractPointVelocityTracker);
				if ((bool)component)
				{
					playerRigidBody.velocity = component.velocity;
				}
				else if ((bool)currentSwing)
				{
					playerRigidBody.velocity = currentSwing.velocityTracker.GetAverageVelocity(worldSpace: true, 0.25f);
				}
				else if ((bool)currentZipline)
				{
					playerRigidBody.velocity = currentZipline.GetCurrentDirection() * currentZipline.currentSpeed;
				}
				force = turnParent.transform.rotation * -gorillaVelocityTracker.GetAverageVelocity(worldSpace: false, 0.1f, doMagnitudeCheck: true);
				force = Vector3.ClampMagnitude(force, 5.5f);
				playerRigidBody.AddForce(force, ForceMode.VelocityChange);
			}
		}
		if ((bool)currentSwing)
		{
			currentSwing.DetachLocalPlayer();
		}
		if (!startingNewClimb && force.magnitude > 2f && (bool)currentClimbable && (bool)currentClimbable.clipOnFullRelease)
		{
			GorillaTagger.Instance.offlineVRRig.PlayClimbSound(currentClimbable.clipOnFullRelease, hand.xrNode == XRNode.LeftHand);
		}
		currentClimbable = null;
		currentClimber = null;
		currentSwing = null;
		currentZipline = null;
		isClimbing = false;
	}

	private void StoreVelocities()
	{
		velocityIndex = (velocityIndex + 1) % velocityHistorySize;
		currentVelocity = (base.transform.position - lastPosition - MovingSurfaceMovement()) / calcDeltaTime;
		velocityHistory[velocityIndex] = currentVelocity;
		denormalizedVelocityAverage = Vector3.zero;
		for (int i = 0; i < velocityHistory.Length; i++)
		{
			denormalizedVelocityAverage += velocityHistory[i];
		}
		denormalizedVelocityAverage /= (float)velocityHistorySize;
		lastPosition = base.transform.position;
	}

	private void AntiTeleportTechnology()
	{
		if ((headCollider.transform.position - lastHeadPosition).magnitude >= teleportThresholdNoVel + playerRigidBody.velocity.magnitude * calcDeltaTime)
		{
			base.transform.position = base.transform.position + lastHeadPosition - headCollider.transform.position;
		}
	}

	private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, bool ignoreOneWay, out float overlapRadiusTest)
	{
		overlapRadiusTest = testRadius;
		overlapAttempts = 0;
		while (overlapAttempts < 100 && overlapRadiusTest > testRadius * 0.75f)
		{
			ClearColliderBuffer(ref overlapColliders);
			bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, overlapColliders, locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
			if (ignoreOneWay)
			{
				int num = 0;
				for (int i = 0; i < bufferCount; i++)
				{
					if (overlapColliders[i].CompareTag("NoCrazyCheck"))
					{
						num++;
					}
				}
				if (num == bufferCount)
				{
					return true;
				}
			}
			if (bufferCount > 0)
			{
				overlapRadiusTest *= 0.99f;
				overlapAttempts++;
				continue;
			}
			overlapRadiusTest *= 0.995f;
			return true;
		}
		return false;
	}

	private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
	{
		for (int i = 0; i < crazyCheckVectors.Length; i++)
		{
			if (NonAllocRaycast(startPosition, startPosition + crazyCheckVectors[i] * sphereSize) > 0)
			{
				return false;
			}
		}
		return true;
	}

	private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
	{
		Vector3 direction = endPosition - startPosition;
		int num = Physics.RaycastNonAlloc(startPosition, direction, rayCastNonAllocColliders, direction.magnitude, locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			if (!rayCastNonAllocColliders[i].collider.gameObject.CompareTag("NoCrazyCheck"))
			{
				num2++;
			}
		}
		return num2;
	}

	private void ClearColliderBuffer(ref Collider[] colliders)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i] = null;
		}
	}

	private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
	{
		for (int i = 0; i < raycastHits.Length; i++)
		{
			raycastHits[i] = emptyHit;
		}
	}

	private Vector3 MovingSurfaceMovement()
	{
		return refMovement;
	}

	private static Vector3 ComputeLocalHitPoint(RaycastHit hit)
	{
		if (hit.collider == null)
		{
			return Vector3.zero;
		}
		return hit.collider.transform.InverseTransformPoint(hit.point);
	}

	private static Vector3 ComputeWorldHitPoint(RaycastHit hit, Vector3 localPoint)
	{
		if (hit.collider == null)
		{
			return Vector3.zero;
		}
		return hit.collider.transform.TransformPoint(localPoint);
	}

	private float ExtraVelMultiplier()
	{
		float num = 1f;
		if (leftHandSurfaceOverride != null)
		{
			num = Mathf.Max(num, leftHandSurfaceOverride.extraVelMultiplier);
		}
		if (rightHandSurfaceOverride != null)
		{
			num = Mathf.Max(num, rightHandSurfaceOverride.extraVelMultiplier);
		}
		return num;
	}

	private float ExtraVelMaxMultiplier()
	{
		float num = 1f;
		if (leftHandSurfaceOverride != null)
		{
			num = Mathf.Max(num, leftHandSurfaceOverride.extraVelMaxMultiplier);
		}
		if (rightHandSurfaceOverride != null)
		{
			num = Mathf.Max(num, rightHandSurfaceOverride.extraVelMaxMultiplier);
		}
		return num * scale;
	}

	public void SetMaximumSlipThisFrame()
	{
		disableGripFrameIdx = Time.frameCount;
	}

	public bool OverrideSlipToMax()
	{
		return disableGripFrameIdx == Time.frameCount;
	}

	public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
	{
		if (playerCollider == headCollider)
		{
			if (!headOverlappingWaterVolumes.Contains(volume))
			{
				headOverlappingWaterVolumes.Add(volume);
			}
		}
		else if (playerCollider == bodyCollider && !bodyOverlappingWaterVolumes.Contains(volume))
		{
			bodyOverlappingWaterVolumes.Add(volume);
		}
	}

	public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
	{
		if (playerCollider == headCollider)
		{
			headOverlappingWaterVolumes.Remove(volume);
		}
		else if (playerCollider == bodyCollider)
		{
			bodyOverlappingWaterVolumes.Remove(volume);
		}
	}

	private bool GetSwimmingVelocityForHand(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, float dt, ref WaterVolume contactingWaterVolume, ref WaterVolume.SurfaceQuery waterSurface, out Vector3 swimmingVelocityChange)
	{
		contactingWaterVolume = null;
		bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, minimumRaycastDistance, overlapColliders, waterLayer.value, QueryTriggerInteraction.Collide);
		if (bufferCount > 0)
		{
			float num = float.MinValue;
			for (int i = 0; i < bufferCount; i++)
			{
				WaterVolume component = overlapColliders[i].GetComponent<WaterVolume>();
				if (component != null && component.GetSurfaceQueryForPoint(endingHandPosition, out var result) && result.surfacePoint.y > num)
				{
					contactingWaterVolume = component;
					waterSurface = result;
				}
			}
		}
		if (contactingWaterVolume != null)
		{
			Vector3 vector = endingHandPosition - startingHandPosition;
			Vector3 vector2 = Vector3.zero;
			Vector3 vector3 = playerRigidBody.transform.position - lastRigidbodyPosition;
			if (turnedThisFrame)
			{
				Vector3 vector4 = startingHandPosition - headCollider.transform.position;
				vector2 = Quaternion.AngleAxis(degreesTurnedThisFrame, Vector3.up) * vector4 - vector4;
			}
			float num2 = Vector3.Dot(vector - vector2 - vector3, palmForwardDirection);
			float num3 = 0f;
			if (num2 > 0f)
			{
				Plane surfacePlane = waterSurface.surfacePlane;
				float distanceToPoint = surfacePlane.GetDistanceToPoint(startingHandPosition);
				float distanceToPoint2 = surfacePlane.GetDistanceToPoint(endingHandPosition);
				if (distanceToPoint <= 0f && distanceToPoint2 <= 0f)
				{
					num3 = 1f;
				}
				else if (distanceToPoint > 0f && distanceToPoint2 <= 0f)
				{
					num3 = (0f - distanceToPoint2) / (distanceToPoint - distanceToPoint2);
				}
				else if (distanceToPoint <= 0f && distanceToPoint2 > 0f)
				{
					num3 = (0f - distanceToPoint) / (distanceToPoint2 - distanceToPoint);
				}
				if (num3 > Mathf.Epsilon)
				{
					float resistance = liquidPropertiesList[(int)contactingWaterVolume.LiquidType].resistance;
					swimmingVelocityChange = -palmForwardDirection * num2 * 2f * resistance * num3;
					return true;
				}
			}
		}
		swimmingVelocityChange = Vector3.zero;
		return false;
	}

	private bool CheckWaterSurfaceJump(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, Vector3 handAvgVelocity, PlayerSwimmingParameters parameters, WaterVolume contactingWaterVolume, WaterVolume.SurfaceQuery waterSurface, out Vector3 jumpVelocity)
	{
		if (contactingWaterVolume != null)
		{
			Plane surfacePlane = waterSurface.surfacePlane;
			bool flag = handAvgVelocity.sqrMagnitude > parameters.waterSurfaceJumpHandSpeedThreshold * parameters.waterSurfaceJumpHandSpeedThreshold;
			if (surfacePlane.GetSide(startingHandPosition) && !surfacePlane.GetSide(endingHandPosition) && flag)
			{
				float value = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
				float value2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
				float num = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(value, 0.01f, 0.99f));
				float num2 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(value2, 0.01f, 0.99f));
				jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * num * num2;
				return true;
			}
		}
		jumpVelocity = Vector3.zero;
		return false;
	}

	private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
	{
		magnitude = input.magnitude;
		if (magnitude > eps)
		{
			normalized = input / magnitude;
			return true;
		}
		normalized = Vector3.zero;
		return false;
	}

	private bool TryNormalizeDown(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
	{
		magnitude = input.magnitude;
		if (magnitude > 1f)
		{
			normalized = input / magnitude;
			return true;
		}
		if (magnitude >= eps)
		{
			normalized = input;
			return true;
		}
		normalized = Vector3.zero;
		return false;
	}
}
