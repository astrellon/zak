using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSet : MonoBehaviour {

    public Texture2D CenterTiles;
    public Texture2D LeftEdge;

	// Use this for initialization
	void Start () {
        var sprites = Resources.LoadAll<Sprite>(CenterTiles.name);

        foreach (var sp in sprites)
        {
            Debug.Log("Sprite: " + sp.name);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
