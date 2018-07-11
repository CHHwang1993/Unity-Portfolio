using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommandPattern;

public class MoveCommand : Command {

	public override bool Execute(GameObject target)
    {
        FSMPlayer player = target.GetComponent<FSMPlayer>();

        if(player!=null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit HitInfo;

            if (Physics.Raycast(ray, out HitInfo, 100f, player.Layer))
            {
                int layer = HitInfo.transform.gameObject.layer; //클릭한 레이어의 값

                if (layer == LayerMask.NameToLayer(player.clickLayer)) //땅을 클릭했을 때
                {
                    Vector3 dest = HitInfo.point; //마우스로 찍은 위치

                    player.Point.transform.position = dest;

                    if (!player.IsWhirlwind()) player.SetState(CH_STATE.Run); //휠윈드 상태가 아닐 때는 달리자

                    return true;
                }

                else if (layer == LayerMask.NameToLayer(player.enemyLayer)) //적을 클릭 했을 때
                {
                    player.Enemy = HitInfo.collider.GetComponent<FSMEnemy>(); //적의 컴퍼넌트를 가져온다.

                    player.Point.SetParent(player.Enemy.transform);
                    player.Point.transform.localPosition = Vector3.zero;

                    if (!player.IsWhirlwind() && !player.Enemy.IsDead())
                    {
                        if (Vector3.Distance(player.transform.position, player.Point.transform.position) <= player.State.AttackRange)
                            player.SetState(CH_STATE.Attack);
                        else
                            player.SetState(CH_STATE.AttackRun);
                    }

                    return false;
                }
            }
        }

        return true;
    }
}
