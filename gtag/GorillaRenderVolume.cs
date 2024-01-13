using UnityEngine;

public class GorillaRenderVolume : MonoBehaviour
{
	[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
	public Renderer[] renderers;

	public Collider[] colliders;

	private Collider[] overlapResults;

	private int layerMask;

	private bool isInside;

	private const float cameraRadius = 0.1f;

	protected void Awake()
	{
		Renderer[] array = renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		overlapResults = new Collider[colliders.Length];
		layerMask = 1 << LayerMask.NameToLayer("Camera Scene Trigger");
	}

	protected void LateUpdate()
	{
		Transform transform = Camera.main.transform;
		Vector3 position = transform.position;
		float radius = 0.1f * transform.lossyScale.x;
		int num = Physics.OverlapSphereNonAlloc(position, radius, overlapResults, layerMask, QueryTriggerInteraction.Collide);
		if (isInside != num > 0)
		{
			isInside = !isInside;
			Renderer[] array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = isInside;
			}
		}
	}
}
