using UnityEngine;

public class MathUtil
{
    public static float SmoothStep(float t)
    {
        return ((t)*(t)*(3 - 2*(t)));
    }

    public static float GetDecimal(float x)
    {
        return x - (int) (x);
    }

    // This is like the built in mod (%) except it supports negative numbers
    public static int Mod(int a, int b)
    {
        return (Mathf.Abs(a * b) + a) % b;
    }
}