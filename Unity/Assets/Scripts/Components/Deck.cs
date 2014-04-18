using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Deck : MonoBehaviour
{
	[SerializeField] GameObject cardPrefab;

	public bool generateStandardCards = false;
	public bool clearAllCards = false;
	
	List<Card> cards = new List<Card>();

	void Awake()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			Initialize();
		}
	}

	void Update()
	{
		if (generateStandardCards)
		{
			generateStandardCards = false;
			GenerateStandardCards();
		}

		if (clearAllCards)
		{
			clearAllCards = false;
			ClearAllCards();
		}
	}

	void GenerateStandardCards()
	{
		for (int i = 0, iMax = System.Enum.GetNames(typeof(CardRank)).Length; i < iMax; i++)
		{
			for (int j = 0, jMax = System.Enum.GetNames(typeof(CardSuit)).Length; j < jMax; j++)
			{
				Card card = CreateNewCard();
				card.Rank = (CardRank)i;
				card.Suit = (CardSuit)j;
				card.Revealed = false;
			}
		}
	}

	void ClearAllCards()
	{
		foreach (Card card in GetComponentsInChildren<Card>())
		{
			GameObject.DestroyImmediate(card.gameObject);
		}
	}

	void Initialize()
	{
		foreach (Card card in GetComponentsInChildren<Card>())
		{
			cards.Add(card);
		}
	}

	public void Shuffle()
	{
		for (int i = 0, iMax = cards.Count; i < iMax; i++)
		{
			int randomIndex = Random.Range(0, iMax);
			Card randomCard = cards[randomIndex];
			cards[randomIndex] = cards[i];
			SetCardDepth(cards[randomIndex], randomIndex);
			cards[i] = randomCard;
			SetCardDepth(cards[i], i);
		}
	}

	public Card CreateNewCard()
	{
		GameObject cardObject = NGUITools.AddChild(gameObject, cardPrefab);
		return cardObject.GetComponent<Card>();
	}

	void SetCardDepth(Card card, int depth)
	{
		Vector3 cardPosition = card.transform.localPosition;
		cardPosition.z = depth;
		card.transform.localPosition = cardPosition;
	}

	void SetAllCardsDepth()
	{
		for (int i = 0, iMax = cards.Count; i < iMax; i++)
		{
			SetCardDepth(cards[i], i);
		}
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

	public Card DealCard()
	{
		Card card = cards[0];
		cards.RemoveAt(0);
		SetAllCardsDepth();
		return card;
	}

	public void AddCardOnTop(Card card)
	{
		cards.Insert(0, card);
		card.transform.parent = transform;
		card.MoveToPosition(Vector3.zero);
		SetAllCardsDepth();
	}

#if UNITY_EDITOR
	public Sprite CardBack
	{
		get
		{
			if (cardPrefab == null) return null;
			if (cardPrefab.GetComponent<Card>() == null) return null;
			return cardPrefab.GetComponent<Card>().CardBack;
		}
	}
#endif
}
