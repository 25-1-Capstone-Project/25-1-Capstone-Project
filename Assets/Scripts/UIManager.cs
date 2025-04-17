using UnityEngine;


public class UIManager : MonoBehaviour
{
  
    public PlayerStatUI playerStatUI;
    public ParryStackUI parryStackUI;
  
    public static UIManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        playerStatUI.UI_ParryCooldownUpdate();
       // playerStatUI.UI_ParryStackUpdate();
        playerStatUI.UI_HPBarUpdate();
    }

}
