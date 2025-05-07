using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomReference : ScriptableObject
{
    public string name;
    public GameObject[] Rooms;
    public GameObject StartRoom;
    public GameObject GetRandomRoom()
    {
        return Rooms[Random.Range(0, Rooms.Length)];
    }
    public GameObject GetStartRoom()
    { return StartRoom; }
}
