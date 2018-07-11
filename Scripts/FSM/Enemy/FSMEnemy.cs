using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Text;

public class FSMEnemy : FSMBase {
 
    public List<Transform> WayPoints;
    public Collider[] Colliders;
    public GameObject MinimapSphere;
    public EnemyHealth Health;
    public UnityEvent EnemyDiedEvent;
  
    protected Transform DummyRoot;
    protected FSMPlayer player;
    protected NavMeshAgent NavMesh;
    protected GameObject Sphere;
    protected bool IsAlive=false;

    protected StringBuilder stringBuilder;

    protected override void Awake()
    {
        base.Awake();

        player = GameSceneManager.Instance.Player;
        Health = this.GetComponent<EnemyHealth>();

        NavMesh = GetComponent<NavMeshAgent>();
        NavMesh.speed = State.RunSpeed;
        NavMesh.angularSpeed = State.TurnSpeed;

        Sphere = GameObject.Instantiate(MinimapSphere) as GameObject;
        Sphere.transform.parent = this.transform;
        Sphere.transform.localPosition = new Vector3(0, 0, 0);

        stringBuilder = new StringBuilder(64);

        if (this.transform.Find("Dummy_root")) DummyRoot = this.transform.Find("Dummy_root").transform;
     
        for(int i=0; i< Colliders.Length; ++i) Colliders[i].enabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Health.HP = Health.maxHP.Value;

        StartCoroutine(Alive());

        SetState(CH_STATE.MS_Wait);
    }

    protected virtual void LateUpdate()
    {
        if (DummyRoot)
            DummyRoot.localPosition = new Vector3(0, 0, 0);
    }

    protected bool DetectPlayer()
    { 
        return !CompareDist(transform.position, player.transform.position, State.ChasingRange);
    }

    public void TakeDamage()
    {
        if (IsDead()) return;

        StartCoroutine(ReDoColor());

        ChangeColor(GetComponentsInChildren<SkinnedMeshRenderer>(), new Color(0.3455882f, 0, 0));

        float Damage;

        if (player.IsWhirlwind())
        {
            Damage = Random.Range(player.MinDamage / 2, player.MaxDamage/2);

            SoundManager.Instance.PlaySFX("Whirlwind_Explosion",0.3f);

            Health.TakeDamage((int)Damage);
        }
        else if (player.IsBash())
        {
            Damage = Random.Range(player.MinDamage*2, player.MaxDamage*2);

            Health.TakeDamage((int)Damage);
        }
        else
        {
            Damage = Random.Range(player.MinDamage, player.MaxDamage);

            SoundManager.Instance.PlaySFX("BasicAttack", 0.5f);

            Health.TakeDamage((int)Damage);
        }

        MemoryPoolManager.Instance.CreateObject("Hit1", transform);

        Vector3 FontPosition;

        if (this.State.Name != "Evil Orca")
            FontPosition = new Vector3(transform.position.x, this.transform.position.y + 2.0f, transform.position.z);
        else
            FontPosition = new Vector3(transform.position.x, this.transform.position.y + 6.0f, transform.position.z);

        MemoryPoolManager.Instance.CreateTextObject("DamageFont", FontPosition, (int)Damage);

        if (Health.IsDeath())
        {
            ChangeShader(GetComponentsInChildren<SkinnedMeshRenderer>(), "Custom/DissolveShader");
            NavMesh.isStopped = true;
            SetState(CH_STATE.MS_Dead);

            stringBuilder.Length = 0;
            stringBuilder.Append(State.Name);
            stringBuilder.Append("_Death");

            SoundManager.Instance.PlaySFX(stringBuilder.ToString());
            return;
        }

        if(CHState != CH_STATE.MS_Attack && CHState != CH_STATE.MS_Attack2 && CHState != CH_STATE.MS_Attack3 && CHState!=CH_STATE.MS_BigAttack)
        {
            MoveUtil.RotateToDirBurst(transform, player.transform);
        }
    }

