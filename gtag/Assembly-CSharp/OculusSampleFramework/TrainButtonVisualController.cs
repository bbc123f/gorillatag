using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x020002E9 RID: 745
	public class TrainButtonVisualController : MonoBehaviour
	{
		// Token: 0x06001430 RID: 5168 RVA: 0x00072510 File Offset: 0x00070710
		private void Awake()
		{
			this._materialColorId = Shader.PropertyToID("_Color");
			this._buttonMaterial = this._meshRenderer.material;
			this._buttonDefaultColor = this._buttonMaterial.GetColor(this._materialColorId);
			this._oldPosition = base.transform.localPosition;
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x00072566 File Offset: 0x00070766
		private void OnDestroy()
		{
			if (this._buttonMaterial != null)
			{
				Object.Destroy(this._buttonMaterial);
			}
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00072584 File Offset: 0x00070784
		private void OnEnable()
		{
			this._buttonController.InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
			this._buttonController.ContactZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonController.ActionZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonInContactOrActionStates = false;
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x000725E4 File Offset: 0x000707E4
		private void OnDisable()
		{
			if (this._buttonController != null)
			{
				this._buttonController.InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
				this._buttonController.ContactZoneEvent -= this.ActionOrInContactZoneStayEvent;
				this._buttonController.ActionZoneEvent -= this.ActionOrInContactZoneStayEvent;
			}
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0007264C File Offset: 0x0007084C
		private void ActionOrInContactZoneStayEvent(ColliderZoneArgs collisionArgs)
		{
			if (!this._buttonInContactOrActionStates || collisionArgs.CollidingTool.IsFarFieldTool)
			{
				return;
			}
			Vector3 localScale = this._buttonContactTransform.localScale;
			Vector3 interactionPosition = collisionArgs.CollidingTool.InteractionPosition;
			float num = (this._buttonContactTransform.InverseTransformPoint(interactionPosition) - 0.5f * Vector3.one).y * localScale.y;
			if (num > -this._contactMaxDisplacementDistance && num <= 0f)
			{
				base.transform.localPosition = new Vector3(this._oldPosition.x, this._oldPosition.y + num, this._oldPosition.z);
			}
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x000726FC File Offset: 0x000708FC
		private void InteractableStateChanged(InteractableStateArgs obj)
		{
			this._buttonInContactOrActionStates = false;
			this._glowRenderer.gameObject.SetActive(obj.NewInteractableState > InteractableState.Default);
			switch (obj.NewInteractableState)
			{
			case InteractableState.ProximityState:
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonDefaultColor);
				this.LerpToOldPosition();
				return;
			case InteractableState.ContactState:
				this.StopResetLerping();
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonContactColor);
				this._buttonInContactOrActionStates = true;
				return;
			case InteractableState.ActionState:
				this.StopResetLerping();
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonActionColor);
				this.PlaySound(this._actionSoundEffect);
				this._buttonInContactOrActionStates = true;
				return;
			default:
				this._buttonMaterial.SetColor(this._materialColorId, this._buttonDefaultColor);
				this.LerpToOldPosition();
				return;
			}
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x000727D7 File Offset: 0x000709D7
		private void PlaySound(AudioClip clip)
		{
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x000727FC File Offset: 0x000709FC
		private void StopResetLerping()
		{
			if (this._lerpToOldPositionCr != null)
			{
				base.StopCoroutine(this._lerpToOldPositionCr);
			}
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x00072814 File Offset: 0x00070A14
		private void LerpToOldPosition()
		{
			if ((base.transform.localPosition - this._oldPosition).sqrMagnitude < Mathf.Epsilon)
			{
				return;
			}
			this.StopResetLerping();
			this._lerpToOldPositionCr = base.StartCoroutine(this.ResetPosition());
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0007285F File Offset: 0x00070A5F
		private IEnumerator ResetPosition()
		{
			float startTime = Time.time;
			float endTime = Time.time + 1f;
			while (Time.time < endTime)
			{
				base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, this._oldPosition, (Time.time - startTime) / 1f);
				yield return null;
			}
			base.transform.localPosition = this._oldPosition;
			this._lerpToOldPositionCr = null;
			yield break;
		}

		// Token: 0x040016D2 RID: 5842
		private const float LERP_TO_OLD_POS_DURATION = 1f;

		// Token: 0x040016D3 RID: 5843
		private const float LOCAL_SIZE_HALVED = 0.5f;

		// Token: 0x040016D4 RID: 5844
		[SerializeField]
		private MeshRenderer _meshRenderer;

		// Token: 0x040016D5 RID: 5845
		[SerializeField]
		private MeshRenderer _glowRenderer;

		// Token: 0x040016D6 RID: 5846
		[SerializeField]
		private ButtonController _buttonController;

		// Token: 0x040016D7 RID: 5847
		[SerializeField]
		private Color _buttonContactColor = new Color(0.51f, 0.78f, 0.92f, 1f);

		// Token: 0x040016D8 RID: 5848
		[SerializeField]
		private Color _buttonActionColor = new Color(0.24f, 0.72f, 0.98f, 1f);

		// Token: 0x040016D9 RID: 5849
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040016DA RID: 5850
		[SerializeField]
		private AudioClip _actionSoundEffect;

		// Token: 0x040016DB RID: 5851
		[SerializeField]
		private Transform _buttonContactTransform;

		// Token: 0x040016DC RID: 5852
		[SerializeField]
		private float _contactMaxDisplacementDistance = 0.0141f;

		// Token: 0x040016DD RID: 5853
		private Material _buttonMaterial;

		// Token: 0x040016DE RID: 5854
		private Color _buttonDefaultColor;

		// Token: 0x040016DF RID: 5855
		private int _materialColorId;

		// Token: 0x040016E0 RID: 5856
		private bool _buttonInContactOrActionStates;

		// Token: 0x040016E1 RID: 5857
		private Coroutine _lerpToOldPositionCr;

		// Token: 0x040016E2 RID: 5858
		private Vector3 _oldPosition;
	}
}
