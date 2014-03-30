using UnityEngine;

public static class DebugHelper
{
	public static void Log(object message)
	{
		if (Debug.isDebugBuild) Debug.Log(message);
	}
}
