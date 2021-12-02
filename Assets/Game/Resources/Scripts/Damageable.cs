using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [HideInInspector] public UnityEvent onDestroy = new UnityEvent();
    [HideInInspector] public UnityEvent<int> onHit = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<int> onHealthChange = new UnityEvent<int>();
    [SerializeField] public int totalHealth = 100;
    [SerializeField] private int currentHealth;
    public int health {
        get {
            return currentHealth;
        }
        set {
            int healthLost = currentHealth - value;
            currentHealth = value;
            Mathf.Clamp(currentHealth, 0, totalHealth);
            onHealthChange.Invoke(healthLost);
        }
    }
    private void Start()
    {
        currentHealth = totalHealth;
    }
    public void Hit(int damage)
    {
        health -= damage;
        onHit.Invoke(damage);
        if (currentHealth <= 0)
            Destroy();
    }
    void Destroy()
    {
        onDestroy.Invoke();
        Destroy(gameObject);
    }
}