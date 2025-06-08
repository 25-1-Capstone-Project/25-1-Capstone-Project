using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float cameraZ = -10f; // 카메라의 Z축 위치
    [SerializeField] CinemachineCamera cineCam;
    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(0, 0, cameraZ); // 초기 카메라 위치 설정
    }
    public void SetCameraPosition(Vector3 position)
    {
        mainCamera.transform.position = new Vector3(position.x, position.y, cameraZ);

    }
    public IEnumerator LerpCameraPosition(Vector3 position)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = mainCamera.transform.position;
        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, new Vector3(position.x, position.y, cameraZ), elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void CameraShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }
    public void SetActiveCineCam(bool active)
    {
        if (active == true)
        {
            cineCam.Follow = PlayerScript.Instance.GetPlayerTransform();
            cineCam.gameObject.SetActive(true);
        }
        else { cineCam.gameObject.SetActive(false); }

    }
}
