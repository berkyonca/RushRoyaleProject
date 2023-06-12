using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject damageIndicatorObj;
    public Vector3 offSet;

    private const string DeadPosition = "DeadPosition";

    [SerializeField]
    private EnemyProperty scriptable;

    public State state;

    // fields
    private float speed = 0;
    private int health = 0;
    private int manaGivenToPlayer = 0;
    private TMP_Text healthText;

    // props
    public int Health => health;
    public float Speed => speed;

    // unity
    private void Start()
    {
        speed = scriptable.Speed;
        health = scriptable.Health;
        manaGivenToPlayer = scriptable.ManaIncrease;

        healthText = GetComponentInChildren<TMP_Text>();
        healthText.text = health.ToString();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnEnemyHit += UpdateHealth;
        EventManager.Instance.OnEnemyDie += GiveMana;
        EventManager.Instance.OnEnemyHit += DamageIndicator;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEnemyHit -= UpdateHealth;
        EventManager.Instance.OnEnemyDie -= GiveMana; 
        EventManager.Instance.OnEnemyHit -= DamageIndicator;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == DeadPosition)
        {
            EventManager.Instance.OnPlayerTakeDamage?.Invoke(1);
            Destroy(gameObject);
        }
    }

    public void UpdateHealth(Enemy enemy, int value)
    {
        if (enemy != this)
            return;

        health -= value;
        healthText.text = health.ToString();

        if (health <= 0)
        {
            EventManager.Instance.OnEnemyDie?.Invoke(this, manaGivenToPlayer);
        }
    }

    public void GiveMana(Enemy enemy, int value)
    {
        if (enemy != this) return;

        GameManager.Instance.ManaTotal += value;
        GameManager.Instance.ManaHealthUISync();
        Die();
    }

    public void DamageIndicator(Enemy enemy, int value)
    {
        if (enemy != this)
            return;
        Vector3 randomPos = transform.position + new Vector3(Random.Range(-.5f,.5f), 0, 0);
       GameObject indicator = Instantiate(damageIndicatorObj, randomPos + offSet, Quaternion.identity, transform.GetChild(0));
        indicator.GetComponent<TMP_Text>().text = value.ToString();
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}
