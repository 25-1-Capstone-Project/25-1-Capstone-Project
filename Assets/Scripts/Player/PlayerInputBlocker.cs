using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 플레이어의 입력을 차단하는 스크립트입니다.
/// </summary>
public static class PlayerInputBlocker
{
    static bool isBlocked = false;

    public static bool IsBlocked() => isBlocked;
    public static void Block(bool state)
    {
        isBlocked = state;
        PlayerInput input = PlayerScript.instance.GetComponent<PlayerInput>();
        input.enabled = !state;
    }
}
