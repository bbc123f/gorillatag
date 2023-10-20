using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x020002EB RID: 747
	public class TrainButtonVisualController : MonoBehaviour
	{
		// Token: 0x06001437 RID: 5175 RVA: 0x000729DC File Offset: 0x00070BDC
		private void Awake()
		{
			this._materialColorId = Shader.PropertyToID("_Color");
			this._buttonMaterial = this._meshRenderer.material;
			this._buttonDefaultColor = this._buttonMaterial.GetColor(this._materialColorId);
			this._oldPosition = base.transform.localPosition;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x00072A32 File Offset: 0x00070C32
		private void OnDestroy()
		{
			if (this._buttonMaterial != null)
			{
				Object.Destroy(this._buttonMaterial);
			}
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x00072A50 File Offset: 0x00070C50
		private void OnEnable()
		{
			this._buttonController.InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
			this._buttonController.ContactZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonController.ActionZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonInContactOrActionStates = false;
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x00072AB0 File Offset: 0x00070CB0
		private void OnDisable()
		{
			if (this._buttonController != null)
			{
				this._buttonController.InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
				this._buttonController.ContactZoneEvent -= this.ActionOrInContactZoneStayEvent;
				this._buttonController.ActionZoneEvent -= this.ActionOrInContactZoneStayEvent;
			}
		}

		// Token: 0x0600143B RID: 5179 RVA: 0x00072B18 File Offset: 0x00070D18
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

		// Token: 0x0600143C RID: 5180 RVA: 0x00072BC8 File Offset: 0x00070DC8
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

		// Token: 0x0600143D RID: 5181 RVA: 0x00072CA3 File Offset: 0x00070EA3
		private void PlaySound(AudioClip clip)
		{
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x00072CC8 File Offset: 0x00070EC8
		private void StopResetLerping()
		{
			if (this._lerpToOldPositionCr != null)
			{
				base.StopCoroutine(this._lerpToOldPositionCr);
			}
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x00072CE0 File Offset: 0x00070EE0
		private void LerpToOldPosition()
		{
			if ((base.transform.localPosition - this._oldPosition).sqrMagnitude < Mathf.Epsilon)
			{
				return;
			}
			this.StopResetLerping();
			this._lerpToOldPositionCr = base.StartCoroutine(this.ResetPosition());
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x00072D2B File Offset: 0x00070F2B
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

		// Token: 0x040016DF RID: 5855
		private const float LERP_TO_OLD_POS_DURATION = 1f;

		// Token: 0x040016E0 RID: 5856
		private const float LOCAL_SIZE_HALVED = 0.5f;

		// Token: 0x040016E1 RID: 5857
		[SerializeField]
		private MeshRenderer _meshRenderer;

		// Token: 0x040016E2 RID: 5858
		[SerializeField]
		private MeshRenderer _glowRenderer;

		// Token: 0x040016E3 RID: 5859
		[SerializeField]
		private ButtonController _buttonController;

		// Token: 0x040016E4 RID: 5860
		[SerializeField]
		private Color _buttonContactColor = new Color(0.51f, 0.78f, 0.92f, 1f);

		// Token: 0x040016E5 RID: 5861
		[SerializeField]
		private Color _buttonActionColor = new Color(0.24f, 0.72f, 0.98f, 1f);

		// Token: 0x040016E6 RID: 5862
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040016E7 RID: 5863
		[SerializeField]
		private AudioClip _actionSoundEffect;

		// Token: 0x040016E8 RID: 5864
		[SerializeField]
		private Transform _buttonContactTransform;

		// Token: 0x040016E9 RID: 5865
		[SerializeField]
		private float _contactMaxDisplacementDistance = 0.0141f;

		// Token: 0x040016EA RID: 5866
		private Material _buttonMaterial;

		// Token: 0x040016EB RID: 5867
		private Color _buttonDefaultColor;

		// Token: 0x040016EC RID: 5868
		private int _materialColorId;

		// Token: 0x040016ED RID: 5869
		private bool _buttonInContactOrActionStates;

		// Token: 0x040016EE RID: 5870
		private Coroutine _lerpToOldPositionCr;

		// Token: 0x040016EF RID: 5871
		private Vector3 _oldPosition;
	}
}
