using AgToolkit.Core.DesignPattern.Singleton;

namespace AgToolkit.Core.Helper
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
