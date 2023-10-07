using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class HitTargetWithScoreCounter : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000504 RID: 1284 RVA: 0x0001FFC8 File Offset: 0x0001E1C8
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

	// Token: 0x06000505 RID: 1285 RVA: 0x00020040 File Offset: 0x0001E240
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

	// Token: 0x06000506 RID: 1286 RVA: 0x000200EA File Offset: 0x0001E2EA
	public override void OnDisconnected(DisconnectCause cause)
	{
		this.OnLeftRoom();
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x000200F2 File Offset: 0x0001E2F2
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.SetInitialState();
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00020100 File Offset: 0x0001E300
	public override void OnEnable()
	{
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00020122 File Offset: 0x0001E322
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

	// Token: 0x0600050A RID: 1290 RVA: 0x00020134 File Offset: 0x0001E334
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

	// Token: 0x0600050B RID: 1291 RVA: 0x000201C8 File Offset: 0x0001E3C8
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

	// Token: 0x0600050C RID: 1292 RVA: 0x000203F2 File Offset: 0x0001E5F2
	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit();
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x000203FA File Offset: 0x0001E5FA
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

	// Token: 0x0600050E RID: 1294 RVA: 0x0002043C File Offset: 0x0001E63C
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
			this.tensChange = (this.tensOld != this.tensPlace);
			this.tensOld = this.tensPlace;
			this.hundredsPlace = this.currentScore / 100 % 10;
			this.hundredsChange = (this.hundredsOld != this.hundredsPlace);
			this.hundredsOld = this.hundredsPlace;
			this.isRotating = true;
			this.digitsChange = true;
		}
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0002052E File Offset: 0x0001E72E
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

	// Token: 0x040005EC RID: 1516
	private int networkedScore;

	// Token: 0x040005ED RID: 1517
	private int currentScore;

	// Token: 0x040005EE RID: 1518
	private int singlesPlace;

	// Token: 0x040005EF RID: 1519
	private int tensPlace;

	// Token: 0x040005F0 RID: 1520
	private int hundredsPlace;

	// Token: 0x040005F1 RID: 1521
	private int tensOld;

	// Token: 0x040005F2 RID: 1522
	private int hundredsOld;

	// Token: 0x040005F3 RID: 1523
	private float timeElapsedSinceHit;

	// Token: 0x040005F4 RID: 1524
	private bool isRotating;

	// Token: 0x040005F5 RID: 1525
	private float rotateTimeTotal;

	// Token: 0x040005F6 RID: 1526
	private bool tensChange;

	// Token: 0x040005F7 RID: 1527
	private bool hundredsChange;

	// Token: 0x040005F8 RID: 1528
	private bool digitsChange = true;

	// Token: 0x040005F9 RID: 1529
	private int shaderPropID_MainTex_ST = Shader.PropertyToID("_BaseMap_ST");

	// Token: 0x040005FA RID: 1530
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x040005FB RID: 1531
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

	// Token: 0x040005FC RID: 1532
	public int rotateSpeed = 180;

	// Token: 0x040005FD RID: 1533
	public int hitCooldownTime = 1;

	// Token: 0x040005FE RID: 1534
	public Transform singlesCard;

	// Token: 0x040005FF RID: 1535
	public Transform tensCard;

	// Token: 0x04000600 RID: 1536
	public Transform hundredsCard;

	// Token: 0x04000601 RID: 1537
	public Renderer singlesRend;

	// Token: 0x04000602 RID: 1538
	public Renderer tensRend;

	// Token: 0x04000603 RID: 1539
	public Renderer hundredsRend;

	// Token: 0x04000604 RID: 1540
	public AudioSource audioPlayer;

	// Token: 0x04000605 RID: 1541
	public AudioClip[] audioClips;

	// Token: 0x04000606 RID: 1542
	public bool testPress;
}
