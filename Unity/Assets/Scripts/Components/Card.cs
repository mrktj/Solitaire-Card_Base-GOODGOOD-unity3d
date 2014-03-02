using UnityEngine;

public enum CardRank
{
	Ace,
	Two,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
	Nine,
	Ten,
	Jack,
	Queen,
	King
}

public enum CardSuit
{
	Spades,
	Hearts,
	Clubs,
	Diamonds
}

[ExecuteInEditMode]
public class Card : MonoBehaviour
{
	[SerializeField] Sprite front;
	[SerializeField] Sprite back;
	[SerializeField] Sprite[] ranks;
	[SerializeField] Sprite[] suits;

	[SerializeField] Color blackSuitColor;
	[SerializeField] Color redSuitColor;

	[SerializeField] SpriteRenderer backgroundSprite;
	[SerializeField] SpriteRenderer[] rankSprites;
	[SerializeField] bool colorizeRankSprites;
	[SerializeField] SpriteRenderer[] suitSprites;
	[SerializeField] bool colorizeSuitSprites;
	
	[SerializeField] bool revealed;
	[SerializeField] CardRank rank;
	[SerializeField] CardSuit suit;

	void Awake()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.size = back.bounds.size;
		}

		UpdateCard();
	}

	void Update()
	{
#if UNITY_EDITOR
		UpdateCard();
#endif
	}

	void UpdateCard()
	{
		backgroundSprite.sprite = revealed ? front : back;

		foreach (SpriteRenderer rankSprite in rankSprites)
		{
			if (!revealed)
			{
				rankSprite.gameObject.SetActive(false);
				continue;
			}

			rankSprite.gameObject.SetActive(true);
			rankSprite.sprite = ranks[(int)rank];

			if (colorizeRankSprites)
			{
				rankSprite.color = suit == CardSuit.Spades || suit == CardSuit.Clubs ? blackSuitColor : redSuitColor;
			}
			else
			{
				rankSprite.color = Color.white;
			}
		}

		foreach (SpriteRenderer suitSprite in suitSprites)
		{
			if (!revealed)
			{
				suitSprite.gameObject.SetActive(false);
				continue;
			}

			suitSprite.gameObject.SetActive(true);
			suitSprite.sprite = suits[(int)suit];

			if (colorizeSuitSprites)
			{
				suitSprite.color = suit == CardSuit.Spades || suit == CardSuit.Clubs ? blackSuitColor : redSuitColor;
			}
			else
			{
				suitSprite.color = Color.white;
			}
		}
	}

	public void MoveToPosition(Vector3 movePosition)
	{
		transform.localPosition = movePosition;
	}

	public bool Revealed
	{
		get
		{
			return revealed;
		}
		set
		{
			revealed = value;
			UpdateCard();
		}
	}

	public CardRank Rank
	{
		get
		{
			return rank;
		}
		set
		{
			rank = value;
			UpdateCard();
		}
	}

	public CardSuit Suit
	{
		get
		{
			return suit;
		}
		set
		{
			suit = value;
			UpdateCard();
		}
	}
	
	public Sprite CardBack
	{
		get
		{
			return back;
		}
	}
}
