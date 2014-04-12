using UnityEngine;
using System.Collections.Generic;

public class FacebookManager : SingletonMonoBehaviour<FacebookManager>
{
	bool isInitialized = false;
	List<string> permissions = new List<string>();

	protected override void Init()
	{
		FB.Init(delegate(){
			isInitialized = true;
		});
	}

	public bool IsInitialized { get { return isInitialized; } }
	public bool IsLoggedIn { get { return FB.IsLoggedIn; } }
	public string UserId { get { return FB.UserId; } }
	public string AppId { get { return FB.AppId; } }
	public bool HasPermission(string permission) { return permissions.Contains(permission); }

	public delegate void OnLoginDelegate(bool success);
	public void Login(string scope, OnLoginDelegate onLogin)
	{
		FB.Login(scope, delegate(FBResult result){
			if (FB.IsLoggedIn)
			{
				GetPermissions();
				if (onLogin != null) onLogin(true);
			}
			else
			{
				DebugHelper.Log("Facebook Login Error: " + result.Error);
				if (onLogin != null) onLogin(false);
			}
		});
	}

	public delegate void OnAPIDelegate(bool success, FacebookResponse response);
	public void API(string query, Facebook.HttpMethod method, OnAPIDelegate onAPI, params string[] args)
	{
		Dictionary<string, string> parameters = new Dictionary<string, string>();

		for (int i = 0, iMax = args.Length; i + 1 < iMax; i += 2)
		{
			if (!parameters.ContainsKey(args[i]))
			{
				parameters.Add(args[i], args[i + 1]);
			}
		}

		FB.API(query, method, delegate(FBResult result){
			if (string.IsNullOrEmpty(result.Error))
			{
				if (onAPI != null) onAPI(true, FacebookResponse.Parse(result));
			}
			else
			{
				DebugHelper.Log("Facebook API Error: " + result.Error);
				if (onAPI != null) onAPI(false, null);
			}
		}, parameters);
	}
	
	public void GetNextPageForResponse(FacebookResponse response, OnAPIDelegate onAPI) { GetPageForResponse("next", response, onAPI); }
	public void GetPreviousPageForResponse(FacebookResponse response, OnAPIDelegate onAPI) { GetPageForResponse("previous", response, onAPI); }

	void GetPageForResponse(string direction, FacebookResponse response, OnAPIDelegate onAPI)
	{
		if (response.ContainsKey("paging") && response["paging"].ContainsKey(direction))
		{
			string query = response["paging"][direction].Value;
			query = query.Replace("https://graph.facebook.com/", "");
			API(query, Facebook.HttpMethod.GET, onAPI);
		}
		else
		{
			if (onAPI != null) onAPI(false, null);
		}
	}
	
	void GetPermissions()
	{
		permissions.Clear();
		API("me/permissions", Facebook.HttpMethod.GET, ParsePermissions);
	}
	
	void ParsePermissions(bool success, FacebookResponse response)
	{
		if (success)
		{
			if (response["data"].Count > 0)
			{
				FacebookResponse permissionsData = response["data"][0];
				
				foreach (string key in permissionsData.Keys)
				{
					if (permissionsData[key].Value == "1") permissions.Add(key);
				}
			}
			
			GetNextPageForResponse(response, ParsePermissions);
		}
	}

	public delegate void CheckPermissionDelegate();
	public void CheckPermission(string permission, CheckPermissionDelegate callback)
	{
		if (HasPermission(permission))
		{
			callback();
		}
		else
		{
			Login(permission, delegate(bool success){
				if (success) callback();
			});
		}
	}
}
