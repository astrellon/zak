using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class TileLayer : MonoBehaviour {

    public List<TileInstance> Tiles = new List<TileInstance>();
    public List<TileInstance> Transitions = new List<TileInstance>();
    public Texture2D TileSet;
    public Texture2D TransitionSet;

	// Use this for initialization
	void Start ()
    {
        CreateTiles();
	}

    void CreateTiles()
    {
        var children = transform.Cast<Transform>().ToList();
        foreach (Transform trans in children)
        {
            DestroyImmediate(trans.gameObject);
        }

        foreach (var instance in Tiles)
        {
            var name = instance.Position.x + "_" + instance.Position.y;
            var tileInstance = new GameObject();
            tileInstance.transform.parent = transform;
            tileInstance.transform.localPosition = new Vector3(instance.Position.x * 32.0f, instance.Position.y * 32.0f, 0.0f);
            tileInstance.name = name;

            var spriteTexture = tileInstance.AddComponent<SpriteTexture>();
            spriteTexture.Animated = instance.Animated;
            spriteTexture.internalTexture = TileSet;
            spriteTexture.Frame = instance.Frame;
        }

        foreach (var instance in Transitions)
        {
            var name = instance.Position.x + "_" + instance.Position.y + "_trans_" + instance.Frame;
            var tileInstance = new GameObject();
            tileInstance.transform.parent = transform;
            tileInstance.transform.localPosition = new Vector3(instance.Position.x * 32.0f, instance.Position.y * 32.0f, 0.0f);
            tileInstance.name = name;

            var spriteTexture = tileInstance.AddComponent<SpriteTexture>();
            spriteTexture.Animated = instance.Animated;
            spriteTexture.internalTexture = TransitionSet;
            spriteTexture.Frame = instance.Frame;
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
