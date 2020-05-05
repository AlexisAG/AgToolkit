using System;
using UnityEngine;

namespace AgToolkit.Core.Helper.Events
{
    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/String", fileName = "NewStringGameEvent")]
    public class StringGameEvent : GameEvent<string> { }

    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/Bool", fileName = "NewBoolGameEvent")]
    public class BoolGameEvent : GameEvent<bool> { }

    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/Int", fileName = "NewIntGameEvent")]
    public class IntGameEvent : GameEvent<int> { }

    [CreateAssetMenu(menuName = "AgToolkit/GameEvents/Float", fileName = "NewFloatGameEvent")]
    public class FloatGameEvent : GameEvent<float> { }
}
