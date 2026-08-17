using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using TMPro;
using UnityEngine;

public class LoginManager : SingletonBehaviour<LoginManager>, IManager
{
    private FirebaseAuth _auth;
    private TMP_Text _textLog;

    private Action _onLoginSuccess = null;

    void Start()
    {
#if UNITY_EDITOR || DEBUG
        PlayGamesPlatform.DebugLogEnabled = true;
#endif
        PlayGamesPlatform.Activate();

        _auth = FirebaseAuth.DefaultInstance;
        _textLog = null;

        SetLog(string.Empty);
    }

    public void Login(TMP_Text textLog, Action onLoginSuccess)
    {
        _textLog = textLog;
        _onLoginSuccess = onLoginSuccess;

        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuth);
    }

    private void ProcessAuth(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            SetLog("Login...");
            GetServerSideAccess();
        }
        else
        {
            SetLog("Login Fail");
        }
    }

    private void GetServerSideAccess()
    {
        try
        {
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, authCode =>
            {
                FirebaseLogin(authCode);
            });
        }
        catch (System.Exception e)
        {
            SetLog(e.ToString());
        }
    }

    private void FirebaseLogin(string authCode)
    {
        try
        {
            Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

            _auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                    SetLog("Login Cancel");
                else if (task.IsFaulted)
                    SetLog("Login Fail");
                else
                {
                    SetLog("Login Success");
                    _onLoginSuccess?.Invoke();
                }
            });
        }
        catch (System.Exception e)
        {
            Utils.Log(e.ToString());
        }
    }

    private void SetLog(string msg)
    {
        if (_textLog != null)
            _textLog.text = msg;
    }
}
