using System.Collections.Generic;
using CjLib;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

public class TransferrableBall : TransferrableObject
{
	[Header("Transferrable Ball")]
	public float ballRadius = 0.1f;

	public float depenetrationSpeed = 5f;

	[Range(0f, 1f)]
	public float hitSpeedThreshold = 0.8f;

	public float maxHitSpeed = 10f;

	public Vector2 hitSpeedToHitMultiplierMinMax = Vector2.one;

	public AnimationCurve hitMultiplierCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float hitTorqueMultiplier = 0.5f;

	public float reflectOffHandAmount = 0.5f;

	public float minHitSpeedThreshold = 0.2f;

	public float surfaceGripDistance = 0.02f;

	public Vector2 reflectOffHandSpeedInputMinMax = Vector2.one;

	public Vector2 reflectOffHandAmountOutputMinMax = Vector2.one;

	public SoundBankPlayer hitSoundBank;

	public Vector2 hitSpeedToAudioMinMax = Vector2.one;

	public float handHitAudioMultiplier = 2f;

	public Vector2 hitSoundPitchMinMax = Vector2.one;

	public Vector2 hitSoundVolumeMinMax = Vector2.one;

	public bool allowHeadButting = true;

	public float headButtRadius = 0.1f;

	public float headButtHitMultiplier = 1.5f;

	public float gravityCounterAmount;

	public bool debugDraw;

	private Dictionary<GorillaHandClimber, int> handClimberMap = new Dictionary<GorillaHandClimber, int>();

	private SphereCollider playerHeadCollider;

	private ContactPoint[] collisionContacts = new ContactPoint[8];

	private int collisionContactsCount;

	private float handRadius = 0.1f;

	private float depenetrationBias = 1f;

	private bool leftHandOverlapping;

	private bool rightHandOverlapping;

	private bool headOverlapping;

	private bool onGround;

	private ContactPoint groundContact;

	private bool applyFrictionHolding;

	private Vector3 frictionHoldLocalPosLeft;

	private Quaternion frictionHoldLocalRotLeft;

	private Vector3 frictionHoldLocalPosRight;

	private Quaternion frictionHoldLocalRotRight;

	private float hitSoundSpamLastHitTime;

	private int hitSoundSpamCount;

	private int hitSoundSpamLimit = 5;

	private float hitSoundSpamCooldownResetTime = 0.2f;

