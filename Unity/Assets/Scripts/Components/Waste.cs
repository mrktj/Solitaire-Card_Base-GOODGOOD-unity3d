﻿using UnityEngine;
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
	}
}