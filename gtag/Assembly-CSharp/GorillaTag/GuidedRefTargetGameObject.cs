using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200030C RID: 780
	public class GuidedRefTargetGameObject : MonoBehaviour, IGuidedRefTarget, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060015AB RID: 5547 RVA: 0x00077D3D File Offset: 0x00075F3D
		public GuidedRefTargetIdSO GuidedRefTargetId
		{
			get
			{
				return this.guidedRefTargetInfo.targetId;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060015AC RID: 5548 RVA: 0x00077D4A File Offset: 0x00075F4A
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x00077D52 File Offset: 0x00075F52
		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

		// Token: 0x060015AE RID: 5550 RVA: 0x00077D5A File Offset: 0x00075F5A
		public void GuidedRefInitialize()
		{
			GuidedRefRelayHub.RegisterTargetWithParentRelays(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x00077D76 File Offset: 0x00075F76
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x00077D7E File Offset: 0x00075F7E
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040017BC RID: 6076
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
