using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct TilePosition
{
    public Vector2Int Position;
    public TileInstance Instance;

    public static TilePosition Empty = new TilePosition
    {
        Position = Vector2Int.Zero,
        Instance = null
    };

    public static bool operator == (TilePosition lhs, TilePosition rhs)
    {
        return lhs.Position == rhs.Position && lhs.Instance == rhs.Instance;
    }
    public static bool operator != (TilePosition lhs, TilePosition rhs)
    {
        return lhs.Position != rhs.Position || lhs.Instance != rhs.Instance;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TilePosition))
        {
            return false;
        }

        var pos = (TilePosition)obj;
        return this == pos;
    }
    public override string ToString()
    {
        return string.Format("[{0},{1}] ({2})", Position.x, Position.y, Instance != null ? Instance.ToString() : "null");
    }
    public bool IsEmpty
    {
        get { return Position == Vector2Int.Zero && Instance == null; }
    }
}

[ExecuteInEditMode]
public class TileLayer : MonoBehaviour
{

    public List<TilePosition> Tiles = new List<TilePosition>();
    public List<TilePosition> Edges = new List<TilePosition>();
    public List<TilePosition> Corners = new List<TilePosition>();
    public TileSet Set;

	// Use this for initialization
	void Start ()
    {
	}

    void CreateTiles()
    {
        var children = transform.Cast<Transform>().ToList();
        foreach (Transform trans in children)
        {
            DestroyImmediate(trans.gameObject);
        }

        CreateTileInstances(Tiles);
        CreateTransitionInstances(Edges, false);
        CreateTransitionInstances(Corners, true);
    }

    void CreateTileInstances(IEnumerable<TilePosition> instances)
    {
        foreach (var kvp in instances)
        {
            var instance = kvp.Instance;
            var commonTexture = Set.CenterTiles;
            if (!commonTexture.HasValue)
            {
                continue;
            }

            var position = kvp.Position;
            var name = position.x + "_" + position.y;
            var tileInstance = new GameObject();
            tileInstance.transform.parent = transform;
            tileInstance.transform.localPosition = new Vector3(position.x * 32.0f, position.y * 32.0f, position.y);
            tileInstance.name = name;

            var spriteTexture = tileInstance.AddComponent<SpriteTexture>();
            spriteTexture.Animated = instance.Animated;
            if (commonTexture.IsTexture)
            {
                spriteTexture.internalTexture = commonTexture.Texture;
                spriteTexture.Frame = instance.Frame;
                spriteTexture.Animated = commonTexture.Animated;
                spriteTexture.AnimationFPS = commonTexture.FrameRate;
            }
            else
            {
                spriteTexture.internalSprite = commonTexture.Sprite;
            }
        }
    }

    void CreateTransitionInstances(IEnumerable<TilePosition> instances, bool isCorner)
    {
        foreach (var kvp in instances)
        {
            var instance = kvp.Instance;
            var transitions = isCorner ? Set.CornerTiles : Set.EdgeTiles;

            var commonTexture = transitions[instance.TransitionFrame];
            if (!commonTexture.HasValue)
            {
                continue;
            }

            var position = kvp.Position;
            var name = position.x + "_" + position.y;
            if (isCorner)
            {
                name += "_corner_" + instance.Frame;
            }
            else
            {
                name += "_edge_" + instance.Frame;
            }

            var tileInstance = new GameObject();
            tileInstance.transform.parent = transform;
            tileInstance.transform.localPosition = new Vector3(position.x * 32.0f, position.y * 32.0f, position.y);
            tileInstance.name = name;

            var spriteTexture = tileInstance.AddComponent<SpriteTexture>();
            spriteTexture.Animated = instance.Animated;

            if (commonTexture.IsTexture)
            {
                spriteTexture.internalTexture = commonTexture.Texture;
                spriteTexture.Frame = instance.Frame;
                spriteTexture.Animated = commonTexture.Animated;
                spriteTexture.AnimationFPS = commonTexture.FrameRate;
            }
            else
            {
                spriteTexture.internalSprite = commonTexture.Sprite;
            }
        }
    }

