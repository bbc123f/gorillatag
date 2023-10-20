using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200011C RID: 284
public class TransferrableItemSlotTransformOverride : MonoBehaviour
{
	// Token: 0x0600073A RID: 1850 RVA: 0x0002D9A0 File Offset: 0x0002BBA0
	private void Start()
	{
		this.defaultPosition = new SlotTransformOverride
		{
			overrideTransform = this.defaultTransform,
			positionState = TransferrableObject.PositionState.None
		};
		this.lastPosition = TransferrableObject.PositionState.None;
		if (this.transformOverrides == null)
		{
			this.transformOverrides = new List<SlotTransformOverride>();
		}
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			slotTransformOverride.Initialize(this.anchor);
		}
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x0002DA30 File Offset: 0x0002BC30
	public void AddGripPosition(TransferrableObject.PositionState state, TransferrableObjectGripPosition togp)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if (slotTransformOverride.positionState == state)
			{
				slotTransformOverride.AddSubGrabPoint(togp);
				return;
			}
		}
		SlotTransformOverride slotTransformOverride2 = new SlotTransformOverride
		{
			positionState = state
		};
		this.transformOverridesDeprecated.Add(slotTransformOverride2);
		slotTransformOverride2.AddSubGrabPoint(togp);
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0002DAB0 File Offset: 0x0002BCB0
	private void Update()
	{
		if (this.followingTransferrableObject == null)
		{
			return;
		}
		if (this.followingTransferrableObject.currentState != this.lastPosition)
		{
			SlotTransformOverride slotTransformOverride = this.transformOverridesDeprecated.Find((SlotTransformOverride x) => (x.positionState & this.followingTransferrableObject.currentState) > TransferrableObject.PositionState.None);
			if (slotTransformOverride != null && slotTransformOverride.positionState == TransferrableObject.PositionState.None)
			{
				slotTransformOverride = this.defaultPosition;
			}
		}
		this.lastPosition = this.followingTransferrableObject.currentState;
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x0002DB1A File Offset: 0x0002BD1A
	private void Awake()
	{
		this.GenerateTransformFromPositionState();
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0002DB24 File Offset: 0x0002BD24
	public void GenerateTransformFromPositionState()
	{
		this.transformFromPosition = new Dictionary<TransferrableObject.PositionState, Transform>();
		if (this.transformOverridesDeprecated.Count > 0)
		{
			this.transformFromPosition[TransferrableObject.PositionState.None] = this.transformOverridesDeprecated[0].overrideTransform;
		}
		foreach (TransferrableObject.PositionState positionState in Enum.GetValues(typeof(TransferrableObject.PositionState)).Cast<TransferrableObject.PositionState>())
		{
			if (positionState == TransferrableObject.PositionState.None)
			{
				this.transformFromPosition[positionState] = null;
			}
			else
			{
				Transform value = null;
				foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
				{
					if ((slotTransformOverride.positionState & positionState) != TransferrableObject.PositionState.None)
					{
						value = slotTransformOverride.overrideTransform;
						break;
					}
				}
				this.transformFromPosition[positionState] = value;
			}
		}
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0002DC20 File Offset: 0x0002BE20
	[CanBeNull]
	public Transform GetTransformFromPositionState(TransferrableObject.PositionState currentState)
	{
		if (this.transformFromPosition == null)
		{
			this.GenerateTransformFromPositionState();
		}
		return this.transformFromPosition[currentState];
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0002DC3C File Offset: 0x0002BE3C
	public bool GetTransformFromPositionState(TransferrableObject.PositionState currentState, AdvancedItemState advancedItemState, Transform targetDockXf, out Matrix4x4 matrix4X4)
	{
		if (currentState != TransferrableObject.PositionState.None)
		{
			foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
			{
				if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None)
				{
					if (!slotTransformOverride.useAdvancedGrab)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					if (advancedItemState.index >= slotTransformOverride.multiPoints.Count)
					{
						matrix4X4 = slotTransformOverride.overrideTransformMatrix;
						return true;
					}
					SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[advancedItemState.index];
					matrix4X4 = subGrabPoint.GetTransformFromPositionState(advancedItemState, slotTransformOverride, targetDockXf);
					return true;
				}
			}
			matrix4X4 = Matrix4x4.identity;
			return false;
		}
		if (this.transformOverridesDeprecated.Count > 0)
		{
			matrix4X4 = this.transformOverridesDeprecated[0].overrideTransformMatrix;
			return true;
		}
		matrix4X4 = Matrix4x4.identity;
		return false;
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0002DD40 File Offset: 0x0002BF40
	public AdvancedItemState GetAdvancedItemStateFromHand(TransferrableObject.PositionState currentState, Transform handTransform, Transform targetDock)
	{
		foreach (SlotTransformOverride slotTransformOverride in this.transformOverridesDeprecated)
		{
			if ((slotTransformOverride.positionState & currentState) != TransferrableObject.PositionState.None && slotTransformOverride.multiPoints.Count != 0)
			{
				SubGrabPoint subGrabPoint = slotTransformOverride.multiPoints[0];
				float num = float.PositiveInfinity;
				int index = -1;
				for (int i = 0; i < slotTransformOverride.multiPoints.Count; i++)
				{
					SubGrabPoint subGrabPoint2 = slotTransformOverride.multiPoints[i];
					if (!(subGrabPoint2.gripPoint == null))
					{
						float num2 = subGrabPoint2.EvaluateScore(base.transform, handTransform, targetDock);
						if (num2 < num)
						{
							subGrabPoint = subGrabPoint2;
							num = num2;
							index = i;
						}
					}
				}
				AdvancedItemState advancedItemStateFromHand = subGrabPoint.GetAdvancedItemStateFromHand(base.transform, handTransform, targetDock, slotTransformOverride);
				advancedItemStateFromHand.index = index;
				return advancedItemStateFromHand;
			}
		}
		return new AdvancedItemState();
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0002DE40 File Offset: 0x0002C040
	public void Edit()
	{
		if (TransferrableItemSlotTransformOverride.OnBringUpWindow != null)
		{
			TransferrableItemSlotTransformOverride.OnBringUpWindow(base.GetComponent<TransferrableObject>());
		}
	}

	// Token: 0x040008D9 RID: 2265
	[FormerlySerializedAs("transformOverridesList")]
	public List<SlotTransformOverride> transformOverridesDeprecated;

	// Token: 0x040008DA RID: 2266
	[SerializeReference]
	public List<SlotTransformOverride> transformOverrides;

	// Token: 0x040008DB RID: 2267
	public TransferrableObject.PositionState lastPosition;

	// Token: 0x040008DC RID: 2268
	public TransferrableObject followingTransferrableObject;

	// Token: 0x040008DD RID: 2269
	public SlotTransformOverride defaultPosition;

	// Token: 0x040008DE RID: 2270
	public Transform defaultTransform;

	// Token: 0x040008DF RID: 2271
	public Transform anchor;

	// Token: 0x040008E0 RID: 2272
	public Dictionary<TransferrableObject.PositionState, Transform> transformFromPosition;

	// Token: 0x040008E1 RID: 2273
	public static Action<TransferrableObject> OnBringUpWindow;
}
