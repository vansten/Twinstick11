using UnityEngine;

[System.Serializable]
public struct RangeFloat
{
    public float Min;
    public float Max;

    public float Rand()
    {
        return Random.Range(Min, Max);
    }

    public float Clamp(float Value)
    {
        return Mathf.Clamp(Value, Min, Max);
    }
}

[System.Serializable]
public struct RangeInt
{
    public int Min;
    public int Max;

    public int Rand()
    {
        return Random.Range(Min, Max);
    }

    public int Clamp(int Value)
    {
        return Mathf.Clamp(Value, Min, Max);
    }
}