    public void AddTile(Vector2Int position, int frame)
    {
        var findTile = GetTile(position);
        if (findTile != TilePosition.Empty)
        {
            return;
        }

        RemoveTransitionsAt(position);

        Tiles.Add(new TilePosition {
            Position = position,
            Instance = new TileInstance{Frame = frame}
        });

        for (var y = -1; y <= 1; y++)
        {
            for (var x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                var checkPosition = position.Change(x, y);
                CheckTransitions(checkPosition);
            }
        }

    }

    private void CheckTransitions(Vector2Int position)
    {
        RemoveTransitionsAt(position);

        if (GetTile(position) != TilePosition.Empty)
        {
            return;
        }

        var hasLeftEdge = GetTile(position.Change(-1, 0)) != TilePosition.Empty;
        var hasRightEdge = GetTile(position.Change(1, 0)) != TilePosition.Empty;
        var hasTopEdge = GetTile(position.Change(0, 1)) != TilePosition.Empty;
        var hasBottomEdge = GetTile(position.Change(0, -1)) != TilePosition.Empty;

        var edgeFrame = TileSet.EdgeFlags.None;
        if (hasLeftEdge) { edgeFrame |= TileSet.EdgeFlags.Left; }
        if (hasRightEdge) { edgeFrame |= TileSet.EdgeFlags.Right; }
        if (hasTopEdge) { edgeFrame |= TileSet.EdgeFlags.Top; }
        if (hasBottomEdge) { edgeFrame |= TileSet.EdgeFlags.Bottom; }

        if (edgeFrame != TileSet.EdgeFlags.None)
        {
            AddTransition(position, false, (int)edgeFrame, 0);
        }

        var cornerFrame = TileSet.CornerFlags.None;
        if (!hasTopEdge && !hasLeftEdge && GetTile(position.Change(-1, 1)) != TilePosition.Empty)
        {
            cornerFrame |= TileSet.CornerFlags.TopLeft;
        }
        if (!hasTopEdge && !hasRightEdge && GetTile(position.Change(1, 1)) != TilePosition.Empty)
        {
            cornerFrame |= TileSet.CornerFlags.TopRight;
        }
        if (!hasBottomEdge && !hasLeftEdge && GetTile(position.Change(-1, -1)) != TilePosition.Empty)
        {
            cornerFrame |= TileSet.CornerFlags.BottomLeft;
        }
        if (!hasBottomEdge && !hasRightEdge && GetTile(position.Change(1, -1)) != TilePosition.Empty)
        {
            cornerFrame |= TileSet.CornerFlags.BottomRight;
        }

        if (cornerFrame != TileSet.CornerFlags.None)
        {
            AddTransition(position, true, (int)cornerFrame, 0);
        }
    }

    public void AddTransition(Vector2Int position, bool isCorner, int transitionFrame, int frame)
    {
        var findTransition = GetTransition(position, isCorner, transitionFrame);
        if (findTransition != TilePosition.Empty)
        {
            return;
        }

        var transitions = isCorner ? Corners : Edges;
        transitions.Add(new TilePosition
        {
            Position = position,
            Instance = new TileInstance { Frame = frame, TransitionFrame = transitionFrame }
        });
    }
    private TilePosition GetTile(Vector2Int position)
    {
        foreach (var tile in Tiles)
        {
            if (tile.Position == position)
            {
                return tile;
            }
        }
        return TilePosition.Empty;
    }
    private void RemoveTransitionsAt(Vector2Int position)
    {
        for (var i = Edges.Count - 1; i >= 0; i--)
        {
            if (Edges[i].Position == position)
            {
                Edges.RemoveAt(i);
            }
        }
        for (var i = Corners.Count - 1; i >= 0; i--)
        {
            if (Corners[i].Position == position)
            {
                Corners.RemoveAt(i);
            }
        }
    }
    private TilePosition GetTransition(Vector2Int position, bool isCorner, int transitionFrame)
    {
        foreach (var tile in isCorner ? Corners : Edges)
        {
            if (tile.Position == position && tile.Instance.TransitionFrame == transitionFrame)
            {
                return tile;
            }
        }
        return TilePosition.Empty;
    }

    // Update is called once per frame
    void Update () {
        if (!Application.isPlaying)
        {
            CreateTiles();
        }
	}
}
