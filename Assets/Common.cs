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

    public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }
    public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }
    public bool Equals(Vector2Int obj)
    {
        return this == obj;
    }
    public override bool Equals(object obj)
    {
        if (!(obj is Vector2Int))
        {
            return false;
        }

        var vecObj = (Vector2Int)obj;
        return this == vecObj;
    }
}

[System.Serializable]
public class TileInstance
{
    public int Frame;
    public bool Animated;
}
