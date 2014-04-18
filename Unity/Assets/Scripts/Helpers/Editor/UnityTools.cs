using UnityEngine;
using UnityEditor;

public class UnityTools
{
	#region Clear PlayerPrefs
	
	[MenuItem("Tools/Clear PlayerPrefs")]
	private static void ClearPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("Cleared PlayerPrefs.");
	}
	
	#endregion
}
