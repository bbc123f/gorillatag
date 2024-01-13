using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SlingshotProjectile : MonoBehaviour
{
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

	public void Launch(Vector3 position, Vector3 velocity, Player player, bool blueTeam, bool orangeTeam, int projectileCount, float scale, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		particleLaunched = true;
		timeCreated = Time.time;
		Transform obj = base.transform;
		obj.position = position;
		obj.localScale = Vector3.one * scale;
		GetComponent<Collider>().contactOffset = 0.01f * scale;
		if (scale != 1f)
		{
			projectileRigidbody.useGravity = false;
			forceComponent.force = Physics.gravity * scale * projectileRigidbody.mass;
		}
		projectileRigidbody.velocity = velocity;
		projectileOwner = player;
		myProjectileCount = projectileCount;
		ApplyTeamModelAndColor(blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
	}

	protected void Awake()
	{
		projectileRigidbody = GetComponent<Rigidbody>();
		forceComponent = GetComponent<ConstantForce>();
		initialScale = base.transform.localScale.x;
		matPropBlock = new MaterialPropertyBlock();
	}

	public void Deactivate()
	{
		base.transform.localScale = Vector3.one * initialScale;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
	{
		Vector3 position2 = position + normal * impactEffectOffset;
		GameObject obj = ObjectPools.instance.Instantiate(prefab, position2);
		Vector3 localScale = base.transform.localScale;
		obj.transform.localScale = localScale;
		obj.transform.up = normal;
		obj.GetComponent<GorillaColorizableBase>().SetColor(teamColor);
		obj.GetComponent<SurfaceImpactFX>()?.SetScale(localScale.x);
	}

	public void ApplyTeamModelAndColor(bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (shouldOverrideColor)
		{
			teamColor = overrideColor;
		}
		else
		{
			teamColor = (blueTeam ? blueColor : (orangeTeam ? orangeColor : defaultColor));
		}
		teamRenderer = (blueTeam ? blueBall : (orangeTeam ? orangeBall : defaultBall));
		ApplyColor(teamRenderer, (colorizeBalls || shouldOverrideColor) ? teamColor : Color.white);
	}

	protected void OnEnable()
	{
		timeCreated = 0f;
		particleLaunched = false;
	}

	protected void OnDisable()
	{
		particleLaunched = false;
	}

	protected void Update()
	{
		if (particleLaunched)
		{
			if (Time.time > timeCreated + lifeTime)
			{
				Deactivate();
			}
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Vector3 forward = position - previousPosition;
			transform.rotation = ((forward.sqrMagnitude > 0f) ? Quaternion.LookRotation(forward) : transform.rotation);
			previousPosition = position;
		}
	}

	protected void OnCollisionEnter(Collision collision)
	{
		if (particleLaunched)
		{
			if (collision.gameObject.TryGetComponent<HitTargetWithScoreCounter>(out var component))
			{
				component.TargetHit();
			}
			ContactPoint contact = collision.GetContact(0);
			SpawnImpactEffect(surfaceImpactEffectPrefab, contact.point, contact.normal);
			Deactivate();
		}
	}

	protected void OnCollisionStay(Collision collision)
	{
		if (particleLaunched)
		{
			if (collision.gameObject.TryGetComponent<HitTargetWithScoreCounter>(out var component))
			{
				component.TargetHit();
			}
			ContactPoint contact = collision.GetContact(0);
			SpawnImpactEffect(surfaceImpactEffectPrefab, contact.point, contact.normal);
			Deactivate();
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (!particleLaunched || projectileOwner != PhotonNetwork.LocalPlayer || !PhotonNetwork.InRoom || GorillaGameManager.instance == null)
		{
			return;
		}
		GorillaBattleManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaBattleManager>();
		if (other.gameObject.layer != LayerMask.NameToLayer("Gorilla Tag Collider") && other.gameObject.layer != LayerMask.NameToLayer("GorillaSlingshotCollider"))
		{
			return;
		}
		Player player = other.GetComponentInParent<VRRig>()?.creator;
		if (player != null && PhotonNetwork.LocalPlayer != player && (!component || component.LocalCanHit(PhotonNetwork.LocalPlayer, player)))
		{
			if ((bool)component)
			{
				PhotonView.Get(component).RPC("ReportSlingshotHit", RpcTarget.MasterClient, player, base.transform.position, myProjectileCount);
			}
			PhotonView.Get(GorillaGameManager.instance).RPC("SpawnSlingshotPlayerImpactEffect", RpcTarget.All, base.transform.position, teamColor.r, teamColor.g, teamColor.b, teamColor.a, myProjectileCount);
			Deactivate();
		}
	}

	private void ApplyColor(Renderer rend, Color color)
	{
		matPropBlock.SetColor(colorShaderProp, color);
		rend.SetPropertyBlock(matPropBlock);
	}
}
