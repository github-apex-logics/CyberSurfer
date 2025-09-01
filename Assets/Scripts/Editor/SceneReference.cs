using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SceneReference
{
    [SerializeField] private UnityEngine. Object sceneAsset;
    [SerializeField] private string scenePath;

    public string ScenePath => scenePath;

#if UNITY_EDITOR
    public void UpdatePath()
    {
        if (sceneAsset != null)
        {
            scenePath = AssetDatabase.GetAssetPath(sceneAsset);
        }
        else
        {
            scenePath = string.Empty;
        }
    }
#endif
}
