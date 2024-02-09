using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HitTargetWithScoreCounter : MonoBehaviourPunCallbacks, IPunObservable
{
	protected void Awake()
	{
		this.rotateTimeTotal = 180f / (float)this.rotateSpeed;
		this.matPropBlock = new MaterialPropertyBlock();
		this.audioPlayer = base.GetComponent<AudioSource>();
		SlingshotProjectileHitNotifier component = base.GetComponent<SlingshotProjectileHitNotifier>();
		if (component != null)
		{
			component.OnProjectileHit += this.ProjectileHitReciever;
			component.OnProjectileCollisionStay += this.ProjectileHitReciever;
			return;
		}
		Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject to increment score");
	}

	private void SetInitialState()
	{
		this.networkedScore = 0;
		this.currentScore = 0;
		this.timeElapsedSinceHit = 0f;
		this.ResetRotation();
		this.audioPlayer.Stop();
		this.tensOld = 0;
		this.hundredsOld = 0;
		this.tensChange = false;
		this.hundredsChange = false;
		this.digitsChange = false;
		this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[0]);
		this.singlesRend.SetPropertyBlock(this.matPropBlock);
		this.tensRend.SetPropertyBlock(this.matPropBlock);
		this.hundredsRend.SetPropertyBlock(this.matPropBlock);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		this.OnLeftRoom();
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.SetInitialState();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.TargetHit();
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		this.singlesCard.rotation = rotation;
		this.tensCard.rotation = rotation;
		this.hundredsCard.rotation = rotation;
		this.singlesCard.Rotate(-90f, 0f, 0f);
		this.tensCard.Rotate(-90f, 0f, 0f);
		this.hundredsCard.Rotate(-90f, 0f, 0f);
		this.isRotating = false;
	}

	protected void Update()
	{
		this.timeElapsedSinceHit += Time.deltaTime;
		if (this.isRotating)
		{
			if (this.timeElapsedSinceHit >= this.rotateTimeTotal)
			{
				this.ResetRotation();
			}
			else
			{
				this.singlesCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles = this.singlesCard.localEulerAngles;
				localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
				this.singlesCard.localEulerAngles = localEulerAngles;
				if (this.tensChange)
				{
					this.tensCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
					Vector3 localEulerAngles2 = this.tensCard.localEulerAngles;
					localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
					this.tensCard.localEulerAngles = localEulerAngles2;
				}
				if (this.hundredsChange)
				{
					this.hundredsCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
					Vector3 localEulerAngles3 = this.hundredsCard.localEulerAngles;
					localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
					this.hundredsCard.localEulerAngles = localEulerAngles3;
				}
			}
			if (this.digitsChange && this.timeElapsedSinceHit >= this.rotateTimeTotal / 2f)
			{
				this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[this.singlesPlace]);
				this.singlesRend.SetPropertyBlock(this.matPropBlock);
				if (this.tensChange)
				{
					this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[this.tensPlace]);
					this.tensRend.SetPropertyBlock(this.matPropBlock);
				}
				if (this.hundredsChange)
				{
					this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[this.hundredsPlace]);
					this.hundredsRend.SetPropertyBlock(this.matPropBlock);
				}
				this.digitsChange = false;
			}
		}
	}

	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit();
	}

	public void TargetHit()
	{
		if (PhotonNetwork.IsMasterClient && this.timeElapsedSinceHit >= (float)this.hitCooldownTime)
		{
			this.networkedScore++;
			if (this.networkedScore >= 1000)
			{
				this.networkedScore = 0;
			}
		}
		this.UpdateTargetState();
	}

	private void UpdateTargetState()
	{
		if (this.networkedScore != this.currentScore)
		{
			if (this.currentScore > this.networkedScore)
			{
				this.audioPlayer.PlayOneShot(this.audioClips[1]);
			}
			else
			{
				this.audioPlayer.PlayOneShot(this.audioClips[0]);
			}
			this.currentScore = this.networkedScore;
			this.timeElapsedSinceHit = 0f;
			this.singlesPlace = this.currentScore % 10;
			this.tensPlace = this.currentScore / 10 % 10;
			this.tensChange = this.tensOld != this.tensPlace;
			this.tensOld = this.tensPlace;
			this.hundredsPlace = this.currentScore / 100 % 10;
			this.hundredsChange = this.hundredsOld != this.hundredsPlace;
			this.hundredsOld = this.hundredsPlace;
			this.isRotating = true;
			this.digitsChange = true;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.networkedScore);
		}
		else
		{
			this.networkedScore = (int)stream.ReceiveNext();
		}
		this.UpdateTargetState();
	}

	private int networkedScore;

	private int currentScore;

	private int singlesPlace;

	private int tensPlace;

	private int hundredsPlace;

	private int tensOld;

	private int hundredsOld;

	private float timeElapsedSinceHit;

	private bool isRotating;

	private float rotateTimeTotal;

	private bool tensChange;

	private bool hundredsChange;

	private bool digitsChange = true;

	private int shaderPropID_MainTex_ST = Shader.PropertyToID("_BaseMap_ST");

	private MaterialPropertyBlock matPropBlock;

	private readonly Vector4[] numberSheet = new Vector4[]
	{
		new Vector4(1f, 1f, 0.8f, -0.5f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0.2f, 0f),
		new Vector4(1f, 1f, 0.4f, 0f),
		new Vector4(1f, 1f, 0.6f, 0f),
		new Vector4(1f, 1f, 0.8f, 0f),
		new Vector4(1f, 1f, 0f, -0.5f),
		new Vector4(1f, 1f, 0.2f, -0.5f),
		new Vector4(1f, 1f, 0.4f, -0.5f),
		new Vector4(1f, 1f, 0.6f, -0.5f)
	};

	public int rotateSpeed = 180;

	public int hitCooldownTime = 1;

	public Transform singlesCard;

	public Transform tensCard;

	public Transform hundredsCard;

	public Renderer singlesRend;

	public Renderer tensRend;

	public Renderer hundredsRend;

	public AudioSource audioPlayer;

	public AudioClip[] audioClips;

	public bool testPress;
}
