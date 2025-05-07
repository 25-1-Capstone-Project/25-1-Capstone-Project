using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : Singleton<MinimapManager>
{

    [SerializeField] Transform minimapContainer; // minimap용 UI 부모 (예: Canvas 아래)
    [SerializeField] GameObject minimapIconPrefab; // minimap에 표시할 작은 아이콘
    [SerializeField] float roomGap;

    private Dictionary<Vector2Int, GameObject> minimapIcons = new Dictionary<Vector2Int, GameObject>();
    public void RegisterRoom(Vector2Int roomPos)
    {
        GameObject icon = Instantiate(minimapIconPrefab, minimapContainer);
        icon.transform.localPosition = new Vector3(roomPos.x * roomGap, roomPos.y * roomGap, 0); // 20f = minimap 격자 간격
        icon.SetActive(false);
        minimapIcons.Add(roomPos, icon);
    }

    public void RevealRoom(Vector2Int roomPos)
    {
        if (minimapIcons.TryGetValue(roomPos, out GameObject icon))
        {
            icon.SetActive(true);
        }
    }
}