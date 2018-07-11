using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {

	// Use this for initialization

    private FSMPlayer player;
    
	void Start () {
        player = GetComponent<FSMPlayer>();
    }

    public void PlayWhirlwind()
    {
        if (CommonSkill(0, false, true))
        {
            SoundManager.Instance.PlaySFX("Whirlwind_start", 0.3f);

            player.SetState(CH_STATE.Whirlwind);
        }
    }

    public void StopWhirlwind()
    {
        player.WhirlwindFX.SetActive(false);
        SoundManager.Instance.StopSFX("Whirlwind_Loop");
        UIManager.Instance.CoolTimes[0].fillAmount = 1;
        StartCoroutine(UIManager.Instance.SkillCoolTime(0));
        player.SetState(CH_STATE.Wait);
    }

    public void Bash()
    {
        if (CommonSkill(1, true))
        {
            if (player.Point.transform.position.y - player.transform.position.y > 3) return;

            player.Mana.ConsumeMP(20);

            MoveUtil.RotateToDirBurst(transform, player.Point);

            player.SetState(CH_STATE.Bash);
        }
    }
    public void Dash()
    {
        if (CommonSkill(2, true))
        {
            player.Mana.ConsumeMP(10);

            player.DashTrail.SetActive(true);
            SoundManager.Instance.PlaySFX("Dash", 0.5f);
            player.SetState(CH_STATE.Dash);
        }
    }

    public void ThrowAxe()
    {
        if (CommonSkill(3, true))
        {
            player.Mana.ConsumeMP(15);

            MoveUtil.RotateToDirBurst(transform, player.Point);

            player.SetState(CH_STATE.ThrowAxe);
        }
    }
    public void Avoid()
    {
        if(CommonSkill(4, true))
        {
            MoveUtil.RotateToDirBurst(transform, player.Point);

            player.SetState(CH_STATE.Avoid);
        }
    }

    public void Buff()
    {
        if (CommonSkill(5, false))
        {
            player.Mana.ConsumeMP(25);

            player.State.AttackDamage *= 2;

            player.MinDamage = player.State.AttackDamage - player.State.AttackDamage * 0.1f;
            player.MaxDamage = player.State.AttackDamage + player.State.AttackDamage * 0.1f;

            StartCoroutine(player.EndBuff());

            SoundManager.Instance.PlaySFX("Buff", 0.5f);
            player.SetState(CH_STATE.OnBuff);
        }
    }

    private bool CommonSkill(int index, bool isRaycast, bool isPlayingWhirlwind=false)
    {
        if (player.IsThrowAxe()) return false;
        if (player.IsAvoid()) return false;
        if (player.IsBuff()) return false;
        if (player.IsDash()) return false;
        if (player.IsBash()) return false;
        if (player.IsWhirlwind()) return false;
        if (UIManager.Instance.CoolTimes[index].fillAmount > 0) return false;

        if (!player.IsWeaponEquiped) player.EquipWeapon(true);

        if (isRaycast)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit HitInfo;

            if (Physics.Raycast(ray, out HitInfo, 100f, player.Layer))
            {
                Vector3 dest = HitInfo.point;

                player.Point.transform.position = dest;

                if (player.Point.parent)
                {
                    player.ChangeShader(player.Point.parent.GetComponentsInChildren<SkinnedMeshRenderer>(), "Custom/DissolveShader");
                    player.Point.parent = null;
                }
            }
            else return false;
        }

        if (isPlayingWhirlwind == false)
        {
            UIManager.Instance.CoolTimes[index].fillAmount = 1;
            StartCoroutine(UIManager.Instance.SkillCoolTime(index));
        }

        return true;
    }
}
