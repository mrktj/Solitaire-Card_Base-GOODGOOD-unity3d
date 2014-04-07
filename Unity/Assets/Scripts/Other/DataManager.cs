using UnityEngine;
using System.Collections.Generic;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
	// Create keys and values for all data here.
	const string highScoreKey = "DATA_HIGH_SCORE";
	int highScoreValue;

	bool changesPending = false;

	protected override void Init()
	{
		// Initialize all data here.
		highScoreValue = PlayerPrefs.GetInt(highScoreKey);
	}

	void Update()
	{
		if (changesPending)
		{
			changesPending = false;

			// Save all data here.
			PlayerPrefs.SetInt(highScoreKey, highScoreValue);

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
}
