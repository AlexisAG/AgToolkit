using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Singleton
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static bool ShuttingDown = false;
		private static readonly object Lock = new object();

		private static T sInstance;
		public static T Instance
		{
			get
			{
				if (ShuttingDown)
				{
					Debug.LogWarning($"[Singleton] Instance <{typeof(T)}> already destroyed. Returning null.");
					return null;
				}

				lock (Lock)
				{
					if (sInstance == null)
					{
						Debug.LogWarning($"[Singleton] Instance <{typeof(T)}> does not exists, creating on runtime. Prefer adding the component in Scene directly.");

						//create GameObject with Component
						sInstance = new GameObject(typeof(T).Name).AddComponent<T>();
					}
					return sInstance;
				}
			}
			private set => sInstance = value;
		}
		public static bool IsInstanced => sInstance != null;

		protected virtual void Awake()
		{
			CreateInstance();

			if (Instance != this)
			{
				Debug.LogError($"Singleton <{typeof(T)}> already instanced, destroying {name}");
				Destroy(this);
			}
		}

		protected virtual void OnApplicationQuit()
		{
			ShuttingDown = true;
		}

		protected virtual void OnDestroy()
		{
			ShuttingDown = true;
		}

		private void CreateInstance()
		{
			lock (Lock)
			{
				if (!IsInstanced)
				{
					Instance = this as T;

					//Make persistent
					DontDestroyOnLoad(this);

					Debug.Log($"Singleton <{typeof(T)}> instanced.");
				}
			}
		}
	}
}