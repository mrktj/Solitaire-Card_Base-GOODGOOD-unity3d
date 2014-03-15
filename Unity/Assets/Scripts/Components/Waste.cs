using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Waste : MonoBehaviour
{
	List<Card> cards = new List<Card>();
	
	void Awake()
	{
		CardHelper.TemporaryEditorSprite(gameObject);
	}

	public Card this[int index]
	{
		get
		{
			return cards[index];
		}
	}
	
	public int Size
	{
		get
		{
			return cards.Count;
		}
	}

	public void AddCard(Card card)
	{
		foreach (Card cardBelow in cards)
		{
			Vector3 cardPosition = cardBelow.transform.localPosition;
			cardPosition.z += 1f;
			cardBelow.transform.localPosition = cardPosition;
		}

		cards.Insert(0, card);
		card.transform.parent = transform;
		card.MoveToPosition(Vector3.zero);
	}

	public Card TakeTopCard()
	{
		if (cards.Count == 0) return null;

		Card card = cards[0];
		cards.RemoveAt(0);

		foreach (Card cardBelow in cards)
		{
			Vector3 cardPosition = cardBelow.transform.localPosition;
			cardPosition.z -= 1f;
			cardBelow.transform.localPosition = cardPosition;
		}

		return card;
	}
}
