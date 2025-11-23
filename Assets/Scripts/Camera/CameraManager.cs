using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float duration = 1f;
    [SerializeField] CinemachineCamera cineCam;
    private CinemachineBasicMultiChannelPerlin noise;
    protected override void Awake()
    {
        base.Awake();
        noise = cineCam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();

        // 초기값 보정
        if (noise != null)
        {
            noise.AmplitudeGain = 0f;
            noise.FrequencyGain = 0f;
        }

        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(0, 0, mainCamera.transform.position.z); // 초기 카메라 위치 설정
    }
    public void SetCameraPosition(Vector3 position)
    {
        mainCamera.transform.position = new Vector3(position.x, position.y, mainCamera.transform.position.z);

    }
    public IEnumerator LerpCameraPosition(Vector3 position)
    {
        float elapsedTime = 0f;
        Vector3 start = mainCamera.transform.position;
        Vector3 target = new Vector3(position.x, position.y, start.z);

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(start, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = target; // 마지막 보정
    }

    public void CameraShake(float shakeIntensity, float shakeDuration)
    {
        StopAllCoroutines(); // 기존 코루틴 중복 방지
        StartCoroutine(DoCameraShake(shakeIntensity, shakeDuration));
    }
    private IEnumerator DoCameraShake(float shakeIntensity, float shakeDuration)
    {
        noise.AmplitudeGain = shakeIntensity;
        noise.FrequencyGain = 2f; // 빠른 진동 (짧고 강하게)

        yield return new WaitForSeconds(shakeDuration);

        noise.AmplitudeGain = 0f; // 원래 상태 복원
        noise.FrequencyGain = 0f;
    }
    public void SetActiveCineCam(bool active)
    {
        if (active == true)
        {
            cineCam.Follow = GameManager.Instance.playerScript.GetPlayerTransform();
            cineCam.gameObject.SetActive(true);
        }
        else { cineCam.gameObject.SetActive(false); }

    }
    public void SetLensSize(float size)
    {
        cineCam.Lens.OrthographicSize = size;
    }
}
