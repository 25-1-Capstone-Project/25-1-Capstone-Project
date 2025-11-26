
using UnityEngine;

enum ECursorType
{
    Default,
    Aim,
    Action
}

public class CursorManager : Singleton<CursorManager>
{
    private int currentType = (int)ECursorType.Default;

    [SerializeField] Texture2D[] cursorTextures;
    protected override void Awake()
    {
        base.Awake();

    }

    public void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
    }
    
    public void SetCursorIcon(int cursorNum, bool calculateHotspot = false)
    {
        if (currentType == cursorNum) return;
        currentType = cursorNum;

        Vector2 hotspot = Vector2.zero;
        Texture2D cursorTexture = cursorTextures[cursorNum];
        if(calculateHotspot)
        hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        var hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            Vector2.zero
        );

        if (hit.collider != null)
        {
            var enemy = hit.collider.GetComponent<EnemyBase>();
            if (enemy != null && enemy.CanBeExecuted())
            {
                SetCursorIcon((int)ECursorType.Action, true);
                return;
            }
        }

        SetCursorIcon((int)ECursorType.Aim, true);
    }


}
