using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoLogin : MonoBehaviour
{
    [SerializeField] private float retryTime = 5f;


    private void Start()
    {
        NakamaManager.Instance.OnLoginSuccess += LoginFailed;

        TryLogin();
    }

    private void OnDestroy()
    {
        NakamaManager.Instance.OnLoginSuccess -= LoginFailed;
    }


    private void TryLogin()
    {
        NakamaManager.Instance.LoginWithUdid();
    }

    private void LoginFailed()
    {
        Invoke(nameof(TryLogin), retryTime);
    }
}
