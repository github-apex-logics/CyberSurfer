//#if UNITY_EDITOR
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//[InitializeOnLoad]
//public class HierarchyColorizer
//{
//    static HierarchyColorizer()
//    {
//        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
//    }

//    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
//    {
//        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
//        if (obj == null) return;

//        // Set your color logic here
//        Color color = Color.white;
//        if (obj.CompareTag("Enemy")) color = Color.red;
//        else if (obj.CompareTag("Player")) color = Color.green;

//        // Draw selection background
//        if (Selection.instanceIDs.Contains(instanceID))
//            EditorGUI.DrawRect(selectionRect, new Color(0.24f, 0.48f, 0.90f, 0.3f)); // blue highlight

//        // Create style with custom color
//        GUIStyle style = new GUIStyle(EditorStyles.label);
//        style.normal.textColor = color;

//        // Draw your label manually, shifting slightly so it replaces Unity's label
//        Rect offsetRect = new Rect(selectionRect.x + 17f, selectionRect.y -1.45f, selectionRect.width, selectionRect.height);
//        EditorGUI.LabelField(offsetRect, obj.name, style);
//    }
//}
//#endif
