using System;
using UnityEngine;

public class CharacterCameraConstraint : MonoBehaviour
{
	private CharacterCameraConstraint()
	{
	}

	private void Awake()
	{
		this._character = base.GetComponent<CapsuleCollider>();
		this._simplePlayerController = base.GetComponent<SimpleCapsuleWithStickMovement>();
	}

	private void OnEnable()
	{
		this._simplePlayerController.CameraUpdated += this.CameraUpdate;
	}

	private void OnDisable()
	{
		this._simplePlayerController.CameraUpdated -= this.CameraUpdate;
	}

	private void CameraUpdate()
	{
		float num = 0f;
		if (this.CheckCameraOverlapped())
		{
			OVRScreenFade.instance.SetExplicitFade(1f);
		}
		else if (this.CheckCameraNearClipping(out num))
		{
			float num2 = Mathf.InverseLerp(0f, 0.1f, num);
			float num3 = Mathf.Lerp(0f, 1f, num2);
			OVRScreenFade.instance.SetExplicitFade(num3);
		}
		else
		{
			OVRScreenFade.instance.SetExplicitFade(0f);
		}
		float num4 = 0.25f;
		float num5 = this.CameraRig.centerEyeAnchor.localPosition.y + this.HeightOffset + num4;
		float num6 = this.MinimumHeight;
		num6 = Mathf.Min(this._character.height, num6);
		float num7 = this.MaximumHeight;
		RaycastHit raycastHit;
		if (Physics.SphereCast(this._character.transform.position, this._character.radius * 0.2f, Vector3.up, out raycastHit, this.MaximumHeight - this._character.transform.position.y, this.CollideLayers, QueryTriggerInteraction.Ignore))
		{
			num7 = raycastHit.point.y;
		}
		num7 = Mathf.Max(this._character.height, num7);
		this._character.height = Mathf.Clamp(num5, num6, num7);
		float num8 = this.HeightOffset - this._character.height * 0.5f - num4;
		this.CameraRig.transform.localPosition = new Vector3(0f, num8, 0f);
	}

	private bool CheckCameraOverlapped()
	{
		Camera component = this.CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3 position = this._character.transform.position;
		float num = Mathf.Max(0f, this._character.height * 0.5f - component.nearClipPlane - 0.01f);
		position.y = Mathf.Clamp(this.CameraRig.centerEyeAnchor.position.y, this._character.transform.position.y - num, this._character.transform.position.y + num);
		Vector3 vector = this.CameraRig.centerEyeAnchor.position - position;
		float magnitude = vector.magnitude;
		Vector3 vector2 = vector / magnitude;
		RaycastHit raycastHit;
		return Physics.SphereCast(position, component.nearClipPlane, vector2, out raycastHit, magnitude, this.CollideLayers, QueryTriggerInteraction.Ignore);
	}

	private bool CheckCameraNearClipping(out float result)
	{
		Camera component = this.CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3[] array = new Vector3[4];
		component.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), component.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, array);
		Vector3 vector = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[0])) * 0.25f;
		Vector3 vector2 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[1])) * 0.25f;
		Vector3 vector3 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[2])) * 0.25f;
		Vector3 vector4 = this.CameraRig.centerEyeAnchor.position + Vector3.Normalize(this.CameraRig.centerEyeAnchor.TransformVector(array[3])) * 0.25f;
		Vector3 vector5 = (vector2 + vector4) / 2f;
		bool flag = false;
		result = 0f;
		foreach (Vector3 vector6 in new Vector3[] { vector, vector2, vector3, vector4, vector5 })
		{
			RaycastHit raycastHit;
			if (Physics.Linecast(this.CameraRig.centerEyeAnchor.position, vector6, out raycastHit, this.CollideLayers, QueryTriggerInteraction.Ignore))
			{
				flag = true;
				result = Mathf.Max(result, Vector3.Distance(raycastHit.point, vector6));
			}
		}
		return flag;
	}

	private const float FADE_RAY_LENGTH = 0.25f;

	private const float FADE_OVERLAP_MAXIMUM = 0.1f;

	private const float FADE_AMOUNT_MAXIMUM = 1f;

	[Tooltip("This should be a reference to the OVRCameraRig that is usually a child of the PlayerController.")]
	public OVRCameraRig CameraRig;

	[Tooltip("Collision layers to be used for the purposes of fading out the screen when the HMD is inside world geometry and adjusting the capsule height.")]
	public LayerMask CollideLayers;

	[Tooltip("Offset is added to camera's real world height, effectively treating it as though the player was taller/standing higher.")]
	public float HeightOffset;

	[Tooltip("Minimum height that the character capsule can shrink to.  To disable, set to capsule's height.")]
	public float MinimumHeight;

	[Tooltip("Maximum height that the character capsule can grow to.  To disable, set to capsule's height.")]
	public float MaximumHeight;

	private CapsuleCollider _character;

	private SimpleCapsuleWithStickMovement _simplePlayerController;
}
