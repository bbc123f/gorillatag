using GorillaTag;
using UnityEngine;

public class UmbrellaItem : TransferrableObject
{
	private enum UmbrellaStates
	{
		UmbrellaOpen = 1,
		UmbrellaClosed
	}

	[AssignInCorePrefab]
	public Transform[] umbrellaBones;

	[AssignInCorePrefab]
	public Quaternion[] startingAngles;

	[AssignInCorePrefab]
	public Quaternion[] endingAngles;

	[AssignInCorePrefab]
	[Tooltip("Assign to use the 'Generate Angles' button")]
	public UmbrellaItem umbrellaToCopy;

	[AssignInCorePrefab]
	public float lerpValue = 0.25f;

	[AssignInCorePrefab]
	public Collider umbrellaRainDestroyTrigger;

	[AssignInCorePrefab]
	public GameObject[] gameObjectsActivatedOnOpen;

	[AssignInCorePrefab]
	public ParticleSystem[] particlesEmitOnOpen;

	private UmbrellaStates previousUmbrellaState = UmbrellaStates.UmbrellaOpen;

	protected override void Start()
	{
		base.Start();
		itemState = ItemStates.State1;
	}

	public override void OnActivate()
	{
		base.OnActivate();
		float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float soundVolume = 0.08f;
		int num = -1;
		if (itemState == ItemStates.State1)
		{
			num = 64;
			itemState = ItemStates.State0;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Add(umbrellaRainDestroyTrigger);
		}
		else
		{
			num = 65;
			itemState = ItemStates.State1;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(umbrellaRainDestroyTrigger);
		}
		ActivateItemFX(hapticStrength, fixedDeltaTime, num, soundVolume);
		OnUmbrellaStateChanged();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		OnUmbrellaStateChanged();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(umbrellaRainDestroyTrigger);
	}

	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(umbrellaRainDestroyTrigger);
		itemState = ItemStates.State1;
		OnUmbrellaStateChanged();
	}

	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (!InHand() && itemState == ItemStates.State0)
		{
			OnActivate();
		}
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		UmbrellaStates umbrellaStates = (UmbrellaStates)itemState;
		if (umbrellaStates != previousUmbrellaState)
		{
			OnUmbrellaStateChanged();
		}
		UpdateAngles((umbrellaStates == UmbrellaStates.UmbrellaOpen) ? startingAngles : endingAngles, lerpValue);
		previousUmbrellaState = umbrellaStates;
	}

	protected virtual void OnUmbrellaStateChanged()
	{
		bool flag = itemState == ItemStates.State0;
		GameObject[] array = gameObjectsActivatedOnOpen;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		if (flag)
		{
			ParticleSystem[] array2 = particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
		}
		else
		{
			ParticleSystem[] array2 = particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Stop();
			}
		}
	}

	protected virtual void UpdateAngles(Quaternion[] toAngles, float t)
	{
		for (int i = 0; i < umbrellaBones.Length; i++)
		{
			umbrellaBones[i].localRotation = Quaternion.Lerp(umbrellaBones[i].localRotation, toAngles[i], t);
		}
	}

	protected void GenerateAngles()
	{
		startingAngles = new Quaternion[umbrellaBones.Length];
		for (int i = 0; i < endingAngles.Length; i++)
		{
			startingAngles[i] = umbrellaToCopy.startingAngles[i];
		}
		endingAngles = new Quaternion[umbrellaBones.Length];
		for (int j = 0; j < endingAngles.Length; j++)
		{
			endingAngles[j] = umbrellaToCopy.endingAngles[j];
		}
	}

	public override bool CanActivate()
	{
		return true;
	}

	public override bool CanDeactivate()
	{
		return true;
	}
}
