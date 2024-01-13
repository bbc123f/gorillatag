using UnityEngine;

public class GorillaUIParent : MonoBehaviour
{
	public static volatile GorillaUIParent instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
