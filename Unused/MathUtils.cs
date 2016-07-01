using System;
using UnityEngine;
using System.Collections;

public class MathUtils
{

    public static bool IsFloatEquals(float a, float b)
    {
        return Math.Abs(a - b) < 1e-3;
    }
}
