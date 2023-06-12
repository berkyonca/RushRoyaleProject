using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FindMatch : MonoBehaviour
{
    [SerializeField] private Button findMatchButton;
    [SerializeField] private Button cancelMatchButton;

    [SerializeField] private GameObject matchMakingPanel;
    [SerializeField] private GameObject findMatchPanel;

    private void Start()
    {
        findMatchButton.onClick.AddListener(StartFindingMatch);
        cancelMatchButton.onClick.AddListener(CancelMatchmakingAsync);

        NakamaManager.Instance.Socket.ReceivedMatchmakerMatched += matchmakerMatched => MultiplayerManager.Instance.OnReceivedMatchmakerMatched(matchmakerMatched);
    }

    private void StartFindingMatch()
    {
        matchMakingPanel.SetActive(true);
        findMatchPanel.SetActive(false);

        MultiplayerManager.Instance.FindMatchAsync();
    }

    public void CancelMatchmakingAsync()
    {
        matchMakingPanel.SetActive(false);
        findMatchPanel.SetActive(true);

        MultiplayerManager.Instance.LeaveMatchAsync();
    }
}
