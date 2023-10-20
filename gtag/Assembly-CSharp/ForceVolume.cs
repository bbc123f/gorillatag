using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class ForceVolume : MonoBehaviour
{
	// Token: 0x06000C52 RID: 3154 RVA: 0x0004AC4B File Offset: 0x00048E4B
	private void Awake()
	{
		this.volume = base.GetComponent<Collider>();
		this.audioState = ForceVolume.AudioState.None;
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x0004AC60 File Offset: 0x00048E60
	private void LateUpdate()
	{
		if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
		{
			this.audioSource.enabled = false;
		}
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x0004ACB0 File Offset: 0x00048EB0
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

	// Token: 0x06000C55 RID: 3157 RVA: 0x0004AD10 File Offset: 0x00048F10
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

	// Token: 0x06000C56 RID: 3158 RVA: 0x0004AD7C File Offset: 0x00048F7C
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

	// Token: 0x06000C57 RID: 3159 RVA: 0x0004ADCC File Offset: 0x00048FCC
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

	// Token: 0x06000C58 RID: 3160 RVA: 0x0004B1B8 File Offset: 0x000493B8
	public void OnDrawGizmosSelected()
	{
		base.GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
	}

	// Token: 0x04000FB5 RID: 4021
	[SerializeField]
	public bool scaleWithSize = true;

	// Token: 0x04000FB6 RID: 4022
	[SerializeField]
	private float accel;

	// Token: 0x04000FB7 RID: 4023
	[SerializeField]
	private float maxDepth = -1f;

	// Token: 0x04000FB8 RID: 4024
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04000FB9 RID: 4025
	[SerializeField]
	private bool disableGrip;

	// Token: 0x04000FBA RID: 4026
	[SerializeField]
	private float dampenXVelPerc;

	// Token: 0x04000FBB RID: 4027
	[SerializeField]
	private float dampenZVelPerc;

	// Token: 0x04000FBC RID: 4028
	[SerializeField]
	private float pullToCenterAccel;

	// Token: 0x04000FBD RID: 4029
	[SerializeField]
	private float pullToCenterMaxSpeed;

	// Token: 0x04000FBE RID: 4030
	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	// Token: 0x04000FBF RID: 4031
	private Collider volume;

	// Token: 0x04000FC0 RID: 4032
	public AudioClip enterClip;

	// Token: 0x04000FC1 RID: 4033
	public AudioClip exitClip;

	// Token: 0x04000FC2 RID: 4034
	public AudioClip loopClip;

	// Token: 0x04000FC3 RID: 4035
	public AudioClip loopCresendoClip;

	// Token: 0x04000FC4 RID: 4036
	public AudioSource audioSource;

	// Token: 0x04000FC5 RID: 4037
	private Vector3 enterPos;

	// Token: 0x04000FC6 RID: 4038
	private ForceVolume.AudioState audioState;

	// Token: 0x0200046A RID: 1130
	private enum AudioState
	{
		// Token: 0x04001E72 RID: 7794
		None,
		// Token: 0x04001E73 RID: 7795
		Enter,
		// Token: 0x04001E74 RID: 7796
		Crescendo,
		// Token: 0x04001E75 RID: 7797
		Loop,
		// Token: 0x04001E76 RID: 7798
		Exit
	}
}
