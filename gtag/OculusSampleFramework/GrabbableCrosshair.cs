using System;
using UnityEngine;

namespace OculusSampleFramework
{
	public class GrabbableCrosshair : MonoBehaviour
	{
		private void Start()
		{
			this.m_centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
		}

		public void SetState(GrabbableCrosshair.CrosshairState cs)
		{
			this.m_state = cs;
			if (cs == GrabbableCrosshair.CrosshairState.Disabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(false);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Enabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(true);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Targeted)
			{
				this.m_targetedCrosshair.SetActive(true);
				this.m_enabledCrosshair.SetActive(false);
			}
		}

		private void Update()
		{
			if (this.m_state != GrabbableCrosshair.CrosshairState.Disabled)
			{
				base.transform.LookAt(this.m_centerEyeAnchor);
			}
		}

		public GrabbableCrosshair()
		{
		}

		private GrabbableCrosshair.CrosshairState m_state;

		private Transform m_centerEyeAnchor;

		[SerializeField]
		private GameObject m_targetedCrosshair;

		[SerializeField]
		private GameObject m_enabledCrosshair;

		public enum CrosshairState
		{
			Disabled,
			Enabled,
			Targeted
		}
	}
}
