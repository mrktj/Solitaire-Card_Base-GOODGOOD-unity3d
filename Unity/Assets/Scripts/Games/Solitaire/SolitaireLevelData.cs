using UnityEngine;

public class SolitaireLevelData
{
	[SerializeField] int level;
	[SerializeField] int round;
	[SerializeField] int roundTime;
	[SerializeField] Board.Shape boardShape;
	[SerializeField] int numPeaks;
	[SerializeField] int peakHeight;
	[SerializeField] int numColumns;
	[SerializeField] int columnHeight;
	[SerializeField] int numDecks;
}
