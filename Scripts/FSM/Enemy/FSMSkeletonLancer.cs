using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMSkeletonLancer : FSMEnemy {


    protected override IEnumerator MS_AttackRun()
    {
        int AttackPattern = Random.Range(0, 2);

        do
        {
            yield return null;

            if (IsDead()) break;

            if (Vector3.Distance(this.transform.position, player.transform.position) > State.AttackRange)
            {
                NavMesh.isStopped = false;
                NavMesh.SetDestination(player.transform.position);
            }
            else
            {
                NavMesh.isStopped = true;
                if (AttackPattern == 0) SetState(CH_STATE.MS_Attack);
                else if (AttackPattern == 1) SetState(CH_STATE.MS_Attack2);
                break;
            }

            if (Vector3.Distance(transform.position, player.transform.position) >= State.ChasingRange)
            {
                NavMesh.isStopped = true;
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



            if (Vector3.Distance(transform.position, player.transform.position) > State.AttackRange
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.7f)
            {
                SetState(CH_STATE.MS_AttackRun);
                break;
            }

            else if (Vector3.Distance(transform.position, player.transform.position) <= State.AttackRange
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
}
