using System;

namespace BuildSafe
{
	public class SessionState
	{
		public string this[string key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public SessionState()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SessionState()
		{
		}

		public static readonly SessionState Shared = new SessionState();
	}
}
