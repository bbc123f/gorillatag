using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
	public Material[] materialArray;

	public float minTime;

	public float maxTime;

	private float nextChange;

	private MeshRenderer meshRenderer;

	private void Awake()
	{
		nextChange = Random.Range(0f, maxTime);
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		if (Time.time > nextChange)
		{
			nextChange = Time.time + Random.Range(minTime, maxTime);
			if (materialArray.Length != 0)
			{
				meshRenderer.material = materialArray[Random.Range(0, materialArray.Length)];
			}
		}
	}
}
