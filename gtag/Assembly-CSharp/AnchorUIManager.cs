using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020000C7 RID: 199
[RequireComponent(typeof(SpatialAnchorLoader))]
public class AnchorUIManager : MonoBehaviour
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600045F RID: 1119 RVA: 0x0001C4A8 File Offset: 0x0001A6A8
	public Anchor AnchorPrefab
	{
		get
		{
			return this._anchorPrefab;
		}
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001C4B0 File Offset: 0x0001A6B0
	private void Awake()
	{
		if (AnchorUIManager.Instance == null)
		{
			AnchorUIManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0001C4CC File Offset: 0x0001A6CC
	private void Start()
	{
		this._raycastOrigin = this._trackedDevice;
		this._selectedButton = this._buttonList[0];
		this._buttonList[0].OnSelect(null);
		this._lineRenderer.startWidth = 0.005f;
		this._lineRenderer.endWidth = 0.005f;
		this.ToggleCreateMode();
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x0001C530 File Offset: 0x0001A730
	private void Update()
	{
		if (this._drawRaycast)
		{
			this.ControllerRaycast();
		}
		if (this._selectedAnchor == null)
		{
			this._selectedButton.OnSelect(null);
			this._isFocused = true;
		}
		this.HandleMenuNavigation();
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			AnchorUIManager.PrimaryPressDelegate primaryPressDelegate = this._primaryPressDelegate;
			if (primaryPressDelegate == null)
			{
				return;
			}
			primaryPressDelegate();
		}
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x0001C58F File Offset: 0x0001A78F
	public void OnCreateModeButtonPressed()
	{
		this.ToggleCreateMode();
		this._createModeButton.SetActive(!this._createModeButton.activeSelf);
		this._selectModeButton.SetActive(!this._selectModeButton.activeSelf);
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0001C5C9 File Offset: 0x0001A7C9
	public void OnLoadAnchorsButtonPressed()
	{
		base.GetComponent<SpatialAnchorLoader>().LoadAnchorsByUuid();
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x0001C5D6 File Offset: 0x0001A7D6
	private void ToggleCreateMode()
	{
		if (this._mode == AnchorUIManager.AnchorMode.Select)
		{
			this._mode = AnchorUIManager.AnchorMode.Create;
			this.EndSelectMode();
			this.StartPlacementMode();
			return;
		}
		this._mode = AnchorUIManager.AnchorMode.Select;
		this.EndPlacementMode();
		this.StartSelectMode();
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0001C608 File Offset: 0x0001A808
	private void StartPlacementMode()
	{
		this.ShowAnchorPreview();
		this._primaryPressDelegate = new AnchorUIManager.PrimaryPressDelegate(this.PlaceAnchor);
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x0001C622 File Offset: 0x0001A822
	private void EndPlacementMode()
	{
		this.HideAnchorPreview();
		this._primaryPressDelegate = null;
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0001C631 File Offset: 0x0001A831
	private void StartSelectMode()
	{
		this.ShowRaycastLine();
		this._primaryPressDelegate = new AnchorUIManager.PrimaryPressDelegate(this.SelectAnchor);
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001C64B File Offset: 0x0001A84B
	private void EndSelectMode()
	{
		this.HideRaycastLine();
		this._primaryPressDelegate = null;
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0001C65C File Offset: 0x0001A85C
	private void HandleMenuNavigation()
	{
		if (!this._isFocused)
		{
			return;
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp, OVRInput.Controller.Active))
		{
			this.NavigateToIndexInMenu(false);
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown, OVRInput.Controller.Active))
		{
			this.NavigateToIndexInMenu(true);
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.Active))
		{
			this._selectedButton.OnSubmit(null);
		}
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0001C6C0 File Offset: 0x0001A8C0
	private void NavigateToIndexInMenu(bool moveNext)
	{
		if (moveNext)
		{
			this._menuIndex++;
			if (this._menuIndex > this._buttonList.Count - 1)
			{
				this._menuIndex = 0;
			}
		}
		else
		{
			this._menuIndex--;
			if (this._menuIndex < 0)
			{
				this._menuIndex = this._buttonList.Count - 1;
			}
		}
		this._selectedButton.OnDeselect(null);
		this._selectedButton = this._buttonList[this._menuIndex];
		this._selectedButton.OnSelect(null);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0001C755 File Offset: 0x0001A955
	private void ShowAnchorPreview()
	{
		this._placementPreview.SetActive(true);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0001C763 File Offset: 0x0001A963
	private void HideAnchorPreview()
	{
		this._placementPreview.SetActive(false);
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0001C771 File Offset: 0x0001A971
	private void PlaceAnchor()
	{
		Object.Instantiate<Anchor>(this._anchorPrefab, this._anchorPlacementTransform.position, this._anchorPlacementTransform.rotation);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0001C795 File Offset: 0x0001A995
	private void ShowRaycastLine()
	{
		this._drawRaycast = true;
		this._lineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001C7AF File Offset: 0x0001A9AF
	private void HideRaycastLine()
	{
		this._drawRaycast = false;
		this._lineRenderer.gameObject.SetActive(false);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0001C7CC File Offset: 0x0001A9CC
	private void ControllerRaycast()
	{
		Ray ray = new Ray(this._raycastOrigin.position, this._raycastOrigin.TransformDirection(Vector3.forward));
		this._lineRenderer.SetPosition(0, this._raycastOrigin.position);
		this._lineRenderer.SetPosition(1, this._raycastOrigin.position + this._raycastOrigin.TransformDirection(Vector3.forward) * 10f);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
		{
			Anchor component = raycastHit.collider.GetComponent<Anchor>();
			if (component != null)
			{
				this._lineRenderer.SetPosition(1, raycastHit.point);
				this.HoverAnchor(component);
				return;
			}
		}
		this.UnhoverAnchor();
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0001C88B File Offset: 0x0001AA8B
	private void HoverAnchor(Anchor anchor)
	{
		this._hoveredAnchor = anchor;
		this._hoveredAnchor.OnHoverStart();
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x0001C89F File Offset: 0x0001AA9F
	private void UnhoverAnchor()
	{
		if (this._hoveredAnchor == null)
		{
			return;
		}
		this._hoveredAnchor.OnHoverEnd();
		this._hoveredAnchor = null;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0001C8C4 File Offset: 0x0001AAC4
	private void SelectAnchor()
	{
		if (this._hoveredAnchor != null)
		{
			if (this._selectedAnchor != null)
			{
				this._selectedAnchor.OnSelect();
				this._selectedAnchor = null;
			}
			this._selectedAnchor = this._hoveredAnchor;
			this._selectedAnchor.OnSelect();
			this._selectedButton.OnDeselect(null);
			this._isFocused = false;
			return;
		}
		if (this._selectedAnchor != null)
		{
			this._selectedAnchor.OnSelect();
			this._selectedAnchor = null;
			this._selectedButton.OnSelect(null);
			this._isFocused = true;
		}
	}

	// Token: 0x0400050D RID: 1293
	public static AnchorUIManager Instance;

	// Token: 0x0400050E RID: 1294
	[SerializeField]
	[FormerlySerializedAs("createModeButton_")]
	private GameObject _createModeButton;

	// Token: 0x0400050F RID: 1295
	[SerializeField]
	[FormerlySerializedAs("selectModeButton_")]
	private GameObject _selectModeButton;

	// Token: 0x04000510 RID: 1296
	[SerializeField]
	[FormerlySerializedAs("trackedDevice_")]
	private Transform _trackedDevice;

	// Token: 0x04000511 RID: 1297
	private Transform _raycastOrigin;

	// Token: 0x04000512 RID: 1298
	private bool _drawRaycast;

	// Token: 0x04000513 RID: 1299
	[SerializeField]
	[FormerlySerializedAs("lineRenderer_")]
	private LineRenderer _lineRenderer;

	// Token: 0x04000514 RID: 1300
	private Anchor _hoveredAnchor;

	// Token: 0x04000515 RID: 1301
	private Anchor _selectedAnchor;

	// Token: 0x04000516 RID: 1302
	private AnchorUIManager.AnchorMode _mode;

	// Token: 0x04000517 RID: 1303
	[SerializeField]
	[FormerlySerializedAs("buttonList_")]
	private List<Button> _buttonList;

	// Token: 0x04000518 RID: 1304
	private int _menuIndex;

	// Token: 0x04000519 RID: 1305
	private Button _selectedButton;

	// Token: 0x0400051A RID: 1306
	[SerializeField]
	private Anchor _anchorPrefab;

	// Token: 0x0400051B RID: 1307
	[SerializeField]
	[FormerlySerializedAs("placementPreview_")]
	private GameObject _placementPreview;

	// Token: 0x0400051C RID: 1308
	[SerializeField]
	[FormerlySerializedAs("anchorPlacementTransform_")]
	private Transform _anchorPlacementTransform;

	// Token: 0x0400051D RID: 1309
	private AnchorUIManager.PrimaryPressDelegate _primaryPressDelegate;

	// Token: 0x0400051E RID: 1310
	private bool _isFocused = true;

	// Token: 0x020003DF RID: 991
	public enum AnchorMode
	{
		// Token: 0x04001C4E RID: 7246
		Create,
		// Token: 0x04001C4F RID: 7247
		Select
	}

	// Token: 0x020003E0 RID: 992
	// (Invoke) Token: 0x06001BAD RID: 7085
	private delegate void PrimaryPressDelegate();
}
