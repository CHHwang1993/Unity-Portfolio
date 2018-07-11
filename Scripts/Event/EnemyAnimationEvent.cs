using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class EnemyAnimationEvent : MonoBehaviour {

    Collider[] colliders;
    FSMEnemy enemy;
    FSMPlayer player;

    StringBuilder stringBuilder;
    string soundName;

    // Use this for initialization
    void Start () {
        enemy = GetComponent<FSMEnemy>();
        colliders = enemy.Colliders;
        player = GameSceneManager.Instance.Player;
        stringBuilder = new StringBuilder(64);
    }

    void ColliderEnabled()
    {
        for (int i = 0; i < colliders.Length; ++i)
        {
            colliders[i].enabled = true;
        }
    }
    
    void ColliderDisabled()
    {
        for (int i = 0; i < colliders.Length; ++i)
        {
            colliders[i].enabled = false;
        }
    }

    public void SkelAtkStart()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        ColliderEnabled();
    }

    public void SkelAtkEnd()
    {
        ColliderDisabled();
    }

    public void ZombieAtkStart()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        ColliderEnabled();
    }

    public void ZombieAtkEnd()
    {
        ColliderDisabled();
    }

    //랜서
    public void LancerAtkStart()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        ColliderEnabled();
    }

    public void LancerAtkEnd()
    {
        ColliderDisabled();
    }

    public void LancerAtk2Start()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack2");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        ColliderEnabled();
    }

    public void LancerAtk2End()
    {
        ColliderDisabled();
    }

    //헬하운드
    public void HellAtkStart()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        ColliderEnabled();
    }

    public void HellAtkEnd()
    {
        ColliderDisabled();
    }

    //아쳐
    public void OnArcherAttack()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        MemoryPoolManager.Instance.CreateObject("Arrow", colliders[0].transform.position, this.transform.rotation);
    }


    public void OnChargingAttack()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_ChargingAttack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        MemoryPoolManager.Instance.CreateObject("ChargingAttack", colliders[0].transform.position, this.transform.rotation);
    }

    //메이지
    public void OnFireBallAttack()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        Transform newTransform = this.transform;

        newTransform.position = new Vector3(newTransform.position.x, colliders[0].transform.position.y, newTransform.position.z);

        MemoryPoolManager.Instance.CreateObject("FireBall", newTransform);
    }

    public void OnReviveStart()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Revive");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        MemoryPoolManager.Instance.CreateObject("ReviveEffect", this.transform);
    }

    public void OnRevive()
    {
        string enemyLayer = "Enemy";

        int layerMask = LayerMask.GetMask(enemyLayer);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 3.0f, layerMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
            FSMEnemy enemy = colliders[i].GetComponent<FSMEnemy>();

            if (enemy.CHState == CH_STATE.MS_Dead)
            {
                enemy.gameObject.SetActive(false);
                enemy.gameObject.SetActive(true);
            }
        }
    }
    /////////////////////////////////////보스 기술///////////////////////////////

    public void OnOrcaAttackStart()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        colliders[0].enabled = true;
    }

    public void OnOrcaAttackEnd()
    {
        colliders[0].enabled = false;
    }

    public void OnOrcaAttackStart2()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack_1");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);
        colliders[1].enabled = true;
    }

    public void OnOrcaAttackEnd2()
    {
        colliders[1].enabled = false;
    }

    public void OnOrcaAttack2()
    {
        Transform Attack2Point = GameObject.Find("Attack2Point").transform;

        Attack2Point.position = new Vector3(Attack2Point.transform.position.x, Attack2Point.transform.position.y, Attack2Point.transform.position.z);

        MemoryPoolManager.Instance.CreateObject("OrcaEffect", Attack2Point.position, new Quaternion(0, 0, 0, 0));

        int layerMask;

        string PlayerLayer = "Player";

        layerMask = LayerMask.GetMask(PlayerLayer);

        Collider[] colliders = Physics.OverlapSphere(Attack2Point.position, 4.0f, layerMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
            FSMPlayer player = colliders[i].GetComponent<FSMPlayer>();
            if(!player.IsInvicibility)
            player.TakeDamage(250);
        }
    }


    public void OnOrcaAttack3()
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(enemy.State.Name);
        stringBuilder.Append("_Attack3");
        soundName = stringBuilder.ToString();

        SoundManager.Instance.PlaySFX(soundName);

        Vector3 PlayerPos = new Vector3(player.transform.position.x, player.transform.position.y + 7, player.transform.position.z);

        MemoryPoolManager.Instance.CreateObject("ElectricBall", PlayerPos, Quaternion.AngleAxis(90.0f,new Vector3(1,0,0)));
    }

    public void OnIceAge()
    {
        MemoryPoolManager.Instance.CreateObject("IceAge", this.transform);

        int layerMask;

        string PlayerLayer = "Player";

        layerMask = LayerMask.GetMask(PlayerLayer);

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 6.0f, layerMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
            FSMPlayer player = colliders[i].GetComponent<FSMPlayer>();

            player.TakeDamage(player.Health.maxHP.Value);
        }
    }
}
