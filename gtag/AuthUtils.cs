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
		string text;
		bool flag = authDict.TryGetValue("deviceId", out text);
		Fusion.Photon.Realtime.AuthenticationValues authenticationValues = null;
		if (flag)
		{
			authenticationValues = new Fusion.Photon.Realtime.AuthenticationValues(text);
		}
		else
		{
			authenticationValues = new Fusion.Photon.Realtime.AuthenticationValues();
		}
		authenticationValues.AuthType = Fusion.Photon.Realtime.CustomAuthenticationType.Custom;
		string text2;
		if (authDict.TryGetValue("username", out text2))
		{
			authenticationValues.AddAuthParameter("username", text2);
		}
		string text3;
		if (authDict.TryGetValue("token", out text3))
		{
			authenticationValues.AddAuthParameter("token", text3);
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
		string text;
		bool flag = authDict.TryGetValue("deviceId", out text);
		Photon.Realtime.AuthenticationValues authenticationValues = null;
		if (flag)
		{
			authenticationValues = new Photon.Realtime.AuthenticationValues(text);
		}
		else
		{
			authenticationValues = new Photon.Realtime.AuthenticationValues();
		}
		authenticationValues.AuthType = Photon.Realtime.CustomAuthenticationType.Custom;
		string text2;
		if (authDict.TryGetValue("username", out text2))
		{
			authenticationValues.AddAuthParameter("username", text2);
		}
		string text3;
		if (authDict.TryGetValue("token", out text3))
		{
			authenticationValues.AddAuthParameter("token", text3);
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
