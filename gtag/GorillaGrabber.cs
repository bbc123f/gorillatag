using System;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.XR;

public class GorillaGrabber : MonoBehaviour
{
	public XRNode XrNode
	{
		get
		{
			return this.xrNode;
		}
	}

	public Player Player
	{
		get
		{
			return this.player;
		}
	}

	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.player = base.GetComponentInParent<Player>();
		this.breakDistance = this.grabRadius * 2f;
	}

	private void Update()
	{
		bool grabMomentary = ControllerInputPoller.GetGrabMomentary(this.xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
		if (this.currentGrabbable != null && (grabRelease || this.GrabDistanceOverCheck()))
		{
			this.Ungrab();
		}
		if (grabMomentary && this.currentGrabbable == null)
		{
			this.currentGrabbable = this.GetGrabable();
		}
	}

	private bool GrabDistanceOverCheck()
	{
		return this.currentGrabbedTransform == null || Vector3.Distance(base.transform.position, this.currentGrabbedTransform.position) > this.breakDistance;
	}

	private void Ungrab()
	{
		this.currentGrabbable.OnGrabReleased(this);
		this.currentGrabbable = null;
		this.gripEffects.Stop();
	}

	private IGorillaGrabable GetGrabable()
	{
		IGorillaGrabable gorillaGrabable = null;
		Debug.DrawRay(base.transform.position, base.transform.forward * this.grabRadius, Color.blue, 1f);
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius, this.grabCastResults);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IGorillaGrabable gorillaGrabable2;
			if (this.grabCastResults[i].TryGetComponent<IGorillaGrabable>(out gorillaGrabable2))
			{
				float num3 = Vector3.Distance(base.transform.position, this.grabCastResults[i].ClosestPoint(base.transform.position));
				if (num3 < num2)
				{
					num2 = num3;
					gorillaGrabable = gorillaGrabable2;
				}
			}
		}
		if (gorillaGrabable != null)
		{
			this.currentGrabbedTransform = gorillaGrabable.OnGrabbed(this);
		}
		return gorillaGrabable;
	}

	public GorillaGrabber()
	{
	}

	private Player player;

	[SerializeField]
	private XRNode xrNode = XRNode.LeftHand;

	private AudioSource audioSource;

	private Transform currentGrabbedTransform;

	private IGorillaGrabable currentGrabbable;

	[SerializeField]
	private float grabRadius = 0.015f;

	private float breakDistance = 0.015f;

	[SerializeField]
	private ParticleSystem gripEffects;

	private Collider[] grabCastResults = new Collider[32];
}
