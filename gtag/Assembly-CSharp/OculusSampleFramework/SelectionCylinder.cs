using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x020002E9 RID: 745
	public class SelectionCylinder : MonoBehaviour
	{
		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600141E RID: 5150 RVA: 0x0007216D File Offset: 0x0007036D
		// (set) Token: 0x0600141F RID: 5151 RVA: 0x00072178 File Offset: 0x00070378
		public SelectionCylinder.SelectionState CurrSelectionState
		{
			get
			{
				return this._currSelectionState;
			}
			set
			{
				SelectionCylinder.SelectionState currSelectionState = this._currSelectionState;
				this._currSelectionState = value;
				if (currSelectionState != this._currSelectionState)
				{
					if (this._currSelectionState > SelectionCylinder.SelectionState.Off)
					{
						this._selectionMeshRenderer.enabled = true;
						this.AffectSelectionColor((this._currSelectionState == SelectionCylinder.SelectionState.Selected) ? this._defaultSelectionColors : this._highlightColors);
						return;
					}
					this._selectionMeshRenderer.enabled = false;
				}
			}
		}

		// Token: 0x06001420 RID: 5152 RVA: 0x000721DC File Offset: 0x000703DC
		private void Awake()
		{
			this._selectionMaterials = this._selectionMeshRenderer.materials;
			int num = this._selectionMaterials.Length;
			this._defaultSelectionColors = new Color[num];
			this._highlightColors = new Color[num];
			for (int i = 0; i < num; i++)
			{
				this._defaultSelectionColors[i] = this._selectionMaterials[i].GetColor(SelectionCylinder._colorId);
				this._highlightColors[i] = new Color(1f, 1f, 1f, this._defaultSelectionColors[i].a);
			}
			this.CurrSelectionState = SelectionCylinder.SelectionState.Off;
		}

		// Token: 0x06001421 RID: 5153 RVA: 0x00072280 File Offset: 0x00070480
		private void OnDestroy()
		{
			if (this._selectionMaterials != null)
			{
				foreach (Material material in this._selectionMaterials)
				{
					if (material != null)
					{
						Object.Destroy(material);
					}
				}
			}
		}

		// Token: 0x06001422 RID: 5154 RVA: 0x000722C0 File Offset: 0x000704C0
		private void AffectSelectionColor(Color[] newColors)
		{
			int num = newColors.Length;
			for (int i = 0; i < num; i++)
			{
				this._selectionMaterials[i].SetColor(SelectionCylinder._colorId, newColors[i]);
			}
		}

		// Token: 0x040016CC RID: 5836
		[SerializeField]
		private MeshRenderer _selectionMeshRenderer;

		// Token: 0x040016CD RID: 5837
		private static int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x040016CE RID: 5838
		private Material[] _selectionMaterials;

		// Token: 0x040016CF RID: 5839
		private Color[] _defaultSelectionColors;

		// Token: 0x040016D0 RID: 5840
		private Color[] _highlightColors;

		// Token: 0x040016D1 RID: 5841
		private SelectionCylinder.SelectionState _currSelectionState;

		// Token: 0x020004EF RID: 1263
		public enum SelectionState
		{
			// Token: 0x0400208E RID: 8334
			Off,
			// Token: 0x0400208F RID: 8335
			Selected,
			// Token: 0x04002090 RID: 8336
			Highlighted
		}
	}
}
