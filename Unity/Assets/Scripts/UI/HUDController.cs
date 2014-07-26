using UnityEngine;

[ExecuteInEditMode]
public class HUDController : MonoBehaviour
{
	[SerializeField] SolitaireGamePlayer game;
	[SerializeField] UILabel roundLabel;
	[SerializeField] UILabel timeLabel;
	[SerializeField] UILabel scoreLabel;
	[SerializeField] UILabel coinsLabel;
	[SerializeField] UISprite livesSprite;
	[SerializeField] GameObject endGameButton;
	[SerializeField] ResultsPanel resultsPanel;

	void Start()
	{
		resultsPanel.game = game;

#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			game.OnEndOfGame += HandleOnEndOfGame;
		}
	}

	void OnDestroy()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			game.OnEndOfGame -= HandleOnEndOfGame;
		}
	}

	void Update()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			roundLabel.text = game.Round.ToString();
			timeLabel.text = game.RoundTime.ToString();
		}
		else
#endif
		{
			roundLabel.text = game.Round.ToString();
			timeLabel.text = Mathf.Floor(game.TimeRemaining).ToString();
			scoreLabel.text = game.Score.ToString();
			coinsLabel.text = DataManager.Instance.Coins.ToString();
			livesSprite.width = livesSprite.atlas.GetSprite(livesSprite.spriteName).width * game.Lives;
			NGUITools.SetActive(endGameButton, game.DeckSize == 0);
		}
	}

	void HandleOnEndOfGame(bool won, int baseScore, int deckScore, int timeScore, int totalScore, int coinsCollected, int deckCoins, int timeCoins, int totalCoins)
	{
		resultsPanel.Toggle(true);
		resultsPanel.Setup(won, baseScore, deckScore, timeScore, totalScore, coinsCollected, deckCoins, timeCoins, totalCoins);
	}
}
