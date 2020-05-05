namespace AgToolkit.Core.Helper.Events.Listeners
{
    public class StringGameEventListener : GameEventListener<string, StringGameEvent, StringUnityEvent> { }
    public class IntGameEventListener : GameEventListener<int, IntGameEvent, IntUnityEvent> { }
    public class FloatGameEventListener : GameEventListener<float, FloatGameEvent, FloatUnityEvent> { }
    public class BoolGameEventListener : GameEventListener<bool, BoolGameEvent, BoolUnityEvent> { }

}
