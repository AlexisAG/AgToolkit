﻿using System;

namespace AgToolkit.Core.Events
{
    [Serializable]
    public class StringGameVar : GameVar<string> { }

    [Serializable]
    public class IntGameVar : GameVar<int> { }

    [Serializable]
    public class FloatGameVar : GameVar<float> { }

    [Serializable]
    public class BoolGameVar : GameVar<bool> { }
}