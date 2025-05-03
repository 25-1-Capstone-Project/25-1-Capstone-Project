using UnityEngine;
using System.Collections;
public class CameraManager : Singleton<CameraManager>
{
    public Camera mainCamera;
    public float duration = 1f;
    public float cameraZ = -10f; // 카메라의 Z축 위치
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

            mainCamera.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }
}
