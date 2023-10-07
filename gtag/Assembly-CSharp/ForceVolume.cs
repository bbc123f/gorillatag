using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class ForceVolume : MonoBehaviour
{
	// Token: 0x06000C4C RID: 3148 RVA: 0x0004A9E3 File Offset: 0x00048BE3
	private void Awake()
	{
		this.volume = base.GetComponent<Collider>();
		this.audioState = ForceVolume.AudioState.None;
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0004A9F8 File Offset: 0x00048BF8
	private void LateUpdate()
	{
		if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
		{
			this.audioSource.enabled = false;
		}
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0004AA48 File Offset: 0x00048C48
	private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
	{
		rb = null;
		xf = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
			xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
		}
		return rb != null && xf != null;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0004AAA8 File Offset: 0x00048CA8
	public void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.enterClip == null)
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.PlayOneShot(this.enterClip);
			this.audioState = ForceVolume.AudioState.Enter;
		}
		this.enterPos = transform.position;
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x0004AB14 File Offset: 0x00048D14
	public void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.PlayOneShot(this.exitClip);
			this.audioState = ForceVolume.AudioState.None;
		}
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0004AB64 File Offset: 0x00048D64
	public void OnTriggerStay(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource && !this.audioSource.isPlaying)
		{
			ForceVolume.AudioState audioState = this.audioState;
			if (audioState != ForceVolume.AudioState.Enter)
			{
				if (audioState == ForceVolume.AudioState.Loop)
				{
					if (this.loopClip != null)
					{
						this.audioSource.enabled = true;
						this.audioSource.PlayOneShot(this.loopClip);
					}
					this.audioState = ForceVolume.AudioState.Loop;
				}
			}
			else
			{
				if (this.loopCresendoClip != null)
				{
					this.audioSource.enabled = true;
					this.audioSource.PlayOneShot(this.loopCresendoClip);
				}
				this.audioState = ForceVolume.AudioState.Crescendo;
			}
		}
		if (this.disableGrip)
		{
			Player.Instance.SetMaximumSlipThisFrame();
		}
		SizeManager sizeManager = null;
		if (this.scaleWithSize)
		{
			sizeManager = rigidbody.GetComponent<SizeManager>();
		}
		Vector3 vector = rigidbody.velocity;
		if (this.scaleWithSize && sizeManager)
		{
			vector /= sizeManager.currentScale;
		}
		Vector3 b = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
		Vector3 a = base.transform.position + b - transform.position;
		float num = a.magnitude + 0.0001f;
		Vector3 vector2 = a / num;
		float num2 = Vector3.Dot(vector, vector2);
		if (this.maxDepth > -1f)
		{
			float num3 = Vector3.Dot(transform.position - this.enterPos, vector2);
			float num4 = this.maxDepth - num3;
			float b2 = 0f;
			if (num4 > 0.0001f)
			{
				b2 = num2 * num2 / num4;
			}
			this.accel = Mathf.Max(this.accel, b2);
		}
		float deltaTime = Time.deltaTime;
		Vector3 b3 = base.transform.up * this.accel * deltaTime;
		vector += b3;
		Vector3 a2 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
		Vector3 a3 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
		Vector3 a4 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
		float d = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
		float d2 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
		vector = a2 + d * a3 + d2 * a4;
		if (this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
		{
			vector -= num2 * vector2;
			if (num > this.pullTOCenterMinDistance)
			{
				num2 += this.pullToCenterAccel * deltaTime;
				float b4 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
				num2 = Mathf.Min(num2, b4);
			}
			else
			{
				num2 = 0f;
			}
			vector += num2 * vector2;
			if (vector.magnitude > 0.0001f)
			{
				Vector3 vector3 = Vector3.Cross(base.transform.up, vector2);
				float magnitude = vector3.magnitude;
				if (magnitude > 0.0001f)
				{
					vector3 /= magnitude;
					num2 = Vector3.Dot(vector, vector3);
					vector -= num2 * vector3;
					num2 -= this.pullToCenterAccel * deltaTime;
					num2 = Mathf.Max(0f, num2);
					vector += num2 * vector3;
				}
			}
		}
		if (this.scaleWithSize && sizeManager)
		{
			vector *= sizeManager.currentScale;
		}
		rigidbody.velocity = vector;
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0004AF50 File Offset: 0x00049150
	public void OnDrawGizmosSelected()
	{
		base.GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
	}

	// Token: 0x04000FB1 RID: 4017
	[SerializeField]
	public bool scaleWithSize = true;

	// Token: 0x04000FB2 RID: 4018
	[SerializeField]
	private float accel;

	// Token: 0x04000FB3 RID: 4019
	[SerializeField]
	private float maxDepth = -1f;

	// Token: 0x04000FB4 RID: 4020
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04000FB5 RID: 4021
	[SerializeField]
	private bool disableGrip;

	// Token: 0x04000FB6 RID: 4022
	[SerializeField]
	private float dampenXVelPerc;

	// Token: 0x04000FB7 RID: 4023
	[SerializeField]
	private float dampenZVelPerc;

	// Token: 0x04000FB8 RID: 4024
	[SerializeField]
	private float pullToCenterAccel;

	// Token: 0x04000FB9 RID: 4025
	[SerializeField]
	private float pullToCenterMaxSpeed;

	// Token: 0x04000FBA RID: 4026
	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	// Token: 0x04000FBB RID: 4027
	private Collider volume;

	// Token: 0x04000FBC RID: 4028
	public AudioClip enterClip;

	// Token: 0x04000FBD RID: 4029
	public AudioClip exitClip;

	// Token: 0x04000FBE RID: 4030
	public AudioClip loopClip;

	// Token: 0x04000FBF RID: 4031
	public AudioClip loopCresendoClip;

	// Token: 0x04000FC0 RID: 4032
	public AudioSource audioSource;

	// Token: 0x04000FC1 RID: 4033
	private Vector3 enterPos;

	// Token: 0x04000FC2 RID: 4034
	private ForceVolume.AudioState audioState;

	// Token: 0x02000468 RID: 1128
	private enum AudioState
	{
		// Token: 0x04001E65 RID: 7781
		None,
		// Token: 0x04001E66 RID: 7782
		Enter,
		// Token: 0x04001E67 RID: 7783
		Crescendo,
		// Token: 0x04001E68 RID: 7784
		Loop,
		// Token: 0x04001E69 RID: 7785
		Exit
	}
}
