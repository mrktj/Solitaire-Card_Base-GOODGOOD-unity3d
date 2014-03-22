using UnityEngine;

public class UIHelper
{
	public static void TogglePanel(GameObject panelObject, bool toggle)
	{
		UIPlayTween tween = panelObject.GetComponent<UIPlayTween>();
		if (tween != null)
		{
			tween.Play(toggle);
		}
	}
}
