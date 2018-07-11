using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMMage : FSMEnemy {

    protected override void LateUpdate()
    {
        if (DummyRoot)
        {
            //마법사가 피할때는 루트모션을 사용할 수 있도록 더미루트의 고정값을 품
            if(CHState!=CH_STATE.MS_Avoid) DummyRoot.localPosition = new Vector3(0, 0, 0);
        }
    }


    protected override IEnumerator MS_Wait()
    {
        float tTime = 0.0f;

        do
        {
            yield return null;

            //현재 살아있고 플레이어와의 거리가 50미만일 경우 작동
            if (IsAlive && Vector3.Distance(this.transform.position, player.transform.position) < 50.0f)
            {
                tTime += Time.deltaTime;

                NavMesh.isStopped = true;

                //웨이포인트 순찰
                if (tTime >= State.RestTime)
                {
                    NavMesh.isStopped = false;
                    SetState(CH_STATE.MS_Run);
                    break;
                }

                //죽지 않은 플레이어를 찾고 공격범위보다 멀면 쫓아가는 상태로 변경
                if (DetectPlayer() && !player.IsDead() && Vector3.Distance(this.transform.position, player.transform.position) > State.AttackRange)
                {
                    NavMesh.isStopped = false;
                    SetState(CH_STATE.MS_AttackRun);
                    break;
                }
                //죽지 않은 플레이어를 찾고 공격범위보다 가까우면 공격하는 상태로 변경
                else if (DetectPlayer() && !player.IsDead() && Vector3.Distance(this.transform.position, player.transform.position) <= State.AttackRange)
                {
                    tTime = 0;
                    StartCoroutine(AttackDelay());
                }
            }
           
        } while (!isNewState);
    }

    //회피
    protected virtual IEnumerator MS_Avoid()
    {
        SoundManager.Instance.PlaySFX(State.Name +"_Avoid");
        NavMesh.isStopped = true; //루트모션을 사용할 것이기 때문에 네비메쉬 잠시 끔
        do
        {
            yield return null;

            //현재 피하는 애니메이션이면서 90%이상의 진행을 보였다면 대기상태로 전환
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Avoid") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.9f)
            { 
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }

    //부활
    protected virtual IEnumerator MS_Revive()
    {
        NavMesh.isStopped = true; //부활중 움직임을 멈춤

        do
        {
            yield return null;

            StopCoroutine(AttackDelay());

            //현재 부활 애니메이션이면서 진행상태가 80% 이상 지났다면 부활이 완료 되었기 때문에 대기상태로 전환
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Revive") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_Wait);

                break;
            }

        } while (!isNewState);
    }

    //공격
    protected override IEnumerator MS_Attack()
    {
       
        do
        {
            yield return null;

            if (IsDead()) break; //죽었을 때는 공격을 못함

            //플레이어에게 바로 회전
            MoveUtil.RotateToDirBurst(transform, player.transform);

            NavMesh.isStopped = true; //공격하는중에는 이동을 멈추자
            

            //공격 후 거리가 벌어졌다면 쫓아가자
            if (Vector3.Distance(transform.position, player.transform.position) > State.AttackRange
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_AttackRun);
                break;
            }
            //공격 후 거리가 가까워졌다면 피하자
            else if (Vector3.Distance(transform.position, player.transform.position) <= State.AttackRange / 2
               && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
               
                SetState(CH_STATE.MS_Avoid);
                break;
            }
            // 공격 후 공격범위 안에 있다면 다시 대기 상태에서 공격 준비
            else if (Vector3.Distance(transform.position, player.transform.position) <= State.AttackRange
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_Wait);
                break;
            }

            
            //플레이어가 죽었다면? 다시 순찰상태로 돌아가자
            if (player.IsDead())
            {
                NavMesh.isStopped = false;
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }

    //공격시 딜레이 적용
    IEnumerator AttackDelay()
    {
        while (true)
        {
 
            yield return new WaitForSeconds(1.0f);

            if (IsDead()) break;

            SetState(CH_STATE.MS_Attack);

            StopCoroutine(AttackDelay());
            break;
        }
 
    }

    //부활 딜레이
    public void  ReviveEvent()
    {
        //현재 자신의 상태가 대기 또는 공격 상태일 경우
        if (CHState == CH_STATE.MS_Wait || CHState == CH_STATE.MS_Attack)
        {
            string enemyLayer = "Enemy";

            int layerMask = LayerMask.GetMask(enemyLayer);

            Collider[] colliders = Physics.OverlapSphere(transform.position, 3.0f, layerMask); // 3.0 반지름만큼 적 레이어를 확인하여 충돌자 배열에 집어넣음

            int Count = 0;

            //반복문을 돌면서 죽은 마리수를 확인
            for (int i = 0; i < colliders.Length; ++i)
            {
                FSMEnemy enemy = colliders[i].GetComponent<FSMEnemy>();

                if (enemy.IsDead())
                {
                    Count++;
                }
            }

            //3명 이상이라면 부활 가능이므로 부활 코루틴 실행
            if (Count > 2)
                SetState(CH_STATE.MS_Revive);
        }
    }
}