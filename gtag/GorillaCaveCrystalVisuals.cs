using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

public class GorillaCaveCrystalVisuals : MonoBehaviour, ISerializationCallbackReceiver
{
	public CrystalVisualsPreset crysalPreset;

	[FormerlySerializedAs("_lerpState")]
	[SerializeField]
	[Range(0f, 1f)]
	private float _lerp;

	[Space]
	public MeshRenderer _renderer;

	public Material _sharedMaterial;

	[FormerlySerializedAs("albedoOverride")]
	[SerializeField]
	public Texture2D instanceAlbedo;

	[SerializeField]
	private bool _initialized;

	[SerializeField]
	private int _lastState;

	[SerializeField]
	public GorillaCaveCrystalSetup _setup;

	private MaterialPropertyBlock _block;

	private static readonly int kIdEmission = Shader.PropertyToID("_EmissionColor");

	private static readonly int kIdAlbedo = Shader.PropertyToID("_Color");

	private static readonly int kIdMainTex = Shader.PropertyToID("_MainTex");

	public float lerp
	{
		get
		{
			return _lerp;
		}
		set
		{
			_lerp = value;
		}
	}

	public void Setup()
	{
		TryGetComponent<MeshRenderer>(out _renderer);
		if (!(_renderer == null))
		{
			_setup = GorillaCaveCrystalSetup.Instance;
			_sharedMaterial = _renderer.sharedMaterial;
			_initialized = crysalPreset != null && _renderer != null && _sharedMaterial != null;
			Update();
		}
	}

	public void UpdateAlbedo()
	{
		if (_initialized && !(instanceAlbedo == null))
		{
			if (_block == null)
			{
				_block = new MaterialPropertyBlock();
			}
			_renderer.GetPropertyBlock(_block);
			_block.SetTexture(kIdMainTex, instanceAlbedo);
			_renderer.SetPropertyBlock(_block);
		}
	}

	private void Awake()
	{
		UpdateAlbedo();
		Update();
	}

	private void Update()
	{
		if (!_initialized)
		{
			return;
		}
		if (Application.isPlaying)
		{
			int hashCode = (crysalPreset, _lerp).GetHashCode();
			if (_lastState == hashCode)
			{
				return;
			}
			_lastState = hashCode;
		}
		if (_block == null)
		{
			_block = new MaterialPropertyBlock();
		}
		CrystalVisualsPreset.VisualState stateA = crysalPreset.stateA;
		CrystalVisualsPreset.VisualState stateB = crysalPreset.stateB;
		Color value = Color.Lerp(stateA.albedo, stateB.albedo, _lerp);
		Color value2 = Color.Lerp(stateA.emission, stateB.emission, _lerp);
		_renderer.GetPropertyBlock(_block);
		_block.SetColor(kIdAlbedo, value);
		_block.SetColor(kIdEmission, value2);
		_renderer.SetPropertyBlock(_block);
	}

	public void ForceUpdate()
	{
		_lastState = 0;
		Update();
	}

	private void OnValidate()
	{
		ForceUpdate();
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		ForceUpdate();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
	}
}
