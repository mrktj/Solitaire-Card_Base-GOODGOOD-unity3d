using UnityEngine;
using System.Collections;

public class ResultsPanel : MonoBehaviour
{
	[System.NonSerialized] public SolitaireLevelPlayer game;

	[SerializeField] GameObject winScreen;
	[SerializeField] UILabel winRoundScoreLabel;
	[SerializeField] UILabel winCardRemainingScoreLabel;
	[SerializeField] UILabel winTimeRemainingScoreLabel;
	[SerializeField] UILabel winTotalScoreLabel;
	[SerializeField] UILabel winCoinsCollectedLabel;
	[SerializeField] GameObject loseScreen;
	[SerializeField] UILabel loseTotalScoreLabel;
	
	public void Setup(bool won, int baseScore, int deckScore, int timeScore, int totalScore, int coinsCollected, int deckCoins, int timeCoins, int totalCoins)
	{
		NGUITools.SetActive(winScreen, won);
		NGUITools.SetActive(loseScreen, !won);

		winRoundScoreLabel.text = baseScore.ToString();
		winCardRemainingScoreLabel.text = deckScore.ToString();
		winTimeRemainingScoreLabel.text = timeScore.ToString();
		winTotalScoreLabel.text = totalScore.ToString();
		winCoinsCollectedLabel.text = totalCoins.ToString();

		loseTotalScoreLabel.text = totalScore.ToString();
	}

	public void Toggle(bool toggle)
	{
		UIHelper.TogglePanel(gameObject, toggle);
	}

	public void CloseClicked()
	{
		game.RestartGame();
		Toggle(false);
	}

	public void ContinueClicked()
	{
		game.RestartGame();
		Toggle(false);
	}
}
