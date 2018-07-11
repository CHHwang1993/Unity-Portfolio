using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackTrigger : MonoBehaviour {

    public FSMEnemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.OnAttack();
        }
    }
}
