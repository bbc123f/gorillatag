using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace OculusSampleFramework
{
	public class PanelHMDFollower : MonoBehaviour
	{
		private void Awake()
		{
			this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			this._panelInitialPosition = base.transform.position;
		}

		private void Update()
		{
			Vector3 position = this._cameraRig.centerEyeAnchor.position;
			Vector3 position2 = base.transform.position;
			float num = Vector3.Distance(position, this._lastMovedToPos);
			float num2 = (this._cameraRig.centerEyeAnchor.position - this._prevPos).magnitude / Time.deltaTime;
			Vector3 vector = base.transform.position - position;
			float magnitude = vector.magnitude;
			if ((num > this._maxDistance || this._minZDistance > vector.z || this._minDistance > magnitude) && num2 < 0.3f && this._coroutine == null && this._coroutine == null)
			{
				this._coroutine = base.StartCoroutine(this.LerpToHMD());
			}
			this._prevPos = this._cameraRig.centerEyeAnchor.position;
		}

		private Vector3 CalculateIdealAnchorPosition()
		{
			return this._cameraRig.centerEyeAnchor.position + this._panelInitialPosition;
		}

		private IEnumerator LerpToHMD()
		{
			Vector3 newPanelPosition = this.CalculateIdealAnchorPosition();
			this._lastMovedToPos = this._cameraRig.centerEyeAnchor.position;
			float startTime = Time.time;
			float endTime = Time.time + 3f;
			while (Time.time < endTime)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, newPanelPosition, (Time.time - startTime) / 3f);
				yield return null;
			}
			base.transform.position = newPanelPosition;
			this._coroutine = null;
			yield break;
		}

		public PanelHMDFollower()
		{
		}

		private const float TOTAL_DURATION = 3f;

		private const float HMD_MOVEMENT_THRESHOLD = 0.3f;

		[SerializeField]
		private float _maxDistance = 0.3f;

		[SerializeField]
		private float _minDistance = 0.05f;

		[SerializeField]
		private float _minZDistance = 0.05f;

		private OVRCameraRig _cameraRig;

		private Vector3 _panelInitialPosition = Vector3.zero;

		private Coroutine _coroutine;

		private Vector3 _prevPos = Vector3.zero;

		private Vector3 _lastMovedToPos = Vector3.zero;

		[CompilerGenerated]
		private sealed class <LerpToHMD>d__13 : IEnumerator<object>, IEnumerator, IDisposable
		{
			[DebuggerHidden]
			public <LerpToHMD>d__13(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				PanelHMDFollower panelHMDFollower = this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
					newPanelPosition = panelHMDFollower.CalculateIdealAnchorPosition();
					panelHMDFollower._lastMovedToPos = panelHMDFollower._cameraRig.centerEyeAnchor.position;
					startTime = Time.time;
					endTime = Time.time + 3f;
				}
				if (Time.time >= endTime)
				{
					panelHMDFollower.transform.position = newPanelPosition;
					panelHMDFollower._coroutine = null;
					return false;
				}
				panelHMDFollower.transform.position = Vector3.Lerp(panelHMDFollower.transform.position, newPanelPosition, (Time.time - startTime) / 3f);
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			}

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			private int <>1__state;

			private object <>2__current;

			public PanelHMDFollower <>4__this;

			private Vector3 <newPanelPosition>5__2;

			private float <startTime>5__3;

			private float <endTime>5__4;
		}
	}
}
