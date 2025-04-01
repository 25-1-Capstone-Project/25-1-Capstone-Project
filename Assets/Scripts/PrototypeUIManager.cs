using UnityEngine;
using UnityEngine.UI;

public class PrototypeUIManager : MonoBehaviour
{
    public Player player;
    public Text parryStackText;

    private void Update()
    {
        parryStackText.text = player.ParryStack.ToString();
    }
}
