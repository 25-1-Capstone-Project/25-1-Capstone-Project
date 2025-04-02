using UnityEngine;


public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerStatUI playerStatUI;
    void Start()
    {
        playerStatUI = GetComponent<PlayerStatUI>();
    }
    private void LateUpdate()
    {
        playerStatUI.UI_ParryCooldownUpdate();
        playerStatUI.UI_ParryStackUpdate();
        playerStatUI.UI_HPBarUpdate();
    }

}
