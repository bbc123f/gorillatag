using UnityEngine;

namespace OVR;

public class TestScript : MonoBehaviour
{
	[InspectorNote("Sound Setup", "Press '1' to play testSound1 and '2' to play testSound2")]
	public SoundFXRef testSound1;

	public SoundFXRef testSound2;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			testSound1.PlaySoundAt(base.transform.position);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			testSound2.PlaySoundAt(new Vector3(5f, 0f, 0f));
		}
	}
}
