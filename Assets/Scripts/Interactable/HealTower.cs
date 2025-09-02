using UnityEngine;

public class HealTower : MonoBehaviour, Interactable {
    public void Interact()
    {
        PlayerScript.Instance.SetMaxHealth();
    }
}
