using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct TilePosition
{
    public Vector2Int Position;
    public TileInstance Instance;
}


[ExecuteInEditMode]
public class TileLayer : MonoBehaviour {

    //public Dictionary<Vector2Int, TileInstance> Tiles = new Dictionary<Vector2Int, TileInstance>();
    //public Dictionary<Vector2Int, TileInstance> Transitions = new Dictionary<Vector2Int, TileInstance>();
    public List<TilePosition> Tiles = new List<TilePosition>();
    public List<TilePosition> Transitions = new List<TilePosition>();
    public Texture2D TileSet;
    public Texture2D TransitionSet;

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
        foreach (var kvp in Tiles)
        {
            if (kvp.Position.Equals(position))
            {
                return;
            }
        }
        Tiles.Add(new TilePosition {Position = position,
            Instance = new TileInstance
        {
            Frame = frame
        }});
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            //return;
        }

        GUI.Button(new Rect(10, 10, 100, 40), "Hi");
    }

    void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            Debug.Log("Mouse down");
            Ray ray = Camera.current.ScreenPointToRay(Event.current.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                Debug.Log(Event.current.mousePosition);
                //Vector3 newTilePosition = hit.point;
                //Instantiate(newTile, newTilePosition, Quaternion.identity);
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (!Application.isPlaying)
        {
            CreateTiles();
        }
	}
}
