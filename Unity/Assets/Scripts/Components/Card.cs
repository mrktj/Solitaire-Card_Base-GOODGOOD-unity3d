using UnityEngine;
using System.Collections;

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

public enum CardType
{
	Normal,
	Wild
}

[ExecuteInEditMode]
public class Card : MonoBehaviour
{
	[SerializeField] Sprite front;
	[SerializeField] Sprite back;
	[SerializeField] Sprite[] ranks;
	[SerializeField] Sprite[] suits;
	[SerializeField] Sprite wild;

	[SerializeField] Color blackSuitColor;
	[SerializeField] Color redSuitColor;

	[SerializeField] SpriteRenderer backgroundSprite;
	[SerializeField] SpriteRenderer[] rankSprites;
	[SerializeField] bool colorizeRankSprites;
	[SerializeField] SpriteRenderer[] suitSprites;
	[SerializeField] bool colorizeSuitSprites;
	[SerializeField] SpriteRenderer[] specialSprites;
	
	[SerializeField] bool revealed;
	[SerializeField] CardRank rank;
	[SerializeField] CardSuit suit;
	[SerializeField] CardType type;

	bool isGeneratedCard;

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

			if (type == CardType.Normal)
			{
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
			else if (type == CardType.Wild)
			{
				rankSprite.gameObject.SetActive(false);
			}
		}

		foreach (SpriteRenderer suitSprite in suitSprites)
		{
			if (!revealed)
			{
				suitSprite.gameObject.SetActive(false);
				continue;
			}

			if (type == CardType.Normal)
			{
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
			else if (type == CardType.Wild)
			{
				suitSprite.gameObject.SetActive(false);
			}
		}

		foreach (SpriteRenderer specialSprite in specialSprites)
		{
			if (!revealed)
			{
				specialSprite.gameObject.SetActive(false);
				continue;
			}

			if (type == CardType.Normal)
			{
				specialSprite.gameObject.SetActive(false);
			}
			else if (type == CardType.Wild)
			{
				specialSprite.gameObject.SetActive(true);
				specialSprite.sprite = wild;
			}
		}
	}

	public void MoveToPosition(Vector3 movePosition)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, movePosition.z);
		TweenPosition.Begin(gameObject, 0.25f, movePosition);
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

	public CardType Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
			UpdateCard();
		}
	}

	public bool IsGeneratedCard
	{
		get
		{
			return isGeneratedCard;
		}
		set
		{
			isGeneratedCard = value;
		}
	}

#if UNITY_EDITOR
	public Sprite CardBack
	{
		get
		{
			return back;
		}
	}
#endif
}
