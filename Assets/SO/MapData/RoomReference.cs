using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomReference : ScriptableObject
{
    public string name;
    public GameObject[] Level1_Rooms;
    public GameObject[] Level2_Rooms;
    public GameObject[] Level3_Rooms;
    public GameObject[] BossRooms;
    public GameObject[] ShopRooms;
    public GameObject StartRoom;
    public GameObject GetRandomRoom(int depth)
    {
        return SelectLevelRoom(depth);
    }
    public GameObject GetStartRoom()
    {
        return StartRoom;
    }

    private GameObject SelectLevelRoom(int depth)
    {
        if (depth > 4)
            return Level3_Rooms[Random.Range(0, Level3_Rooms.Length)];
        else if (depth > 2)
            return Level2_Rooms[Random.Range(0, Level2_Rooms.Length)];
        else if (depth >= 0)
            return Level1_Rooms[Random.Range(0, Level1_Rooms.Length)];


        return null;
    }
}

