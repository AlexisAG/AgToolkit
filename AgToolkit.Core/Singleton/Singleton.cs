using UnityEngine;

namespace AgToolkit.AgToolkit.Core.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static bool _ShuttingDown = false;
		private static readonly object _Lock = new object();

		private static T _Instance;

        /// <summary>
        /// Instance of the Singleton, create the instance if it does not exists.
        /// </summary>
        public static T Instance
		{
			get
			{
				if (_ShuttingDown)
				{
					Debug.LogWarning($"[Singleton] Instance <{typeof(T)}> already destroyed. Returning null.");
					return null;
				}

				lock (_Lock)
				{
                    if (_Instance != null) return _Instance;

                    Debug.LogWarning($"[Singleton] Instance <{typeof(T)}> does not exists, creating on runtime. Prefer adding the component in Scene directly.");

                    //create GameObject with Component
                    _Instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    return _Instance;
				}
			}
			private set => _Instance = value;
		}
		public static bool IsInstanced => _Instance != null;

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
			_ShuttingDown = true;
		}

		protected virtual void OnDestroy()
		{
			_ShuttingDown = true;
		}

		private void CreateInstance()
		{
			lock (_Lock)
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