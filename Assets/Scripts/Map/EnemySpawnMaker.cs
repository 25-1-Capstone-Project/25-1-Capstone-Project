using UnityEngine;

public class EnemySpawnMarker : MonoBehaviour
{
    public string label;
    public Color waveColor = Color.white; // wave에 따라 자동 설정됨

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = waveColor;
        Gizmos.DrawWireSphere(transform.position, 0.28f);

        UnityEditor.Handles.color = waveColor;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.35f, label);
    }
#endif
}
