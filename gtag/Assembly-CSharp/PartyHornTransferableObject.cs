using System;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000033 RID: 51
public class PartyHornTransferableObject : TransferrableObject
{
	// Token: 0x06000128 RID: 296 RVA: 0x0000A60A File Offset: 0x0000880A
	public override void OnEnable()
	{
		base.OnEnable();
		this.localHead = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
		this.InitToDefault();
	}

	// Token: 0x06000129 RID: 297 RVA: 0x0000A637 File Offset: 0x00008837
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x0600012A RID: 298 RVA: 0x0000A648 File Offset: 0x00008848
	protected Vector3 CalcMouthPiecePos()
	{
		Transform transform = base.transform;
		Vector3 vector = transform.position;
		if (this.mouthPiece)
		{
			vector += transform.InverseTransformPoint(this.mouthPiece.position);
		}
		else
		{
			vector += transform.forward * this.mouthPieceZOffset;
		}
		return vector;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x0000A6A4 File Offset: 0x000088A4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState != TransferrableObject.ItemStates.State0)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		Transform transform = base.transform;
		Vector3 b = this.CalcMouthPiecePos();
		float num = this.mouthPieceRadius * this.mouthPieceRadius;
		bool flag = (this.localHead.TransformPoint(this.mouthOffset) - b).sqrMagnitude < num;
		if (this.soundActivated && PhotonNetwork.InRoom)
		{
			bool flag2;
			if (flag)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				if (instance == null)
				{
					flag2 = false;
				}
				else
				{
					Recorder myRecorder = instance.myRecorder;
					bool? flag3 = (myRecorder != null) ? new bool?(myRecorder.IsCurrentlyTransmitting) : null;
					bool flag4 = true;
					flag2 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
				}
			}
			else
			{
				flag2 = false;
			}
			flag = flag2;
		}
		for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
		{
			VRRig vrrig = GorillaParent.instance.vrrigs[i];
			if (vrrig.head == null || vrrig.head.rigTarget == null || flag)
			{
				break;
			}
			flag = ((vrrig.head.rigTarget.transform.TransformPoint(this.mouthOffset) - b).sqrMagnitude < num);
			if (this.soundActivated)
			{
				bool flag5;
				if (flag)
				{
					RigContainer component = vrrig.GetComponent<RigContainer>();
					if (component == null)
					{
						flag5 = false;
					}
					else
					{
						PhotonVoiceView voice = component.Voice;
						bool? flag3 = (voice != null) ? new bool?(voice.IsSpeaking) : null;
						bool flag4 = true;
						flag5 = (flag3.GetValueOrDefault() == flag4 & flag3 != null);
					}
				}
				else
				{
					flag5 = false;
				}
				flag = flag5;
			}
		}
		this.itemState = (flag ? TransferrableObject.ItemStates.State1 : this.itemState);
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000A860 File Offset: 0x00008A60
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (TransferrableObject.ItemStates.State1 != this.itemState)
		{
			return;
		}
		if (!this.localWasActivated)
		{
			this.effectsGameObject.SetActive(true);
			this.cooldownRemaining = this.cooldown;
			this.localWasActivated = true;
			UnityEvent onCooldownStart = this.OnCooldownStart;
			if (onCooldownStart != null)
			{
				onCooldownStart.Invoke();
			}
		}
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.InitToDefault();
		}
	}

	// Token: 0x0600012D RID: 301 RVA: 0x0000A8DA File Offset: 0x00008ADA
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		this.effectsGameObject.SetActive(false);
		this.cooldownRemaining = this.cooldown;
		this.localWasActivated = false;
		UnityEvent onCooldownReset = this.OnCooldownReset;
		if (onCooldownReset == null)
		{
			return;
		}
		onCooldownReset.Invoke();
	}

	// Token: 0x04000192 RID: 402
	[Tooltip("This GameObject will activate when held to any gorilla's mouth.")]
	public GameObject effectsGameObject;

	// Token: 0x04000193 RID: 403
	public float cooldown = 2f;

	// Token: 0x04000194 RID: 404
	public float mouthPieceZOffset = -0.18f;

	// Token: 0x04000195 RID: 405
	public float mouthPieceRadius = 0.05f;

	// Token: 0x04000196 RID: 406
	public Transform mouthPiece;

	// Token: 0x04000197 RID: 407
	public Vector3 mouthOffset = new Vector3(0f, 0.02f, 0.17f);

	// Token: 0x04000198 RID: 408
	public bool soundActivated;

	// Token: 0x04000199 RID: 409
	public UnityEvent OnCooldownStart;

	// Token: 0x0400019A RID: 410
	public UnityEvent OnCooldownReset;

	// Token: 0x0400019B RID: 411
	private float cooldownRemaining;

	// Token: 0x0400019C RID: 412
	private Transform localHead;

	// Token: 0x0400019D RID: 413
	private PartyHornTransferableObject.PartyHornState partyHornStateLastFrame;

	// Token: 0x0400019E RID: 414
	private bool localWasActivated;

	// Token: 0x0200038E RID: 910
	private enum PartyHornState
	{
		// Token: 0x04001B0D RID: 6925
		None = 1,
		// Token: 0x04001B0E RID: 6926
		CoolingDown
	}
}
