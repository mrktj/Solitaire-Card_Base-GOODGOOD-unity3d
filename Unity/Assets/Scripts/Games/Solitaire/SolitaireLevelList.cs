using UnityEngine;

public class SolitaireLevelList : MonoBehaviour
{
	static SolitaireLevelList mData = null;

	public static SolitaireLevelList Data
	{
		get
		{
			if (mData == null) mData = (Resources.Load("SolitaireLevelList") as GameObject).GetComponent<SolitaireLevelList>();
			return mData;
		}
	}

	public SolitaireLevelData[] master;

	public void AddLevelData(SolitaireLevelData levelData, ref SolitaireLevelData[] levelList)
	{
		System.Array.Resize<SolitaireLevelData>(ref levelList, levelList.Length + 1);
		levelList[levelList.Length - 1] = levelData;
		Sort(ref levelList);
	}

	public void DeleteLevelDataAt(int index, ref SolitaireLevelData[] levelList)
	{
		for (int j = index + 1; j < levelList.Length; j++)
		{
			levelList[j - 1] = levelList[j];
		}

		System.Array.Resize<SolitaireLevelData>(ref levelList, levelList.Length - 1);
	}

	public void Sort(ref SolitaireLevelData[] levelList)
	{
		System.Array.Sort<SolitaireLevelData>(levelList, delegate(SolitaireLevelData x, SolitaireLevelData y){
			if (x.level == y.level) return x.round.CompareTo(y.round);
			else return x.level.CompareTo(y.level);
		});
	}
}
