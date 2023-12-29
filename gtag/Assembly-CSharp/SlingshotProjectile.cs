using System;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SlingshotProjectile : MonoBehaviour
{
	public event SlingshotProjectile.ProjectileHitEvent OnHitPlayer;

	public void Launch(Vector3 position, Vector3 velocity, Player player, bool blueTeam, bool orangeTeam, int projectileCount, float scale, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		this.particleLaunched = true;
		this.timeCreated = Time.time;
		Transform transform = base.transform;
		transform.position = position;
		transform.localScale = Vector3.one * scale;
		base.GetComponent<Collider>().contactOffset = 0.01f * scale;
		RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
		if (component != null)
		{
			component.objectRadiusForWaterCollision = 0.02f * scale;
		}
		if (scale != 1f)
		{
			this.projectileRigidbody.useGravity = false;
			this.forceComponent.force = Physics.gravity * scale * this.projectileRigidbody.mass;
		}
		this.projectileRigidbody.velocity = velocity;
		this.projectileOwner = player;
		this.myProjectileCount = projectileCount;
		this.projectileRigidbody.position = position;
		this.ApplyTeamModelAndColor(blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
	}

	protected void Awake()
	{
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.forceComponent = base.GetComponent<ConstantForce>();
		this.initialScale = base.transform.localScale.x;
		this.matPropBlock = new MaterialPropertyBlock();
	}

	public void Deactivate()
	{
		base.transform.localScale = Vector3.one * this.initialScale;
		this.projectileRigidbody.useGravity = true;
		this.forceComponent.force = Vector3.zero;
		this.OnHitPlayer = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
	{
		Vector3 position2 = position + normal * this.impactEffectOffset;
		GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2);
		Vector3 localScale = base.transform.localScale;
		gameObject.transform.localScale = localScale;
		gameObject.transform.up = normal;
		GorillaColorizableBase component = gameObject.GetComponent<GorillaColorizableBase>();
		if (component != null)
		{
			component.SetColor(this.teamColor);
		}
		SurfaceImpactFX component2 = gameObject.GetComponent<SurfaceImpactFX>();
		if (component2 != null)
		{
			component2.SetScale(localScale.x);
		}
	}

	public void ApplyTeamModelAndColor(bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (shouldOverrideColor)
		{
			this.teamColor = overrideColor;
		}
		else
		{
			this.teamColor = (blueTeam ? this.blueColor : (orangeTeam ? this.orangeColor : this.defaultColor));
		}
		this.blueBall.enabled = blueTeam;
		this.orangeBall.enabled = orangeTeam;
		this.defaultBall.enabled = (!blueTeam && !orangeTeam);
		this.teamRenderer = (blueTeam ? this.blueBall : (orangeTeam ? this.orangeBall : this.defaultBall));
		this.ApplyColor(this.teamRenderer, (this.colorizeBalls || shouldOverrideColor) ? this.teamColor : Color.white);
	}

	protected void OnEnable()
	{
		this.timeCreated = 0f;
		this.particleLaunched = false;
		SlingshotProjectileManager.RegisterSP(this);
	}

	protected void OnDisable()
	{
		this.particleLaunched = false;
		SlingshotProjectileManager.UnregisterSP(this);
	}

	public void InvokeUpdate()
	{
		if (this.particleLaunched)
		{
			if (Time.time > this.timeCreated + this.lifeTime)
			{
				this.Deactivate();
			}
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Vector3 forward = position - this.previousPosition;
			transform.rotation = ((forward.sqrMagnitude > 0f) ? Quaternion.LookRotation(forward) : transform.rotation);
			this.previousPosition = position;
		}
	}

	protected void OnCollisionEnter(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.collider.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeHit(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		this.Deactivate();
	}

	protected void OnCollisionStay(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeCollisionStay(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		this.Deactivate();
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.projectileOwner == PhotonNetwork.LocalPlayer)
		{
			if (!PhotonNetwork.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			GorillaBattleManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			Player player = (componentInParent != null) ? componentInParent.creator : null;
			if (player == null)
			{
				return;
			}
			if (PhotonNetwork.LocalPlayer == player)
			{
				return;
			}
			SlingshotProjectile.ProjectileHitEvent onHitPlayer = this.OnHitPlayer;
			if (onHitPlayer != null)
			{
				onHitPlayer(player);
			}
			if (component && !component.LocalCanHit(PhotonNetwork.LocalPlayer, player))
			{
				return;
			}
			if (component)
			{
				PhotonView.Get(component).RPC("ReportSlingshotHit", RpcTarget.MasterClient, new object[]
				{
					player,
					base.transform.position,
					this.myProjectileCount
				});
			}
			PhotonView.Get(GorillaGameManager.instance).RPC("SpawnSlingshotPlayerImpactEffect", RpcTarget.All, new object[]
			{
				base.transform.position,
				this.teamColor.r,
				this.teamColor.g,
				this.teamColor.b,
				this.teamColor.a,
				this.myProjectileCount
			});
			this.Deactivate();
		}
	}

	private void ApplyColor(Renderer rend, Color color)
	{
		if (!rend)
		{
			return;
		}
		Material[] array;
		if (Application.isPlaying)
		{
			array = rend.materials;
		}
		else
		{
			array = rend.sharedMaterials;
		}
		foreach (Material material in array)
		{
			if (!(material == null))
			{
				if (material.HasProperty("_BaseColor"))
				{
					material.SetColor("_BaseColor", color);
				}
				if (material.HasProperty("_Color"))
				{
					material.SetColor("_Color", color);
				}
			}
		}
	}

	public Player projectileOwner;

	[Tooltip("Rotates to point along the Y axis after spawn.")]
	public GameObject surfaceImpactEffectPrefab;

	[Tooltip("Distance from the surface that the particle should spawn.")]
	private float impactEffectOffset;

	public float lifeTime = 20f;

	public Color defaultColor = Color.white;

	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer defaultBall;

	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer orangeBall;

	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer blueBall;

	public bool colorizeBalls;

	private bool particleLaunched;

	private float timeCreated;

	private Rigidbody projectileRigidbody;

	private Color teamColor = Color.white;

	private Renderer teamRenderer;

	public int myProjectileCount;

	private float initialScale;

	private Vector3 previousPosition;

	private ConstantForce forceComponent;

	private MaterialPropertyBlock matPropBlock;

	private static readonly int colorShaderProp = Shader.PropertyToID("_Color");

	public delegate void ProjectileHitEvent(Player hitPlayer);
}
