using System;
using UnityEngine;

namespace GorillaNetworking
{
	public class SafeAccountObjectSwapper : MonoBehaviour
	{
		public void Start()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.SwitchToSafeMode();
			}
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.SafeAccountUpdated));
		}

		public void SafeAccountUpdated(bool isSafety)
		{
			if (isSafety)
			{
				this.SwitchToSafeMode();
			}
		}

		public void SwitchToSafeMode()
		{
			GameObject[] array = this.UnSafeGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.UnSafeTexts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.SafeTexts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			array = this.SafeModeObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
		}

		public SafeAccountObjectSwapper()
		{
		}

		public GameObject[] UnSafeGameObjects;

		public GameObject[] UnSafeTexts;

		public GameObject[] SafeTexts;

		public GameObject[] SafeModeObjects;
	}
}
