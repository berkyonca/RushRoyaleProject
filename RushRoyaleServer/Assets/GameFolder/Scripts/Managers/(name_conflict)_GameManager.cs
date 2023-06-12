using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Transform heroInitialSpawnPosition;

    [SerializeField]
    private Button heroSpawnButton;

    [SerializeField]
    private Transform heroStorage;

    [SerializeField]
    private GameObject heroPrefab;

    [SerializeField]
    private GameObject remoteHeroPrefab;

    [SerializeField]
    private List<GameObject> heroSpawnPositions = new List<GameObject>();
    [SerializeField]
    private List<GameObject> opponentHeroSpawnPositions = new List<GameObject>();
    public int SpawnPositionNumber;

    private Vector3 _localSpawnPosition;
    private Vector3 _remoteSpawnPosition;

    public bool IsAreaFull = false;
    public int FilledAreaCount = 0;
    public int ManaCost = 10;
    public int ManaTotal = 1000;
    public int PlayerHealth = 5;

    public static GameManager Instance;


    public void Awake()
    {
        Instance = this;
    }


    private void OnEnable()
    {
        EventManager.Instance.OnMergeHero += MakeHeroPositionEmpty;
        EventManager.Instance.OnPlayerTakeDamage += TakeDamage;
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.Subscribe(MultiplayerManager.Code.Transform, SpawnRemoteHero);
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.OnMergeHero -= MakeHeroPositionEmpty;
        EventManager.Instance.OnPlayerTakeDamage -= TakeDamage;

        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.Unsubscribe(MultiplayerManager.Code.Transform, SpawnRemoteHero);
        }

    }

    private void Start()
    {
        heroSpawnButton.onClick.AddListener(RandomGridPosition);
        UIManager.Instance.UpdatePlayerHeart(PlayerHealth);

        foreach (var item in heroSpawnPositions)
        {
            item.GetComponent<HeroSpawnArea>().ListNumber = SpawnPositionNumber;
            SpawnPositionNumber++;
        }
    }

    private void LateUpdate()
    {
       

        foreach (var item in heroSpawnPositions)
        {
            if (item.GetComponent<HeroSpawnArea>().IsSpawnable)
            {
                IsAreaFull = false;
            }
        }
    }

    private void SpawnRemoteHero(MultiplayerMessage message)
    {
        if (message.SessionId == MultiplayerManager.Instance.Self.SessionId)
            return;

        float[] positions = message.GetData<float[]>();

        Vector2 spawnPosition = new Vector2(positions[0], positions[1]);

        GameObject hero = Instantiate(remoteHeroPrefab, spawnPosition, Quaternion.identity);
        hero.transform.DOMove(spawnPosition, 1f);
    }

    private void SpawnLocalHero()
    {
        GameObject localHero = Instantiate(heroPrefab, heroInitialSpawnPosition.position, Quaternion.identity, heroStorage);
        localHero.transform.DOMove(_localSpawnPosition, 1f);
        if (MultiplayerManager.Instance != null)
        {
            MultiplayerManager.Instance.Send(MultiplayerManager.Code.Transform, MatchDataJson.SetTransform(_remoteSpawnPosition));
        }
    }

    private void RandomGridPosition()
    {
        if (IsAreaFull || ManaCost > ManaTotal) return;
        int randomArea = Random.Range(0, heroSpawnPositions.Count);
        if (heroSpawnPositions[randomArea].GetComponent<HeroSpawnArea>().IsSpawnable)
        {
            FilledAreaCount++;

            if (FilledAreaCount == heroSpawnPositions.Count) IsAreaFull = true;

            ManaCost += 10;
            ManaTotal -= ManaCost;
            heroSpawnPositions[randomArea].GetComponent<HeroSpawnArea>().IsSpawnable = false;
            _localSpawnPosition = heroSpawnPositions[randomArea].transform.position;
            _remoteSpawnPosition = opponentHeroSpawnPositions[randomArea].transform.position;
            SpawnLocalHero();
            ManaHealthUISync();
        }
        else
        {
            RandomGridPosition();
        }
    }

    public void ManaHealthUISync()
    {
        ManaTotal = Mathf.Clamp(ManaTotal, 0, 1200);
        UIManager.Instance.UpdateManaTotalText(ManaTotal);
        UIManager.Instance.UpdateManaCostText(ManaCost);
    }

    private void MakeHeroPositionEmpty(int spawnPositionIndex)
    {
        heroSpawnPositions[spawnPositionIndex].GetComponent<HeroSpawnArea>().IsSpawnable = true;
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