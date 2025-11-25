using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShaderManager : Singleton<ShaderManager>
{
    [SerializeField] private ShockWaveController shockwave;
    [SerializeField] private BlackScreenController blackScreen;
    [SerializeField] private VolumeProfile volumeProfile;
    protected override void Awake()
    {
        base.Awake();
        blackScreen.CallBlackScreen(false);
        shockwave.CallShockWave(Vector2.zero);
        SetVignette();
    }
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     CallShockWave();
        // }

    }
    public void CallShockWave()
    {
        Vector2 point = Camera.main.WorldToViewportPoint( GameManager.Instance.playerScript.GetPlayerTransform().position);
        shockwave.CallShockWave(point);
    }

    public void SetBlackScreen(bool enabled)
    {
        blackScreen.CallBlackScreen(enabled);
    }
    public void SetVignette(float intensity = 0.2f, Color color = default)
    {
        if (volumeProfile.TryGet<Vignette>(out var vignette))
        {
            vignette.color.value = color;
            vignette.intensity.value = intensity;
        }
    }
}
