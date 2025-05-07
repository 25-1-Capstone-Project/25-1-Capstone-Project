using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/MapData")]
public class MapReference : ScriptableObject
{

    public SceneAsset[] sceneList;

    public List<string> sceneNames;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var scene in sceneList)

            if (!sceneNames.Contains(scene.name))
                sceneNames.Add(scene.name);
    }
#endif
}