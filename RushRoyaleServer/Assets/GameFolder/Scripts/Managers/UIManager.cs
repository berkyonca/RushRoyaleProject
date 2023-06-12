using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerHearthObj, PlayerHealthStorage;

    [SerializeField]
    private TMP_Text manaTotalText, manaCostText;

    [SerializeField]
    private List<GameObject> PlayerHeartList = new List<GameObject>();

    [SerializeField]
    private Button RestartButton;

    [SerializeField]
    private GameObject DeadScreenPanel;

    int i = 0;

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RestartButton.onClick.AddListener(RestartGame);
        DeadScreenPanel.SetActive(false);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene((int)Scenes.Battle);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerDie += DeadScreen;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPlayerDie -= DeadScreen;
    }

    private void DeadScreen()
    {
        DeadScreenPanel.SetActive(true);   
    }


    public void UpdateManaCostText(int value)
    {
        manaCostText.text = value.ToString();
    }

    public void UpdateManaTotalText(int value)
    {
        manaTotalText.text = value.ToString();
    }

    public void UpdatePlayerHeart(int value)
    {
        for (int i = 0; i < value; i++)
        {
       GameObject heart = Instantiate(playerHearthObj, PlayerHealthStorage.transform);
            PlayerHeartList.Add(heart);
        }
    }
    
    public void PlayerHeartDown()
    {
        
        Destroy(PlayerHeartList[i]);
        i++;
    }
}