	private string gorillaHeadTriggerTag = "PlayerHeadTrigger";

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (Time.time - hitSoundSpamLastHitTime > hitSoundSpamCooldownResetTime)
		{
			hitSoundSpamCount = 0;
		}
		bool flag = false;
		bool flag2 = false;
		float num = 1f;
		bool flag3 = leftHandOverlapping;
		bool flag4 = rightHandOverlapping;
		Player instance = Player.Instance;
		bool flag5 = false;
		foreach (KeyValuePair<GorillaHandClimber, int> item in handClimberMap)
		{
			if (item.Value <= 0)
			{
				continue;
			}
			flag2 = true;
			Vector3 vector = Vector3.zero;
			bool flag6 = item.Key.xrNode == XRNode.LeftHand;
			Vector3 hitPoint;
			Vector3 hitNormal;
			float penetrationDist;
			if (flag6)
			{
				Vector3 position = instance.leftHandFollower.position;
				Quaternion rotation = instance.leftHandFollower.rotation;
				leftHandOverlapping = CheckCollisionWithHand(position, rotation, rotation * Vector3.right, out hitPoint, out hitNormal, out penetrationDist);
				if (leftHandOverlapping)
				{
					vector = instance.leftHandCenterVelocityTracker.GetAverageVelocity(worldSpace: true);
				}
				else if ((position - base.transform.position).sqrMagnitude > num * num)
				{
					handClimberMap[item.Key] = 0;
					continue;
				}
			}
			else
			{
				Vector3 position2 = instance.rightHandFollower.position;
				Quaternion rotation2 = instance.rightHandFollower.rotation;
				rightHandOverlapping = CheckCollisionWithHand(position2, rotation2, rotation2 * -Vector3.right, out hitPoint, out hitNormal, out penetrationDist);
				if (rightHandOverlapping)
				{
					vector = instance.rightHandCenterVelocityTracker.GetAverageVelocity(worldSpace: true);
				}
				else if ((position2 - base.transform.position).sqrMagnitude > num * num)
				{
					handClimberMap[item.Key] = 0;
					continue;
				}
			}
			if ((leftHandOverlapping || rightHandOverlapping) && (currentState == PositionState.None || currentState == PositionState.Dropped))
			{
				if (applyFrictionHolding)
				{
					if (flag6 && leftHandOverlapping)
					{
						if (!flag3)
						{
							Vector3 normalized = (instance.leftHandFollower.position - base.transform.position).normalized;
							Vector3 position3 = normalized * ballRadius + base.transform.position;
							frictionHoldLocalPosLeft = base.transform.InverseTransformPoint(position3);
							frictionHoldLocalRotLeft = Quaternion.LookRotation(normalized, instance.leftHandFollower.forward);
						}
						Vector3 vector2 = base.transform.TransformPoint(frictionHoldLocalPosLeft);
						frictionHoldLocalRotLeft = Quaternion.LookRotation(vector2 - base.transform.position, instance.leftHandFollower.forward);
						if (debugDraw)
						{
							Quaternion rotation3 = frictionHoldLocalRotLeft * Quaternion.AngleAxis(90f, Vector3.right);
							DebugUtil.DrawRect(vector2, rotation3, new Vector2(0.08f, 0.15f), Color.green, depthTest: false);
							Vector3 normalized2 = (instance.leftHandFollower.position - base.transform.position).normalized;
							Vector3 center = normalized2 * ballRadius + base.transform.position;
							Quaternion rotation4 = Quaternion.LookRotation(normalized2, instance.leftHandFollower.forward) * Quaternion.AngleAxis(90f, Vector3.right);
							DebugUtil.DrawRect(center, rotation4, new Vector2(0.08f, 0.15f), Color.yellow, depthTest: false);
						}
					}
					else if (!flag6 && rightHandOverlapping)
					{
						if (!flag4)
						{
							Vector3 normalized3 = (instance.rightHandFollower.position - base.transform.position).normalized;
							Vector3 position4 = normalized3 * ballRadius + base.transform.position;
							frictionHoldLocalPosRight = base.transform.InverseTransformPoint(position4);
							frictionHoldLocalRotRight = Quaternion.LookRotation(normalized3, instance.rightHandFollower.forward);
						}
						Vector3 vector3 = base.transform.TransformPoint(frictionHoldLocalPosRight);
						frictionHoldLocalRotRight = Quaternion.LookRotation(vector3 - base.transform.position, instance.rightHandFollower.forward);
						if (debugDraw)
						{
							Quaternion rotation5 = frictionHoldLocalRotRight * Quaternion.AngleAxis(90f, Vector3.right);
							DebugUtil.DrawRect(vector3, rotation5, new Vector2(0.08f, 0.15f), Color.green, depthTest: false);
							Vector3 normalized4 = (instance.rightHandFollower.position - base.transform.position).normalized;
							Vector3 center2 = normalized4 * ballRadius + base.transform.position;
							Quaternion rotation6 = Quaternion.LookRotation(normalized4, instance.rightHandFollower.forward) * Quaternion.AngleAxis(90f, Vector3.right);
							DebugUtil.DrawRect(center2, rotation6, new Vector2(0.08f, 0.15f), Color.yellow, depthTest: false);
						}
					}
				}
				bool flag7 = (flag6 && leftHandOverlapping && !flag3) || (!flag6 && rightHandOverlapping && !flag4);
				if (!flag5 && flag7)
				{
					Vector3 vector4 = (flag6 ? instance.leftHandFollower.position : instance.rightHandFollower.position);
					float magnitude = vector.magnitude;
					Vector3 vector5 = vector / magnitude;
					Vector3 vector6 = -(vector4 - base.transform.position).normalized;
					Vector3 hitDir = (vector5 + vector6) * 0.5f;
					flag5 = ApplyHit(vector4, hitDir, magnitude);
				}
				if (!flag5)
				{
					Vector3 vector7 = (flag6 ? instance.leftHandFollower.position : instance.rightHandFollower.position);
					Vector3 vector8 = vector7 - base.transform.position;
					float magnitude2 = vector8.magnitude;
					float num2 = ballRadius - vector8.magnitude;
					if (num2 > 0f)
					{
						Vector3 vector9 = -(vector8 / magnitude2) * num2;
						rigidbodyInstance.AddForce(-(vector8 / magnitude2) * depenetrationSpeed * Time.deltaTime, ForceMode.VelocityChange);
						if (collisionContactsCount == 0)
						{
							rigidbodyInstance.MovePosition(base.transform.position + vector9 * depenetrationBias);
						}
						if (debugDraw)
						{
							DebugUtil.DrawLine(vector7, vector7 - vector9, Color.magenta, depthTest: false);
						}
					}
				}
				if (debugDraw)
				{
					DebugUtil.DrawSphere(hitPoint, 0.01f, 6, 6, Color.green, depthTest: true, DebugUtil.Style.SolidColor);
					DebugUtil.DrawArrow(hitPoint, hitPoint - hitNormal * 0.05f, 0.01f, Color.green);
				}
			}
			flag = flag || leftHandOverlapping || rightHandOverlapping;
		}
		bool flag8 = headOverlapping;
		headOverlapping = false;
		if (allowHeadButting && !flag5 && playerHeadCollider != null)
		{
			headOverlapping = CheckCollisionWithHead(playerHeadCollider, out var hitPoint2, out var _, out var _);
			Vector3 currentVelocity = instance.currentVelocity;
			float magnitude3 = currentVelocity.magnitude;
			if (headOverlapping && !flag8 && (double)magnitude3 > 0.0001)
			{
				Vector3 hitDir2 = currentVelocity / magnitude3;
				flag5 = ApplyHit(hitPoint2, hitDir2, magnitude3 * headButtHitMultiplier);
			}
			else if ((playerHeadCollider.transform.position - base.transform.position).sqrMagnitude > num * num)
			{
				playerHeadCollider = null;
			}
		}
		if (debugDraw && onGround)
		{
			DebugUtil.DrawLine(groundContact.point, groundContact.point + groundContact.normal * 0.2f, Color.yellow, depthTest: false);
			DebugUtil.DrawRect(groundContact.point, Quaternion.LookRotation(groundContact.normal) * Quaternion.AngleAxis(90f, Vector3.right), Vector2.one * 0.2f, Color.yellow, depthTest: false);
		}
		if (flag2 && debugDraw)
		{
			DebugUtil.DrawSphereTripleCircles(base.transform.position, ballRadius, 16, flag ? Color.green : Color.white);
			for (int i = 0; i < collisionContactsCount; i++)
			{
				ContactPoint contactPoint = collisionContacts[i];
				DebugUtil.DrawArrow(contactPoint.point, contactPoint.point + contactPoint.normal * 0.2f, 0.02f, Color.red, depthTest: false);
			}
		}
	}

	private void TakeOwnershipAndEnablePhysics()
	{
		currentState = PositionState.Dropped;
		rigidbodyInstance.isKinematic = false;
		if (worldShareableInstance != null)
		{
			if (!worldShareableInstance.guard.isTrulyMine)
			{
				worldShareableInstance.guard.RequestOwnershipImmediately(delegate
				{
				});
			}
			worldShareableInstance.transferableObjectState = currentState;
		}
	}

	private bool CheckCollisionWithHand(Vector3 handCenter, Quaternion handRotation, Vector3 palmForward, out Vector3 hitPoint, out Vector3 hitNormal, out float penetrationDist)
	{
		Vector3 position = base.transform.position;
		bool flag = false;
		hitPoint = position;
		hitNormal = Vector3.forward;
		penetrationDist = 0f;
		Vector3 lhs = position - handCenter;
		Vector3 vector = position - Vector3.Dot(lhs, palmForward) * palmForward;
		Vector3 vector2 = vector;
		if ((vector - handCenter).sqrMagnitude > handRadius * handRadius)
		{
			vector2 = handCenter + (vector - handCenter).normalized * handRadius;
		}
		if ((vector2 - position).sqrMagnitude < ballRadius * ballRadius)
		{
			flag = true;
			hitNormal = (position - vector2).normalized;
			hitPoint = position - hitNormal * ballRadius;
			penetrationDist = ballRadius - (vector2 - position).magnitude;
		}
		if (debugDraw)
		{
			Color color = (flag ? Color.green : Color.white);
			DebugUtil.DrawCircle(handCenter, handRotation * Quaternion.AngleAxis(90f, Vector3.forward), handRadius, 16, color);
			DebugUtil.DrawArrow(handCenter, handCenter + palmForward * 0.05f, 0.01f, color);
		}
		return flag;
	}

	private bool CheckCollisionWithHead(SphereCollider headCollider, out Vector3 hitPoint, out Vector3 hitNormal, out float penetrationDist)
	{
		Vector3 vector = base.transform.position - headCollider.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = ballRadius + headButtRadius;
		if (sqrMagnitude < num * num)
		{
			float num2 = Mathf.Sqrt(sqrMagnitude);
			hitNormal = vector / num2;
			penetrationDist = num - num2;
			hitPoint = headCollider.transform.position + hitNormal * headButtRadius;
			return true;
		}
		hitNormal = Vector3.forward;
		hitPoint = Vector3.zero;
		penetrationDist = 0f;
		return false;
	}

	private bool ApplyHit(Vector3 hitPoint, Vector3 hitDir, float hitSpeed)
	{
		bool result = false;
		TakeOwnershipAndEnablePhysics();
		float num = 0f;
		Vector3 vector = Vector3.zero;
		if (hitSpeed > 0.0001f)
		{
			float num2 = Vector3.Dot(rigidbodyInstance.velocity, hitDir);
			float num3 = hitSpeed - num2;
			if (num3 > 0f)
			{
				num = num3;
				vector = num * hitDir;
			}
		}
		Vector3 normalized = (hitPoint - base.transform.position).normalized;
		float num4 = Vector3.Dot(rigidbodyInstance.velocity, -normalized);
		if (num4 < 0f)
		{
			float num5 = Mathf.Lerp(reflectOffHandAmountOutputMinMax.x, reflectOffHandAmountOutputMinMax.y, Mathf.InverseLerp(reflectOffHandSpeedInputMinMax.x, reflectOffHandSpeedInputMinMax.y, num4));
			rigidbodyInstance.velocity = num5 * Vector3.Reflect(rigidbodyInstance.velocity, -normalized);
		}
		if (num > hitSpeedThreshold)
		{
			result = true;
			float num6 = hitMultiplierCurve.Evaluate(Mathf.InverseLerp(hitSpeedToHitMultiplierMinMax.x, hitSpeedToHitMultiplierMinMax.y, num));
			if (onGround)
			{
				if (Vector3.Dot(vector, groundContact.normal) < 0f)
				{
					vector = Vector3.Reflect(vector, groundContact.normal);
				}
				Vector3 vector2 = vector / num;
				if (Vector3.Dot(vector2, groundContact.normal) < 0.707f)
				{
					vector = num * (vector2 + groundContact.normal) * 0.5f;
				}
			}
			rigidbodyInstance.AddForce(Vector3.ClampMagnitude(vector * num6, maxHitSpeed), ForceMode.VelocityChange);
			Vector3 rhs = hitDir * hitSpeed - Vector3.Dot(hitDir * hitSpeed, normalized) * normalized;
			Vector3 normalized2 = Vector3.Cross(normalized, rhs).normalized;
			float num7 = Vector3.Dot(rigidbodyInstance.angularVelocity, normalized2);
			float num8 = rhs.magnitude / ballRadius - num7;
			if (num8 > 0f)
			{
				rigidbodyInstance.AddTorque(num6 * hitTorqueMultiplier * num8 * normalized2, ForceMode.VelocityChange);
			}
		}
		PlayHitSound(num * handHitAudioMultiplier);
		return result;
	}

	private void PlayHitSound(float hitSpeed)
	{
		float t = Mathf.InverseLerp(hitSpeedToAudioMinMax.x, hitSpeedToAudioMinMax.y, hitSpeed);
		float value = Mathf.Lerp(hitSoundVolumeMinMax.x, hitSoundVolumeMinMax.y, t);
		float value2 = Mathf.Lerp(hitSoundPitchMinMax.x, hitSoundPitchMinMax.y, t);
		if (hitSoundBank != null && hitSoundSpamCount < hitSoundSpamLimit)
		{
			hitSoundSpamLastHitTime = Time.time;
			hitSoundSpamCount++;
			hitSoundBank.Play(value, value2);
		}
	}

	private void FixedUpdate()
	{
		collisionContactsCount = 0;
		onGround = false;
		rigidbodyInstance.AddForce(-Physics.gravity * gravityCounterAmount, ForceMode.Acceleration);
	}

	private void OnTriggerEnter(Collider other)
	{
		GorillaHandClimber component = other.GetComponent<GorillaHandClimber>();
		if (component != null)
		{
			if (handClimberMap.TryGetValue(component, out var value))
			{
				handClimberMap[component] = Mathf.Min(value + 1, 2);
			}
			else
			{
				handClimberMap.Add(component, 1);
			}
		}
		else if (other.CompareTag(gorillaHeadTriggerTag))
		{
			playerHeadCollider = other as SphereCollider;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		GorillaHandClimber component = other.GetComponent<GorillaHandClimber>();
		if (component != null)
		{
			if (handClimberMap.TryGetValue(component, out var value))
			{
				handClimberMap[component] = Mathf.Max(value - 1, 0);
			}
		}
		else if (other.CompareTag(gorillaHeadTriggerTag))
		{
			playerHeadCollider = null;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		PlayHitSound(collision.relativeVelocity.magnitude);
	}

	private void OnCollisionStay(Collision collision)
	{
		collisionContactsCount = collision.GetContacts(collisionContacts);
		float num = -1f;
		for (int i = 0; i < collisionContactsCount; i++)
		{
			float num2 = Vector3.Dot(collisionContacts[i].normal, Vector3.up);
			if (num2 > num)
			{
				groundContact = collisionContacts[i];
				num = num2;
			}
		}
		float num3 = 0.5f;
		onGround = num > num3;
	}
}
