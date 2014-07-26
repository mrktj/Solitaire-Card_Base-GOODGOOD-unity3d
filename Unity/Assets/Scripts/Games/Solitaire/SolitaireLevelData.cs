using UnityEngine;

[System.Serializable]
public class SolitaireLevelData
{
	[SerializeField] public int level = 1;
	[SerializeField] public int round = 1;
	[SerializeField] public int roundTime = 60;
	[SerializeField] public Board.Shape boardShape = Board.Shape.Peaks;
	[SerializeField] public int numPeaks = 1;
	[SerializeField] public int peakHeight = 2;
	[SerializeField] public int numColumns = 1;
	[SerializeField] public int columnHeight = 2;
	[SerializeField] public int numDecks = 1;
}
