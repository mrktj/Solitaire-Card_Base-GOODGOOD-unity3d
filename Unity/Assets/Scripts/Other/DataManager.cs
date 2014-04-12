using UnityEngine;
using System.Collections.Generic;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
	// Create keys and values for all data here.
	const string highScoreKey = "DATA_HIGH_SCORE";
	int highScoreValue;
	const string coinsKey = "DATA_COINS";
	int coinsValue;

	bool changesPending = false;

	protected override void Init()
	{
		// Initialize all data here.
		highScoreValue = PlayerPrefs.GetInt(highScoreKey);
		coinsValue = PlayerPrefs.GetInt(coinsKey);
	}

	void Update()
	{
		if (changesPending)
		{
			changesPending = false;

			// Save all data here.
			PlayerPrefs.SetInt(highScoreKey, highScoreValue);
			PlayerPrefs.SetInt(coinsKey, coinsValue);

			PlayerPrefs.Save();
		}
	}

	// Create accessors for all data here.

	public bool HasHighScore { get { return PlayerPrefs.HasKey(highScoreKey); } }

	public int HighScore
	{
		get { return highScoreValue; }
		set
		{
			highScoreValue = value;
			changesPending = true;
		}
	}

	public bool HasCoins { get { return PlayerPrefs.HasKey(coinsKey); }}

	public int Coins
	{
		get { return coinsValue; }
		set
		{
			coinsValue = value;
			changesPending = true;
		}
	}
}
