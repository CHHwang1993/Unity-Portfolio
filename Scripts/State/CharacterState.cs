using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CH_STATE
{
    Idle,
    Wait,
    Run,
    AttackRun,
    Attack,
    Dead=6,
    Whirlwind,
    Bash,
    Dash,
    Avoid,
    OnBuff,
    ThrowAxe,
    MS_Wait=20,
    MS_Run,
    MS_AttackRun,
    MS_Attack,
    MS_Attack2,
    MS_Attack3,
    MS_Avoid,
    MS_Revive,
    MS_Dead,
    MS_BigAttack
}

public class CharacterState : MonoBehaviour {

    public string Name;
    public float AttackDamage = 40.0f;
    public float AttackRange = 3.0f;
    public float WalkSpeed = 1.5f;
    public float RunSpeed = 3.0f;
    public float TurnSpeed = 360.0f;
    public float RestTime = 1.5f;
    public float ChasingRange = 10.0f;
}
