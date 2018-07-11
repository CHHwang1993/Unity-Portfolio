using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public float currentHP;
    public FloatReference maxHP;

    private void Awake()
    {
        currentHP =maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP-=damage;

        if (IsDeath())
            currentHP = 0.0f;
    }

    public void RecoveryHP(float hp)
    {
        currentHP += hp;

        if (IsFullHealth())
            currentHP = maxHP;
    }

    public bool IsDeath() { return (currentHP <= 0); }
    public bool IsFullHealth() { return (currentHP >= maxHP); }
    public float HP { get { return currentHP; } set { currentHP = value; } }
    public float RemainingAmount() { return Mathf.Clamp01(currentHP / maxHP); }
}
