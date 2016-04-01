using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileRender))]
[CanEditMultipleObjects]
public class TileRenderEditor : Editor
{
    private TileSet SelectedTileSet = null;

    private Vector3 cursorPosition = Vector3.zero;
    private Vector2Int tileCursorPosition = Vector2Int.Zero;
    private Plane plane = new Plane();
    private bool createdPlane = false;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var tileRender = (TileRender)target;
        if (tileRender.AvailableTilesets.Count == 0)
        {
            EditorGUILayout.LabelField("You need to add some tilesets!");
        }
        else
        {
            foreach (var tileSet in tileRender.AvailableTilesets)
            {
                if (EditorGUILayout.ToggleLeft(tileSet.Name, tileSet == SelectedTileSet))
                {
                    SelectedTileSet = tileSet;
                }
            }
        }

        if (GUILayout.Button("Refesh"))
        {
            EditorUtility.SetDirty(tileRender);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        var e = Event.current;
        var controlId = GUIUtility.GetControlID(FocusType.Passive);
        var tileRender = (TileRender)target;

        if (!createdPlane)
        {
            plane = new Plane(Vector3.back, tileRender.transform.position);
            plane = new Plane(Vector3.back, Vector3.zero);
            createdPlane = true;
        }

        if (e.type == EventType.MouseDown && SelectedTileSet != null)
        {
            GUIUtility.hotControl = controlId;

            var layer = tileRender.GetLayerForTileSet(SelectedTileSet);
            layer.AddTile(tileCursorPosition, 0);
            EditorUtility.SetDirty(layer);

            e.Use();
        }
        else if (e.type == EventType.MouseMove)
        {
            float rayDistance;
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (plane.Raycast(ray, out rayDistance))
            {
                cursorPosition = ray.GetPoint(rayDistance);
                tileCursorPosition = new Vector2Int(Mathf.RoundToInt(cursorPosition.x / 32.0f), Mathf.RoundToInt(cursorPosition.y / 32.0f));
                SceneView.RepaintAll();
            }
        }

        if (SelectedTileSet == null || !SelectedTileSet.CenterTiles.IsTexture)
        {
            Handles.color = new Color(0.35f, 0.4f, 0.8f, 0.5f);
            Handles.DotCap(0, new Vector2(tileCursorPosition.x * 32.0f, tileCursorPosition.y * 32.0f), Quaternion.identity, 16);
        }
        else
        {
            var texture = SelectedTileSet.CenterTiles.Texture;
            var positionRect = new Rect(tileCursorPosition.x * 32.0f - 16.0f, tileCursorPosition.y * 32.0f - 16.0f, 32.0f, 32.0f);
            var drawRect = new Rect(0, 0, 32.0f / (float)texture.width, 32.0f / (float)texture.height);
            Graphics.DrawTexture(positionRect, texture, drawRect, 0, 0, 0, 0, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }
    }
}
