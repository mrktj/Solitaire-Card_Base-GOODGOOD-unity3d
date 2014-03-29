using UnityEngine;
using System.Collections;

public class ResultsPanel : MonoBehaviour
{
	[SerializeField] GameObject winScreen;
	[SerializeField] GameObject loseScreen;
	[SerializeField] TriPeaksGame game;
	
	public void Setup(bool won)
	{
		NGUITools.SetActive(winScreen, won);
		NGUITools.SetActive(loseScreen, !won);
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
