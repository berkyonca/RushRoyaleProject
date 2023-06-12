using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D _bulletRigidbody;
    private Hero _ownerHero;
    private Opponent _targetOpponent; 

    private void Awake()
    {
        _bulletRigidbody = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, 3f);
    }

    public IEnumerator MoveToTarget(Hero owner, Opponent target, float speed)
    {
        _targetOpponent = target;
        _ownerHero = owner;

        while (true)
        {
            if (target == null)
            {
                Destroy(this.gameObject);
                break;
            }
            _bulletRigidbody.MovePosition(_bulletRigidbody.position + ((Vector2)target.transform.position - _bulletRigidbody.position) * Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_targetOpponent == null)
        {
            Destroy(this.gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && collision.gameObject.GetInstanceID() == _targetOpponent.gameObject.GetInstanceID())
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            EventManager.Instance.OnEnemyHit?.Invoke(enemy, _ownerHero.Damage);
            Destroy(this.gameObject);
        }
    }
}
