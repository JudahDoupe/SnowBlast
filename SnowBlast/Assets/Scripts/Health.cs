using System;
using Assets.Utils;
using FluentAssertions;
using UnityEngine;

public class HealthNotification
{
    public readonly int CurrentHealth;
    public readonly int MaxHealth;

    public HealthNotification(int currentHealth, int maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}


public class Health : MonoBehaviour, INotifier<HealthNotification>
{
    [SerializeField]
    private int Hitpoints;

    public bool Invulnerable;

    public Allegiance Allegiance = Allegiance.Enemy;

    private int MaxHealth;

    private readonly Notifier<HealthNotification> Notifier = new Notifier<HealthNotification>();

    public void Start()
    {
        MaxHealth = Hitpoints;
        MaxHealth.Should().BeGreaterThan(0, "GameObject needs a starting health > 0");
    }

    public void ApplyDamage(int amount, Allegiance damageSource)
    {
        if (Invulnerable) return;
        if (damageSource == Allegiance) return;
        Hitpoints = Math.Max(0, Hitpoints - amount);
        if (Hitpoints <= 0)
        {
            gameObject.SetActive(false);
        }
        Notifier.Notify(new HealthNotification(Hitpoints, MaxHealth));
    }

    public IDisposable Subscribe(Action<HealthNotification> subscriber)
    {
        return Notifier.Subscribe(subscriber);
    }
}

public enum Allegiance
{
    Player,
    Enemy
}