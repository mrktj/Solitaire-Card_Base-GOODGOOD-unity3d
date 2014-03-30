using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
	where T : SingletonMonoBehaviour<T>
{
	static T instance;

	public T Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject instanceObject = new GameObject();
				instanceObject.name = typeof(T).ToString();
				instance = instanceObject.AddComponent<T>();
				DontDestroyOnLoad(instanceObject);
			}

			return instance;
		}
	}

	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = gameObject.GetComponent<T>();
		}
		else
		{
			GameObject.Destroy(gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
