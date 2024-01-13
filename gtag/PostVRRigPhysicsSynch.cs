using UnityEngine;

public class PostVRRigPhysicsSynch : MonoBehaviour
{
	private void LateUpdate()
	{
		Physics.SyncTransforms();
	}
}
