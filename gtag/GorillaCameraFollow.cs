using UnityEngine;

public class GorillaCameraFollow : MonoBehaviour
{
	public Transform playerHead;

	public GameObject cameraParent;

	public Vector3 headOffset;

	public Vector3 eulerRotationOffset;

	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			cameraParent.SetActive(value: false);
		}
	}

	private void LateUpdate()
	{
		if (!(playerHead == null))
		{
			Vector3 vector = Vector3.ProjectOnPlane(playerHead.forward, Vector3.up);
			float y = Vector3.SignedAngle(Vector3.forward, vector, Vector3.up);
			Vector3 to = Vector3.ProjectOnPlane(playerHead.forward, Quaternion.AngleAxis(90f, Vector3.up) * vector);
			float num = Vector3.SignedAngle(Vector3.up, to, Quaternion.AngleAxis(90f, Vector3.up) * vector);
			base.transform.eulerAngles = new Vector3(num - 90f, y, 0f);
		}
	}
}
