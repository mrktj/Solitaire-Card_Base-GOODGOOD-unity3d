using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
	Dictionary<string, FacebookResponse> scores = new Dictionary<string, FacebookResponse>(), tempScores;

	protected override void Init()
	{
		StartCoroutine(InitialFacebookSync());
	}

	IEnumerator InitialFacebookSync()
	{
		while (!FacebookManager.Instance.IsInitialized) yield return new WaitForEndOfFrame();

		SyncWithFacebook();
	}

	public void SyncWithFacebook()
	{
		if (FacebookManager.Instance.IsInitialized && FacebookManager.Instance.IsLoggedIn)
		{
			if (DataManager.Instance.HasHighScore && (!scores.ContainsKey(FacebookManager.Instance.UserId) || System.Convert.ToInt32(scores[FacebookManager.Instance.UserId]["score"].Value) < DataManager.Instance.HighScore))
			{
				FacebookManager.Instance.CheckPermission("publish_actions", delegate(){
					FacebookManager.Instance.API("me/scores", Facebook.HttpMethod.POST, delegate(bool success, FacebookResponse response){
						SyncScores();
					},
					"score", DataManager.Instance.HighScore.ToString());
				});
			}
			else
			{
				SyncScores();
			}
		}
	}

	void SyncScores()
	{
		tempScores = new Dictionary<string, FacebookResponse>();
		FacebookManager.Instance.API(FacebookManager.Instance.AppId + "/scores", Facebook.HttpMethod.GET, ParseScores);
	}

	void ParseScores(bool success, FacebookResponse response)
	{
		if (success)
		{
			for (int i = 0, iMax = response.Count; i < iMax; i++)
			{
				tempScores.Add(response[i]["user"]["id"].Value, response);
			}

			FacebookManager.Instance.GetNextPageForResponse(response, ParseScores);
		}
		else
		{
			if (tempScores.Count > 0)
			{
				scores = tempScores;
			}
		}
	}

	public bool HasSyncedScores
	{
		get
		{
			return scores != null;
		}
	}
}
