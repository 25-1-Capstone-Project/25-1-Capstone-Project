using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System;
using Firebase.Extensions; // 🔥 중요! MainThread에서 콜백 실행을 위해 꼭 필요함

public class FirebaseAuthManager : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;
    public Button loginButton;

    void Start()
    {
        loginButton.interactable = false;  // 초기에는 비활성화

        FirebaseInit init = FindFirstObjectByType<FirebaseInit>();
        if (init != null)
        {
            init.OnFirebaseInitialized += () =>
            {
                loginButton.interactable = true;  // 초기화 끝나면 버튼 활성화
                Debug.Log("Firebase 초기화 완료, 로그인 버튼 활성화됨.");
            };
        }
        else
        {
            Debug.LogError("FirebaseInit 스크립트를 찾을 수 없습니다.");
        }

        loginButton.onClick.AddListener(LoginUser);
    }

    void LoginUser()
    {
        Debug.Log("로그인 버튼 클릭됨"); // 디버깅 로그

        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (FirebaseInit.auth == null)
        {
            Debug.LogError("Firebase가 아직 초기화되지 않았습니다.");
            return;
        }

        FirebaseInit.auth.SignInWithEmailAndPasswordAsync(email, password)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인 취소됨.");
                return;
            }

            if (task.IsFaulted)
            {
                AggregateException ex = task.Exception;
                foreach (var e in ex.InnerExceptions)
                {
                    Debug.LogError("로그인 실패: " + e.Message);
                }
                return;
            }

            if (task.IsCompletedSuccessfully)
            {
                var result = task.Result;
                FirebaseUser user = result.User;

                Debug.LogFormat("로그인 성공: {0} ({1})", user.DisplayName, user.Email);

                // 로그인 성공하면 Bootstrap 씬으로 이동
                Debug.Log("Bootstrap 씬으로 이동합니다.");
                
                SceneManager.LoadScene("Bootstrap");
            }
        });
    }
}
