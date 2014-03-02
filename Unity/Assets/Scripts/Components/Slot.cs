using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Slot : MonoBehaviour
{
	[SerializeField] List<Slot> overlappingSlots = new List<Slot>();

	Card card = null;
	
	void Awake()
	{
		CardHelper.TemporaryEditorSprite(gameObject);
	}

	public void PlaceCard(Card card)
	{
		this.card = card;
		card.transform.parent = transform;
	}

	public Card TakeCard()
	{
		Card card = this.card;
		this.card = null;
		return card;
	}

	public List<Slot> OverlappingSlots
	{
		get
		{
			return overlappingSlots;
		}
	}

	public Card Card
	{
		get
		{
			return card;
		}
	}
}
