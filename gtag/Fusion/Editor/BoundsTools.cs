using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Editor
{
	public static class BoundsTools
	{
		public static Bounds CollectMyBounds(this GameObject go, BoundsTools.BoundsType factorIn, out int numOfBoundsFound, bool includeChildren = true, bool includeInactive = false)
		{
			if (!go.activeInHierarchy && includeInactive)
			{
				numOfBoundsFound = 0;
				return default(Bounds);
			}
			bool flag = factorIn == BoundsTools.BoundsType.Both;
			bool flag2 = flag || factorIn == BoundsTools.BoundsType.MeshRenderer;
			bool flag3 = flag || factorIn == BoundsTools.BoundsType.Collider;
			BoundsTools.meshFilters.Clear();
			BoundsTools.meshRenderers.Clear();
			BoundsTools.colliders.Clear();
			BoundsTools.spriteRenderers.Clear();
			BoundsTools.validColliders.Clear();
			BoundsTools.validColliders2D.Clear();
			int num = 0;
			if (flag2 && go.activeInHierarchy)
			{
				if (includeChildren)
				{
					go.GetComponentsInChildren<Renderer>(includeInactive, BoundsTools.meshRenderers);
					go.GetComponentsInChildren<MeshFilter>(includeInactive, BoundsTools.meshFilters);
					go.GetComponentsInChildren<SpriteRenderer>(includeInactive, BoundsTools.spriteRenderers);
				}
				else
				{
					go.GetComponents<Renderer>(BoundsTools.meshRenderers);
					go.GetComponents<MeshFilter>(BoundsTools.meshFilters);
					go.GetComponents<SpriteRenderer>(BoundsTools.spriteRenderers);
				}
			}
			if (flag3 && go.activeInHierarchy)
			{
				if (includeChildren)
				{
					go.GetComponentsInChildren<Collider>(includeInactive, BoundsTools.colliders);
					go.GetComponentsInChildren<Collider2D>(includeInactive, BoundsTools.colliders2D);
				}
				else
				{
					go.GetComponents<Collider>(BoundsTools.colliders);
					go.GetComponents<Collider2D>(BoundsTools.colliders2D);
				}
			}
			for (int i = 0; i < BoundsTools.meshFilters.Count; i++)
			{
				Renderer component = BoundsTools.meshFilters[i].GetComponent<Renderer>();
				if (component && (component.enabled || includeInactive) && !BoundsTools.meshRenderers.Contains(component))
				{
					BoundsTools.meshRenderers.Add(component);
				}
			}
			for (int j = 0; j < BoundsTools.colliders.Count; j++)
			{
				if ((BoundsTools.colliders[j].enabled || includeInactive) && BoundsTools.colliders[j])
				{
					BoundsTools.validColliders.Add(BoundsTools.colliders[j]);
				}
			}
			for (int k = 0; k < BoundsTools.colliders2D.Count; k++)
			{
				if (((BoundsTools.colliders2D[k] && BoundsTools.colliders2D[k].enabled) || includeInactive) && BoundsTools.colliders2D[k])
				{
					BoundsTools.validColliders2D.Add(BoundsTools.colliders2D[k]);
				}
			}
			numOfBoundsFound = BoundsTools.meshRenderers.Count + BoundsTools.spriteRenderers.Count + BoundsTools.validColliders.Count + BoundsTools.validColliders2D.Count;
			if (numOfBoundsFound == 0)
			{
				return default(Bounds);
			}
			Bounds bounds;
			if (BoundsTools.meshRenderers.Count > 0)
			{
				bounds = BoundsTools.meshRenderers[0].bounds;
			}
			else if (BoundsTools.validColliders.Count > 0)
			{
				bounds = BoundsTools.validColliders[0].bounds;
			}
			else if (BoundsTools.validColliders2D.Count > 0 && BoundsTools.validColliders2D[0])
			{
				bounds = BoundsTools.validColliders2D[0].bounds;
			}
			else
			{
				if (BoundsTools.spriteRenderers.Count <= 0)
				{
					return default(Bounds);
				}
				bounds = BoundsTools.spriteRenderers[0].bounds;
			}
			for (int l = 0; l < BoundsTools.spriteRenderers.Count; l++)
			{
				num++;
				bounds.Encapsulate(BoundsTools.spriteRenderers[l].bounds);
			}
			for (int m = 0; m < BoundsTools.meshRenderers.Count; m++)
			{
				num++;
				bounds.Encapsulate(BoundsTools.meshRenderers[m].bounds);
			}
			for (int n = 0; n < BoundsTools.validColliders.Count; n++)
			{
				num++;
				bounds.Encapsulate(BoundsTools.validColliders[n].bounds);
			}
			for (int num2 = 0; num2 < BoundsTools.validColliders2D.Count; num2++)
			{
				num++;
				if (BoundsTools.validColliders2D[num2])
				{
					bounds.Encapsulate(BoundsTools.validColliders2D[num2].bounds);
				}
			}
			return bounds;
		}

		public static Bounds CollectMyBounds(GameObject go, BoundsTools.BoundsType factorIn, bool includeChildren = true)
		{
			int num;
			return go.CollectMyBounds(factorIn, out num, includeChildren, false);
		}

		private static readonly List<MeshFilter> meshFilters = new List<MeshFilter>();

		private static readonly List<Renderer> meshRenderers = new List<Renderer>();

		private static readonly List<Collider> colliders = new List<Collider>();

		private static readonly List<Collider> validColliders = new List<Collider>();

		private static readonly List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

		private static readonly List<Collider2D> colliders2D = new List<Collider2D>();

		private static readonly List<Collider2D> validColliders2D = new List<Collider2D>();

		public enum BoundsType
		{
			Both,
			MeshRenderer,
			Collider,
			Manual
		}
	}
}
