using UnityEngine;

[ExecuteInEditMode]
public class Slot : MonoBehaviour
{
	[SerializeField] BetterList<Slot> overlappingSlots = new BetterList<Slot>();

	Card card = null;
	
	public static Slot Create(GameObject parent)
	{
		Slot slot = NGUITools.AddChild<Slot>(parent);
		return slot;
	}

#if UNITY_EDITOR
	void Awake()
	{
		CardHelper.TemporaryEditorSprite(gameObject);
	}
#endif

	public void PlaceCard(Card card)
	{
		this.card = card;
		card.transform.parent = transform;
		card.MoveToPosition(Vector3.zero);
	}

	public Card TakeCard()
	{
		Card card = this.card;
		this.card = null;
		return card;
	}

	public BetterList<Slot> OverlappingSlots
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
