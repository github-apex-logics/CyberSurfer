using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefab : EditorWindow
{
    public GameObject prefab;
    public string targetTag = "PowerUp"; // Optional: filter by tag

    [MenuItem("Tools/Replace With Prefab")]
    static void Init()
    {
        GetWindow<ReplaceWithPrefab>("Replace With Prefab");
    }

    void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        targetTag = EditorGUILayout.TextField("Target Tag", targetTag);

        if (GUILayout.Button("Replace All"))
        {
            if (prefab == null) return;

            GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);
            foreach (GameObject obj in objs)
            {
                // Instantiate prefab
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, obj.transform.parent);

                // Keep transform values
                newObj.transform.localPosition = obj.transform.localPosition;
                newObj.transform.localRotation = obj.transform.localRotation;
                newObj.transform.localScale = obj.transform.localScale;

                Undo.RegisterCreatedObjectUndo(newObj, "Replace With Prefab");
                Undo.DestroyObjectImmediate(obj);
            }
        }
    }
}
