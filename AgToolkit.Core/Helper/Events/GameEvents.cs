using System;
using UnityEngine;

namespace AgToolkit.Core.Helper.Events
{
    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/string", fileName = "NewStringGameEvent")]
    public class StringGameEvent : GameEvent<string> { }

    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/bool", fileName = "NewBoolGameEvent")]
    public class BoolGameEvent : GameEvent<bool> { }

    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/int", fileName = "NewIntGameEvent")]
    public class IntGameEvent : GameEvent<int> { }

    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/float", fileName = "NewFloatGameEvent")]
    public class FloatGameEvent : GameEvent<float> { }
}
