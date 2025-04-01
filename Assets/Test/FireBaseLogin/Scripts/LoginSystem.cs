using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class LoginSystem : MonoBehaviour
{

    public InputField email;
    public InputField password;

    public Text outputText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.Init();
    }

    public void OnChangedState(bool sign) {
        outputText.text = sign? "로그인: " : "로그아웃 :";
        outputText.text += FirebaseAuthManager.Instance.UserId;
    }
    public void Create() {
        string e = email.text;
        string p = password.text;

        FirebaseAuthManager.Instance.Create(e, p);

    }

    public void LogIn() { 
        FirebaseAuthManager.Instance.Login(email.text, password.text);
    }

    public void LogOut() {
        FirebaseAuthManager.Instance.LogOut();
    }
}
