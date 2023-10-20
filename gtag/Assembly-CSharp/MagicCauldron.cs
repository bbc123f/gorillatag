using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class MagicCauldron : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000A53 RID: 2643 RVA: 0x0004043C File Offset: 0x0003E63C
	private void Awake()
	{
		this.currentIngredients.Clear();
		this.witchesComponent.Clear();
		this.currentStateElapsedTime = 0f;
		this.currentRecipeIndex = -1;
		this.ingredientIndex = -1;
		if (this.flyingWitchesContainer != null)
		{
			for (int i = 0; i < this.flyingWitchesContainer.transform.childCount; i++)
			{
				NoncontrollableBroomstick componentInChildren = this.flyingWitchesContainer.transform.GetChild(i).gameObject.GetComponentInChildren<NoncontrollableBroomstick>();
				this.witchesComponent.Add(componentInChildren);
				if (componentInChildren)
				{
					componentInChildren.gameObject.SetActive(false);
				}
			}
		}
		if (this.reusableFXContext == null)
		{
			this.reusableFXContext = new MagicCauldron.IngrediantFXContext();
		}
		if (this.reusableIngrediantArgs == null)
		{
			this.reusableIngrediantArgs = new MagicCauldron.IngredientArgs();
		}
		this.reusableFXContext.fxCallBack = new MagicCauldron.IngrediantFXContext.Callback(this.OnIngredientAdd);
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0004051A File Offset: 0x0003E71A
	private void Start()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x00040523 File Offset: 0x0003E723
	private void LateUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0004052B File Offset: 0x0003E72B
	private IEnumerator LevitationSpellCoroutine()
	{
		Player.Instance.SetHalloweenLevitation(this.levitationStrength, this.levitationDuration, this.levitationBlendOutDuration, this.levitationBonusStrength, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield return new WaitForSeconds(this.levitationSpellDuration);
		Player.Instance.SetHalloweenLevitation(0f, this.levitationDuration, this.levitationBlendOutDuration, 0f, this.levitationBonusOffAtYSpeed, this.levitationBonusFullAtYSpeed);
		yield break;
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0004053C File Offset: 0x0003E73C
	private void ChangeState(MagicCauldron.CauldronState state)
	{
		this.currentState = state;
		if (base.photonView.IsMine)
		{
			this.currentStateElapsedTime = 0f;
		}
		bool flag = state == MagicCauldron.CauldronState.summoned;
		foreach (NoncontrollableBroomstick noncontrollableBroomstick in this.witchesComponent)
		{
			if (noncontrollableBroomstick.gameObject.activeSelf != flag)
			{
				noncontrollableBroomstick.gameObject.SetActive(flag);
			}
		}
		if (this.currentState == MagicCauldron.CauldronState.summoned && Vector3.Distance(Player.Instance.transform.position, base.transform.position) < this.levitationRadius)
		{
			base.StartCoroutine(this.LevitationSpellCoroutine());
		}
		switch (this.currentState)
		{
		case MagicCauldron.CauldronState.notReady:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronNotReadyColor);
			return;
		case MagicCauldron.CauldronState.ready:
			this.UpdateCauldronColor(this.CauldronActiveColor);
			return;
		case MagicCauldron.CauldronState.recipeCollecting:
			if (this.ingredientIndex >= 0 && this.ingredientIndex < this.allIngredients.Length)
			{
				this.UpdateCauldronColor(this.allIngredients[this.ingredientIndex].color);
				return;
			}
			break;
		case MagicCauldron.CauldronState.recipeActivated:
			if (this.audioSource && this.recipes[this.currentRecipeIndex].successAudio)
			{
				this.audioSource.PlayOneShot(this.recipes[this.currentRecipeIndex].successAudio);
			}
			if (this.successParticle)
			{
				this.successParticle.Play();
				return;
			}
			break;
		case MagicCauldron.CauldronState.summoned:
			break;
		case MagicCauldron.CauldronState.failed:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			this.audioSource.PlayOneShot(this.recipeFailedAudio);
			return;
		case MagicCauldron.CauldronState.cooldown:
			this.currentIngredients.Clear();
			this.UpdateCauldronColor(this.CauldronFailedColor);
			break;
		default:
			return;
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00040730 File Offset: 0x0003E930
	private void UpdateState()
	{
		if (base.photonView.IsMine)
		{
			this.currentStateElapsedTime += Time.deltaTime;
			switch (this.currentState)
			{
			case MagicCauldron.CauldronState.notReady:
			case MagicCauldron.CauldronState.ready:
				break;
			case MagicCauldron.CauldronState.recipeCollecting:
				if (this.currentStateElapsedTime >= this.maxTimeToAddAllIngredients && !this.CheckIngredients())
				{
					this.ChangeState(MagicCauldron.CauldronState.failed);
					return;
				}
				break;
			case MagicCauldron.CauldronState.recipeActivated:
				if (this.currentStateElapsedTime >= this.waitTimeToSummonWitches)
				{
					this.ChangeState(MagicCauldron.CauldronState.summoned);
					return;
				}
				break;
			case MagicCauldron.CauldronState.summoned:
				if (this.currentStateElapsedTime >= this.summonWitchesDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.cooldown);
					return;
				}
				break;
			case MagicCauldron.CauldronState.failed:
				if (this.currentStateElapsedTime >= this.recipeFailedDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
					return;
				}
				break;
			case MagicCauldron.CauldronState.cooldown:
				if (this.currentStateElapsedTime >= this.cooldownDuration)
				{
					this.ChangeState(MagicCauldron.CauldronState.ready);
				}
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x000407FE File Offset: 0x0003E9FE
	public void OnEventStart()
	{
		this.ChangeState(MagicCauldron.CauldronState.ready);
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00040807 File Offset: 0x0003EA07
	public void OnEventEnd()
	{
		this.ChangeState(MagicCauldron.CauldronState.notReady);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00040810 File Offset: 0x0003EA10
	[PunRPC]
	public void OnIngredientAdd(int _ingredientIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "OnIngredientAdd");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.reusableFXContext.playerSettings = rigContainer.Rig.fxSettings;
		this.reusableIngrediantArgs.key = _ingredientIndex;
		FXSystem.PlayFX<MagicCauldron.IngredientArgs>(FXType.HWIngredients, this.reusableFXContext, this.reusableIngrediantArgs, default(PhotonMessageInfo));
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0004087C File Offset: 0x0003EA7C
	private void OnIngredientAdd(int _ingredientIndex)
	{
		if (this.audioSource)
		{
			this.audioSource.PlayOneShot(this.ingredientAddedAudio);
		}
		if (!RoomSystem.AmITheHost)
		{
			return;
		}
		if (_ingredientIndex < 0 || _ingredientIndex >= this.allIngredients.Length || (this.currentState != MagicCauldron.CauldronState.ready && this.currentState != MagicCauldron.CauldronState.recipeCollecting))
		{
			return;
		}
		MagicIngredientType magicIngredientType = this.allIngredients[_ingredientIndex];
		Debug.Log(string.Format("Received ingredient RPC {0} = {1}", _ingredientIndex, magicIngredientType));
		MagicIngredientType magicIngredientType2 = null;
		if (this.recipes[0].recipeIngredients.Count > this.currentIngredients.Count)
		{
			magicIngredientType2 = this.recipes[0].recipeIngredients[this.currentIngredients.Count];
		}
		if (!(magicIngredientType == magicIngredientType2))
		{
			Debug.Log(string.Format("Failure: Expected ingredient {0}, got {1} from recipe[{2}]", magicIngredientType2, magicIngredientType, this.currentIngredients.Count));
			this.ChangeState(MagicCauldron.CauldronState.failed);
			return;
		}
		this.ingredientIndex = _ingredientIndex;
		this.currentIngredients.Add(magicIngredientType);
		if (this.CheckIngredients())
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeActivated);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.ready)
		{
			this.ChangeState(MagicCauldron.CauldronState.recipeCollecting);
			return;
		}
		this.UpdateCauldronColor(magicIngredientType.color);
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x000409AC File Offset: 0x0003EBAC
	private bool CheckIngredients()
	{
		foreach (MagicCauldron.Recipe recipe in this.recipes)
		{
			if (this.currentIngredients.SequenceEqual(recipe.recipeIngredients))
			{
				this.currentRecipeIndex = this.recipes.IndexOf(recipe);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00040A24 File Offset: 0x0003EC24
	private void UpdateCauldronColor(Color color)
	{
		if (this.bubblesParticle)
		{
			if (this.bubblesParticle.isPlaying)
			{
				if (this.currentState == MagicCauldron.CauldronState.failed || this.currentState == MagicCauldron.CauldronState.notReady)
				{
					this.bubblesParticle.Stop();
				}
			}
			else
			{
				this.bubblesParticle.Play();
			}
		}
		this.currentColor = this.cauldronColor;
		if (this.currentColor == color)
		{
			return;
		}
		if (this.rendr)
		{
			this._liquid.AnimateColorFromTo(this.cauldronColor, color, 1f);
			this.cauldronColor = color;
		}
		if (this.bubblesParticle)
		{
			this.bubblesParticle.main.startColor = color;
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00040AE0 File Offset: 0x0003ECE0
	private void OnTriggerEnter(Collider other)
	{
		ThrowableSetDressing componentInParent = other.GetComponentInParent<ThrowableSetDressing>();
		if (componentInParent == null || componentInParent.IngredientTypeSO == null || componentInParent.InHand())
		{
			return;
		}
		if (componentInParent.IsLocalOwnedWorldShareable)
		{
			if (componentInParent.IngredientTypeSO != null && (this.currentState == MagicCauldron.CauldronState.ready || this.currentState == MagicCauldron.CauldronState.recipeCollecting))
			{
				int num = this.allIngredients.IndexOfRef(componentInParent.IngredientTypeSO);
				Debug.Log(string.Format("Sending ingredient RPC {0} = {1}", componentInParent.IngredientTypeSO, num));
				base.photonView.RPC("OnIngredientAdd", RpcTarget.Others, new object[]
				{
					num
				});
				this.OnIngredientAdd(num);
			}
			componentInParent.StartRespawnTimer(0f);
		}
		if (componentInParent.IngredientTypeSO != null && this.splashParticle)
		{
			this.splashParticle.Play();
		}
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00040BC1 File Offset: 0x0003EDC1
	public override void OnDisable()
	{
		base.OnDisable();
		this.currentIngredients.Clear();
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00040BD4 File Offset: 0x0003EDD4
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.currentStateElapsedTime);
			stream.SendNext(this.currentRecipeIndex);
			stream.SendNext(this.currentState);
			stream.SendNext(this.ingredientIndex);
			return;
		}
		int num = this.ingredientIndex;
		MagicCauldron.CauldronState cauldronState = this.currentState;
		this.currentStateElapsedTime = (float)stream.ReceiveNext();
		this.currentRecipeIndex = (int)stream.ReceiveNext();
		this.currentState = (MagicCauldron.CauldronState)stream.ReceiveNext();
		this.ingredientIndex = (int)stream.ReceiveNext();
		if (cauldronState != this.currentState)
		{
			this.ChangeState(this.currentState);
			return;
		}
		if (this.currentState == MagicCauldron.CauldronState.recipeCollecting && num != this.ingredientIndex && this.ingredientIndex >= 0 && this.ingredientIndex < this.allIngredients.Length)
		{
			this.UpdateCauldronColor(this.allIngredients[this.ingredientIndex].color);
		}
	}

	// Token: 0x04000CF8 RID: 3320
	public List<MagicCauldron.Recipe> recipes = new List<MagicCauldron.Recipe>();

	// Token: 0x04000CF9 RID: 3321
	public float maxTimeToAddAllIngredients = 30f;

	// Token: 0x04000CFA RID: 3322
	public float summonWitchesDuration = 20f;

	// Token: 0x04000CFB RID: 3323
	public float recipeFailedDuration = 5f;

	// Token: 0x04000CFC RID: 3324
	public float cooldownDuration = 30f;

	// Token: 0x04000CFD RID: 3325
	public MagicIngredientType[] allIngredients;

	// Token: 0x04000CFE RID: 3326
	public GameObject flyingWitchesContainer;

	// Token: 0x04000CFF RID: 3327
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000D00 RID: 3328
	public AudioClip ingredientAddedAudio;

	// Token: 0x04000D01 RID: 3329
	public AudioClip recipeFailedAudio;

	// Token: 0x04000D02 RID: 3330
	public ParticleSystem bubblesParticle;

	// Token: 0x04000D03 RID: 3331
	public ParticleSystem successParticle;

	// Token: 0x04000D04 RID: 3332
	public ParticleSystem splashParticle;

	// Token: 0x04000D05 RID: 3333
	public Color CauldronActiveColor;

	// Token: 0x04000D06 RID: 3334
	public Color CauldronFailedColor;

	// Token: 0x04000D07 RID: 3335
	[Tooltip("only if we are using the time of day event")]
	public Color CauldronNotReadyColor;

	// Token: 0x04000D08 RID: 3336
	private readonly List<NoncontrollableBroomstick> witchesComponent = new List<NoncontrollableBroomstick>();

	// Token: 0x04000D09 RID: 3337
	private readonly List<MagicIngredientType> currentIngredients = new List<MagicIngredientType>();

	// Token: 0x04000D0A RID: 3338
	private float currentStateElapsedTime;

	// Token: 0x04000D0B RID: 3339
	private MagicCauldron.CauldronState currentState;

	// Token: 0x04000D0C RID: 3340
	[SerializeField]
	private Renderer rendr;

	// Token: 0x04000D0D RID: 3341
	private Color cauldronColor;

	// Token: 0x04000D0E RID: 3342
	private Color currentColor;

	// Token: 0x04000D0F RID: 3343
	private int currentRecipeIndex;

	// Token: 0x04000D10 RID: 3344
	private int ingredientIndex;

	// Token: 0x04000D11 RID: 3345
	private float waitTimeToSummonWitches = 2f;

	// Token: 0x04000D12 RID: 3346
	[Space]
	[SerializeField]
	private MagicCauldronLiquid _liquid;

	// Token: 0x04000D13 RID: 3347
	private MagicCauldron.IngrediantFXContext reusableFXContext = new MagicCauldron.IngrediantFXContext();

	// Token: 0x04000D14 RID: 3348
	private MagicCauldron.IngredientArgs reusableIngrediantArgs = new MagicCauldron.IngredientArgs();

	// Token: 0x04000D15 RID: 3349
	public bool testLevitationAlwaysOn;

	// Token: 0x04000D16 RID: 3350
	public float levitationRadius;

	// Token: 0x04000D17 RID: 3351
	public float levitationSpellDuration;

	// Token: 0x04000D18 RID: 3352
	public float levitationStrength;

	// Token: 0x04000D19 RID: 3353
	public float levitationDuration;

	// Token: 0x04000D1A RID: 3354
	public float levitationBlendOutDuration;

	// Token: 0x04000D1B RID: 3355
	public float levitationBonusStrength;

	// Token: 0x04000D1C RID: 3356
	public float levitationBonusOffAtYSpeed;

	// Token: 0x04000D1D RID: 3357
	public float levitationBonusFullAtYSpeed;

	// Token: 0x0200043D RID: 1085
	private enum CauldronState
	{
		// Token: 0x04001D98 RID: 7576
		notReady,
		// Token: 0x04001D99 RID: 7577
		ready,
		// Token: 0x04001D9A RID: 7578
		recipeCollecting,
		// Token: 0x04001D9B RID: 7579
		recipeActivated,
		// Token: 0x04001D9C RID: 7580
		summoned,
		// Token: 0x04001D9D RID: 7581
		failed,
		// Token: 0x04001D9E RID: 7582
		cooldown
	}

	// Token: 0x0200043E RID: 1086
	[Serializable]
	public struct Recipe
	{
		// Token: 0x04001D9F RID: 7583
		public List<MagicIngredientType> recipeIngredients;

		// Token: 0x04001DA0 RID: 7584
		public AudioClip successAudio;
	}

	// Token: 0x0200043F RID: 1087
	private class IngredientArgs : FXSArgs
	{
		// Token: 0x04001DA1 RID: 7585
		public int key;
	}

	// Token: 0x02000440 RID: 1088
	private class IngrediantFXContext : IFXContextParems<MagicCauldron.IngredientArgs>
	{
		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06001CC4 RID: 7364 RVA: 0x00098E14 File Offset: 0x00097014
		FXSystemSettings IFXContextParems<MagicCauldron.IngredientArgs>.settings
		{
			get
			{
				return this.playerSettings;
			}
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x00098E1C File Offset: 0x0009701C
		void IFXContextParems<MagicCauldron.IngredientArgs>.OnPlayFX(MagicCauldron.IngredientArgs args)
		{
			this.fxCallBack(args.key);
		}

		// Token: 0x04001DA2 RID: 7586
		public FXSystemSettings playerSettings;

		// Token: 0x04001DA3 RID: 7587
		public MagicCauldron.IngrediantFXContext.Callback fxCallBack;

		// Token: 0x02000550 RID: 1360
		// (Invoke) Token: 0x06001FCC RID: 8140
		public delegate void Callback(int key);
	}
}
