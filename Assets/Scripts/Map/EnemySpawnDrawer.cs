#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(EnemySpawn))]
public class EnemySpawnDrawer : PropertyDrawer
{
    const float BtnHeight = 20f;
    const float Extra = 22f;
    private const string MARKER_PREFAB_PATH = "Assets/Prefab/Enemy/EnemySpawnMaker.prefab";
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => EditorGUI.GetPropertyHeight(property, label, true) + Extra;

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, prop);

        var fieldRect = new Rect(pos.x, pos.y, pos.width, pos.height - Extra);
        EditorGUI.PropertyField(fieldRect, prop, label, true);

        var enemyProp = prop.FindPropertyRelative("enemyData");
        var markerProp = prop.FindPropertyRelative("marker");

        int waveIdx0 = GetWaveIndex(prop);
        int waveNo = (waveIdx0 >= 0) ? (waveIdx0 + 1) : -1;

        var btnRect = new Rect(pos.x, pos.yMax - BtnHeight, pos.width, BtnHeight);

        using (new EditorGUI.DisabledScope(enemyProp.objectReferenceValue == null))
        {
            // 항상 "생성/동기화" 방식, 선택/핑 없음
            var btnLabel = "스폰 마커 생성/동기화";
            if (GUI.Button(btnRect, btnLabel))
            {
                if (markerProp.objectReferenceValue != null)
                {
                    // 이미 있으면 동기화만
                    var marker = markerProp.objectReferenceValue as EnemySpawnMarker;
                    SyncMarker(marker, enemyProp, waveNo);
                }
                else
                {
                    // 없으면 생성
                    CreateMarker(prop, waveNo);
                }
            }
        }

        EditorGUI.EndProperty();
    }

    void CreateMarker(SerializedProperty spawnProp, int waveNo)
    {
        var targetObj = spawnProp.serializedObject.targetObject;
        var room = targetObj as Stage;

        Transform parent = null;
        if (room != null)
        {
            if (room.markerRoot == null)
            {
                var holder = new GameObject("Markers");
                Undo.RegisterCreatedObjectUndo(holder, "Create Marker Root");
                holder.transform.SetParent(room.transform, false);
                room.markerRoot = holder.transform;
                EditorUtility.SetDirty(room);
            }
            parent = room.markerRoot;
        }
        if (parent == null) parent = Selection.activeTransform;

        // 프리펩에서 인스턴스 생성
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(MARKER_PREFAB_PATH);
        if (prefab == null)
        {
            Debug.LogError($"프리펩을 찾을 수 없습니다: {MARKER_PREFAB_PATH}");
            return;
        }

        GameObject markerGO = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        Undo.RegisterCreatedObjectUndo(markerGO, "Create Enemy Spawn Marker");

        Vector3 spawnPos = SceneView.lastActiveSceneView ? SceneView.lastActiveSceneView.pivot : Vector3.zero;
        markerGO.transform.position = spawnPos;

        var marker = markerGO.GetComponent<EnemySpawnMarker>();
        if (marker == null)
        {
            Debug.LogError("프리펩에 EnemySpawnMarker 컴포넌트가 없습니다");
            return;
        }

        var enemyProp = spawnProp.FindPropertyRelative("enemyData");
        SyncMarker(marker, enemyProp, waveNo);

        var markerProp = spawnProp.FindPropertyRelative("marker");
        markerProp.objectReferenceValue = marker;

        spawnProp.serializedObject.ApplyModifiedProperties();
    }

    void SyncMarker(EnemySpawnMarker marker, SerializedProperty enemyProp, int waveNo)
    {
        var enemySO = enemyProp.objectReferenceValue as EnemyDataBase;
        var enemyName = enemySO ? (string.IsNullOrEmpty(enemySO.Name) ? enemySO.name : enemySO.Name) : "Enemy";

        string waveTag = (waveNo > 0) ? $"W{waveNo}" : "W?";
        string label = $"{waveTag}-{enemyName}";

        if (marker.label != label)
        {
            marker.label = label;
            EditorUtility.SetDirty(marker);
        }

        if (marker.gameObject.name != label)
        {
            Undo.RecordObject(marker.gameObject, "Rename Spawn Marker");
            marker.gameObject.name = label;
            EditorUtility.SetDirty(marker.gameObject);
        }

        marker.waveColor = GetWaveColor(waveNo);
        EditorUtility.SetDirty(marker);
    }

    int GetWaveIndex(SerializedProperty prop)
    {
        var m = Regex.Match(prop.propertyPath, @"waves\.Array\.data\[(\d+)\]");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int i)) return i;
        return -1;
    }

    // 무지개 팔레트 (빨, 주, 노, 초, 파, 남, 보)
    static readonly Color[] kRainbow = new Color[]
    {
        new Color(1.00f, 0.00f, 0.00f), // 빨강
        new Color(1.00f, 0.50f, 0.00f), // 주황
        new Color(1.00f, 1.00f, 0.00f), // 노랑
        new Color(0.00f, 1.00f, 0.00f), // 초록
        new Color(0.00f, 0.00f, 1.00f), // 파랑
        new Color(0.06f, 0.20f, 0.65f), // 남색
        new Color(0.56f, 0.00f, 1.00f), // 보라
    };

    Color GetWaveColor(int waveNo)
    {
        if (waveNo <= 0) return Color.white;
        int idx = (waveNo - 1) % kRainbow.Length;
        return kRainbow[idx];
    }
}
#endif
