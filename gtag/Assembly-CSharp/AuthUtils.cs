using System;
using System.Collections.Generic;
using Fusion.Photon.Realtime;
using Photon.Realtime;

public static class AuthUtils
{
	public static Fusion.Photon.Realtime.AuthenticationValues ToAuthValues_F(this Dictionary<string, string> authDict)
	{
		if (authDict == null)
		{
			return null;
		}
		string userId;
		bool flag = authDict.TryGetValue("deviceId", out userId);
		Fusion.Photon.Realtime.AuthenticationValues authenticationValues = null;
		if (flag)
		{
			authenticationValues = new Fusion.Photon.Realtime.AuthenticationValues(userId);
		}
		else
		{
			authenticationValues = new Fusion.Photon.Realtime.AuthenticationValues();
		}
		authenticationValues.AuthType = Fusion.Photon.Realtime.CustomAuthenticationType.Custom;
		string value;
		if (authDict.TryGetValue("username", out value))
		{
			authenticationValues.AddAuthParameter("username", value);
		}
		string value2;
		if (authDict.TryGetValue("token", out value2))
		{
			authenticationValues.AddAuthParameter("token", value2);
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<string, string> keyValuePair in authDict)
		{
			if (keyValuePair.Key != "username" && keyValuePair.Key != "token")
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
		authenticationValues.SetAuthPostData(dictionary);
		return authenticationValues;
	}

	public static Photon.Realtime.AuthenticationValues ToAuthValues_P(this Dictionary<string, string> authDict)
	{
		if (authDict == null)
		{
			return null;
		}
		string userId;
		bool flag = authDict.TryGetValue("deviceId", out userId);
		Photon.Realtime.AuthenticationValues authenticationValues = null;
		if (flag)
		{
			authenticationValues = new Photon.Realtime.AuthenticationValues(userId);
		}
		else
		{
			authenticationValues = new Photon.Realtime.AuthenticationValues();
		}
		authenticationValues.AuthType = Photon.Realtime.CustomAuthenticationType.Custom;
		string value;
		if (authDict.TryGetValue("username", out value))
		{
			authenticationValues.AddAuthParameter("username", value);
		}
		string value2;
		if (authDict.TryGetValue("token", out value2))
		{
			authenticationValues.AddAuthParameter("token", value2);
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<string, string> keyValuePair in authDict)
		{
			if (keyValuePair.Key != "username" && keyValuePair.Key != "token")
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
		authenticationValues.SetAuthPostData(dictionary);
		return authenticationValues;
	}
}
