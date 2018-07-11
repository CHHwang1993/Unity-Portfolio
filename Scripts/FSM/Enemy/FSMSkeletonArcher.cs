using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSkeletonArcher : FSMEnemy {

    bool IsAttacked=false;
    int ChargeAttack = 0;

    protected override IEnumerator MS_Wait()
    {
        float _t = 0.0f;

        do
        {
            yield return null;

            if (!IsDead() && Vector3.Distance(this.transform.position, player.transform.position) < 50.0f)
            {
                _t += Time.deltaTime;
               
                NavMesh.isStopped = true;

                if (_t >= State.RestTime)
                {
                    NavMesh.isStopped = false;
                    SetState(CH_STATE.MS_Run);
                    break;
                }

                if (DetectPlayer() && !player.IsDead() && Vector3.Distance(this.transform.position, player.transform.position) > State.AttackRange)
                {
                    NavMesh.isStopped = false;
                    SetState(CH_STATE.MS_AttackRun);
                    break;
                }
                else if (DetectPlayer() && !player.IsDead() && Vector3.Distance(this.transform.position, player.transform.position) <= State.AttackRange)
                {
                    _t = 0;
                    if(!IsAttacked && ChargeAttack<3) SetState(CH_STATE.MS_Attack);
                    else if(!IsAttacked && ChargeAttack >= 3)
                    {
                        SoundManager.Instance.PlaySFX(State.Name +"_ChargingStart");
                        SetState(CH_STATE.MS_Attack2);
                    }
                }
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator MS_Avoid()
    {
        SoundManager.Instance.PlaySFX(State.Name + "_Avoid");
        NavMesh.isStopped = true;
        do
        {
            yield return null;

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Avoid") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.85f)
            {
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }

    protected override IEnumerator MS_Attack()
    {
        do
        {
            yield return null;

            if (IsDead()) break;

            MoveUtil.RotateToDirBurst(transform, player.transform);

            NavMesh.isStopped = true;

            if (Vector3.Distance(transform.position, player.transform.position) > State.AttackRange
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
                IsAttacked = true;
                StartCoroutine(AttackDelay());
                SetState(CH_STATE.MS_AttackRun);
                break;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) <= State.AttackRange / 2
               && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
            {
                NavMesh.isStopped = false;
                IsAttacked = true;
                StartCoroutine(AttackDelay());
                SetState(CH_STATE.MS_Avoid);
                break;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) <= State.AttackRange
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
                IsAttacked = true;
                StartCoroutine(AttackDelay());
                SetState(CH_STATE.MS_Wait);
                break;
            }

            if (player.IsDead())
            {
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }


    protected virtual IEnumerator MS_Attack2()
    {
        do
        {
            yield return null;

            if (IsDead()) break;

            StopCoroutine(AttackDelay());

            MoveUtil.RotateToDirBurst(transform, player.transform);

            NavMesh.isStopped = true;

            if (Vector3.Distance(transform.position, player.transform.position) > State.AttackRange
                && m_Anim.GetCurrentAnimatorStateInfo(0).IsName("chargeAttack")
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
                IsAttacked = true;
                StartCoroutine(AttackDelay());
                SetState(CH_STATE.MS_AttackRun);
                break;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) <= State.AttackRange / 2
               && m_Anim.GetCurrentAnimatorStateInfo(0).IsName("chargeAttack")
               && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f)
            {
                NavMesh.isStopped = false;
                IsAttacked = true;
                StartCoroutine(AttackDelay());
                SetState(CH_STATE.MS_Avoid);
                break;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) <= State.AttackRange
                && m_Anim.GetCurrentAnimatorStateInfo(0).IsName("chargeAttack")
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
                IsAttacked = true;
                StartCoroutine(AttackDelay());
                SetState(CH_STATE.MS_Wait);
                break;
            }


            if (player.IsDead())
            {
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(1.0f);


        if (ChargeAttack < 3) ChargeAttack++;
        else ChargeAttack = 0;

        IsAttacked = false;

        StopCoroutine(AttackDelay());
    }
}
