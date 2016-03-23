using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileLayer))]
public class LevelScriptEditor : Editor
{
    private Vector3 cursorPosition = Vector3.zero;
    private Vector2Int tileCursorPosition = Vector2Int.Zero;

    void OnSceneGUI()
    {
        var e = Event.current;
        var controlId = GUIUtility.GetControlID(FocusType.Passive);
        var layerTarget = (TileLayer)target;

        if (e.type == EventType.MouseDown)
        {
            GUIUtility.hotControl = controlId;

            layerTarget.AddTile(tileCursorPosition, 0);
            EditorUtility.SetDirty(layerTarget);

            e.Use();
            Debug.Log("Mouse down");
        }
        else if (e.type == EventType.MouseMove)
        {
            float rayDistance;
            var plane = new Plane(Vector3.back, layerTarget.transform.position);
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (plane.Raycast(ray, out rayDistance))
            {
                cursorPosition = ray.GetPoint(rayDistance);
                tileCursorPosition = new Vector2Int(Mathf.RoundToInt(cursorPosition.x / 32.0f), Mathf.RoundToInt(cursorPosition.y / 32.0f));
                SceneView.RepaintAll();
            }
        }

        Handles.color = Color.red;
        //Handles.DotCap(0, cursorPosition, Quaternion.identity, 4);
        Handles.DotCap(0, new Vector2(tileCursorPosition.x * 32.0f, tileCursorPosition.y * 32.0f), Quaternion.identity, 4);

    }
}