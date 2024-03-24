using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

public class HitTargetScoreDisplay : MonoBehaviour
{
	protected void Awake()
	{
		this.rotateTimeTotal = 180f / (float)this.rotateSpeed;
		this.matPropBlock = new MaterialPropertyBlock();
		this.networkedScore.AddCallback(new Action<int>(this.OnScoreChanged), true);
		this.ResetRotation();
		this.tensOld = 0;
		this.hundredsOld = 0;
		this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[0]);
		this.singlesRend.SetPropertyBlock(this.matPropBlock);
		this.tensRend.SetPropertyBlock(this.matPropBlock);
		this.hundredsRend.SetPropertyBlock(this.matPropBlock);
	}

	private void OnDestroy()
	{
		this.networkedScore.RemoveCallback(new Action<int>(this.OnScoreChanged));
	}

	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		this.singlesCard.rotation = rotation;
		this.tensCard.rotation = rotation;
		this.hundredsCard.rotation = rotation;
	}

	private IEnumerator RotatingCo()
	{
		float timeElapsedSinceHit = 0f;
		int singlesPlace = this.currentScore % 10;
		int tensPlace = this.currentScore / 10 % 10;
		bool tensChange = this.tensOld != tensPlace;
		this.tensOld = tensPlace;
		int hundredsPlace = this.currentScore / 100 % 10;
		bool hundredsChange = this.hundredsOld != hundredsPlace;
		this.hundredsOld = hundredsPlace;
		bool digitsChange = true;
		while (timeElapsedSinceHit < this.rotateTimeTotal)
		{
			this.singlesCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
			Vector3 localEulerAngles = this.singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			this.singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				this.tensCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles2 = this.tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				this.tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				this.hundredsCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles3 = this.hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				this.hundredsCard.localEulerAngles = localEulerAngles3;
			}
			if (digitsChange && timeElapsedSinceHit >= this.rotateTimeTotal / 2f)
			{
				this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[singlesPlace]);
				this.singlesRend.SetPropertyBlock(this.matPropBlock);
				if (tensChange)
				{
					this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[tensPlace]);
					this.tensRend.SetPropertyBlock(this.matPropBlock);
				}
				if (hundredsChange)
				{
					this.matPropBlock.SetVector(this.shaderPropID_MainTex_ST, this.numberSheet[hundredsPlace]);
					this.hundredsRend.SetPropertyBlock(this.matPropBlock);
				}
				digitsChange = false;
			}
			yield return null;
			timeElapsedSinceHit += Time.deltaTime;
		}
		this.ResetRotation();
		yield break;
		yield break;
	}

	private void OnScoreChanged(int newScore)
	{
		if (newScore == this.currentScore)
		{
			return;
		}
		if (this.currentRotationCoroutine != null)
		{
			base.StopCoroutine(this.currentRotationCoroutine);
		}
		this.currentScore = newScore;
		this.currentRotationCoroutine = base.StartCoroutine(this.RotatingCo());
	}

	public HitTargetScoreDisplay()
	{
	}

	[SerializeField]
	private WatchableIntSO networkedScore;

	private int currentScore;

	private int tensOld;

	private int hundredsOld;

	private float rotateTimeTotal;

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

	public Transform singlesCard;

	public Transform tensCard;

	public Transform hundredsCard;

	public Renderer singlesRend;

	public Renderer tensRend;

	public Renderer hundredsRend;

	private Coroutine currentRotationCoroutine;

	[CompilerGenerated]
	private sealed class <RotatingCo>d__19 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <RotatingCo>d__19(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			HitTargetScoreDisplay hitTargetScoreDisplay = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				timeElapsedSinceHit += Time.deltaTime;
			}
			else
			{
				this.<>1__state = -1;
				timeElapsedSinceHit = 0f;
				singlesPlace = hitTargetScoreDisplay.currentScore % 10;
				tensPlace = hitTargetScoreDisplay.currentScore / 10 % 10;
				tensChange = hitTargetScoreDisplay.tensOld != tensPlace;
				hitTargetScoreDisplay.tensOld = tensPlace;
				hundredsPlace = hitTargetScoreDisplay.currentScore / 100 % 10;
				hundredsChange = hitTargetScoreDisplay.hundredsOld != hundredsPlace;
				hitTargetScoreDisplay.hundredsOld = hundredsPlace;
				digitsChange = true;
			}
			if (timeElapsedSinceHit >= hitTargetScoreDisplay.rotateTimeTotal)
			{
				hitTargetScoreDisplay.ResetRotation();
				return false;
			}
			hitTargetScoreDisplay.singlesCard.Rotate((float)hitTargetScoreDisplay.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
			Vector3 localEulerAngles = hitTargetScoreDisplay.singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			hitTargetScoreDisplay.singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				hitTargetScoreDisplay.tensCard.Rotate((float)hitTargetScoreDisplay.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles2 = hitTargetScoreDisplay.tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				hitTargetScoreDisplay.tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				hitTargetScoreDisplay.hundredsCard.Rotate((float)hitTargetScoreDisplay.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles3 = hitTargetScoreDisplay.hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				hitTargetScoreDisplay.hundredsCard.localEulerAngles = localEulerAngles3;
			}
			if (digitsChange && timeElapsedSinceHit >= hitTargetScoreDisplay.rotateTimeTotal / 2f)
			{
				hitTargetScoreDisplay.matPropBlock.SetVector(hitTargetScoreDisplay.shaderPropID_MainTex_ST, hitTargetScoreDisplay.numberSheet[singlesPlace]);
				hitTargetScoreDisplay.singlesRend.SetPropertyBlock(hitTargetScoreDisplay.matPropBlock);
				if (tensChange)
				{
					hitTargetScoreDisplay.matPropBlock.SetVector(hitTargetScoreDisplay.shaderPropID_MainTex_ST, hitTargetScoreDisplay.numberSheet[tensPlace]);
					hitTargetScoreDisplay.tensRend.SetPropertyBlock(hitTargetScoreDisplay.matPropBlock);
				}
				if (hundredsChange)
				{
					hitTargetScoreDisplay.matPropBlock.SetVector(hitTargetScoreDisplay.shaderPropID_MainTex_ST, hitTargetScoreDisplay.numberSheet[hundredsPlace]);
					hitTargetScoreDisplay.hundredsRend.SetPropertyBlock(hitTargetScoreDisplay.matPropBlock);
				}
				digitsChange = false;
			}
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public HitTargetScoreDisplay <>4__this;

		private float <timeElapsedSinceHit>5__2;

		private int <singlesPlace>5__3;

		private int <tensPlace>5__4;

		private bool <tensChange>5__5;

		private int <hundredsPlace>5__6;

		private bool <hundredsChange>5__7;

		private bool <digitsChange>5__8;
	}
}
