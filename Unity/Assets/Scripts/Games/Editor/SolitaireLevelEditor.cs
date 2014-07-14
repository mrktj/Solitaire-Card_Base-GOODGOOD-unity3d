using UnityEngine;
using UnityEditor;

public class SolitaireLevelEditor : EditorWindow
{
	Board.Shape boardShape;
	int numPeaks, peakHeight;
	int numColumns, columnHeight;

	int numDecks;

	int roundTime;

	[MenuItem("Solitaire/Level Editor")]
	static void Initialize()
	{
		EditorWindow.GetWindow<SolitaireLevelEditor>("Level Editor", true);
	}

	void OnGUI()
	{
		SolitaireGame game = GameObject.FindObjectOfType<SolitaireGame>();

		if (game == null)
		{
			EditorGUILayout.LabelField("No Soltaire game found!");
			return;
		}

		Board.Shape boardShape = (Board.Shape)EditorGUILayout.EnumPopup("Board Shape", this.boardShape);
		if (boardShape != this.boardShape)
		{
			this.boardShape = boardShape;
			UpdateGame();
		}

		if (boardShape == Board.Shape.Peaks)
		{
			int numPeaks = EditorGUILayout.IntField("Number of Peaks", this.numPeaks);
			numPeaks = Mathf.Max(numPeaks, 1);
			if (numPeaks != this.numPeaks)
			{
				this.numPeaks = numPeaks;
				UpdateGame();
			}
			
			int peakHeight = EditorGUILayout.IntField("Peak Height", this.peakHeight);
			peakHeight = Mathf.Max(peakHeight, 1);
			if (peakHeight != this.peakHeight)
			{
				this.peakHeight = peakHeight;
				UpdateGame();
			}
		}
		else if (boardShape == Board.Shape.Columns)
		{
			int numColumns = EditorGUILayout.IntField("Number of Columns", this.numColumns);
			numColumns = Mathf.Max(numColumns, 1);
			if (numColumns != this.numColumns)
			{
				this.numColumns = numColumns;
				UpdateGame();
			}
			
			int columnHeight = EditorGUILayout.IntField("Column Height", this.columnHeight);
			columnHeight = Mathf.Max(columnHeight, 1);
			if (columnHeight != this.columnHeight)
			{
				this.columnHeight = columnHeight;
				UpdateGame();
			}
		}

		int numDecks = EditorGUILayout.IntField("Number of Decks", this.numDecks);
		numDecks = Mathf.Max(numDecks, 1);
		if (numDecks != this.numDecks)
		{
			this.numDecks = numDecks;
			UpdateGame();
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Slots = " + game.NumSlots.ToString());
		EditorGUILayout.LabelField("Cards = " + (numDecks * System.Enum.GetNames(typeof(CardRank)).Length * System.Enum.GetNames(typeof(CardSuit)).Length).ToString());
		EditorGUILayout.EndHorizontal();

		int roundTime = EditorGUILayout.IntField("Round Time (seconds)", this.roundTime);
		roundTime = Mathf.Max(roundTime, 1);
		if (roundTime != this.roundTime)
		{
			this.roundTime = roundTime;
			UpdateGame();
		}
	}

	void UpdateGame()
	{
		SolitaireGame game = GameObject.FindObjectOfType<SolitaireGame>();
		game.LoadGame(boardShape, numPeaks, peakHeight, numColumns, columnHeight, numDecks, roundTime);
	}
}
