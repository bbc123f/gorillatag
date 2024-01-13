using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag;

[DefaultExecutionOrder(2000)]
public class StaticLodManager : MonoBehaviour
{
	private struct GroupInfo
	{
		public bool componentEnabled;

		public Vector3 center;

		public float radiusSq;

		public bool uiEnabled;

		public float uiEnableDistanceSq;

		public Text[] uiTexts;

		public bool collidersEnabled;

		public float collisionEnableDistanceSq;

		public Collider[] interactableColliders;
	}

	private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(32);

	[DebugReadout]
	private static readonly List<GroupInfo> groupInfos = new List<GroupInfo>(32);

	private static LayerMask gorillaInteractableLayerMask;

	private static bool isApplicationQuitting;

	private Camera mainCamera;

	private bool hasMainCamera;

	private void Awake()
	{
		gorillaInteractableLayerMask = LayerMask.NameToLayer("GorillaInteractable");
		Application.quitting += HandleApplicationQuitting;
	}

	private void OnEnable()
	{
		mainCamera = Camera.main;
		hasMainCamera = mainCamera != null;
	}

	private void HandleApplicationQuitting()
	{
		isApplicationQuitting = true;
	}

	public static int Register(StaticLodGroup lodGroup)
	{
		groupMonoBehaviours.Add(lodGroup);
		StaticLodGroupExcluder componentInParent = lodGroup.GetComponentInParent<StaticLodGroupExcluder>();
		Text[] componentsInChildren = lodGroup.GetComponentsInChildren<Text>(includeInactive: true);
		List<Text> list = new List<Text>(componentsInChildren.Length);
		Text[] array = componentsInChildren;
		foreach (Text text in array)
		{
			StaticLodGroupExcluder componentInParent2 = text.GetComponentInParent<StaticLodGroupExcluder>();
			if (!(componentInParent2 != null) || !(componentInParent2 != componentInParent))
			{
				list.Add(text);
			}
		}
		componentsInChildren = list.ToArray();
		Collider[] componentsInChildren2 = lodGroup.GetComponentsInChildren<Collider>(includeInactive: true);
		List<Collider> list2 = new List<Collider>(componentsInChildren2.Length);
		Collider[] array2 = componentsInChildren2;
		foreach (Collider collider in array2)
		{
			if (collider.gameObject.layer == (int)gorillaInteractableLayerMask)
			{
				StaticLodGroupExcluder componentInParent3 = collider.GetComponentInParent<StaticLodGroupExcluder>();
				if (!(componentInParent3 != null) || !(componentInParent3 != componentInParent))
				{
					list2.Add(collider);
				}
			}
		}
		Bounds bounds = ((componentsInChildren.Length != 0) ? new Bounds(componentsInChildren[0].transform.position, Vector3.one * 0.01f) : ((list2.Count <= 0) ? new Bounds(lodGroup.transform.position, Vector3.one * 0.01f) : new Bounds(list2[0].bounds.center, list2[0].bounds.size)));
		array = componentsInChildren;
		foreach (Text text2 in array)
		{
			bounds.Encapsulate(text2.transform.position);
		}
		foreach (Collider item in list2)
		{
			bounds.Encapsulate(item.bounds);
		}
		groupInfos.Add(new GroupInfo
		{
			componentEnabled = lodGroup.isActiveAndEnabled,
			center = bounds.center,
			radiusSq = bounds.extents.sqrMagnitude,
			uiEnabled = true,
			uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
			uiTexts = componentsInChildren,
			collidersEnabled = true,
			collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance,
			interactableColliders = list2.ToArray()
		});
		return groupMonoBehaviours.Count - 1;
	}

	public static void SetEnabled(int index, bool enable)
	{
		if (isApplicationQuitting)
		{
			GroupInfo value = groupInfos[index];
			value.componentEnabled = enable;
			groupInfos[index] = value;
		}
	}

	protected void LateUpdate()
	{
		if (!hasMainCamera)
		{
			return;
		}
		Vector3 position = mainCamera.transform.position;
		for (int i = 0; i < groupInfos.Count; i++)
		{
			GroupInfo value = groupInfos[i];
			if (!value.componentEnabled)
			{
				continue;
			}
			float num = Mathf.Max(0f, (value.center - position).sqrMagnitude - value.radiusSq);
			float num2 = (value.uiEnabled ? 0.010000001f : 0f);
			bool flag = num < value.uiEnableDistanceSq + num2;
			if (flag != value.uiEnabled)
			{
				for (int j = 0; j < value.uiTexts.Length; j++)
				{
					Text text = value.uiTexts[j];
					if (!(text == null))
					{
						text.enabled = flag;
					}
				}
			}
			value.uiEnabled = flag;
			num2 = (value.collidersEnabled ? 0.010000001f : 0f);
			bool flag2 = num < value.collisionEnableDistanceSq + num2;
			if (flag2 != value.collidersEnabled)
			{
				for (int k = 0; k < value.interactableColliders.Length; k++)
				{
					if (!(value.interactableColliders[k] == null))
					{
						value.interactableColliders[k].enabled = flag2;
					}
				}
			}
			value.collidersEnabled = flag2;
			groupInfos[i] = value;
		}
	}
}
