using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class UmbrellaItem : TransferrableObject
{
	// Token: 0x06000793 RID: 1939 RVA: 0x00030745 File Offset: 0x0002E945
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x00030754 File Offset: 0x0002E954
	public override void OnActivate()
	{
		base.OnActivate();
		float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float soundVolume = 0.08f;
		int soundIndex;
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			soundIndex = this.SoundIdOpen;
			this.itemState = TransferrableObject.ItemStates.State0;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Add(this.umbrellaRainDestroyTrigger);
		}
		else
		{
			soundIndex = this.SoundIdClose;
			this.itemState = TransferrableObject.ItemStates.State1;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		}
		base.ActivateItemFX(hapticStrength, fixedDeltaTime, soundIndex, soundVolume);
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x000307EC File Offset: 0x0002E9EC
	public override void OnEnable()
	{
		base.OnEnable();
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x000307FA File Offset: 0x0002E9FA
	public override void OnDisable()
	{
		base.OnDisable();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0003081A File Offset: 0x0002EA1A
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		this.itemState = TransferrableObject.ItemStates.State1;
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00030847 File Offset: 0x0002EA47
	public override void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (base.InHand())
		{
			return;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.OnActivate();
		}
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0003086C File Offset: 0x0002EA6C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		UmbrellaItem.UmbrellaStates itemState = (UmbrellaItem.UmbrellaStates)this.itemState;
		if (itemState != this.previousUmbrellaState)
		{
			this.OnUmbrellaStateChanged();
		}
		this.UpdateAngles((itemState == UmbrellaItem.UmbrellaStates.UmbrellaOpen) ? this.startingAngles : this.endingAngles, this.lerpValue);
		this.previousUmbrellaState = itemState;
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x000308BC File Offset: 0x0002EABC
	protected virtual void OnUmbrellaStateChanged()
	{
		bool flag = this.itemState == TransferrableObject.ItemStates.State0;
		GameObject[] array = this.gameObjectsActivatedOnOpen;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		ParticleSystem[] array2;
		if (flag)
		{
			array2 = this.particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
			return;
		}
		array2 = this.particlesEmitOnOpen;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop();
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x00030930 File Offset: 0x0002EB30
	protected virtual void UpdateAngles(Quaternion[] toAngles, float t)
	{
		for (int i = 0; i < this.umbrellaBones.Length; i++)
		{
			this.umbrellaBones[i].localRotation = Quaternion.Lerp(this.umbrellaBones[i].localRotation, toAngles[i], t);
		}
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00030978 File Offset: 0x0002EB78
	protected void GenerateAngles()
	{
		this.startingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int i = 0; i < this.endingAngles.Length; i++)
		{
			this.startingAngles[i] = this.umbrellaToCopy.startingAngles[i];
		}
		this.endingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int j = 0; j < this.endingAngles.Length; j++)
		{
			this.endingAngles[j] = this.umbrellaToCopy.endingAngles[j];
		}
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00030A0B File Offset: 0x0002EC0B
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00030A0E File Offset: 0x0002EC0E
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04000933 RID: 2355
	[AssignInCorePrefab]
	public Transform[] umbrellaBones;

	// Token: 0x04000934 RID: 2356
	[AssignInCorePrefab]
	public Quaternion[] startingAngles;

	// Token: 0x04000935 RID: 2357
	[AssignInCorePrefab]
	public Quaternion[] endingAngles;

	// Token: 0x04000936 RID: 2358
	[AssignInCorePrefab]
	[Tooltip("Assign to use the 'Generate Angles' button")]
	public UmbrellaItem umbrellaToCopy;

	// Token: 0x04000937 RID: 2359
	[AssignInCorePrefab]
	public float lerpValue = 0.25f;

	// Token: 0x04000938 RID: 2360
	[AssignInCorePrefab]
	public Collider umbrellaRainDestroyTrigger;

	// Token: 0x04000939 RID: 2361
	[AssignInCorePrefab]
	public GameObject[] gameObjectsActivatedOnOpen;

	// Token: 0x0400093A RID: 2362
	[AssignInCorePrefab]
	public ParticleSystem[] particlesEmitOnOpen;

	// Token: 0x0400093B RID: 2363
	[GorillaSoundLookup]
	public int SoundIdOpen = 64;

	// Token: 0x0400093C RID: 2364
	[GorillaSoundLookup]
	public int SoundIdClose = 65;

	// Token: 0x0400093D RID: 2365
	private UmbrellaItem.UmbrellaStates previousUmbrellaState = UmbrellaItem.UmbrellaStates.UmbrellaOpen;

	// Token: 0x0200040B RID: 1035
	private enum UmbrellaStates
	{
		// Token: 0x04001CD7 RID: 7383
		UmbrellaOpen = 1,
		// Token: 0x04001CD8 RID: 7384
		UmbrellaClosed
	}
}
