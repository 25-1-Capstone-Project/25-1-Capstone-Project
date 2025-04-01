using Firebase.Auth;
using UnityEngine;
using Firebase.Analytics;
using System.Threading.Tasks;
using System;


public class FirebaseAuthManager
{

    private static FirebaseAuthManager instance = null;

    public static FirebaseAuthManager Instance {
        get {
            if (instance == null) {
                instance = new FirebaseAuthManager();
            }
            return instance;
        }
    }
    private FirebaseAuth auth;
    private FirebaseUser user;

    public string UserId => user.UserId;

    public Action<bool> LoginState;

    public void Init() {
        auth = FirebaseAuth.DefaultInstance;

        //임시방편
        if (auth.CurrentUser != null) {
            LogOut();
        }

        auth.StateChanged += OnChanged;
    }

    public void OnChanged(object sender, EventArgs e) {
        if (auth.CurrentUser != user) {
            bool signed = (auth.CurrentUser != user && auth.CurrentUser != null);
            if (!signed && user != null) {
                Debug.Log("");
            }

            user = auth.CurrentUser;
            if (signed) {
                Debug.Log("로그인");
            }
        }
    }

    public void Create(string email, string password) {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(Task => {
            if (Task.IsCanceled) {
                Debug.Log("회원가입 취소");
                return;
            }
            if (Task.IsFaulted) {
                Debug.Log("회원가입 실패");
                return;
            }

            AuthResult authResult = Task.Result;
            FirebaseUser newUser = authResult.User;
        });
    }

    public void Login(string email, string password) {
        auth.SignInWithEmailAndPasswordAsync (email, password).ContinueWith(Task => {
            if (Task.IsCanceled) {
                Debug.Log("로그인 취소");
                return;
            }
            if (Task.IsFaulted) {
                Debug.Log("로그인 실패");
                return;
            }

            AuthResult authResult = Task.Result;
            FirebaseUser newUser = authResult.User;
        });
    }

    public void Logout(string email, string password) {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(Task => {
            if (Task.IsCanceled) {
                Debug.Log("회원가입 취소");
                return;
            }
            if (Task.IsFaulted) {
                Debug.Log("회원가입 실패");
                return;
            }

            AuthResult authResult = Task.Result;
            FirebaseUser newUser = authResult.User;
        });
    }

    public void LogOut() {
        auth.SignOut();
        Debug.Log("로그아웃");
    }
    


}
