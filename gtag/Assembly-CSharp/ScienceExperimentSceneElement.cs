using System;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

public class ScienceExperimentSceneElement : MonoBehaviour, ITickSystemPost
{
	bool ITickSystemPost.PostTickRunning
	{
		[CompilerGenerated]
		get
		{
			return this.<ITickSystemPost.PostTickRunning>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<ITickSystemPost.PostTickRunning>k__BackingField = value;
		}
	}

	void ITickSystemPost.PostTick()
	{
		base.transform.position = this.followElement.position;
		base.transform.rotation = this.followElement.rotation;
		base.transform.localScale = this.followElement.localScale;
	}

	private void Start()
	{
		this.followElement = ScienceExperimentManager.instance.GetElement(this.elementID);
		TickSystem<object>.AddPostTickCallback(this);
	}

	private void OnDestroy()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	public ScienceExperimentSceneElement()
	{
	}

	public ScienceExperimentElementID elementID;

	private Transform followElement;

	[CompilerGenerated]
	private bool <ITickSystemPost.PostTickRunning>k__BackingField;
}
