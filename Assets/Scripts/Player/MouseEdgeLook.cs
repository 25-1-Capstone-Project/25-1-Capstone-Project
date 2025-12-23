using UnityEngine;
using Unity.Cinemachine;

public class MouseEdgeLook : MonoBehaviour
{
    [SerializeField] CinemachineCamera cmCam;   
    [Header("Look")]
    [SerializeField] float maxOffset = 2.0f; 
    [SerializeField, Range(0f, 0.5f)] float deadZone = 0.22f; 
    [SerializeField] float followSpeed = 10f;
    [SerializeField] bool lockY = false;

    CinemachinePositionComposer composer;
    Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        composer = cmCam.GetComponent<CinemachinePositionComposer>();
    }

    void LateUpdate()
    {
        if (composer == null || mainCam == null) return;

        Vector2 m = mainCam.ScreenToViewportPoint(Input.mousePosition); 
        Vector2 c = new Vector2(0.5f, 0.5f);

        Vector2 v = m - c;
        float dist = v.magnitude;

        Vector3 desired = Vector3.zero; // 플레이어 추적

        if (dist > deadZone)
        {
            float t = Mathf.InverseLerp(deadZone, 0.5f, dist); // deadZone 밖부터 0..1
            Vector2 dir = v.normalized;

            float y = lockY ? 0f : dir.y;
            desired = new Vector3(dir.x * maxOffset * t, y * maxOffset * t, 0f);
        }

        // Position Composer의 Target Offset 보간
        composer.TargetOffset = Vector3.Lerp(
            composer.TargetOffset,
            desired,
            Time.unscaledDeltaTime * followSpeed
        );
    }
}