    public void OnAttack()
    {
        if (!player.IsInvicibility) player.TakeDamage(State.AttackDamage);
    }

    public bool CompareDist(Vector3 a, Vector3 b, float dist)
    {
        if (Vector3.Distance(a, b) > dist)
            return true;

        return false;
    }

    public bool IsDead() { return (CH_STATE.MS_Dead == CHState); }

    public NavMeshAgent GetNavMesh() { return NavMesh; }

    IEnumerator Alive()
    {
        yield return new WaitForSeconds(1.0f);

        MonsterRespawn[] Respawns =  this.transform.root.GetComponentsInChildren<MonsterRespawn>();

        ChangeShader(GetComponentsInChildren<SkinnedMeshRenderer>(), "Custom/DissolveShader");

        for (int i=0; i< Respawns.Length; ++i)
        {
            WayPoints.Add(Respawns[i].transform);
        }

        Sphere.SetActive(true);
        m_CC.enabled = true;
        NavMesh.enabled = true;
        IsAlive = true;
    }

    //코루틴
    protected virtual IEnumerator MS_Wait()
    {
        float _t = 0.0f;

        do
        {
            yield return null;

            _t += Time.deltaTime;

            if (IsAlive && !CompareDist(this.transform.position,player.transform.position,50.0f))
            {
                if (_t >= State.RestTime)
                {
                    SetState(CH_STATE.MS_Run);
                    break;
                }

                if (DetectPlayer() && !player.IsDead())
                {
                    SetState(CH_STATE.MS_AttackRun);
                    break;
                }
            }
           
        } while (!isNewState);
    }

    protected virtual IEnumerator MS_Run()
    {
        Transform target = WayPoints[Random.Range(0, WayPoints.Count)];
        do
        {
            yield return null;

            if (IsDead()) break;

            NavMesh.speed = State.WalkSpeed;

            NavMesh.destination = new Vector3(target.position.x +Random.Range(-2.0f, 2.0f), target.position.y,
                target.position.z + Random.Range(-2.0f, 2.0f));
            NavMesh.isStopped = false;

            if(!CompareDist(NavMesh.destination,transform.position,0.3f))
            {
                NavMesh.isStopped = true;
                SetState(CH_STATE.MS_Wait);
                break;
            }
 
            if (DetectPlayer() && !player.IsDead())
            {
                SetState(CH_STATE.MS_AttackRun);
                break;
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator MS_Attack()
    {

        MoveUtil.RotateToDirBurst(transform, player.transform);

        do
        {
            yield return null;

            if (IsDead()) break;

            if (CompareDist(transform.position,player.transform.position, State.AttackRange)
                && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.7f)
            {
                SetState(CH_STATE.MS_AttackRun);
                break;
            }

            else if (!CompareDist(transform.position, player.transform.position, State.AttackRange)
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

    protected virtual IEnumerator MS_AttackRun()
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
                SetState(CH_STATE.MS_Attack);
                break;
            }

            if(!DetectPlayer())
            {
                NavMesh.isStopped = true;
                SetState(CH_STATE.MS_Wait);
                break;
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator MS_Dead()
    {
        Sphere.SetActive(false);
        m_CC.enabled = false;
        IsAlive = false;

        if (EnemyDiedEvent != null)
            EnemyDiedEvent.Invoke();

        for (int i=0; i<Colliders.Length; ++i) Colliders[i].enabled = false;

        yield return new WaitForSeconds(9.5f);
        StartCoroutine(RealDead());
    }

    protected virtual IEnumerator RealDead()
    {
        float Color = 0;

        do
        {
            yield return null;

            Color += Time.smoothDeltaTime* 1.0f;

            SkinnedMeshRenderer[] Meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < Meshes.Length; ++i)
            {
                Material[] materials = Meshes[i].materials;

                for (int j = 0; j < materials.Length; ++j)
                {
                    materials[j].SetFloat("_DissolveAmount", Color);
                }
            }

        } while (Color<1);

        this.gameObject.SetActive(false);
    }
}
