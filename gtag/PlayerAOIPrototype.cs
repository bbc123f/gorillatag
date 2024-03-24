using System;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
[NetworkBehaviourWeaved(0)]
public class PlayerAOIPrototype : NetworkBehaviour
{
	public override void FixedUpdateNetwork()
	{
		if (this.Runner.Topology == SimulationConfig.Topologies.ClientServer)
		{
			if (!this.Object.InputAuthority.IsNone && this.Runner.IsServer)
			{
				this.Runner.AddPlayerAreaOfInterest(this.Object.InputAuthority, base.transform.position, this.Radius, -1);
				return;
			}
		}
		else if (!this.Object.StateAuthority.IsNone && this.Object.StateAuthority == this.Runner.LocalPlayer)
		{
			this.Runner.AddPlayerAreaOfInterest(this.Object.StateAuthority, base.transform.position, this.Radius, -1);
		}
	}

	private void OnDrawGizmos()
	{
		if (this.DrawAreaOfInterestRadius)
		{
			Color color = Gizmos.color;
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(base.transform.position, this.Radius);
			Gizmos.color = color;
		}
	}

	public PlayerAOIPrototype()
	{
	}

	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	public override void CopyStateToBackingFields()
	{
	}

	[InlineHelp]
	[SerializeField]
	[MultiPropertyDrawersFix]
	protected bool DrawAreaOfInterestRadius;

	[InlineHelp]
	public float Radius = 32f;

	static Changed<PlayerAOIPrototype> $IL2CPP_CHANGED;

	static ChangedDelegate<PlayerAOIPrototype> $IL2CPP_CHANGED_DELEGATE;

	static NetworkBehaviourCallbacks<PlayerAOIPrototype> $IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;
}
