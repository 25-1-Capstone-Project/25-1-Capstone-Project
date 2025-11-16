
using System;
using UnityEngine;

enum ECursorType
{
    Default,
    Aim,
 
}
public class CursorManager : Singleton<CursorManager>
{
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
        Vector2 hotspot = Vector2.zero;
        Texture2D cursorTexture = cursorTextures[cursorNum];
        if(calculateHotspot)
        hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}
