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

public struct LayerManager
{
    private static int _player = -1;
    public static int Player
    {
        get
        {
            if(_player == -1)
            {
                _player = LayerMask.NameToLayer("Player");
            }
            return _player;
        }
    }

    private static int _enemies = -1;
    public static int Enemies
    {
        get
        {
            if (_enemies == -1)
            {
                _enemies = LayerMask.NameToLayer("Enemies");
            }
            return _enemies;
        }
    }

    private static int _obstacles = -1;
    public static int Obstacles
    {
        get
        {
            if (_obstacles == -1)
            {
                _obstacles = LayerMask.NameToLayer("Obstacles");
            }
            return _obstacles;
        }
    }
}