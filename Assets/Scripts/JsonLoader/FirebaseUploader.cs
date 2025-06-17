using Firebase.Database;
using UnityEngine;
using System.Threading.Tasks;

public class FirebaseUploader : MonoBehaviour
{
    public void UploadLogToFirebase()
    {
        var log = PlayerLogger.Instance.GetLogData();
        string json = JsonUtility.ToJson(log);

        FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("logs")
            .Child(log.user_id)
            .SetRawJsonValueAsync(json)
            .ContinueWith(task => 
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Firebase 업로드 실패: " + task.Exception);
                }
                else
                {
                    Debug.Log("Firebase 업로드 성공: 로그 저장 완료!");
                }
            });
    }
}
