using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Vector2Int
{
    public readonly static Vector2Int Zero = new Vector2Int(0, 0);

    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public class TileInstance
{
    public int Frame;
    public bool Animated;
}
