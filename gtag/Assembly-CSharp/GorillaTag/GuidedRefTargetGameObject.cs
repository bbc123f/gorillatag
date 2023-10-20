using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200030E RID: 782
	public class GuidedRefTargetGameObject : MonoBehaviour, IGuidedRefTarget, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060015B4 RID: 5556 RVA: 0x00078225 File Offset: 0x00076425
		public GuidedRefTargetIdSO GuidedRefTargetId
		{
			get
			{
				return this.guidedRefTargetInfo.targetId;
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060015B5 RID: 5557 RVA: 0x00078232 File Offset: 0x00076432
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x0007823A File Offset: 0x0007643A
		protected void Awake()
		{
			this.GuidedRefInitialize();
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x00078242 File Offset: 0x00076442
		public void GuidedRefInitialize()
		{
			GuidedRefRelayHub.RegisterTargetWithParentRelays(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x0007825E File Offset: 0x0007645E
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x00078266 File Offset: 0x00076466
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040017C9 RID: 6089
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
