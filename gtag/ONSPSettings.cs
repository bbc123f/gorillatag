using UnityEngine;

public sealed class ONSPSettings : ScriptableObject
{
	[SerializeField]
	public int voiceLimit = 64;

	private static ONSPSettings instance;

	public static ONSPSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<ONSPSettings>("ONSPSettings");
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<ONSPSettings>();
				}
			}
			return instance;
		}
	}
}
