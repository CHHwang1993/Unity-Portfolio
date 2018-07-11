using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{

    FSMPlayer player = null;
    public FollowTrackingCamera mainCamera;

    // Use this for initialization
    void Start()
    {
        player = this.GetComponent<FSMPlayer>();
    }

    public void InWeapon()
    {
        player.EquipWeapon(false);
    }

    public void OnPlayerAttack()
    {
        AxeTrigger trigger = player.ArmWeapon.GetComponent<AxeTrigger>();

        if (trigger.IsAttack)
        {
            player.OnAttack();
        }
    }

    public void OnBashAttack()
    {
        MemoryPoolManager.Instance.CreateObject("BashEffect", this.transform);

        StartCoroutine(mainCamera.PlayShake(0.3f, 0.25f));

        int layerMask;

        layerMask = LayerMask.GetMask(player.enemyLayer);

        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 2.0f, layerMask);

        for (int i = 0; i < colliders.Length; ++i)
        {
            FSMEnemy fsmEnemy = colliders[i].GetComponent<FSMEnemy>();

            fsmEnemy.TakeDamage();
        }
    }

    public void OnBuff()
    {
        MemoryPoolManager.Instance.CreateObject("BuffEffect", player.GetFxCenter().transform);
    }

    public void OnThrowAxeAttack()
    {
        SoundManager.Instance.PlaySFX("SideSlash_Shot", 0.5f);
        MemoryPoolManager.Instance.CreateObject("ThrowMesh", player.GetFxCenter().transform);
    }

    public void InvicibilityStart()
    {
        player.IsInvicibility = true;
    }

    public void InvicibilityEnd()
    {
        player.IsInvicibility = false;
    }
}
