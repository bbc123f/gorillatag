using System;
using GorillaLocomotion.Climbing;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200029A RID: 666
	public class GorillaZipline : MonoBehaviour
	{
		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600115E RID: 4446 RVA: 0x00061B84 File Offset: 0x0005FD84
		// (set) Token: 0x0600115F RID: 4447 RVA: 0x00061B8C File Offset: 0x0005FD8C
		public float currentSpeed { get; private set; }

		// Token: 0x06001160 RID: 4448 RVA: 0x00061B98 File Offset: 0x0005FD98
		private void FindTFromDistance(ref float t, float distance, int steps = 1000)
		{
			float num = distance / (float)steps;
			Vector3 b = this.spline.GetPointLocal(t);
			float num2 = 0f;
			for (int i = 0; i < 1000; i++)
			{
				t += num;
				if (t >= 1f || t <= 0f)
				{
					break;
				}
				Vector3 pointLocal = this.spline.GetPointLocal(t);
				num2 += Vector3.Distance(pointLocal, b);
				if (num2 >= Mathf.Abs(distance))
				{
					break;
				}
				b = pointLocal;
			}
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x00061C0C File Offset: 0x0005FE0C
		private float FindSlideHelperSpot(Vector3 grabPoint)
		{
			int i = 0;
			int num = 200;
			float num2 = 0.001f;
			float num3 = 1f / (float)num;
			float3 y = base.transform.InverseTransformPoint(grabPoint);
			float result = 0f;
			float num4 = float.PositiveInfinity;
			while (i < num)
			{
				float num5 = math.distancesq(this.spline.GetPointLocal(num2), y);
				if (num5 < num4)
				{
					num4 = num5;
					result = num2;
				}
				num2 += num3;
				i++;
			}
			return result;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x00061C88 File Offset: 0x0005FE88
		private void Start()
		{
			this.spline = base.GetComponent<BezierSpline>();
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x00061CBD File Offset: 0x0005FEBD
		private void OnDestroy()
		{
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Remove(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x00061CE6 File Offset: 0x0005FEE6
		public Vector3 GetCurrentDirection()
		{
			return this.spline.GetDirection(this.currentT);
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x00061CFC File Offset: 0x0005FEFC
		private void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
		{
			bool flag = this.currentClimber == null;
			this.currentClimber = hand;
			if (climbRef)
			{
				this.climbOffsetHelper.SetParent(climbRef.transform);
				this.climbOffsetHelper.position = hand.transform.position;
				this.climbOffsetHelper.localPosition = new Vector3(0f, 0f, this.climbOffsetHelper.localPosition.z);
			}
			this.currentT = this.FindSlideHelperSpot(this.climbOffsetHelper.position);
			this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
			if (flag)
			{
				Vector3 currentVelocity = Player.Instance.currentVelocity;
				float num = Vector3.Dot(currentVelocity.normalized, this.spline.GetDirection(this.currentT));
				this.currentSpeed = currentVelocity.magnitude * num * this.currentInheritVelocityMulti;
			}
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x00061DF0 File Offset: 0x0005FFF0
		private void Update()
		{
			if (this.currentClimber)
			{
				Vector3 direction = this.spline.GetDirection(this.currentT);
				float num = Physics.gravity.y * direction.y * this.settings.gravityMulti;
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.settings.maxSpeed, num * Time.deltaTime);
				float num2 = MathUtils.Linear(this.currentSpeed, 0f, this.settings.maxFrictionSpeed, this.settings.friction, this.settings.maxFriction);
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, num2 * Time.deltaTime);
				this.currentSpeed = Mathf.Min(this.currentSpeed, this.settings.maxSpeed);
				this.currentSpeed = Mathf.Max(this.currentSpeed, -this.settings.maxSpeed);
				float value = Mathf.Abs(this.currentSpeed);
				this.FindTFromDistance(ref this.currentT, this.currentSpeed * Time.deltaTime, 1000);
				this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
				if (!this.audioSlide.gameObject.activeSelf)
				{
					this.audioSlide.gameObject.SetActive(true);
				}
				this.audioSlide.volume = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlideVolume, this.settings.maxSlideVolume);
				this.audioSlide.pitch = MathUtils.Linear(value, 0f, this.settings.maxSpeed, this.settings.minSlidePitch, this.settings.maxSlidePitch);
				if (!this.audioSlide.isPlaying)
				{
					this.audioSlide.Play();
				}
				float num3 = MathUtils.Linear(value, 0f, this.settings.maxSpeed, -0.1f, 0.75f);
				if (num3 > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.currentClimber.xrNode, num3, Time.deltaTime);
				}
				if (!this.spline.Loop)
				{
					if (this.currentT >= 1f || this.currentT <= 0f)
					{
						this.currentClimber.ForceStopClimbing(false, true);
					}
				}
				else if (this.currentT >= 1f)
				{
					this.currentT = 0f;
				}
				else if (this.currentT <= 0f)
				{
					this.currentT = 1f;
				}
				if (!this.slideHelper.isBeingClimbed)
				{
					this.Stop();
				}
			}
			if (this.currentInheritVelocityMulti < 1f)
			{
				this.currentInheritVelocityMulti += Time.deltaTime * 0.2f;
				this.currentInheritVelocityMulti = Mathf.Min(this.currentInheritVelocityMulti, 1f);
			}
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x000620DE File Offset: 0x000602DE
		private void Stop()
		{
			this.currentClimber = null;
			this.audioSlide.Stop();
			this.audioSlide.gameObject.SetActive(false);
			this.currentInheritVelocityMulti = 0.55f;
			this.currentSpeed = 0f;
		}

		// Token: 0x040013E8 RID: 5096
		[SerializeField]
		private Transform segmentsRoot;

		// Token: 0x040013E9 RID: 5097
		[SerializeField]
		private GameObject segmentPrefab;

		// Token: 0x040013EA RID: 5098
		[SerializeField]
		private GorillaClimbable slideHelper;

		// Token: 0x040013EB RID: 5099
		[SerializeField]
		private AudioSource audioSlide;

		// Token: 0x040013EC RID: 5100
		private BezierSpline spline;

		// Token: 0x040013ED RID: 5101
		[SerializeField]
		private Transform climbOffsetHelper;

		// Token: 0x040013EE RID: 5102
		[SerializeField]
		private GorillaZiplineSettings settings;

		// Token: 0x040013F0 RID: 5104
		[SerializeField]
		private float ziplineDistance = 15f;

		// Token: 0x040013F1 RID: 5105
		[SerializeField]
		private float segmentDistance = 0.9f;

		// Token: 0x040013F2 RID: 5106
		private GorillaHandClimber currentClimber;

		// Token: 0x040013F3 RID: 5107
		private float currentT;

		// Token: 0x040013F4 RID: 5108
		private const float inheritVelocityRechargeRate = 0.2f;

		// Token: 0x040013F5 RID: 5109
		private const float inheritVelocityValueOnRelease = 0.55f;

		// Token: 0x040013F6 RID: 5110
		private float currentInheritVelocityMulti = 1f;
	}
}
