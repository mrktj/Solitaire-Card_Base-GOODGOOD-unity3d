using UnityEngine;
using System.Collections;

public class ResultsPanel : MonoBehaviour
{
	public TriPeaksGame game;

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
