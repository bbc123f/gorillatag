using UnityEngine;

public class SizeLayerChanger : MonoBehaviour
{
	public float maxScale;

	public float minScale;

	public bool isAssurance;

	public bool affectLayerA = true;

	public bool affectLayerB = true;

	public bool affectLayerC = true;

	public bool affectLayerD = true;

	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (affectLayerA)
			{
				num |= 1;
			}
			if (affectLayerB)
			{
				num |= 2;
			}
			if (affectLayerC)
			{
				num |= 4;
			}
			if (affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	private void Awake()
	{
		minScale = Mathf.Max(minScale, 0.01f);
	}

	public void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<SphereCollider>())
		{
			VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (!(component == null))
			{
				component.sizeManager.currentSizeLayerMaskValue = SizeLayerMask;
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if ((bool)other.GetComponent<SphereCollider>())
		{
			_ = other.attachedRigidbody.gameObject.GetComponent<VRRig>() == null;
		}
	}
}
