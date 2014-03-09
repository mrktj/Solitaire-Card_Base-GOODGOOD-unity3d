using UnityEngine;

public static class CardHelper
{
	public static void TemporaryEditorSprite(GameObject gameObject)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			if (renderer != null) Component.Destroy(renderer);

			BoxCollider collider = gameObject.GetComponent<BoxCollider>();
			if (collider != null) Component.Destroy(collider);
		}
#if UNITY_EDITOR
		else
		{
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			if (renderer == null) renderer = gameObject.AddComponent<SpriteRenderer>();
			
			Deck[] decks = NGUITools.FindActive<Deck>();
			if (decks.Length > 0) renderer.sprite = decks[0].CardBack;

			BoxCollider collider = gameObject.GetComponent<BoxCollider>();
			if (collider == null) collider = gameObject.AddComponent<BoxCollider>();
			collider.size = renderer.sprite.bounds.size;
		}
#endif
	}
}
