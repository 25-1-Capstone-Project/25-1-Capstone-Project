using UnityEngine;

public class ShaderManager : Singleton<ShaderManager>
{
    [SerializeField] private ShockWaveController shockwave;
    [SerializeField] private BlackScreenController blackScreen;
    protected override void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CallShockWave();
        }
       
    }
    public void CallShockWave()
    {
        Vector2 point = Camera.main.WorldToViewportPoint(PlayerScript.Instance.GetPlayerTransform().position);
        shockwave.CallShockWave(point);
    }

    public void SetBleackScreen(bool enabled)
    {
        blackScreen.CallBlackScreen(enabled);
    }

}
