using LightDI.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LightDI.Editor
{
    internal sealed class SystemsCreator : EditorWindow
    {
        private string _className = "";
        private bool _createSystemBtnClicked;

        [MenuItem("Tools/Light DI/Create System")]
        public static void ShowWindow()
        {
            GetWindow<SystemsCreator>("Systems Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create System class", EditorStyles.boldLabel);

            var prevClassName = _className;

            _className = EditorGUILayout.TextField("Class Name", _className);

            if (_className != prevClassName)
            {
                _createSystemBtnClicked = false;
            }

            if (GUILayout.Button("Create System"))
            {
                CreateSystem();
                _createSystemBtnClicked = true;
            }

            if (_createSystemBtnClicked && IsValidClassName(_className))
            {
                if (GUILayout.Button("Instantiate System on scene"))
                {
                    InstantiateSystem();
                }
            }
        }

        private void CreateSystem()
        {
            // Ensure the class name is valid
            if (!IsValidClassName(_className))
            {
                Debug.LogError("Invalid class name. Please provide a valid class name.");
                return;
            }

            // Define the script content
            var scriptContent = $@"using LightDI;
using UnityEngine;

public class {_className} : SystemBase
{{

}}
";

            // Define the path where the script will be saved
            var path = "Assets/" + _className + ".cs";

            // Check if the file already exists
            if (File.Exists(path))
            {
                Debug.LogError($"A script with the name '{_className}' already exists.");
                return;
            }

            // Create the script file and write the content
            File.WriteAllText(path, scriptContent);
            AssetDatabase.Refresh(); // Refresh the AssetDatabase so Unity recognizes the new file

            Debug.Log($"Script '{_className}.cs' has been created at {path}");

            Repaint();
        }

        private void InstantiateSystem()
        {
            var scriptType = TypeUtility.GetTypeByName(_className);

            if (scriptType != null)
            {
                // Create a new GameObject and add the script as a component
                var newObject = new GameObject(_className);
                newObject.AddComponent(scriptType);
                AssetDatabase.Refresh();
            }
        }

        // Simple check for valid class name (can be expanded if needed)
        private static bool IsValidClassName(string className)
        {
            return !string.IsNullOrEmpty(className) && char.IsLetter(className[0]) && !className.Contains(" ");
        }
    }
}
