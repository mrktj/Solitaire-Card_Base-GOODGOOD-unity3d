using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TriPeaksGame : MonoBehaviour
{
	[SerializeField] float timePerRound;
	[SerializeField] int pointsPerCardTakenFromBoard;
	[SerializeField] int pointsPerCardRemainingInDeck;
	[SerializeField] int pointsPerSecondsRemaining;
	[SerializeField] int coinsPerCardTakenFromBoard;
	[SerializeField] int coinsPerCardRemainingInDeck;
	[SerializeField] int coinsPerSecondsRemaining;
	[SerializeField] int cardsForExtraLife;
	[SerializeField] int extraCardsPowerup;
	[SerializeField] float extraTimePowerup;
	[SerializeField] int costForUndoLastMove;
	[SerializeField] int costForShuffleBoard;
	[SerializeField] int costForGenerateWildCard;
	[SerializeField] int costForAddExtraCards;
	[SerializeField] int costForAddExtraTime;

	[SerializeField] Deck deck;
	[SerializeField] Board board;
	[SerializeField] Waste waste;
	[SerializeField] UILabel roundLabel;
	[SerializeField] UILabel timeLabel;
	[SerializeField] UILabel scoreLabel;
	[SerializeField] UILabel coinsLabel;
	[SerializeField] UISprite livesSprite;
	[SerializeField] ResultsPanel resultsPanel;
	[SerializeField] GameObject endGameButton;

	public bool paused = false;
	public bool endGame = false;

#if UNITY_EDITOR
	public bool restartGame = false;
	public bool undoLastMove = false;
	public bool shuffleBoard = false;
	public bool generateWildCard = false;
	public bool addExtraCards = false;
	public bool addExtraTime = false;
#endif

	int score = 0;
	int round = 1;
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

	List<Action> undoHistory = new List<Action>();

	void Awake()
	{
		timeRemaining = timePerRound;
	}

	void Start()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			DealBoard();
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
			UpdateUI();

			if (endGame)
			{
				endGame = false;
				EndGame();
			}
		}

#if UNITY_EDITOR
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

	void UpdateUI()
	{
		roundLabel.text = round.ToString();
		timeLabel.text = Mathf.Floor(timeRemaining).ToString();
		scoreLabel.text = score.ToString();
		coinsLabel.text = DataManager.Instance.Coins.ToString();
		livesSprite.width = livesSprite.atlas.GetSprite(livesSprite.spriteName).width * lives;
		NGUITools.SetActive(endGameButton, deck.Size == 0);
	}

	void DealBoard()
	{
		deck.Shuffle();

		for (int i = 0, iMax = board.Size; i < iMax && deck.Size > 0; i++)
		{
			DealCardToSlot(board[i]);
		}

		RefreshBoard();
		RevealNextCard();
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

	void EndGame()
	{
		paused = true;
		resultsPanel.Toggle(true);

		if (successfulRound && deck.Size >= cardsForExtraLife) lives++;

		int deckScore = successfulRound ? deck.Size * pointsPerCardRemainingInDeck : 0;
		int timeScore = successfulRound ? Mathf.FloorToInt(timeRemaining) * pointsPerSecondsRemaining : 0;
		int totalScore = score + deckScore + timeScore;
		int deckCoins = successfulRound ? deck.Size * coinsPerCardRemainingInDeck : 0;
		int timeCoins = successfulRound ? Mathf.FloorToInt(timeRemaining) * coinsPerSecondsRemaining : 0;
		int totalCoins = coinsCollected + deckCoins + timeCoins;
		resultsPanel.Setup(successfulRound, score, deckScore, timeScore, totalScore, coinsCollected, deckCoins, timeCoins, totalCoins);
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
		
		DealBoard();

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

		timeRemaining = timePerRound;
		undoHistory.Clear();
		paused = false;
		successfulRound = false;
	}
	
	void AddToUndoHistory(Action.Move move, Slot slot = null)
	{
		Action action = new Action();
		action.move = move;
		action.slot = slot;
		undoHistory.Add(action);
	}

	public void UndoLastMove()
	{
		if (paused) return;
		
		if (undoHistory.Count == 0) return;

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

		Action action = undoHistory[undoHistory.Count - 1];
		undoHistory.RemoveAt(undoHistory.Count - 1);

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

		List<Slot> untouchedSlots = new List<Slot>();

		for (int i = 0, iMax = board.Size; i < iMax; i++)
		{
			if (board[i].Card != null)
			{
				untouchedSlots.Add(board[i]);
				ReturnCardFromSlot(board[i]);
			}
		}

		deck.Shuffle();

		for (int i = 0, iMax = untouchedSlots.Count; i < iMax; i++)
		{
			DealCardToSlot(untouchedSlots[i]);
		}

		RefreshBoard();
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
