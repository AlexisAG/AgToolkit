using AgToolkit.Core.DesignPattern;

namespace AgToolkit.Core.Manager
{
	/// <summary>
	/// Simple Monobehaviour object ot ensure coroutine doesn't get killed when an object is inactive
	/// </summary>
	/// <example>
	///	CoroutineManager.Instance.StartCoroutine(MyCoroutine());
	/// </example>
	public class CoroutineManager : Singleton<CoroutineManager>
	{
	}

}
