using System;
using GorillaTag.GuidedRefs;
using GorillaTag.Rendering;
using UnityEngine;

public class GorillaTriggerBoxShaderSettings : GorillaTriggerBox, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	protected void Awake()
	{
		((IGuidedRefObject)this).GuidedRefInitialize();
	}

	protected void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GuidedRefHub.UnregisterReceiver<GorillaTriggerBoxShaderSettings>(this);
	}

	public override void OnBoxTriggered()
	{
		if (!this.allGuidedRefsResolved)
		{
			return;
		}
		if (this.settings != null)
		{
			this.settings.BecomeActiveInstance();
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
	}

	void IGuidedRefObject.GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverField<GorillaTriggerBoxShaderSettings>(this, "settings", ref this.settings_gRef);
		GuidedRefHub.ReceiverFullyRegistered<GorillaTriggerBoxShaderSettings>(this);
	}

	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		return GuidedRefHub.TryResolveField<GorillaTriggerBoxShaderSettings, ZoneShaderSettings>(this, ref this.settings, this.settings_gRef, target);
	}

	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
		this.allGuidedRefsResolved = true;
	}

	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
		this.allGuidedRefsResolved = false;
	}

	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	[SerializeField]
	private ZoneShaderSettings settings;

	[SerializeField]
	private GuidedRefReceiverFieldInfo settings_gRef = new GuidedRefReceiverFieldInfo(true);

	private bool allGuidedRefsResolved;
}
