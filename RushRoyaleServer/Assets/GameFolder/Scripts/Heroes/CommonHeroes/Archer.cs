using UnityEngine;

public class Archer : Hero
{
    public int DamageIncrease = 10;
    public float BulletSpeedIncrease = 1f, AttackSpeedIncrease = .1f, RangeIncrease = .5f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnHeroManaLevelUp += ManaLevelUp;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnHeroManaLevelUp -= ManaLevelUp;
    }

    private void ManaLevelUp(Hero hero)
    {
        if (hero != this)
            return;

        damage += DamageIncrease;
        bulletSpeed += BulletSpeedIncrease;
        attackInterval -= AttackSpeedIncrease;
        range += RangeIncrease;
    }
}
