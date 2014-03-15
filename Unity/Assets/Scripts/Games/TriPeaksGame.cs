using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TriPeaksGame : MonoBehaviour
{
	[SerializeField] Deck deck;
	[SerializeField] Board board;
	[SerializeField] Waste waste;

#if UNITY_EDITOR
	public bool restartGame = false;
#endif

	void Start()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			DealBoard();
		}
	}

#if UNITY_EDITOR
	void Update()
	{
		if (restartGame)
		{
			restartGame = false;
			RestartGame();
		}
	}
#endif

	void DealBoard()
	{
		deck.Shuffle();

		for (int i = 0, iMax = board.Size; i < iMax && deck.Size > 0; i++)
		{
			DealCardToSlot(board[i]);
		}

		UpdateBoard();

		RevealNextCard();
	}
	
	void RestartGame()
	{
		for (int i = 0, iMax = board.Size; i < iMax; i++)
		{
			ReturnCardFromSlot(board[i]);
		}
		
		while (waste.Size > 0)
		{
			ReturnCardFromWaste();
		}
		
		DealBoard();
	}

	void UpdateBoard()
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
		UpdateBoard();
	}

	/// <summary>
	/// Moves card from slot to deck.
	/// </summary>
	void ReturnCardFromSlot(Slot slot)
	{
		Card card = slot.TakeCard();
		if (card != null)
		{
			deck.AddCard(card);
			card.Revealed = false;
		}
	}

	/// <summary>
	/// Moves card from waste to deck.
	/// </summary>
	void ReturnCardFromWaste()
	{
		Card card = waste.TakeTopCard();
		if (card != null)
		{
			deck.AddCard(card);
			card.Revealed = false;
		}
	}

	void OnRecognizeTap(TapGesture gesture)
	{
		// Didn't tap on anything.
		if (gesture.Selection == null) return;

		// Didn't tap on a card.
		Card card = gesture.Selection.GetComponent<Card>();
		if (card == null) return;

		// Tapped on a deck card.
		if (card.transform.parent.GetComponent<Deck>() == deck)
		{
			RevealNextCard();
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
			if ((int)card.Rank == (int)(waste[0].Rank + numRanks - 1) % numRanks ||
			    (int)card.Rank == (int)(waste[0].Rank + 1) % numRanks)
			{
				RemoveCardFromSlot(slot);
				return;
			}
		}
	}
}
