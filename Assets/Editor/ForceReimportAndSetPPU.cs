using UnityEditor;
using UnityEngine;
using System.IO;

public class ForceReimportAndSetPPU
{
    [MenuItem("Tools/Set PPU By Folder")]
    static void SetPPUByFolder()
    {
        // 기준 폴더 경로 (Assets 아래 기준)
        string targetFolder = "Assets/AssetStore/Epic RPG Collection/";
        int targetPPU = 32;

        string[] pngFiles = Directory.GetFiles(targetFolder, "*.png", SearchOption.AllDirectories);

        foreach (string fullPath in pngFiles)
        {
            string assetPath = fullPath.Replace('\\', '/'); // Windows 호환

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = targetPPU;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;

                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                
            }
        }

        AssetDatabase.Refresh();
    }
}
