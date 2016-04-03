using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileSet))]
[CanEditMultipleObjects]
public class TileSetEditor : Editor {

    private Texture2D ProcessableTexture;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var tileset = (TileSet)target;

        tileset.Name = EditorGUILayout.TextField("Name", tileset.Name);
        tileset.ZOrdering = EditorGUILayout.FloatField("Z-Order", tileset.ZOrdering);

        SpriteTextureField(ref tileset.CenterTiles, "Center Tile");
        EditorGUILayout.LabelField("Edge tiles");
        for (var i = 1; i < 16; i++)
        {
            var flag = (TileSet.EdgeFlags)i;
            SpriteTextureField(ref tileset.EdgeTiles[i], flag.ToString());
        }

        EditorGUILayout.LabelField("Corner tiles");
        for (var i = 1; i < 16; i++)
        {
            var flag = (TileSet.CornerFlags)i;
            SpriteTextureField(ref tileset.CornerTiles[i], flag.ToString());
        }

        ProcessableTexture = EditorGUILayout.ObjectField("Process From Texture", ProcessableTexture, typeof(Texture2D), allowSceneObjects: true) as Texture2D;
        if (GUILayout.Button("Process"))
        {
            Debug.Log("Processed! : " + ProcessableTexture);
            ProcessTexture();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SpriteTextureField(ref TileSet.CommonTexture output, string label)
    {
        var result = EditorGUILayout.ObjectField(label, output != null ? output.Value : null , typeof(Object), allowSceneObjects: true);
        if (result is Texture2D)
        {
            if (output == null)
            {
                output = new TileSet.CommonTexture();
            }
            output.Texture = result as Texture2D;
        }
        else if (result is Sprite)
        {
            if (output == null)
            {
                output = new TileSet.CommonTexture();
            }
            output.Texture = null;
            output.Sprite = result as Sprite;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Animated");
        output.Animated = EditorGUILayout.Toggle(output.Animated);
        output.FrameRate = EditorGUILayout.FloatField(output.FrameRate);
        EditorGUILayout.EndHorizontal();
    }

    private void ProcessTexture()
    {
        var tileset = (TileSet)target;

        var sprites = Resources.LoadAll<Sprite>(ProcessableTexture.name);
        for (var i = 0; i < Mathf.Min(16, sprites.Length); i++)
        {
            if (tileset.EdgeTiles[i] == null)
            {
                tileset.EdgeTiles[i] = new TileSet.CommonTexture();
            }
            tileset.EdgeTiles[i].Sprite = sprites[i];
        }
        for (int i = 15, j = 0; i < Mathf.Min(32, sprites.Length); i++, j++)
        {
            if (tileset.CornerTiles[j] == null)
            {
                tileset.CornerTiles[j] = new TileSet.CommonTexture();
            }
            tileset.CornerTiles[j].Sprite = sprites[i];
        }
    }
}
