using System;
using UnityEngine.Events;

namespace AgToolkit.Core.Misc
{

	[Serializable]
	public class BoolUnityEvent : UnityEvent<bool> { }

	[Serializable]
	public class IntUnityEvent : UnityEvent<int> { }

	[Serializable]
	public class FloatUnityEvent : UnityEvent<float> { }

	[Serializable]
	public class StringUnityEvent : UnityEvent<string> { }

}
