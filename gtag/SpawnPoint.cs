using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	public enum Level
	{
		City,
		Forest,
		Canyon,
		Cave,
		Mountain,
		SkyJungle,
		Beach
	}

	public Level level;

	public GTZone startZone;

	public float startSize = 1f;
}
