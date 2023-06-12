using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public Action<Enemy, int> OnEnemyHit;
    public Action<Enemy, int> OnEnemyDie;
    public Action<Hero> OnHeroManaLevelUp;
    public Action OnPlayerDie;
    public Action<int> OnPlayerTakeDamage;

    public Action<int> OnMergeHero;

    public static EventManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}