using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Transform heroInitialSpawnPosition;
    [SerializeField] private Button heroSpawnButton;
    [SerializeField] private Transform heroStorage;
    [SerializeField] private GameObject[] localHeroPrefabs;
    [SerializeField] private GameObject[] remoteHeroPrefabs;
    [SerializeField] private List<GameObject> heroSpawnPositions = new List<GameObject>();
    [SerializeField] private List<GameObject> opponentHeroSpawnPositions = new List<GameObject>();

    public int ManaCost = 10;
    public int ManaTotal = 1000;
    public int PlayerHealth = 5;
    public List<HeroSpawnAreaData> HeroSpawnAreaList { get; private set; } = new List<HeroSpawnAreaData>();

    public static GameManager Instance;


    public void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerTakeDamage += TakeDamage;

        MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.SpawnHero, SpawnHero);
        MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.SpawnAreaDataReceived, UpdateSpawnAreaList);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPlayerTakeDamage -= TakeDamage;

        MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.SpawnHero, SpawnHero);
        MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.SpawnAreaDataReceived, UpdateSpawnAreaList);
    }

    private void Start()
    {
        heroSpawnButton.onClick.AddListener(SendHeroSpawnRequest);
    }

    private void LateUpdate()
    {
        ManaTotal = Mathf.Clamp(ManaTotal, 0, 1200);
    }

    private void SpawnHero(MultiplayerMessage message)
    {
        bool isLocal = message.SessionId == MultiplayerManager.Instance.Self.SessionId ? true : false;
        RandomHeroAndArea data = message.GetData<RandomHeroAndArea>();
        Debug.Log(data);
        int randomArea = data.RandomArea;
        int randomHero = data.RandomHero;

        if (isLocal)
        {
            Vector2 spawnPosition = heroSpawnPositions[randomArea].transform.position;
            GameObject hero = Instantiate(localHeroPrefabs[randomHero], spawnPosition, Quaternion.identity, heroStorage);
            hero.transform.DOMove(spawnPosition, 1f);

            ManaCost += 10;
            ManaTotal -= ManaCost;
            ManaHealthUISync();
        }
        else
        {
            Vector2 spawnPosition = opponentHeroSpawnPositions[randomArea].transform.position;
            GameObject opponentHero = Instantiate(remoteHeroPrefabs[randomHero], spawnPosition, Quaternion.identity, heroStorage);
            opponentHero.transform.DOMove(spawnPosition, 1f);
        }
    }

    private void SendHeroSpawnRequest()
    {
        if (ManaCost > ManaTotal) return;
        MultiplayerManager.Instance.Send(MultiplayerManager.Code.SpawnHero);
    }

    private void UpdateSpawnAreaList(MultiplayerMessage message)
    {
        HeroSpawnAreaList = message.GetData<List<HeroSpawnAreaData>>();
    }

    public void ManaHealthUISync()
    {
        ManaTotal = Mathf.Clamp(ManaTotal, 0, 1200);
        UIManager.Instance.UpdateManaTotalText(ManaTotal);
        UIManager.Instance.UpdateManaCostText(ManaCost);
    }

    private void TakeDamage(int value)
    {
        PlayerHealth--;
        UIManager.Instance.PlayerHeartDown();
        if (PlayerHealth <= 0)
        {
            EventManager.Instance.OnPlayerDie?.Invoke();
        }
    }
}