using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class CharacterCameraConstraint : MonoBehaviour
{
	// Token: 0x06000295 RID: 661 RVA: 0x00011277 File Offset: 0x0000F477
	private CharacterCameraConstraint()
	{
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0001127F File Offset: 0x0000F47F
	private void Awake()
	{
		this._character = base.GetComponent<CapsuleCollider>();
		this._simplePlayerController = base.GetComponent<SimpleCapsuleWithStickMovement>();
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00011299 File Offset: 0x0000F499
	private void OnEnable()
	{
		this._simplePlayerController.CameraUpdated += this.CameraUpdate;
	}

	// Token: 0x06000298 RID: 664 RVA: 0x000112B2 File Offset: 0x0000F4B2
	private void OnDisable()
	{
		this._simplePlayerController.CameraUpdated -= this.CameraUpdate;
	}

	// Token: 0x06000299 RID: 665 RVA: 0x000112CC File Offset: 0x0000F4CC
	private void CameraUpdate()
	{
		float value = 0f;
		if (this.CheckCameraOverlapped())
		{
			OVRScreenFade.instance.SetExplicitFade(1f);
		}
		else if (this.CheckCameraNearClipping(out value))
		{
			float t = Mathf.InverseLerp(0f, 0.1f, value);
			float explicitFade = Mathf.Lerp(0f, 1f, t);
			OVRScreenFade.instance.SetExplicitFade(explicitFade);
		}
		else
		{
			OVRScreenFade.instance.SetExplicitFade(0f);
		}
		float num = 0.25f;
		float value2 = this.CameraRig.centerEyeAnchor.localPosition.y + this.HeightOffset + num;
		float num2 = this.MinimumHeight;
		num2 = Mathf.Min(this._character.height, num2);
		float num3 = this.MaximumHeight;
		RaycastHit raycastHit;
		if (Physics.SphereCast(this._character.transform.position, this._character.radius * 0.2f, Vector3.up, out raycastHit, this.MaximumHeight - this._character.transform.position.y, this.CollideLayers, QueryTriggerInteraction.Ignore))
		{
			num3 = raycastHit.point.y;
		}
		num3 = Mathf.Max(this._character.height, num3);
		this._character.height = Mathf.Clamp(value2, num2, num3);
		float y = this.HeightOffset - this._character.height * 0.5f - num;
		this.CameraRig.transform.localPosition = new Vector3(0f, y, 0f);
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00011454 File Offset: 0x0000F654
	private bool CheckCameraOverlapped()
	{
		Camera component = this.CameraRig.centerEyeAnchor.GetComponent<Camera>();
		Vector3 position = this._character.transform.position;
		float num = Mathf.Max(0f, this._character.height * 0.5f - component.nearClipPlane - 0.01f);
		position.y = Mathf.Clamp(this.CameraRig.centerEyeAnchor.position.y, this._character.transform.position.y - num, this._character.transform.position.y + num);
		Vector3 a = this.CameraRig.centerEyeAnchor.position - position;
		float magnitude = a.magnitude;
		Vector3 direction = a / magnitude;
		RaycastHit raycastHit;
		return Physics.SphereCast(position, component.nearClipPlane, direction, out raycastHit, magnitude, this.CollideLayers, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00011544 File Offset: 0x0000F744
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
		bool result2 = false;
		result = 0f;
		foreach (Vector3 vector6 in new Vector3[]
		{
			vector,
			vector2,
			vector3,
			vector4,
			vector5
		})
		{
			RaycastHit raycastHit;
			if (Physics.Linecast(this.CameraRig.centerEyeAnchor.position, vector6, out raycastHit, this.CollideLayers, QueryTriggerInteraction.Ignore))
			{
				result2 = true;
				result = Mathf.Max(result, Vector3.Distance(raycastHit.point, vector6));
			}
		}
		return result2;
	}

	// Token: 0x04000368 RID: 872
	private const float FADE_RAY_LENGTH = 0.25f;

	// Token: 0x04000369 RID: 873
	private const float FADE_OVERLAP_MAXIMUM = 0.1f;

	// Token: 0x0400036A RID: 874
	private const float FADE_AMOUNT_MAXIMUM = 1f;

	// Token: 0x0400036B RID: 875
	[Tooltip("This should be a reference to the OVRCameraRig that is usually a child of the PlayerController.")]
	public OVRCameraRig CameraRig;

	// Token: 0x0400036C RID: 876
	[Tooltip("Collision layers to be used for the purposes of fading out the screen when the HMD is inside world geometry and adjusting the capsule height.")]
	public LayerMask CollideLayers;

	// Token: 0x0400036D RID: 877
	[Tooltip("Offset is added to camera's real world height, effectively treating it as though the player was taller/standing higher.")]
	public float HeightOffset;

	// Token: 0x0400036E RID: 878
	[Tooltip("Minimum height that the character capsule can shrink to.  To disable, set to capsule's height.")]
	public float MinimumHeight;

	// Token: 0x0400036F RID: 879
	[Tooltip("Maximum height that the character capsule can grow to.  To disable, set to capsule's height.")]
	public float MaximumHeight;

	// Token: 0x04000370 RID: 880
	private CapsuleCollider _character;

	// Token: 0x04000371 RID: 881
	private SimpleCapsuleWithStickMovement _simplePlayerController;
}
