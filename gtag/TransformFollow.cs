using UnityEngine;

public class TransformFollow : MonoBehaviour
{
	public Transform transformToFollow;

	public Vector3 offset;

	public Vector3 prevPos;

	private void Awake()
	{
		prevPos = base.transform.position;
	}

	private void LateUpdate()
	{
		prevPos = base.transform.position;
		base.transform.rotation = transformToFollow.rotation;
		base.transform.position = transformToFollow.position + transformToFollow.rotation * offset;
	}
}
