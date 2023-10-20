using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020001C0 RID: 448
public class GorillaThrowable : MonoBehaviourPun, IPunObservable, IPhotonViewCallback
{
	// Token: 0x06000B64 RID: 2916 RVA: 0x00045E58 File Offset: 0x00044058
	public virtual void Start()
	{
		this.offset = Vector3.zero;
		this.headsetTransform = Player.Instance.headCollider.transform;
		this.velocityHistory = new Vector3[this.trackingHistorySize];
		this.positionHistory = new Vector3[this.trackingHistorySize];
		this.headsetPositionHistory = new Vector3[this.trackingHistorySize];
		this.rotationHistory = new Vector3[this.trackingHistorySize];
		this.rotationalVelocityHistory = new Vector3[this.trackingHistorySize];
		for (int i = 0; i < this.trackingHistorySize; i++)
		{
			this.velocityHistory[i] = Vector3.zero;
			this.positionHistory[i] = base.transform.position - this.headsetTransform.position;
			this.headsetPositionHistory[i] = this.headsetTransform.position;
			this.rotationHistory[i] = base.transform.eulerAngles;
			this.rotationalVelocityHistory[i] = Vector3.zero;
		}
		this.currentIndex = 0;
		this.rigidbody = base.GetComponentInChildren<Rigidbody>();
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x00045F78 File Offset: 0x00044178
	public virtual void LateUpdate()
	{
		if (this.isHeld && base.photonView.IsMine)
		{
			base.transform.rotation = this.transformToFollow.rotation * this.offsetRotation;
			if (!this.initialLerp && (base.transform.position - this.transformToFollow.position).magnitude > this.lerpDistanceLimit)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.transformToFollow.position + this.transformToFollow.rotation * this.offset, this.pickupLerp);
			}
			else
			{
				this.initialLerp = true;
				base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
			}
		}
		if (!base.photonView.IsMine)
		{
			this.rigidbody.isKinematic = true;
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.lerpValue);
		}
		this.StoreHistories();
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x000460E2 File Offset: 0x000442E2
	private void IsHandPushing(XRNode node)
	{
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x000460E4 File Offset: 0x000442E4
	private void StoreHistories()
	{
		this.previousPosition = this.positionHistory[this.currentIndex];
		this.previousRotation = this.rotationHistory[this.currentIndex];
		this.previousHeadsetPosition = this.headsetPositionHistory[this.currentIndex];
		this.currentIndex = (this.currentIndex + 1) % this.trackingHistorySize;
		this.currentVelocity = (base.transform.position - this.headsetTransform.position - this.previousPosition) / Time.deltaTime;
		this.currentHeadsetVelocity = (this.headsetTransform.position - this.previousHeadsetPosition) / Time.deltaTime;
		this.currentRotationalVelocity = (base.transform.eulerAngles - this.previousRotation) / Time.deltaTime;
		this.denormalizedVelocityAverage = Vector3.zero;
		this.denormalizedRotationalVelocityAverage = Vector3.zero;
		this.loopIndex = 0;
		while (this.loopIndex < this.trackingHistorySize)
		{
			this.denormalizedVelocityAverage += this.velocityHistory[this.loopIndex];
			this.denormalizedRotationalVelocityAverage += this.rotationalVelocityHistory[this.loopIndex];
			this.loopIndex++;
		}
		this.denormalizedVelocityAverage /= (float)this.trackingHistorySize;
		this.denormalizedRotationalVelocityAverage /= (float)this.trackingHistorySize;
		this.velocityHistory[this.currentIndex] = this.currentVelocity;
		this.positionHistory[this.currentIndex] = base.transform.position - this.headsetTransform.position;
		this.headsetPositionHistory[this.currentIndex] = this.headsetTransform.position;
		this.rotationHistory[this.currentIndex] = base.transform.eulerAngles;
		this.rotationalVelocityHistory[this.currentIndex] = this.currentRotationalVelocity;
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x00046310 File Offset: 0x00044510
	public virtual void Grabbed(Transform grabTransform)
	{
		this.grabbingTransform = grabTransform;
		this.isHeld = true;
		this.transformToFollow = this.grabbingTransform;
		this.offsetRotation = base.transform.rotation * Quaternion.Inverse(this.transformToFollow.rotation);
		this.initialLerp = false;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		base.photonView.RequestOwnership();
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x00046388 File Offset: 0x00044588
	public virtual void ThrowThisThingo()
	{
		this.transformToFollow = null;
		this.isHeld = false;
		this.synchThrow = true;
		this.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		this.rigidbody.isKinematic = false;
		this.rigidbody.useGravity = true;
		if (this.isLinear || this.denormalizedVelocityAverage.magnitude < this.linearMax)
		{
			if (this.denormalizedVelocityAverage.magnitude * this.throwMultiplier < this.throwMagnitudeLimit)
			{
				this.rigidbody.velocity = this.denormalizedVelocityAverage * this.throwMultiplier + this.currentHeadsetVelocity;
			}
			else
			{
				this.rigidbody.velocity = this.denormalizedVelocityAverage.normalized * this.throwMagnitudeLimit + this.currentHeadsetVelocity;
			}
		}
		else
		{
			this.rigidbody.velocity = this.denormalizedVelocityAverage.normalized * Mathf.Max(Mathf.Min(Mathf.Pow(this.throwMultiplier * this.denormalizedVelocityAverage.magnitude / this.linearMax, this.exponThrowMultMax), 0.1f) * this.denormalizedHeadsetVelocityAverage.magnitude, this.throwMagnitudeLimit) + this.currentHeadsetVelocity;
		}
		this.rigidbody.angularVelocity = this.denormalizedRotationalVelocityAverage * 3.1415927f / 180f;
		this.rigidbody.MovePosition(this.rigidbody.transform.position + this.rigidbody.velocity * Time.deltaTime);
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00046524 File Offset: 0x00044724
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.rigidbody.velocity);
			return;
		}
		this.targetPosition = (Vector3)stream.ReceiveNext();
		this.targetRotation = (Quaternion)stream.ReceiveNext();
		this.rigidbody.velocity = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x000465B4 File Offset: 0x000447B4
	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<GorillaSurfaceOverride>() != null)
		{
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("PlaySurfaceHit", RpcTarget.Others, new object[]
				{
					this.bounceAudioClip,
					this.InterpolateVolume()
				});
			}
			this.PlaySurfaceHit(collision.collider.GetComponent<GorillaSurfaceOverride>().overrideIndex, this.InterpolateVolume());
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0004662C File Offset: 0x0004482C
	[PunRPC]
	public void PlaySurfaceHit(int soundIndex, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < Player.Instance.materialData.Count)
		{
			this.audioSource.volume = tapVolume;
			this.audioSource.clip = (Player.Instance.materialData[soundIndex].overrideAudio ? Player.Instance.materialData[soundIndex].audio : Player.Instance.materialData[0].audio);
			this.audioSource.PlayOneShot(this.audioSource.clip);
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x000466C4 File Offset: 0x000448C4
	public float InterpolateVolume()
	{
		return (Mathf.Clamp(this.rigidbody.velocity.magnitude, this.minVelocity, this.maxVelocity) - this.minVelocity) / (this.maxVelocity - this.minVelocity) * (this.maxVolume - this.minVolume) + this.minVolume;
	}

	// Token: 0x04000EBF RID: 3775
	public int trackingHistorySize;

	// Token: 0x04000EC0 RID: 3776
	public float throwMultiplier;

	// Token: 0x04000EC1 RID: 3777
	public float throwMagnitudeLimit;

	// Token: 0x04000EC2 RID: 3778
	private Vector3[] velocityHistory;

	// Token: 0x04000EC3 RID: 3779
	private Vector3[] headsetVelocityHistory;

	// Token: 0x04000EC4 RID: 3780
	private Vector3[] positionHistory;

	// Token: 0x04000EC5 RID: 3781
	private Vector3[] headsetPositionHistory;

	// Token: 0x04000EC6 RID: 3782
	private Vector3[] rotationHistory;

	// Token: 0x04000EC7 RID: 3783
	private Vector3[] rotationalVelocityHistory;

	// Token: 0x04000EC8 RID: 3784
	private Vector3 previousPosition;

	// Token: 0x04000EC9 RID: 3785
	private Vector3 previousRotation;

	// Token: 0x04000ECA RID: 3786
	private Vector3 previousHeadsetPosition;

	// Token: 0x04000ECB RID: 3787
	private int currentIndex;

	// Token: 0x04000ECC RID: 3788
	private Vector3 currentVelocity;

	// Token: 0x04000ECD RID: 3789
	private Vector3 currentHeadsetVelocity;

	// Token: 0x04000ECE RID: 3790
	private Vector3 currentRotationalVelocity;

	// Token: 0x04000ECF RID: 3791
	public Vector3 denormalizedVelocityAverage;

	// Token: 0x04000ED0 RID: 3792
	private Vector3 denormalizedHeadsetVelocityAverage;

	// Token: 0x04000ED1 RID: 3793
	private Vector3 denormalizedRotationalVelocityAverage;

	// Token: 0x04000ED2 RID: 3794
	private Transform headsetTransform;

	// Token: 0x04000ED3 RID: 3795
	private Vector3 targetPosition;

	// Token: 0x04000ED4 RID: 3796
	private Quaternion targetRotation;

	// Token: 0x04000ED5 RID: 3797
	public bool initialLerp;

	// Token: 0x04000ED6 RID: 3798
	public float lerpValue = 0.4f;

	// Token: 0x04000ED7 RID: 3799
	public float lerpDistanceLimit = 0.01f;

	// Token: 0x04000ED8 RID: 3800
	public bool isHeld;

	// Token: 0x04000ED9 RID: 3801
	public Rigidbody rigidbody;

	// Token: 0x04000EDA RID: 3802
	private int loopIndex;

	// Token: 0x04000EDB RID: 3803
	private Transform transformToFollow;

	// Token: 0x04000EDC RID: 3804
	private Vector3 offset;

	// Token: 0x04000EDD RID: 3805
	private Quaternion offsetRotation;

	// Token: 0x04000EDE RID: 3806
	public AudioSource audioSource;

	// Token: 0x04000EDF RID: 3807
	public int timeLastReceived;

	// Token: 0x04000EE0 RID: 3808
	public bool synchThrow;

	// Token: 0x04000EE1 RID: 3809
	public float tempFloat;

	// Token: 0x04000EE2 RID: 3810
	public Transform grabbingTransform;

	// Token: 0x04000EE3 RID: 3811
	public float pickupLerp;

	// Token: 0x04000EE4 RID: 3812
	public float minVelocity;

	// Token: 0x04000EE5 RID: 3813
	public float maxVelocity;

	// Token: 0x04000EE6 RID: 3814
	public float minVolume;

	// Token: 0x04000EE7 RID: 3815
	public float maxVolume;

	// Token: 0x04000EE8 RID: 3816
	public bool isLinear;

	// Token: 0x04000EE9 RID: 3817
	public float linearMax;

	// Token: 0x04000EEA RID: 3818
	public float exponThrowMultMax;

	// Token: 0x04000EEB RID: 3819
	public int bounceAudioClip;
}
