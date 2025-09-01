#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        SerializedProperty scenePathProp = property.FindPropertyRelative("scenePath");

        EditorGUI.BeginProperty(position, label, property);
        var objFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginChangeCheck();
        EditorGUI.ObjectField(objFieldRect, sceneAssetProp, typeof(SceneAsset), label);
        if (EditorGUI.EndChangeCheck())
        {
            if (sceneAssetProp.objectReferenceValue != null)
            {
                var path = AssetDatabase.GetAssetPath(sceneAssetProp.objectReferenceValue);
                scenePathProp.stringValue = path;
            }
            else
            {
                scenePathProp.stringValue = string.Empty;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
