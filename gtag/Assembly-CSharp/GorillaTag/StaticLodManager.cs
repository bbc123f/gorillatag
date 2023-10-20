using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag
{
	// Token: 0x02000317 RID: 791
	[DefaultExecutionOrder(2000)]
	public class StaticLodManager : MonoBehaviour
	{
		// Token: 0x060015C8 RID: 5576 RVA: 0x00078305 File Offset: 0x00076505
		private void Awake()
		{
			StaticLodManager.gorillaInteractableLayerMask = LayerMask.NameToLayer("GorillaInteractable");
			Application.quitting += this.HandleApplicationQuitting;
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x0007832C File Offset: 0x0007652C
		private void OnEnable()
		{
			this.mainCamera = Camera.main;
			this.hasMainCamera = (this.mainCamera != null);
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x0007834B File Offset: 0x0007654B
		private void HandleApplicationQuitting()
		{
			StaticLodManager.isApplicationQuitting = true;
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x00078354 File Offset: 0x00076554
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

		// Token: 0x060015CC RID: 5580 RVA: 0x000785D8 File Offset: 0x000767D8
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

		// Token: 0x060015CD RID: 5581 RVA: 0x00078610 File Offset: 0x00076810
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

		// Token: 0x040017CF RID: 6095
		private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(32);

		// Token: 0x040017D0 RID: 6096
		[DebugReadout]
		private static readonly List<StaticLodManager.GroupInfo> groupInfos = new List<StaticLodManager.GroupInfo>(32);

		// Token: 0x040017D1 RID: 6097
		private static LayerMask gorillaInteractableLayerMask;

		// Token: 0x040017D2 RID: 6098
		private static bool isApplicationQuitting;

		// Token: 0x040017D3 RID: 6099
		private Camera mainCamera;

		// Token: 0x040017D4 RID: 6100
		private bool hasMainCamera;

		// Token: 0x02000502 RID: 1282
		private struct GroupInfo
		{
			// Token: 0x040020DC RID: 8412
			public bool componentEnabled;

			// Token: 0x040020DD RID: 8413
			public Vector3 center;

			// Token: 0x040020DE RID: 8414
			public float radiusSq;

			// Token: 0x040020DF RID: 8415
			public bool uiEnabled;

			// Token: 0x040020E0 RID: 8416
			public float uiEnableDistanceSq;

			// Token: 0x040020E1 RID: 8417
			public Text[] uiTexts;

			// Token: 0x040020E2 RID: 8418
			public bool collidersEnabled;

			// Token: 0x040020E3 RID: 8419
			public float collisionEnableDistanceSq;

			// Token: 0x040020E4 RID: 8420
			public Collider[] interactableColliders;
		}
	}
}
