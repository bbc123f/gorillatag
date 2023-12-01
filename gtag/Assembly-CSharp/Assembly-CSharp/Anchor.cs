using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(OVRSpatialAnchor))]
public class Anchor : MonoBehaviour
{
	private void Awake()
	{
		this._anchorMenu.SetActive(false);
		this._renderers = base.GetComponentsInChildren<MeshRenderer>();
		this._canvas.worldCamera = Camera.main;
		this._selectedButton = this._buttonList[0];
		this._selectedButton.OnSelect(null);
		this._spatialAnchor = base.GetComponent<OVRSpatialAnchor>();
		this._icon = base.GetComponent<Transform>().FindChildRecursive("Sphere").gameObject;
	}

	private IEnumerator Start()
	{
		while (this._spatialAnchor && !this._spatialAnchor.Created)
		{
			yield return null;
		}
		if (this._spatialAnchor)
		{
			this._anchorName.text = this._spatialAnchor.Uuid.ToString("D");
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		yield break;
	}

	private void Update()
	{
		this.BillboardPanel(this._canvas.transform);
		this.BillboardPanel(this._pivot);
		this.HandleMenuNavigation();
		this.BillboardPanel(this._icon.transform);
	}

	public void OnSaveLocalButtonPressed()
	{
		if (!this._spatialAnchor)
		{
			return;
		}
		this._spatialAnchor.Save(delegate(OVRSpatialAnchor anchor, bool success)
		{
			if (!success)
			{
				return;
			}
			this.ShowSaveIcon = true;
			if (!PlayerPrefs.HasKey("numUuids"))
			{
				PlayerPrefs.SetInt("numUuids", 0);
			}
			int @int = PlayerPrefs.GetInt("numUuids");
			PlayerPrefs.SetString("uuid" + @int.ToString(), anchor.Uuid.ToString());
			PlayerPrefs.SetInt("numUuids", @int + 1);
		});
	}

	public void OnHideButtonPressed()
	{
		Object.Destroy(base.gameObject);
	}

	public void OnEraseButtonPressed()
	{
		if (!this._spatialAnchor)
		{
			return;
		}
		this._spatialAnchor.Erase(delegate(OVRSpatialAnchor anchor, bool success)
		{
			if (success)
			{
				this._saveIcon.SetActive(false);
			}
		});
	}

	public bool ShowSaveIcon
	{
		set
		{
			this._saveIcon.SetActive(value);
		}
	}

	public void OnHoverStart()
	{
		if (this._isHovered)
		{
			return;
		}
		this._isHovered = true;
		MeshRenderer[] renderers = this._renderers;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.SetColor("_EmissionColor", Color.yellow);
		}
		this._labelImage.color = this._labelHighlightColor;
	}

	public void OnHoverEnd()
	{
		if (!this._isHovered)
		{
			return;
		}
		this._isHovered = false;
		MeshRenderer[] renderers = this._renderers;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.SetColor("_EmissionColor", Color.clear);
		}
		if (this._isSelected)
		{
			this._labelImage.color = this._labelSelectedColor;
			return;
		}
		this._labelImage.color = this._labelBaseColor;
	}

	public void OnSelect()
	{
		if (this._isSelected)
		{
			this._anchorMenu.SetActive(false);
			this._isSelected = false;
			this._selectedButton = null;
			if (this._isHovered)
			{
				this._labelImage.color = this._labelHighlightColor;
				return;
			}
			this._labelImage.color = this._labelBaseColor;
			return;
		}
		else
		{
			this._anchorMenu.SetActive(true);
			this._isSelected = true;
			this._menuIndex = -1;
			this.NavigateToIndexInMenu(true);
			if (this._isHovered)
			{
				this._labelImage.color = this._labelHighlightColor;
				return;
			}
			this._labelImage.color = this._labelSelectedColor;
			return;
		}
	}

	private void BillboardPanel(Transform panel)
	{
		panel.LookAt(new Vector3(panel.position.x * 2f - Camera.main.transform.position.x, panel.position.y * 2f - Camera.main.transform.position.y, panel.position.z * 2f - Camera.main.transform.position.z), Vector3.up);
	}

	private void HandleMenuNavigation()
	{
		if (!this._isSelected)
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
		if (this._selectedButton)
		{
			this._selectedButton.OnDeselect(null);
		}
		this._selectedButton = this._buttonList[this._menuIndex];
		this._selectedButton.OnSelect(null);
	}

	public const string NumUuidsPlayerPref = "numUuids";

	[SerializeField]
	[FormerlySerializedAs("canvas_")]
	private Canvas _canvas;

	[SerializeField]
	[FormerlySerializedAs("pivot_")]
	private Transform _pivot;

	[SerializeField]
	[FormerlySerializedAs("anchorMenu_")]
	private GameObject _anchorMenu;

	private bool _isSelected;

	private bool _isHovered;

	[SerializeField]
	[FormerlySerializedAs("anchorName_")]
	private TextMeshProUGUI _anchorName;

	[SerializeField]
	[FormerlySerializedAs("saveIcon_")]
	private GameObject _saveIcon;

	[SerializeField]
	[FormerlySerializedAs("labelImage_")]
	private Image _labelImage;

	[SerializeField]
	[FormerlySerializedAs("labelBaseColor_")]
	private Color _labelBaseColor;

	[SerializeField]
	[FormerlySerializedAs("labelHighlightColor_")]
	private Color _labelHighlightColor;

	[SerializeField]
	[FormerlySerializedAs("labelSelectedColor_")]
	private Color _labelSelectedColor;

	[SerializeField]
	[FormerlySerializedAs("uiManager_")]
	private AnchorUIManager _uiManager;

	[SerializeField]
	[FormerlySerializedAs("renderers_")]
	private MeshRenderer[] _renderers;

	private int _menuIndex;

	[SerializeField]
	[FormerlySerializedAs("buttonList_")]
	private List<Button> _buttonList;

	private Button _selectedButton;

	private OVRSpatialAnchor _spatialAnchor;

	private GameObject _icon;
}
