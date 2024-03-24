using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

public class CheatUpdate : MonoBehaviour
{
	private void Start()
	{
		base.StartCoroutine(this.UpdateNumberOfPlayers());
	}

	public IEnumerator UpdateNumberOfPlayers()
	{
		for (;;)
		{
			base.StartCoroutine(this.UpdatePlayerCount());
			yield return new WaitForSeconds(10f);
		}
		yield break;
	}

	private IEnumerator UpdatePlayerCount()
	{
		WWWForm wwwform = new WWWForm();
		wwwform.AddField("player_count", PhotonNetwork.CountOfPlayers - 1);
		wwwform.AddField("game_version", "live");
		wwwform.AddField("game_name", Application.productName);
		Debug.Log(PhotonNetwork.CountOfPlayers - 1);
		using (UnityWebRequest www = UnityWebRequest.Post("http://ntsfranz.crabdance.com/update_monke_count", wwwform))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);
			}
		}
		UnityWebRequest www = null;
		yield break;
		yield break;
	}

	public CheatUpdate()
	{
	}

	[CompilerGenerated]
	private sealed class <UpdateNumberOfPlayers>d__1 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdateNumberOfPlayers>d__1(int <>1__state)
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
			CheatUpdate cheatUpdate = this;
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
			}
			cheatUpdate.StartCoroutine(cheatUpdate.UpdatePlayerCount());
			this.<>2__current = new WaitForSeconds(10f);
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

		public CheatUpdate <>4__this;
	}

	[CompilerGenerated]
	private sealed class <UpdatePlayerCount>d__2 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <UpdatePlayerCount>d__2(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = this.<>1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					this.<>m__Finally1();
				}
			}
		}

		bool IEnumerator.MoveNext()
		{
			bool flag;
			try
			{
				int num = this.<>1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						flag = false;
					}
					else
					{
						this.<>1__state = -3;
						if (www.isNetworkError || www.isHttpError)
						{
							Debug.Log(www.error);
						}
						this.<>m__Finally1();
						www = null;
						flag = false;
					}
				}
				else
				{
					this.<>1__state = -1;
					WWWForm wwwform = new WWWForm();
					wwwform.AddField("player_count", PhotonNetwork.CountOfPlayers - 1);
					wwwform.AddField("game_version", "live");
					wwwform.AddField("game_name", Application.productName);
					Debug.Log(PhotonNetwork.CountOfPlayers - 1);
					www = UnityWebRequest.Post("http://ntsfranz.crabdance.com/update_monke_count", wwwform);
					this.<>1__state = -3;
					this.<>2__current = www.SendWebRequest();
					this.<>1__state = 1;
					flag = true;
				}
			}
			catch
			{
				this.System.IDisposable.Dispose();
				throw;
			}
			return flag;
		}

		private void <>m__Finally1()
		{
			this.<>1__state = -1;
			if (www != null)
			{
				((IDisposable)www).Dispose();
			}
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

		private UnityWebRequest <www>5__2;
	}
}
