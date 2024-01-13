using UnityEngine;

namespace GorillaTag;

public class StaticLodGroup : MonoBehaviour
{
	private int index;

	public float collisionEnableDistance = 3f;

	public float uiFadeDistanceMin = 1f;

	public float uiFadeDistanceMax = 10f;

	protected void Awake()
	{
		index = StaticLodManager.Register(this);
	}

	protected void OnEnable()
	{
		StaticLodManager.SetEnabled(index, enable: true);
	}

	protected void OnDisable()
	{
		StaticLodManager.SetEnabled(index, enable: false);
	}
}
