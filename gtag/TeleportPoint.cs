using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
	public float dimmingSpeed = 1f;

	public float fullIntensity = 1f;

	public float lowIntensity = 0.5f;

	public Transform destTransform;

	private float lastLookAtTime;

	private void Start()
	{
	}

	public Transform GetDestTransform()
	{
		return destTransform;
	}

	private void Update()
	{
		float value = Mathf.SmoothStep(fullIntensity, lowIntensity, (Time.time - lastLookAtTime) * dimmingSpeed);
		GetComponent<MeshRenderer>().material.SetFloat("_Intensity", value);
	}

	public void OnLookAt()
	{
		lastLookAtTime = Time.time;
	}
}
