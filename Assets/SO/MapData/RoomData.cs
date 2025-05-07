using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomData : ScriptableObject
{
    public string name;
    public GameObject[] Rooms;

    public GameObject GetRandomRoom(){
        return Rooms[Random.Range(0,Rooms.Length)];
    }
}
