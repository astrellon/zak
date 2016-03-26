using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct TilePosition
{
    public Vector2Int Position;
    public TileInstance Instance;

    public static TilePosition Empty = new TilePosition { Position = Vector2Int.Zero, Instance = null };

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
    public bool IsEmpty
    {
        get { return Position == Vector2Int.Zero && Instance == null; }
    }
}

[ExecuteInEditMode]
public class TileLayer : MonoBehaviour
{

    public List<TilePosition> Tiles = new List<TilePosition>();
    public List<TilePosition> Transitions = new List<TilePosition>();
    public Texture2D TileSet;
    public Texture2D TransitionSet;
    public TileSet Test;

	// Use this for initialization
	void Start ()
    {
        //CreateTiles();
	}

    void CreateTiles()
    {
        var children = transform.Cast<Transform>().ToList();
        foreach (Transform trans in children)
        {
            DestroyImmediate(trans.gameObject);
        }

        CreateTileInstances(Tiles, false);
        CreateTileInstances(Transitions, true);
    }

    void CreateTileInstances(IEnumerable<TilePosition> instances, bool isTransition)
    {
        foreach (var kvp in instances)
        {
            var instance = kvp.Instance;
            var position = kvp.Position;
            var name = position.x + "_" + position.y;
            if (isTransition)
            {
                name += "_trans_" + instance.Frame;
            }
            var tileInstance = new GameObject();
            tileInstance.transform.parent = transform;
            tileInstance.transform.localPosition = new Vector3(position.x * 32.0f, position.y * 32.0f, 0.0f);
            tileInstance.name = name;

            var spriteTexture = tileInstance.AddComponent<SpriteTexture>();
            spriteTexture.Animated = instance.Animated;
            spriteTexture.internalTexture = isTransition ? TransitionSet : TileSet;
            spriteTexture.Frame = instance.Frame;
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

        for (var y = -1; y <= 1; y++)
        {
            for (var x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                var checkPosition = position.Change(x, y);
                var topEdge = GetTile(checkPosition);
                if (topEdge == TilePosition.Empty)
                {
                    AddTransition(checkPosition, 1);
                }
            }
        }

        Tiles.Add(new TilePosition
        {
            Position = position,
            Instance = new TileInstance
            {
                Frame = frame
            }
        });
    }

    public void AddTransition(Vector2Int position, int frame)
    {
        var findTransition = GetTransition(position, frame);
        if (findTransition != TilePosition.Empty)
        {
            return;
        }

        Transitions.Add(new TilePosition
        {
            Position = position,
            Instance = new TileInstance
            {
                Frame = frame
            }
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
        for (var i = Transitions.Count - 1; i >= 0; i--)
        {
            if (Transitions[i].Position == position)
            {
                Transitions.RemoveAt(i);
            }
        }
    }
    private TilePosition GetTransition(Vector2Int position, int frame)
    {
        foreach (var tile in Transitions)
        {
            if (tile.Position == position && tile.Instance.Frame == frame)
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
