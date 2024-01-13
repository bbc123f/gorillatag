using UnityEngine;

public class MicInputTest : MonoBehaviour
{
	protected void LateUpdate()
	{
		base.transform.localScale = new Vector3(1f, 1f + MicInput.Loudness, 1f);
	}
}
