using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public FloatVariable currentHP;
    public FloatReference maxHP;
  
    private void Awake()
    {
        currentHP.SetValue(maxHP);
    }

    public void TakeDamage(float damage)
    {
        currentHP.ApplyChange(-damage);

        if (IsDeath())
            currentHP.SetValue(0.0f);
    }

    public void RecoveryHP(float hp)
    {
        currentHP.ApplyChange(hp);

        if (IsFullHealth())
            currentHP.SetValue(maxHP);
    }

    public bool IsDeath() { return (currentHP.Value <= 0); }
    public bool IsFullHealth() { return (currentHP.Value >= maxHP.Value); }
    public float HP { get { return currentHP.Value; } set { currentHP.Value = value; } }
    public float RemainingAmount() { return Mathf.Clamp01(currentHP.Value / maxHP.Value); }
}
