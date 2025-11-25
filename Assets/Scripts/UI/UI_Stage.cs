using UnityEngine;

public class UI_Stage : MonoBehaviour
{
    public void OnClickRoomSelect()
    {
        if(StageManager.Instance.isLoading) return;

        StageManager.Instance.isLoading = true;
        int index = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        GameManager.Instance.ChangeStateByEnum(EGameState.Room);
        StageManager.Instance.SelectRoom(index-1);
    }
}
