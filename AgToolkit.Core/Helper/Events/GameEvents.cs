using System;

namespace AgToolkit.Core.Helper.Events
{
    [Serializable]
    public class StringGameEvent : GameEvent<string> { }

    [Serializable]
    public class BoolGameEvent : GameEvent<bool> { }

    [Serializable]
    public class IntGameEvent : GameEvent<int> { }

    [Serializable]
    public class FloatGameEvent : GameEvent<float> { }
}
