using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTrigger : MonoBehaviour {

    public bool IsAttack = false;
   
    private FSMPlayer Player;

    // Use this for initialization
    void Start () {
        Player = GameSceneManager.Instance.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            IsAttack = true;


        if (Player.IsWhirlwind() && Player.m_Anim.GetCurrentAnimatorStateInfo(0).IsName("WhirlwindShot")
            && other.CompareTag("Enemy"))
        {
            FSMEnemy Enemy = other.GetComponent<FSMEnemy>();
            Enemy.TakeDamage();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            IsAttack = false;
    }
}
