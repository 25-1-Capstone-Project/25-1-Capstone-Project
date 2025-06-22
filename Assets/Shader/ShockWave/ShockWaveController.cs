using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShockWaveController : MonoBehaviour
{
    [SerializeField] private float _shockWaveTime = 0;
    private Coroutine coroutine;
     [SerializeField] private Material _material;

    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
    private static int _RingSpawnPosition = Shader.PropertyToID("_RingSpawnPosition");

    void Awake()
    {
        
       // _material = GetComponent<SpriteRenderer>().material;

    }

    public void CallShockWave(Vector2 startPos)
    {
        _material.SetVector(_RingSpawnPosition, startPos);
        coroutine = StartCoroutine(ShockWaveAction(-0.1f, 1));

    }
    private IEnumerator ShockWaveAction(float start, float end)
    {
        _material.SetFloat(_waveDistanceFromCenter, start);

        float lerpedAmount = 0;
        float elapsedTime = 0;
        while (elapsedTime < _shockWaveTime)
        {
            elapsedTime += Time.deltaTime;
            lerpedAmount = Mathf.Lerp(start, end, elapsedTime / _shockWaveTime);
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);

            yield return null;
        }
         _material.SetFloat(_waveDistanceFromCenter, -0.1f);
    }
}
