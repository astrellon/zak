using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TileRender : MonoBehaviour
{
    public List<TileSet> AvailableTilesets = new List<TileSet>();
    private Dictionary<string, TileLayer> TileLayerMap = new Dictionary<string, TileLayer>();

	// Use this for initialization
	void Start ()
    {
        CheckForTilesets();
	}

    void CheckForTilesets()
    {
        AvailableTilesets = new List<TileSet>(GetComponents<TileSet>());	
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isPlaying)
        {
            return;
        }
        CheckForTilesets();
	}

    private TileLayer CreateLayer(GameObject obj, TileSet tileSet)
    {
        var layer = obj.AddComponent<TileLayer>();
        layer.Set = tileSet;
        layer.name = tileSet.Name + " Layer";
        layer.transform.parent = transform;

        return layer;
    }
    public TileLayer GetLayerForTileSet(TileSet tileSet)
    {
        Transform foundChild = null;
        var name = tileSet.Name + " Layer";
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name == name)
            {
                foundChild = child;
                break;
            }
        }

        if (foundChild != null)
        {
            var layer = foundChild.GetComponent<TileLayer>();
            if (layer == null)
            {
                return CreateLayer(foundChild.gameObject, tileSet);
            }
            return layer;
        }

        var layerObject = new GameObject();
        return CreateLayer(layerObject, tileSet);
    }

}
