using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FSMOrca : FSMEnemy {

    public GameObject WarningCircle;
    public UnityEvent OrcaDiedEvent;

    bool IsAttack3 = false;
    int AttackCount = 0;
    int RageDelay = 1;
    float PercentHP = 0.66f;

	
	// Update is called once per frame
	void Update () {
		
        if(Health.HP <= Health.maxHP.Value / 2)
        {
            RageDelay = 2;
        }

        if(Health.RemainingAmount()< PercentHP && PercentHP>0.32f) 
        {
            PercentHP /= 2;
            WarningCircle.SetActive(true);
            WarningCircle.transform.position = new Vector3(this.transform.position.x, this.transform.position.y +0.5f, this.transform.position.z);
            SoundManager.Instance.PlaySFX(State.Name +"_BigAttack");
            SetState(CH_STATE.MS_BigAttack);
        }
	}

    public void Performance()
    {
        SetState(CH_STATE.MS_Attack2);
    }

    //코루틴
    protected override IEnumerator MS_Wait()
    {
        do
        {
            yield return null;
            
            if (IsAlive && !CompareDist(transform.position, player.transform.position, 50))
            {
                NavMesh.isStopped = true;

                if (DetectPlayer() && !player.IsDead() && CompareDist(transform.position, player.transform.position, State.AttackRange))
                {
                    NavMesh.isStopped = false;
                    SetState(CH_STATE.MS_AttackRun);
                    break;
                }
                else if (DetectPlayer() && !player.IsDead() && !CompareDist(transform.position, player.transform.position, State.AttackRange))
                {
                    if (AttackCount < 3)
                    {
                        StartCoroutine(AttackDelay());
                    }
                    else
                    {
                        StartCoroutine(Attack2Delay());
                    }
                }
                else if (!player.IsDead() && CompareDist(transform.position, player.transform.position, State.ChasingRange) && IsAttack3)
                {
                    StartCoroutine(Attack3Delay());
                    break;
                }
            }

        } while (!isNewState);
    }


    protected override IEnumerator MS_AttackRun()
    {
        do
        {
            yield return null;

            if (IsDead()) break;

            NavMesh.speed = State.RunSpeed;

            if (CompareDist(transform.position, player.transform.position, State.AttackRange))
            {
                NavMesh.isStopped = false;
                NavMesh.SetDestination(player.transform.position);
            }
            else
            {
                NavMesh.isStopped = true;
                if (AttackCount < 3)
                {
                    AttackCount++;
                    SetState(CH_STATE.MS_Attack);
                }
                else
                {
                    AttackCount = 0;
                    SoundManager.Instance.PlaySFX(State.Name +"_Attack2");
                    SetState(CH_STATE.MS_Attack2);
                }
                break;
            }

            if (Vector3.Distance(transform.position, player.transform.position) >= State.ChasingRange)
            {
                NavMesh.isStopped = true;
                IsAttack3 = true;
               
                SetState(CH_STATE.MS_Attack3);
                break;
            }

        } while (!isNewState);
    }

    protected override IEnumerator MS_Attack()
    {
        MoveUtil.RotateToDirBurst(transform, player.transform);

        do
        {
            yield return null;

            if (IsDead()) break;

            NavMesh.isStopped = true;

            if (CompareDist(transform.position, player.transform.position, State.AttackRange)
                  && m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1")
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.7f)
            {
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_AttackRun);
                break;
            }

            else if (!CompareDist(transform.position, player.transform.position, State.AttackRange)
                && m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1")
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.7f)
            {
                SetState(CH_STATE.MS_Wait);
                break;
            }

            if (player.IsDead())
            {
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }


    protected virtual IEnumerator MS_Attack2()
    {
        MoveUtil.RotateToDirBurst(transform, player.transform);

        do
        {
            yield return null;

            if (IsDead()) break;

            NavMesh.isStopped = true;

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2")
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }


    protected virtual IEnumerator MS_Attack3()
    {
        MoveUtil.RotateToDirBurst(transform, player.transform);

        do
        {
            yield return null;

            if (IsDead()) break;

            NavMesh.isStopped = true;

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3")
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.6f)
            {
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }


    protected virtual IEnumerator MS_BigAttack()
    {
        do
        {
            yield return null;

            if (IsDead()) break;

            NavMesh.isStopped = true;

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("BigAttack")
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.75f)
            {
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }


    IEnumerator AttackDelay()
    {
        while (true)
        {

            yield return new WaitForSeconds(1.0f/ RageDelay);

            if (IsDead()) break;

            if (CHState != CH_STATE.MS_Wait) break;

            if (AttackCount < 3) AttackCount++;

            SetState(CH_STATE.MS_Attack);

            StopCoroutine(AttackDelay());
            break;
        }

    }

    IEnumerator Attack2Delay()
    {
        while (true)
        {

            yield return new WaitForSeconds(1.0f/ RageDelay);

            if (IsDead()) break;

            if (CHState != CH_STATE.MS_Wait) break;

            AttackCount = 0;

            SetState(CH_STATE.MS_Attack2);

            StopCoroutine(AttackDelay());
            break;
        }

    }

    IEnumerator Attack3Delay()
    {
        while (true)
        {

            yield return new WaitForSeconds(1.5f);

            if (IsDead()) break;

            SetState(CH_STATE.MS_Attack3);

            StopCoroutine(Attack3Delay());
            break;
        }

    }

    protected override IEnumerator MS_Dead()
    {
        Sphere.SetActive(false);
        m_CC.enabled = false;
        NavMesh.enabled = false;
        IsAlive = false;

        for (int i = 0; i < Colliders.Length; ++i) Colliders[i].enabled = false;

        yield return new WaitForSeconds(9.5f);

        if(OrcaDiedEvent!=null)
            OrcaDiedEvent.Invoke();

        StartCoroutine(RealDead());
    }
}
