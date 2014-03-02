using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TriPeaksGame : MonoBehaviour
{
	[SerializeField] Deck deck;
	[SerializeField] Board board;
	[SerializeField] Waste waste;

	void Start()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			StartCoroutine(DealCards());
		}
	}

	IEnumerator DealCards()
	{
		for (int i = 0, iMax = board.Size; i < iMax && deck.Size > 0; i++)
		{
			Vector3 dealPosition = deck.transform.InverseTransformPoint(board[i].transform.position);
			Card card = deck.DealCard();
			card.MoveToPosition(dealPosition);
			board[i].PlaceCard(card);
		}

		yield return StartCoroutine(UpdateBoard());

		for (int i = 0, iMax = deck.Size; i < iMax; i++)
		{
			Vector3 cardPosition = deck[i].transform.localPosition;
			cardPosition.x -= (float)i * 405f / ((float)deck.Size - 1f);
			deck[i].transform.localPosition = cardPosition;
		}

		yield return StartCoroutine(RevealNextCard());

		yield break;
	}

	IEnumerator UpdateBoard()
	{
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

		yield break;
	}

	IEnumerator RevealNextCard()
	{
		Vector3 wastePosition = deck.transform.InverseTransformPoint(waste.transform.position);
		Card card = deck.DealCard();
		card.MoveToPosition(wastePosition);
		waste.AddCard(card);
		card.Revealed = true;

		yield break;
	}

	IEnumerator RemoveCardFromSlot(Slot slot)
	{
		Vector3 wastePosition = slot.transform.InverseTransformPoint(waste.transform.position);
		Card card = slot.TakeCard();
		card.MoveToPosition(wastePosition);
		waste.AddCard(card);

		yield return StartCoroutine(UpdateBoard());

		yield break;
	}

	void OnRecognizeTap(TapGesture gesture)
	{
		if (gesture.Selection == null) return;

		Card card = gesture.Selection.GetComponent<Card>();
		if (card == null) return;

		if (card.transform.parent.GetComponent<Deck>() == deck)
		{
			StartCoroutine(RevealNextCard());
			return;
		}

		Slot slot = card.transform.parent.GetComponent<Slot>();
		if (slot != null)
		{
			if (!card.Revealed) return;

			int numRanks = System.Enum.GetNames(typeof(CardRank)).Length;

			if ((int)card.Rank == (int)(waste[0].Rank + numRanks - 1) % numRanks ||
			    (int)card.Rank == (int)(waste[0].Rank + 1) % numRanks)
			{
				StartCoroutine(RemoveCardFromSlot(slot));
				return;
			}
		}
	}
}
