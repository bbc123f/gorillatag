using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag
{
	// Token: 0x02000315 RID: 789
	[DefaultExecutionOrder(2000)]
	public class StaticLodManager : MonoBehaviour
	{
		// Token: 0x060015BF RID: 5567 RVA: 0x00077E1D File Offset: 0x0007601D
		private void Awake()
		{
			StaticLodManager.gorillaInteractableLayerMask = LayerMask.NameToLayer("GorillaInteractable");
			Application.quitting += this.HandleApplicationQuitting;
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x00077E44 File Offset: 0x00076044
		private void OnEnable()
		{
			this.mainCamera = Camera.main;
			this.hasMainCamera = (this.mainCamera != null);
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x00077E63 File Offset: 0x00076063
		private void HandleApplicationQuitting()
		{
			StaticLodManager.isApplicationQuitting = true;
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x00077E6C File Offset: 0x0007606C
		public static int Register(StaticLodGroup lodGroup)
		{
			StaticLodManager.groupMonoBehaviours.Add(lodGroup);
			StaticLodGroupExcluder componentInParent = lodGroup.GetComponentInParent<StaticLodGroupExcluder>();
			Text[] array = lodGroup.GetComponentsInChildren<Text>(true);
			List<Text> list = new List<Text>(array.Length);
			foreach (Text text in array)
			{
				StaticLodGroupExcluder componentInParent2 = text.GetComponentInParent<StaticLodGroupExcluder>();
				if (!(componentInParent2 != null) || !(componentInParent2 != componentInParent))
				{
					list.Add(text);
				}
			}
			array = list.ToArray();
			Collider[] componentsInChildren = lodGroup.GetComponentsInChildren<Collider>(true);
			List<Collider> list2 = new List<Collider>(componentsInChildren.Length);
			foreach (Collider collider in componentsInChildren)
			{
				if (collider.gameObject.layer == StaticLodManager.gorillaInteractableLayerMask)
				{
					StaticLodGroupExcluder componentInParent3 = collider.GetComponentInParent<StaticLodGroupExcluder>();
					if (!(componentInParent3 != null) || !(componentInParent3 != componentInParent))
					{
						list2.Add(collider);
					}
				}
			}
			Bounds bounds;
			if (array.Length != 0)
			{
				bounds = new Bounds(array[0].transform.position, Vector3.one * 0.01f);
			}
			else if (list2.Count > 0)
			{
				bounds = new Bounds(list2[0].bounds.center, list2[0].bounds.size);
			}
			else
			{
				bounds = new Bounds(lodGroup.transform.position, Vector3.one * 0.01f);
			}
			foreach (Text text2 in array)
			{
				bounds.Encapsulate(text2.transform.position);
			}
			foreach (Collider collider2 in list2)
			{
				bounds.Encapsulate(collider2.bounds);
			}
			StaticLodManager.groupInfos.Add(new StaticLodManager.GroupInfo
			{
				componentEnabled = lodGroup.isActiveAndEnabled,
				center = bounds.center,
				radiusSq = bounds.extents.sqrMagnitude,
				uiEnabled = true,
				uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
				uiTexts = array,
				collidersEnabled = true,
				collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance,
				interactableColliders = list2.ToArray()
			});
			return StaticLodManager.groupMonoBehaviours.Count - 1;
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x000780F0 File Offset: 0x000762F0
		public static void SetEnabled(int index, bool enable)
		{
			if (!StaticLodManager.isApplicationQuitting)
			{
				return;
			}
			StaticLodManager.GroupInfo value = StaticLodManager.groupInfos[index];
			value.componentEnabled = enable;
			StaticLodManager.groupInfos[index] = value;
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x00078128 File Offset: 0x00076328
		protected void LateUpdate()
		{
			if (!this.hasMainCamera)
			{
				return;
			}
			Vector3 position = this.mainCamera.transform.position;
			for (int i = 0; i < StaticLodManager.groupInfos.Count; i++)
			{
				StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[i];
				if (groupInfo.componentEnabled)
				{
					float num = Mathf.Max(0f, (groupInfo.center - position).sqrMagnitude - groupInfo.radiusSq);
					float num2 = groupInfo.uiEnabled ? 0.010000001f : 0f;
					bool flag = num < groupInfo.uiEnableDistanceSq + num2;
					if (flag != groupInfo.uiEnabled)
					{
						for (int j = 0; j < groupInfo.uiTexts.Length; j++)
						{
							Text text = groupInfo.uiTexts[j];
							if (!(text == null))
							{
								text.enabled = flag;
							}
						}
					}
					groupInfo.uiEnabled = flag;
					num2 = (groupInfo.collidersEnabled ? 0.010000001f : 0f);
					bool flag2 = num < groupInfo.collisionEnableDistanceSq + num2;
					if (flag2 != groupInfo.collidersEnabled)
					{
						for (int k = 0; k < groupInfo.interactableColliders.Length; k++)
						{
							if (!(groupInfo.interactableColliders[k] == null))
							{
								groupInfo.interactableColliders[k].enabled = flag2;
							}
						}
					}
					groupInfo.collidersEnabled = flag2;
					StaticLodManager.groupInfos[i] = groupInfo;
				}
			}
		}

		// Token: 0x040017C2 RID: 6082
		private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(32);

		// Token: 0x040017C3 RID: 6083
		[DebugReadout]
		private static readonly List<StaticLodManager.GroupInfo> groupInfos = new List<StaticLodManager.GroupInfo>(32);

		// Token: 0x040017C4 RID: 6084
		private static LayerMask gorillaInteractableLayerMask;

		// Token: 0x040017C5 RID: 6085
		private static bool isApplicationQuitting;

		// Token: 0x040017C6 RID: 6086
		private Camera mainCamera;

		// Token: 0x040017C7 RID: 6087
		private bool hasMainCamera;

		// Token: 0x02000500 RID: 1280
		private struct GroupInfo
		{
			// Token: 0x040020CF RID: 8399
			public bool componentEnabled;

			// Token: 0x040020D0 RID: 8400
			public Vector3 center;

			// Token: 0x040020D1 RID: 8401
			public float radiusSq;

			// Token: 0x040020D2 RID: 8402
			public bool uiEnabled;

			// Token: 0x040020D3 RID: 8403
			public float uiEnableDistanceSq;

			// Token: 0x040020D4 RID: 8404
			public Text[] uiTexts;

			// Token: 0x040020D5 RID: 8405
			public bool collidersEnabled;

			// Token: 0x040020D6 RID: 8406
			public float collisionEnableDistanceSq;

			// Token: 0x040020D7 RID: 8407
			public Collider[] interactableColliders;
		}
	}
}
