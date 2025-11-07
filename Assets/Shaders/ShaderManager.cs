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
        if (Input.GetKeyDown(KeyCode.V))
        {
            CallBlackScreen(true);
        }
              if (Input.GetKeyDown(KeyCode.B))
        {
            CallBlackScreen(false);
        }
    }
    public void CallShockWave()
    {
        Vector2 point = Camera.main.WorldToViewportPoint(PlayerScript.Instance.GetPlayerTransform().position);
        shockwave.CallShockWave(point);
    }

    public void CallBlackScreen(bool enabled)
    {
        blackScreen.CallBlackScreen(enabled);
    }

}
