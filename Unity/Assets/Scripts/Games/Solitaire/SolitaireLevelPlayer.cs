using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SolitaireLevelPlayer : MonoBehaviour
{
	#region Event Handlers

	public delegate void EndOfGameDelegate(bool won, int baseScore, int deckScore, int timeScore, int totalScore, int coinsCollected, int deckCoins, int timeCoins, int totalCoins);
	public event EndOfGameDelegate OnEndOfGame;

	#endregion

#if UNITY_EDITOR
	public SolitaireLevelData editorLevel = null;
#endif

	Board.Shape boardShape;
	int numPeaks = 1, peakHeight = 1;
	int numColumns = 1, columnHeight = 1;

#if UNITY_EDITOR
	public bool arrangeBoard;
#endif

	int numDecks = 1;

#if UNITY_EDITOR
	public bool generateDeck;
#endif

	[SerializeField] int pointsPerCardTakenFromBoard;
	[SerializeField] int pointsPerCardRemainingInDeck;
	[SerializeField] int pointsPerSecondsRemaining;
	[SerializeField] int coinsPerCardTakenFromBoard;
	[SerializeField] int coinsPerCardRemainingInDeck;
	[SerializeField] int coinsPerSecondsRemaining;

	[SerializeField] int cardsForExtraLife;

	public bool paused = false;
	public bool endGame = false;

#if UNITY_EDITOR
	public bool restartGame = false;
#endif
	
	[SerializeField] int costForUndoLastMove;

#if UNITY_EDITOR
	public bool undoLastMove = false;
#endif
	
	[SerializeField] int costForShuffleBoard;

#if UNITY_EDITOR
	public bool shuffleBoard = false;
#endif
	
	[SerializeField] int costForGenerateWildCard;

#if UNITY_EDITOR
	public bool generateWildCard = false;
#endif
	
	[SerializeField] int extraCardsPowerup;
	[SerializeField] int costForAddExtraCards;

#if UNITY_EDITOR
	public bool addExtraCards = false;
#endif
	
	[SerializeField] float extraTimePowerup;
	[SerializeField] int costForAddExtraTime;

#if UNITY_EDITOR
	public bool addExtraTime = false;
#endif

	UIRoot root;
	new Camera camera;
	UI2DSprite background;
	Deck deck;
	Board board;
	Waste waste;

	int score = 0;
	int round = 1;
	float roundTime = 60;
	int lives = 0;
	float timeRemaining = 0;
	int coinsCollected = 0;
	bool successfulRound = false;

	class Action
	{
		public enum Move
		{
			RevealNextCard,
			RemoveCardFromSlot
		}

		public Move move;
		public Slot slot;
	}

	BetterList<Action> undoHistory = new BetterList<Action>();

	const float ANIMATION_STAGGER_DURATION = 0.1f;

	void Awake()
	{
		root = GetComponent<UIRoot>();
		camera = GetComponentInChildren<Camera>();
		background = GetComponentInChildren<UI2DSprite>();
		deck = GetComponentInChildren<Deck>();
		board = GetComponentInChildren<Board>();
		waste = GetComponentInChildren<Waste>();
	}

	void Start()
	{
#if UNITY_EDITOR
		if (editorLevel != null)
		{
			LoadGame(editorLevel);
		}
		else
#endif
		{
			
		}
	}
	
	void Update()
	{
#if UNITY_EDITOR
		if (Application.isPlaying && !paused)
#else
		if (!paused)
#endif
		{
			UpdateGame();

			if (endGame)
			{
				endGame = false;
				EndGame();
			}
		}

#if UNITY_EDITOR
		if (arrangeBoard)
		{
			arrangeBoard = false;
			ArrangeBoard();
		}

		if (generateDeck)
		{
			generateDeck = false;
			GenerateDeck();
		}
		
		if (restartGame)
		{
			restartGame = false;
			RestartGame();
		}

		if (undoLastMove)
		{
			undoLastMove = false;
			UndoLastMove();
		}

		if (shuffleBoard)
		{
			shuffleBoard = false;
			ShuffleBoard();
		}

		if (generateWildCard)
		{
			generateWildCard = false;
			GenerateWildCard();
		}

		if (addExtraCards)
		{
			addExtraCards = false;
			AddExtraCards();
		}

		if (addExtraTime)
		{
			addExtraTime = false;
			AddExtraTime();
		}
#endif
	}

	public void LoadGame(SolitaireLevelData levelData)
	{
		boardShape = levelData.boardShape;
		numPeaks = levelData.numPeaks;
		peakHeight = levelData.peakHeight;
		numColumns = levelData.numColumns;
		columnHeight = levelData.columnHeight;
		numDecks = levelData.numDecks;
		round = levelData.round;
		roundTime = levelData.roundTime;

#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			timeRemaining = roundTime;
		}

		GenerateDeck();
		ArrangeBoard();

#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			DealBoard();
		}
	}

