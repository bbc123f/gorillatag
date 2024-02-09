using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	public class TrainButtonVisualController : MonoBehaviour
	{
		private void Awake()
		{
			this._materialColorId = Shader.PropertyToID("_Color");
			this._buttonMaterial = this._meshRenderer.material;
			this._buttonDefaultColor = this._buttonMaterial.GetColor(this._materialColorId);
			this._oldPosition = base.transform.localPosition;
		}

		private void OnDestroy()
		{
			if (this._buttonMaterial != null)
			{
				Object.Destroy(this._buttonMaterial);
			}
		}

		private void OnEnable()
		{
			this._buttonController.InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
			this._buttonController.ContactZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonController.ActionZoneEvent += this.ActionOrInContactZoneStayEvent;
			this._buttonInContactOrActionStates = false;
		}

		private void OnDisable()
		{
			if (this._buttonController != null)
			{
				this._buttonController.InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.InteractableStateChanged));
				this._buttonController.ContactZoneEvent -= this.ActionOrInContactZoneStayEvent;
				this._buttonController.ActionZoneEvent -= this.ActionOrInContactZoneStayEvent;
			}
		}

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

		private void PlaySound(AudioClip clip)
		{
			this._audioSource.timeSamples = 0;
			this._audioSource.clip = clip;
			this._audioSource.Play();
		}

		private void StopResetLerping()
		{
			if (this._lerpToOldPositionCr != null)
			{
				base.StopCoroutine(this._lerpToOldPositionCr);
			}
		}

		private void LerpToOldPosition()
		{
			if ((base.transform.localPosition - this._oldPosition).sqrMagnitude < Mathf.Epsilon)
			{
				return;
			}
			this.StopResetLerping();
			this._lerpToOldPositionCr = base.StartCoroutine(this.ResetPosition());
		}

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

		private const float LERP_TO_OLD_POS_DURATION = 1f;

		private const float LOCAL_SIZE_HALVED = 0.5f;

		[SerializeField]
		private MeshRenderer _meshRenderer;

		[SerializeField]
		private MeshRenderer _glowRenderer;

		[SerializeField]
		private ButtonController _buttonController;

		[SerializeField]
		private Color _buttonContactColor = new Color(0.51f, 0.78f, 0.92f, 1f);

		[SerializeField]
		private Color _buttonActionColor = new Color(0.24f, 0.72f, 0.98f, 1f);

		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private AudioClip _actionSoundEffect;

		[SerializeField]
		private Transform _buttonContactTransform;

		[SerializeField]
		private float _contactMaxDisplacementDistance = 0.0141f;

		private Material _buttonMaterial;

		private Color _buttonDefaultColor;

		private int _materialColorId;

		private bool _buttonInContactOrActionStates;

		private Coroutine _lerpToOldPositionCr;

		private Vector3 _oldPosition;
	}
}
