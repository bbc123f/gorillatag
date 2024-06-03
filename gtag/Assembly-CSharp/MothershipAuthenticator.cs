using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

public class MothershipAuthenticator : MonoBehaviour
{
	public GorillaComputer gorillaComputer
	{
		get
		{
			return GorillaComputer.instance;
		}
	}

	public void Awake()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = this;
		}
		else if (MothershipAuthenticator.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (MothershipAuthenticator.Instance.MetaAuthenticator == null)
		{
			MothershipAuthenticator.Instance.MetaAuthenticator = base.gameObject.GetOrAddComponent<MetaAuthenticator>();
		}
		if (!MothershipClientApiUnity.IsEnabled())
		{
			Debug.Log("Mothership is not enabled.");
			return;
		}
		MothershipClientApiUnity.SetAuthRefreshedCallback(delegate(string id)
		{
			this.BeginLoginFlow();
		});
		this.BeginLoginFlow();
	}

	private void BeginLoginFlow()
	{
		Debug.Log("making login call");
		this.LogInWithSteam();
	}

	private void LoginWithInsecure()
	{
		MothershipClientApiUnity.LoginWithInsecure1(this.TestNickname, this.TestAccountId, delegate(LoginResponse LoginResponse)
		{
			Debug.Log(string.Format("Logged in with Mothership Id {0}", LoginResponse.MothershipPlayerId));
		}, delegate(MothershipError MothershipError)
		{
			Debug.Log(string.Format("Failed to log in, error {0}", MothershipError.Details));
		});
	}

	private void Update()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			MothershipClientApiUnity.Tick(Time.deltaTime);
		}
	}

	private void LogInWithSteam()
	{
	}

	public MothershipAuthenticator()
	{
	}

	[CompilerGenerated]
	private void <Awake>b__7_0(string id)
	{
		this.BeginLoginFlow();
	}

	public static volatile MothershipAuthenticator Instance;

	public MetaAuthenticator MetaAuthenticator;

	public string TestNickname = "Foo";

	public string TestAccountId = "Bar";

	private string loggedInUserId;

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal void <LoginWithInsecure>b__9_0(LoginResponse LoginResponse)
		{
			Debug.Log(string.Format("Logged in with Mothership Id {0}", LoginResponse.MothershipPlayerId));
		}

		internal void <LoginWithInsecure>b__9_1(MothershipError MothershipError)
		{
			Debug.Log(string.Format("Failed to log in, error {0}", MothershipError.Details));
		}

		public static readonly MothershipAuthenticator.<>c <>9 = new MothershipAuthenticator.<>c();

		public static Action<LoginResponse> <>9__9_0;

		public static Action<MothershipError> <>9__9_1;
	}
}