#if UNITY_EDITOR
	public int NumSlots
	{
		get
		{
			return board.Size;
		}
	}
#endif

	void UpdateGame()
	{
		timeRemaining -= Time.deltaTime;

		bool boardCleared = true;
		
		for (int i = 0, iMax = board.Size; i < iMax; i++)
		{
			if (board[i].Card != null)
			{
				boardCleared = false;
			}
		}

		if (boardCleared)
		{
			successfulRound = true;
			endGame = true;
		}
		
		if (Mathf.Floor(timeRemaining) <= 0)
		{
			successfulRound = false;
			endGame = true;
		}
	}

	void DealBoard()
	{
		StartCoroutine(AnimateDealBoard());
	}

	IEnumerator AnimateDealBoard()
	{
		paused = true;

		deck.Shuffle();
		
		for (int i = 0, iMax = board.Size; i < iMax && deck.Size > 0; i++)
		{
			DealCardToSlot(board[i]);
			yield return new WaitForSeconds(ANIMATION_STAGGER_DURATION);
		}
		
		RefreshBoard();
		RevealNextCard();

		paused = false;
	}

	void RefreshBoard()
	{
		// Determine which slots should reveal their cards.
		for (int i = 0, iMax = board.Size; i < iMax; i++)
		{
			if (board[i].Card != null)
			{
				bool reveal = true;
				
				foreach (Slot dependentSlot in board[i].OverlappingSlots)
				{
					if (dependentSlot.Card != null)
					{
						reveal = false;
						break;
					}
				}
				
				board[i].Card.Revealed = reveal;
			}
		}
	}

	void GenerateDeck()
	{
		deck.GenerateDeck(numDecks);
	}

	void ArrangeBoard()
	{
		if (boardShape == Board.Shape.Peaks)
		{
			board.ArrangeSlotsAsPeaks(numPeaks, peakHeight);
		}
		else if (boardShape == Board.Shape.Columns)
		{
			board.ArrangeSlotsAsColumns(numColumns, columnHeight);
		}

		Vector2 screenBounds = new Vector2((float)root.activeHeight / (float)Screen.height * (float)Screen.width, root.activeHeight) * 0.5f;
		float xZoom = board.Bounds.x / screenBounds.x, yZoom = board.Bounds.y / screenBounds.y;
		camera.orthographicSize = Mathf.Max(Mathf.Max(xZoom, yZoom), 1f);
		background.transform.parent.localScale = new Vector3(camera.orthographicSize, camera.orthographicSize, 1f);
	}
	
	public bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
		}
	}

	public int Round
	{
		get
		{
			return round;
		}
	}
	
	public float RoundTime
	{
		get
		{
			return roundTime;
		}
	}

	public float TimeRemaining
	{
		get
		{
			return timeRemaining;
		}
	}

	public int Score
	{
		get
		{
			return score;
		}
	}

	public int Lives
	{
		get
		{
			return lives;
		}
	}

	public int DeckSize
	{
		get
		{
			return deck.Size;
		}
	}

	void EndGame()
	{
		paused = true;

		if (successfulRound && deck.Size >= cardsForExtraLife) lives++;

		int deckScore = successfulRound ? deck.Size * pointsPerCardRemainingInDeck : 0;
		int timeScore = successfulRound ? Mathf.FloorToInt(timeRemaining) * pointsPerSecondsRemaining : 0;
		int totalScore = score + deckScore + timeScore;
		int deckCoins = successfulRound ? deck.Size * coinsPerCardRemainingInDeck : 0;
		int timeCoins = successfulRound ? Mathf.FloorToInt(timeRemaining) * coinsPerSecondsRemaining : 0;
		int totalCoins = coinsCollected + deckCoins + timeCoins;

		if (OnEndOfGame != null) OnEndOfGame(successfulRound, score, deckScore, timeScore, totalScore, coinsCollected, deckCoins, timeCoins, totalCoins);

		score = totalScore;
		coinsCollected = 0;

		if (score > DataManager.Instance.HighScore)
		{
			DataManager.Instance.HighScore = score;
		}
	}
	
	public void RestartGame()
	{
		for (int i = 0, iMax = board.Size; i < iMax; i++)
		{
			if (board[i].Card != null)
			{
				if (board[i].Card.IsGeneratedCard)
				{
					Card card = board[i].TakeCard();
					GameObject.Destroy(card.gameObject);
				}
				else
				{
					ReturnCardFromSlot(board[i]);
				}
			}
		}
		
		while (waste.Size > 0)
		{
			if (waste[0].IsGeneratedCard)
			{
				Card card = waste.TakeTopCard();
				GameObject.Destroy(card.gameObject);
			}
			else
			{
				ReturnCardFromWaste();
			}
		}

		if (successfulRound)
		{
			round++;
		}
		else if (lives > 0)
		{
			lives--;
		}
		else
		{
			score = 0;
			round = 1;
		}

		undoHistory.Clear();
		paused = false;
		successfulRound = false;

		timeRemaining = roundTime;
		GenerateDeck();
		ArrangeBoard();
		DealBoard();
	}
	
	void AddToUndoHistory(Action.Move move, Slot slot = null)
	{
		Action action = new Action();
		action.move = move;
		action.slot = slot;
		undoHistory.Add(action);
	}

	public bool CanUndo
	{
		get
		{
			if (paused) return false;

			if (undoHistory.size == 0) return false;

			return true;
		}
	}

	public void UndoLastMove()
	{
		if (!CanUndo) return;

		if (DataManager.Instance.Coins < costForUndoLastMove)
		{
			// TODO: Add method to purchase coins.
			return;
		}

		DataManager.Instance.Coins -= costForUndoLastMove;

		if (waste[0].Type == CardType.Wild)
		{
			// TODO: Warn player that they'll lose their Wild card.
			GameObject.Destroy(waste.TakeTopCard().gameObject);
		}

		Action action = undoHistory[undoHistory.size - 1];
		undoHistory.RemoveAt(undoHistory.size - 1);

		if (action.move == Action.Move.RevealNextCard)
		{
			if (waste.Size > 0)
			{
				ReturnCardFromWaste();
			}
		}
		else if (action.move == Action.Move.RemoveCardFromSlot)
		{
			if (waste.Size > 0)
			{
				ReturnCardToSlot(action.slot);
				RefreshBoard();
			}
		}
	}

	public void ShuffleBoard()
	{
		if (paused) return;
		
		if (DataManager.Instance.Coins < costForShuffleBoard)
		{
			// TODO: Add method to purchase coins.
			return;
		}
		
		DataManager.Instance.Coins -= costForShuffleBoard;

		StartCoroutine(AnimateShuffleBoard());
	}

	IEnumerator AnimateShuffleBoard()
	{
		paused = true;

		BetterList<Slot> untouchedSlots = new BetterList<Slot>();
		
		for (int i = board.Size - 1; i >= 0; i--)
		{
			if (board[i].Card != null)
			{
				untouchedSlots.Add(board[i]);
				ReturnCardFromSlot(board[i]);
				yield return new WaitForSeconds(ANIMATION_STAGGER_DURATION);
			}
		}
		
		deck.Shuffle();
		
		for (int i = untouchedSlots.size - 1; i >= 0; i--)
		{
			DealCardToSlot(untouchedSlots[i]);
			yield return new WaitForSeconds(ANIMATION_STAGGER_DURATION);
		}
		
		RefreshBoard();

		paused = false;
	}

	public void GenerateWildCard()
	{
		if (paused) return;
		
		if (DataManager.Instance.Coins < costForGenerateWildCard)
		{
			// TODO: Add method to purchase coins.
			return;
		}
		
		DataManager.Instance.Coins -= costForGenerateWildCard;

		Card card = deck.CreateNewCard();
		card.Type = CardType.Wild;
		waste.AddCard(card);
		card.Revealed = true;
		card.IsGeneratedCard = true;
	}

	public void AddExtraCards()
	{
		if (paused) return;
		
		if (DataManager.Instance.Coins < costForAddExtraCards)
		{
			// TODO: Add method to purchase coins.
			return;
		}
		
		DataManager.Instance.Coins -= costForAddExtraCards;

		for (int i = 0; i < extraCardsPowerup; i++)
		{
			Card card = deck.CreateNewCard();
			card.Rank = (CardRank)Random.Range(0, System.Enum.GetNames(typeof(CardRank)).Length);
			card.Suit = (CardSuit)Random.Range(0, System.Enum.GetNames(typeof(CardSuit)).Length);
			deck.AddCardOnTop(card);
			card.IsGeneratedCard = true;
		}
	}

	public void AddExtraTime()
	{
		if (paused) return;
		
		if (DataManager.Instance.Coins < costForAddExtraTime)
		{
			// TODO: Add method to purchase coins.
			return;
		}
		
		DataManager.Instance.Coins -= costForAddExtraTime;

		timeRemaining += extraTimePowerup;
	}

	/// <summary>
	/// Moves card from deck to slot.
	/// </summary>
	void DealCardToSlot(Slot slot)
	{
		Card card = deck.DealCard();
		slot.PlaceCard(card);
	}

	/// <summary>
	/// Moves card from deck to waste.
	/// </summary>
	void RevealNextCard()
	{
		Card card = deck.DealCard();
		waste.AddCard(card);
		card.Revealed = true;
	}

	/// <summary>
	/// Moves card from slot to waste.
	/// </summary>
	void RemoveCardFromSlot(Slot slot)
	{
		Card card = slot.TakeCard();
		waste.AddCard(card);
	}

	/// <summary>
	/// Moves card from slot to deck.
	/// </summary>
	void ReturnCardFromSlot(Slot slot)
	{
		Card card = slot.TakeCard();
		deck.AddCardOnTop(card);
		card.Revealed = false;
	}

	/// <summary>
	/// Moves card from waste to deck.
	/// </summary>
	void ReturnCardFromWaste()
	{
		Card card = waste.TakeTopCard();
		deck.AddCardOnTop(card);
		card.Revealed = false;
	}

	/// <summary>
	/// Moves card from waste to slot.
	/// </summary>
	void ReturnCardToSlot(Slot slot)
	{
		Card card = waste.TakeTopCard();
		slot.PlaceCard(card);
	}

	void OnRecognizeTap(TapGesture gesture)
	{
		// Game is paused.
		if (paused) return;

		// Didn't tap on anything.
		if (gesture.Selection == null) return;

		// Didn't tap on a card.
		Card card = gesture.Selection.GetComponent<Card>();
		if (card == null) return;

		// Tapped on a deck card.
		if (card.transform.parent.GetComponent<Deck>() == deck)
		{
			RevealNextCard();
			AddToUndoHistory(Action.Move.RevealNextCard);
			return;
		}

		// Tapped on a slot card.
		Slot slot = card.transform.parent.GetComponent<Slot>();
		if (slot != null)
		{
			// Didn't tap on a revealed slot card.
			if (!card.Revealed) return;

			int numRanks = System.Enum.GetNames(typeof(CardRank)).Length;

			// Tapped on a valid slot card (i.e. +1/-1).
			if (card.Type == CardType.Normal && (int)card.Rank == (int)(waste[0].Rank + numRanks - 1) % numRanks ||
			    card.Type == CardType.Normal && (int)card.Rank == (int)(waste[0].Rank + 1) % numRanks ||
			    waste[0].Type == CardType.Wild)
			{
				RemoveCardFromSlot(slot);
				RefreshBoard();
				AddToUndoHistory(Action.Move.RemoveCardFromSlot, slot);
				score += pointsPerCardTakenFromBoard;
				DataManager.Instance.Coins += coinsPerCardTakenFromBoard;
				coinsCollected += coinsPerCardTakenFromBoard;
				return;
			}
		}
	}
}
