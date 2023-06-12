using System;
using UnityEngine;
using Nakama;

public class NakamaUserManager : MonoBehaviour
{
    private IApiAccount account = null;

    public event Action OnLoaded = null;

    public bool LoadingFinished { get; private set; } = false;
    public IApiUser User { get => account.User; }
    public string Wallet { get => account.Wallet; }
    public string DisplayName { get => account.User.DisplayName; }

    public static NakamaUserManager Instance;


    private void Awake()
    {
        Instance = this;    
    }

    private void Start()
    {
        NakamaManager.Instance.OnLoginSuccess += AutoLoad;
    }

    private void OnDestroy()
    {
        NakamaManager.Instance.OnLoginSuccess -= AutoLoad;
    }

    private async void AutoLoad()
    {
        account = await NakamaManager.Instance.Client.GetAccountAsync(NakamaManager.Instance.Session);
        LoadingFinished = true;
        OnLoaded?.Invoke();
    }

    public async void UpdateDisplayName(string displayName)
    {
        if (!string.IsNullOrEmpty(displayName))
            await NakamaManager.Instance.Client.UpdateAccountAsync(NakamaManager.Instance.Session, NakamaManager.Instance.Username, displayName);
        else
            await NakamaManager.Instance.Client.UpdateAccountAsync(NakamaManager.Instance.Session, NakamaManager.Instance.Username, NakamaManager.Instance.Username);
    }

    public T GetWallet<T>()
    {
        if (account == null || account.Wallet == null)
            return default(T);

        return account.Wallet.Deserialize<T>();
    }

}
