using System;
using UnityEngine;

namespace GorillaTag
{
	public class GuidedRefTargetGameObject : MonoBehaviour, IGuidedRefTarget, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		public GuidedRefTargetIdSO GuidedRefTargetId
		{
			get
			{
				return this.guidedRefTargetInfo.targetId;
			}
		}

		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

		public void GuidedRefInitialize()
		{
			GuidedRefRelayHub.RegisterTargetWithParentRelays(this, this.guidedRefTargetInfo.hubIds, this);
		}

		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
