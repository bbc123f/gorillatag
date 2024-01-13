using System;
using GorillaLocomotion.Climbing;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay;

public class GorillaZipline : MonoBehaviour
{
	[SerializeField]
	private Transform segmentsRoot;

	[SerializeField]
	private GameObject segmentPrefab;

	[SerializeField]
	private GorillaClimbable slideHelper;

	[SerializeField]
	private AudioSource audioSlide;

	private BezierSpline spline;

	[SerializeField]
	private Transform climbOffsetHelper;

	[SerializeField]
	private GorillaZiplineSettings settings;

	[SerializeField]
	private float ziplineDistance = 15f;

	[SerializeField]
	private float segmentDistance = 0.9f;

	private GorillaHandClimber currentClimber;

	private float currentT;

	private const float inheritVelocityRechargeRate = 0.2f;

	private const float inheritVelocityValueOnRelease = 0.55f;

	private float currentInheritVelocityMulti = 1f;

	public float currentSpeed { get; private set; }

	private void FindTFromDistance(ref float t, float distance, int steps = 1000)
	{
		float num = distance / (float)steps;
		Vector3 b = spline.GetPointLocal(t);
		float num2 = 0f;
		for (int i = 0; i < 1000; i++)
		{
			t += num;
			if (!(t >= 1f) && !(t <= 0f))
			{
				Vector3 pointLocal = spline.GetPointLocal(t);
				num2 += Vector3.Distance(pointLocal, b);
				if (!(num2 >= Mathf.Abs(distance)))
				{
					b = pointLocal;
					continue;
				}
				break;
			}
			break;
		}
	}

	private float FindSlideHelperSpot(Vector3 grabPoint)
	{
		int i = 0;
		int num = 200;
		float num2 = 0.001f;
		float num3 = 1f / (float)num;
		float3 y = base.transform.InverseTransformPoint(grabPoint);
		float result = 0f;
		float num4 = float.PositiveInfinity;
		for (; i < num; i++)
		{
			float num5 = math.distancesq(spline.GetPointLocal(num2), y);
			if (num5 < num4)
			{
				num4 = num5;
				result = num2;
			}
			num2 += num3;
		}
		return result;
	}

	private void Start()
	{
		spline = GetComponent<BezierSpline>();
		GorillaClimbable gorillaClimbable = slideHelper;
		gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(OnBeforeClimb));
	}

	private void OnDestroy()
	{
		GorillaClimbable gorillaClimbable = slideHelper;
		gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Remove(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(OnBeforeClimb));
	}

	public Vector3 GetCurrentDirection()
	{
		return spline.GetDirection(currentT);
	}

	private void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
	{
		bool num = currentClimber == null;
		currentClimber = hand;
		if ((bool)climbRef)
		{
			climbOffsetHelper.SetParent(climbRef.transform);
			climbOffsetHelper.position = hand.transform.position;
			climbOffsetHelper.localPosition = new Vector3(0f, 0f, climbOffsetHelper.localPosition.z);
		}
		currentT = FindSlideHelperSpot(climbOffsetHelper.position);
		slideHelper.transform.localPosition = spline.GetPointLocal(currentT);
		if (num)
		{
			Vector3 currentVelocity = Player.Instance.currentVelocity;
			float num2 = Vector3.Dot(currentVelocity.normalized, spline.GetDirection(currentT));
			currentSpeed = currentVelocity.magnitude * num2 * currentInheritVelocityMulti;
		}
	}

	private void Update()
	{
		if ((bool)currentClimber)
		{
			float num = 0f;
			Vector3 direction = spline.GetDirection(currentT);
			num = Physics.gravity.y * direction.y * settings.gravityMulti;
			currentSpeed = Mathf.MoveTowards(currentSpeed, settings.maxSpeed, num * Time.deltaTime);
			float num2 = Maths.Linear(currentSpeed, 0f, settings.maxFrictionSpeed, settings.friction, settings.maxFriction);
			currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, num2 * Time.deltaTime);
			currentSpeed = Mathf.Min(currentSpeed, settings.maxSpeed);
			currentSpeed = Mathf.Max(currentSpeed, 0f - settings.maxSpeed);
			float value = Mathf.Abs(currentSpeed);
			FindTFromDistance(ref currentT, currentSpeed * Time.deltaTime);
			slideHelper.transform.localPosition = spline.GetPointLocal(currentT);
			if (!audioSlide.gameObject.activeSelf)
			{
				audioSlide.gameObject.SetActive(value: true);
			}
			audioSlide.volume = Maths.Linear(value, 0f, settings.maxSpeed, settings.minSlideVolume, settings.maxSlideVolume);
			audioSlide.pitch = Maths.Linear(value, 0f, settings.maxSpeed, settings.minSlidePitch, settings.maxSlidePitch);
			if (!audioSlide.isPlaying)
			{
				audioSlide.Play();
			}
			float num3 = Maths.Linear(value, 0f, settings.maxSpeed, -0.1f, 0.75f);
			if (num3 > 0f)
			{
				GorillaTagger.Instance.DoVibration(currentClimber.xrNode, num3, Time.deltaTime);
			}
			if (!spline.Loop)
			{
				if (currentT >= 1f || currentT <= 0f)
				{
					currentClimber.ForceStopClimbing(startingNewClimb: false, doDontReclimb: true);
				}
			}
			else if (currentT >= 1f)
			{
				currentT = 0f;
			}
			else if (currentT <= 0f)
			{
				currentT = 1f;
			}
			if (!slideHelper.isBeingClimbed)
			{
				Stop();
			}
		}
		if (currentInheritVelocityMulti < 1f)
		{
			currentInheritVelocityMulti += Time.deltaTime * 0.2f;
			currentInheritVelocityMulti = Mathf.Min(currentInheritVelocityMulti, 1f);
		}
	}

	private void Stop()
	{
		currentClimber = null;
		audioSlide.Stop();
		audioSlide.gameObject.SetActive(value: false);
		currentInheritVelocityMulti = 0.55f;
		currentSpeed = 0f;
	}
}
