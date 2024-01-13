using UnityEngine;

public class CharacterCameraConstraint : MonoBehaviour
{
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

	private CharacterCameraConstraint()
	{
	}

	private void Awake()
	{
		_character = GetComponent<CapsuleCollider>();
		_simplePlayerController = GetComponent<SimpleCapsuleWithStickMovement>();
	}

	private void OnEnable()
	{
		_simplePlayerController.CameraUpdated += CameraUpdate;
	}

	private void OnDisable()
	{
		_simplePlayerController.CameraUpdated -= CameraUpdate;
	}

	private void CameraUpdate()
	{
		float result = 0f;
		if (CheckCameraOverlapped())
		{
			OVRScreenFade.instance.SetExplicitFade(1f);
		}
		else if (CheckCameraNearClipping(out result))
		{
			float t = Mathf.InverseLerp(0f, 0.1f, result);
			float explicitFade = Mathf.Lerp(0f, 1f, t);
			OVRScreenFade.instance.SetExplicitFade(explicitFade);
		}
		else
		{
			OVRScreenFade.instance.SetExplicitFade(0f);
		}
		float num = 0.25f;
		float value = CameraRig.centerEyeAnchor.localPosition.y + HeightOffset + num;
		float minimumHeight = MinimumHeight;
		minimumHeight = Mathf.Min(_character.height, minimumHeight);
		float b = MaximumHeight;
		if (Physics.SphereCast(_character.transform.position, _character.radius * 0.2f, Vector3.up, out var hitInfo, MaximumHeight - _character.transform.position.y, CollideLayers, QueryTriggerInteraction.Ignore))
		{
			b = hitInfo.point.y;
		}
		b = Mathf.Max(_character.height, b);
		_character.height = Mathf.Clamp(value, minimumHeight, b);
		float y = HeightOffset - _character.height * 0.5f - num;
		CameraRig.transform.localPosition = new Vector3(0f, y, 0f);
	}

	private bool CheckCameraOverlapped()
	{
		Camera component = CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3 position = _character.transform.position;
		float num = Mathf.Max(0f, _character.height * 0.5f - component.nearClipPlane - 0.01f);
		position.y = Mathf.Clamp(CameraRig.centerEyeAnchor.position.y, _character.transform.position.y - num, _character.transform.position.y + num);
		Vector3 vector = CameraRig.centerEyeAnchor.position - position;
		float magnitude = vector.magnitude;
		Vector3 direction = vector / magnitude;
		RaycastHit hitInfo;
		return Physics.SphereCast(position, component.nearClipPlane, direction, out hitInfo, magnitude, CollideLayers, QueryTriggerInteraction.Ignore);
	}

	private bool CheckCameraNearClipping(out float result)
	{
		Camera component = CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3[] array = new Vector3[4];
		component.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), component.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, array);
		Vector3 vector = CameraRig.centerEyeAnchor.position + Vector3.Normalize(CameraRig.centerEyeAnchor.TransformVector(array[0])) * 0.25f;
		Vector3 vector2 = CameraRig.centerEyeAnchor.position + Vector3.Normalize(CameraRig.centerEyeAnchor.TransformVector(array[1])) * 0.25f;
		Vector3 vector3 = CameraRig.centerEyeAnchor.position + Vector3.Normalize(CameraRig.centerEyeAnchor.TransformVector(array[2])) * 0.25f;
		Vector3 vector4 = CameraRig.centerEyeAnchor.position + Vector3.Normalize(CameraRig.centerEyeAnchor.TransformVector(array[3])) * 0.25f;
		Vector3 vector5 = (vector2 + vector4) / 2f;
		bool result2 = false;
		result = 0f;
		Vector3[] array2 = new Vector3[5] { vector, vector2, vector3, vector4, vector5 };
		foreach (Vector3 vector6 in array2)
		{
			if (Physics.Linecast(CameraRig.centerEyeAnchor.position, vector6, out var hitInfo, CollideLayers, QueryTriggerInteraction.Ignore))
			{
				result2 = true;
				result = Mathf.Max(result, Vector3.Distance(hitInfo.point, vector6));
			}
		}
		return result2;
	}
}
