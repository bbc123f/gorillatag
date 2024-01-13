using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HitTargetWithScoreCounter : MonoBehaviourPunCallbacks, IPunObservable
{
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

	private int shaderPropID_MainTex_ST = Shader.PropertyToID("_MainTex_ST");

	private MaterialPropertyBlock matPropBlock;

	private readonly Vector4[] numberSheet = new Vector4[10]
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

	protected void Awake()
	{
		rotateTimeTotal = 180f / (float)rotateSpeed;
		matPropBlock = new MaterialPropertyBlock();
		audioPlayer = GetComponent<AudioSource>();
	}

	private void SetInitialState()
	{
		networkedScore = 0;
		currentScore = 0;
		timeElapsedSinceHit = 0f;
		ResetRotation();
		audioPlayer.Stop();
		tensOld = 0;
		hundredsOld = 0;
		tensChange = false;
		hundredsChange = false;
		digitsChange = false;
		matPropBlock.SetVector(shaderPropID_MainTex_ST, numberSheet[0]);
		singlesRend.SetPropertyBlock(matPropBlock);
		tensRend.SetPropertyBlock(matPropBlock);
		hundredsRend.SetPropertyBlock(matPropBlock);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		OnLeftRoom();
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		SetInitialState();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (Application.isEditor)
		{
			StartCoroutine(TestPressCheck());
		}
		SetInitialState();
	}

	private IEnumerator TestPressCheck()
	{
		while (true)
		{
			if (testPress)
			{
				testPress = false;
				TargetHit();
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		singlesCard.rotation = rotation;
		tensCard.rotation = rotation;
		hundredsCard.rotation = rotation;
		singlesCard.Rotate(-90f, 0f, 0f);
		tensCard.Rotate(-90f, 0f, 0f);
		hundredsCard.Rotate(-90f, 0f, 0f);
		isRotating = false;
	}

	protected void Update()
	{
		timeElapsedSinceHit += Time.deltaTime;
		if (!isRotating)
		{
			return;
		}
		if (timeElapsedSinceHit >= rotateTimeTotal)
		{
			ResetRotation();
		}
		else
		{
			singlesCard.Rotate((float)rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
			Vector3 localEulerAngles = singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				tensCard.Rotate((float)rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles2 = tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				hundredsCard.Rotate((float)rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles3 = hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				hundredsCard.localEulerAngles = localEulerAngles3;
			}
		}
		if (digitsChange && timeElapsedSinceHit >= rotateTimeTotal / 2f)
		{
			matPropBlock.SetVector(shaderPropID_MainTex_ST, numberSheet[singlesPlace]);
			singlesRend.SetPropertyBlock(matPropBlock);
			if (tensChange)
			{
				matPropBlock.SetVector(shaderPropID_MainTex_ST, numberSheet[tensPlace]);
				tensRend.SetPropertyBlock(matPropBlock);
			}
			if (hundredsChange)
			{
				matPropBlock.SetVector(shaderPropID_MainTex_ST, numberSheet[hundredsPlace]);
				hundredsRend.SetPropertyBlock(matPropBlock);
			}
			digitsChange = false;
		}
	}

	public void TargetHit()
	{
		if (PhotonNetwork.IsMasterClient && timeElapsedSinceHit >= (float)hitCooldownTime)
		{
			networkedScore++;
			if (networkedScore >= 1000)
			{
				networkedScore = 0;
			}
		}
		UpdateTargetState();
	}

	private void UpdateTargetState()
	{
		if (networkedScore != currentScore)
		{
			if (currentScore > networkedScore)
			{
				audioPlayer.PlayOneShot(audioClips[1]);
			}
			else
			{
				audioPlayer.PlayOneShot(audioClips[0]);
			}
			currentScore = networkedScore;
			timeElapsedSinceHit = 0f;
			singlesPlace = currentScore % 10;
			tensPlace = currentScore / 10 % 10;
			tensChange = tensOld != tensPlace;
			tensOld = tensPlace;
			hundredsPlace = currentScore / 100 % 10;
			hundredsChange = hundredsOld != hundredsPlace;
			hundredsOld = hundredsPlace;
			isRotating = true;
			digitsChange = true;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(networkedScore);
		}
		else
		{
			networkedScore = (int)stream.ReceiveNext();
		}
		UpdateTargetState();
	}
}
