using UnityEngine;

public class ONSPPropagationSettings : MonoBehaviour
{
	public float quality = 100f;

	private void Update()
	{
		ONSPPropagation.Interface.SetPropagationQuality(quality / 100f);
	}
}
