using System;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class SlingshotProjectile : MonoBehaviour
{
	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000557 RID: 1367 RVA: 0x0002226C File Offset: 0x0002046C
	// (remove) Token: 0x06000558 RID: 1368 RVA: 0x000222A4 File Offset: 0x000204A4
	public event SlingshotProjectile.ProjectileHitEvent OnHitPlayer;

	// Token: 0x06000559 RID: 1369 RVA: 0x000222DC File Offset: 0x000204DC
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

	// Token: 0x0600055A RID: 1370 RVA: 0x000223BB File Offset: 0x000205BB
	protected void Awake()
	{
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.forceComponent = base.GetComponent<ConstantForce>();
		this.initialScale = base.transform.localScale.x;
		this.matPropBlock = new MaterialPropertyBlock();
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x000223F6 File Offset: 0x000205F6
	public void Deactivate()
	{
		base.transform.localScale = Vector3.one * this.initialScale;
		this.OnHitPlayer = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0002242C File Offset: 0x0002062C
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

	// Token: 0x0600055D RID: 1373 RVA: 0x000224B4 File Offset: 0x000206B4
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

	// Token: 0x0600055E RID: 1374 RVA: 0x00022562 File Offset: 0x00020762
	protected void OnEnable()
	{
		this.timeCreated = 0f;
		this.particleLaunched = false;
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00022576 File Offset: 0x00020776
	protected void OnDisable()
	{
		this.particleLaunched = false;
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00022580 File Offset: 0x00020780
	protected void Update()
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

	// Token: 0x06000561 RID: 1377 RVA: 0x000225F4 File Offset: 0x000207F4
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

	// Token: 0x06000562 RID: 1378 RVA: 0x00022650 File Offset: 0x00020850
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

	// Token: 0x06000563 RID: 1379 RVA: 0x000226A8 File Offset: 0x000208A8
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
			if (other.gameObject.layer != LayerMask.NameToLayer("Gorilla Tag Collider") && other.gameObject.layer != LayerMask.NameToLayer("GorillaSlingshotCollider"))
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

	// Token: 0x06000564 RID: 1380 RVA: 0x00022844 File Offset: 0x00020A44
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

	// Token: 0x0400064E RID: 1614
	public Player projectileOwner;

	// Token: 0x0400064F RID: 1615
	[Tooltip("Rotates to point along the Y axis after spawn.")]
	public GameObject surfaceImpactEffectPrefab;

	// Token: 0x04000650 RID: 1616
	[Tooltip("Distance from the surface that the particle should spawn.")]
	private float impactEffectOffset;

	// Token: 0x04000651 RID: 1617
	public float lifeTime = 20f;

	// Token: 0x04000652 RID: 1618
	public Color defaultColor = Color.white;

	// Token: 0x04000653 RID: 1619
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x04000654 RID: 1620
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x04000655 RID: 1621
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer defaultBall;

	// Token: 0x04000656 RID: 1622
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer orangeBall;

	// Token: 0x04000657 RID: 1623
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer blueBall;

	// Token: 0x04000658 RID: 1624
	public bool colorizeBalls;

	// Token: 0x04000659 RID: 1625
	private bool particleLaunched;

	// Token: 0x0400065A RID: 1626
	private float timeCreated;

	// Token: 0x0400065B RID: 1627
	private Rigidbody projectileRigidbody;

	// Token: 0x0400065C RID: 1628
	private Color teamColor = Color.white;

	// Token: 0x0400065D RID: 1629
	private Renderer teamRenderer;

	// Token: 0x0400065E RID: 1630
	public int myProjectileCount;

	// Token: 0x0400065F RID: 1631
	private float initialScale;

	// Token: 0x04000660 RID: 1632
	private Vector3 previousPosition;

	// Token: 0x04000661 RID: 1633
	private ConstantForce forceComponent;

	// Token: 0x04000663 RID: 1635
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04000664 RID: 1636
	private static readonly int colorShaderProp = Shader.PropertyToID("_Color");

	// Token: 0x020003EB RID: 1003
	// (Invoke) Token: 0x06001BC7 RID: 7111
	public delegate void ProjectileHitEvent(Player hitPlayer);
}
