using UnityEngine;
using UnityEditor;

public class SolitaireLevelEditor : EditorWindow
{
	SolitaireLevelList mLevelList = null;
	SolitaireLevelData mCurrentLevelData = null;

	int confirmDelete = -1;

	Vector2 fullScrollPosition = Vector2.zero;
	Vector2 levelSelectionScrollPosition = Vector2.zero;

	[MenuItem("Solitaire/Level Editor")]
	static void Initialize()
	{
		EditorWindow.GetWindow<SolitaireLevelEditor>("Level Editor", true);
	}

	SolitaireLevelList LevelList
	{
		get
		{
			if (mLevelList == null) mLevelList = (Resources.Load("SolitaireLevelList") as GameObject).GetComponent<SolitaireLevelList>();
			return mLevelList;
		}
	}

	SolitaireLevelData CurrentLevel
	{
		get
		{
			return mCurrentLevelData;
		}
		set
		{
			mCurrentLevelData = value;
			UpdateGame();
		}
	}

	void OnGUI()
	{
		SolitaireLevelPlayer game = GameObject.FindObjectOfType<SolitaireLevelPlayer>();

		if (game == null)
		{
			EditorGUILayout.LabelField("No Soltaire Level Player found! Are you in the correct scene?");
			return;
		}

		fullScrollPosition = EditorGUILayout.BeginScrollView(fullScrollPosition);
		EditorGUILayout.BeginVertical();

		#region Level Selection List

		NGUIEditorTools.DrawHeader("Level Selection List");
		NGUIEditorTools.BeginContents(false);
		levelSelectionScrollPosition = EditorGUILayout.BeginScrollView(levelSelectionScrollPosition, GUILayout.Height(200f));

		int runningLevel = 0;

		for (int i = 0; i < LevelList.master.Length; i++)
		{
			SolitaireLevelData levelData = LevelList.master[i];

			if (runningLevel != levelData.level)
			{
				if (runningLevel != 0) NGUIEditorTools.EndContents();

				runningLevel = levelData.level;

				NGUIEditorTools.BeginContents();
				EditorGUILayout.BeginHorizontal();

				GUIStyle levelLabel = new GUIStyle(EditorStyles.boldLabel);
				if (CurrentLevel.level == levelData.level) levelLabel.normal.textColor = Color.green;
				EditorGUILayout.LabelField("Level " + levelData.level.ToString(), levelLabel, GUILayout.Width(80));

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(90);

			GUIStyle roundLabel = new GUIStyle(EditorStyles.label);
			if (CurrentLevel.level == levelData.level && CurrentLevel.round == levelData.round) roundLabel.normal.textColor = Color.green;
			EditorGUILayout.LabelField("Round " + levelData.round.ToString(), roundLabel, GUILayout.Width(80));

			GUILayout.Space(10);

			if (CurrentLevel != levelData)
			{
				if (GUILayout.Button("Load", GUILayout.Width(80)))
				{
					CurrentLevel = levelData;
				}
			}
			else
			{
				GUIStyle loadedLabel = new GUIStyle(EditorStyles.label);
				loadedLabel.alignment = TextAnchor.MiddleCenter;
				EditorGUILayout.LabelField("Loaded", loadedLabel, GUILayout.Width(80));
			}

			if (i != confirmDelete)
			{
				if (GUILayout.Button("Delete", GUILayout.Width(80)))
				{
					confirmDelete = i;
				}
			}
			else if (i == confirmDelete)
			{
				if (GUILayout.Button("Confirm", GUILayout.Width(80)))
				{
					LevelList.DeleteLevelDataAt(i, ref LevelList.master);

					confirmDelete = -1;
				}
			}

			EditorGUILayout.EndHorizontal();
		}

		if (LevelList.master.Length > 0) NGUIEditorTools.EndContents();

		EditorGUILayout.EndScrollView();
		
		NGUIEditorTools.BeginContents();
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Create New Level"))
		{
			SolitaireLevelData newLevel = new SolitaireLevelData();
			newLevel.level = LevelList.master.Length == 0 ? 1 : LevelList.master[LevelList.master.Length - 1].level + 1;
			CurrentLevel = newLevel;
			LevelList.AddLevelData(CurrentLevel, ref LevelList.master);
		}

		if (CurrentLevel != null)
		{
			if (GUILayout.Button("Add New Round"))
			{
				SolitaireLevelData newRound = new SolitaireLevelData();
				newRound.level = CurrentLevel.level;
				newRound.round = CurrentLevel.round + 1;
				CurrentLevel = newRound;
				LevelList.AddLevelData(CurrentLevel, ref LevelList.master);
			}
		}
		
		EditorGUILayout.EndHorizontal();
		NGUIEditorTools.EndContents();

		NGUIEditorTools.EndContents();

		#endregion

		if (CurrentLevel == null)
		{
			EditorGUILayout.EndVertical();
			return;
		}

		#region Level Properties

		NGUIEditorTools.DrawHeader("Level Properties");
		NGUIEditorTools.BeginContents(false);

		int level = EditorGUILayout.IntField("Level", CurrentLevel.level);
		if (level != CurrentLevel.level)
		{
			CurrentLevel.level = level;
			LevelList.Sort(ref LevelList.master);
			UpdateGame();
		}

		int round = EditorGUILayout.IntField("Round", CurrentLevel.round);
		if (round != CurrentLevel.round)
		{
			CurrentLevel.round = round;
			LevelList.Sort(ref LevelList.master);
			UpdateGame();
		}

		Board.Shape boardShape = (Board.Shape)EditorGUILayout.EnumPopup("Board Shape", CurrentLevel.boardShape);
		if (boardShape != CurrentLevel.boardShape)
		{
			CurrentLevel.boardShape = boardShape;
			UpdateGame();
		}

		if (boardShape == Board.Shape.Peaks)
		{
			int numPeaks = EditorGUILayout.IntField("Number of Peaks", CurrentLevel.numPeaks);
			numPeaks = Mathf.Max(numPeaks, 1);
			if (numPeaks != CurrentLevel.numPeaks)
			{
				CurrentLevel.numPeaks = numPeaks;
				UpdateGame();
			}
			
			int peakHeight = EditorGUILayout.IntField("Peak Height", CurrentLevel.peakHeight);
			peakHeight = Mathf.Max(peakHeight, 1);
			if (peakHeight != CurrentLevel.peakHeight)
			{
				CurrentLevel.peakHeight = peakHeight;
				UpdateGame();
			}
		}
		else if (boardShape == Board.Shape.Columns)
		{
			int numColumns = EditorGUILayout.IntField("Number of Columns", CurrentLevel.numColumns);
			numColumns = Mathf.Max(numColumns, 1);
			if (numColumns != CurrentLevel.numColumns)
			{
				CurrentLevel.numColumns = numColumns;
				UpdateGame();
			}
			
			int columnHeight = EditorGUILayout.IntField("Column Height", CurrentLevel.columnHeight);
			columnHeight = Mathf.Max(columnHeight, 1);
			if (columnHeight != CurrentLevel.columnHeight)
			{
				CurrentLevel.columnHeight = columnHeight;
				UpdateGame();
			}
		}

		int numDecks = EditorGUILayout.IntField("Number of Decks", CurrentLevel.numDecks);
		numDecks = Mathf.Max(numDecks, 1);
		if (numDecks != CurrentLevel.numDecks)
		{
			CurrentLevel.numDecks = numDecks;
			UpdateGame();
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Slots = " + game.NumSlots.ToString());
		EditorGUILayout.LabelField("Cards = " + (numDecks * System.Enum.GetNames(typeof(CardRank)).Length * System.Enum.GetNames(typeof(CardSuit)).Length).ToString());
		EditorGUILayout.EndHorizontal();

		int roundTime = EditorGUILayout.IntField("Round Time (seconds)", CurrentLevel.roundTime);
		roundTime = Mathf.Max(roundTime, 1);
		if (roundTime != CurrentLevel.roundTime)
		{
			CurrentLevel.roundTime = roundTime;
			UpdateGame();
		}

		NGUIEditorTools.EndContents();

		#endregion

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}

	void UpdateGame()
	{
		SolitaireLevelPlayer game = GameObject.FindObjectOfType<SolitaireLevelPlayer>();
		game.LoadGame(CurrentLevel);
	}
}
