using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMana : MonoBehaviour {

    public FloatVariable currentMP;
    public FloatReference maxMP;

    private void Awake()
    {
        currentMP.SetValue(maxMP);
    }

    public void ConsumeMP(float mana)
    {
        currentMP.ApplyChange(-mana);

        if (IsEmptyMana())
            currentMP.SetValue(0.0f);
    }

    public void RecoveryMP(float mp)
    {
        currentMP.ApplyChange(mp);

        if (IsFullMana())
            currentMP.SetValue(maxMP);
    }

    public bool IsEmptyMana() { return (currentMP.Value <= 0); }
    public bool IsFullMana() { return (currentMP.Value >= maxMP.Value); }
    public float MP { get { return currentMP.Value; } set { currentMP.SetValue(value); } }
    public float RemainingAmount() { return Mathf.Clamp01(currentMP.Value / maxMP.Value); }
}
