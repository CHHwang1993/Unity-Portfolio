using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommandPattern;

public class FSMPlayer : FSMBase
{
    //////////////////플레이어 정보///////////////////////////////
    public bool IsInvicibility = false;
    public bool IsWeaponEquiped = false;
    public bool IsClickEnemy = false;
    public IntVariable gold;
    public CharacterHealth Health;
    public CharacterMana Mana;
    //////////////////////////////////////////////////////////

    //////////////////쿨타임///////////////////////////////////
    public float[] CoolTimes = new float[6];
    //////////////////////////////////////////////////////////
    public Transform Point;
    public Transform DummyTF;
    public GameObject BackWeapon;
    public GameObject ArmWeapon;
    public GameObject WhirlwindFX;
    public GameObject DashTrail;
    public GameObject MinimapSphere;

    public string clickLayer = "Click";
    public string enemyLayer = "Enemy";
    
    Transform FxCenter = null;
    FSMEnemy enemy;
    GameObject Sphere;
    bool IsOpenInventory = false;
    int layerMask;
    float MinAttackDamage;
    float MaxAttackDamage;


    ///커맨드
    private Command moveCommand;
    private Command[] SkillCommands = new Command[7];

    //플레이어 정보 초기화
    protected override void Awake()
    {
        base.Awake();
       
        Health = this.GetComponent<CharacterHealth>();
        Mana = this.GetComponent<CharacterMana>();

        layerMask = LayerMask.GetMask(clickLayer, enemyLayer);

        Sphere = GameObject.Instantiate(MinimapSphere) as GameObject;
        Sphere.transform.parent = this.transform;
        Sphere.transform.localPosition = new Vector3(0, 0, 0);

        FxCenter = DummyTF.transform.Find("Bip01/Bip01-Pelvis/Bip01-Spine/Bip01-Spine1/Bip01-Spine2/FxCenter");

        MinAttackDamage = State.AttackDamage - State.AttackDamage * 0.1f;
        MaxAttackDamage = State.AttackDamage + State.AttackDamage * 0.1f;
    }

    // Use this for initialization
    void Start()
    {
        //메모리풀 세팅
        MemoryPoolManager.Instance.SetObject("ThrowMesh", 1);
        MemoryPoolManager.Instance.SetObject("BashEffect", 1);
        MemoryPoolManager.Instance.SetObject("BuffEffect", 1, FxCenter);
        MemoryPoolManager.Instance.SetObject("RecoveryHP", 10, FxCenter);
        MemoryPoolManager.Instance.SetObject("RecoveryMP", 10, FxCenter);

        PlayerIO.LoadData();

        moveCommand = new MoveCommand();
       
        for(int i=0; i<7; i++)
        {
            SkillCommands[i] = new SkillCommand(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead()) return;

        ClickMovement(); //클릭했을때 움직임

        if (!IsOpenInventory)
            OnSkillAction();

        OpenInventory(); //인벤토리를 연다.

        UseItem();//아이템을 사용한다.

        if (IsWhirlwind()) //현재 휠윈드중인지 확인하고 마나가 다 떨어졌을 경우 멈춤
        {
            if (Mana.IsEmptyMana())
            {
                WhirlwindFX.SetActive(false);
                SetState(CH_STATE.Wait);
            }
        }
    }

    private void LateUpdate()
    {
       DummyTF.localPosition = new Vector3(0, DummyTF.localPosition.y, 0);
    }


    /////////////////////////////////////일반 함수////////////////////////////////////////////////////
    void ClickMovement()
    {
        if (Input.GetMouseButtonDown(0) && !IsOpenInventory)
        {
            if (IsBash()) return;
            if (IsAvoid()) return;
            if (IsBuff()) return;
            if (IsThrowAxe()) return;

            bool result = moveCommand.Execute(this.gameObject);
            OnClickAction(result);
        }
    }

    void OnClickAction(bool b)
    {
        if (b == true)
        {
            IsClickEnemy = false;

            if (!IsWeaponEquiped) EquipWeapon(true);

            DashTrail.SetActive(false);

            Point.parent = null;

            if (enemy != null && !enemy.IsDead())
                ChangeShader(enemy.GetComponentsInChildren<SkinnedMeshRenderer>(), "Custom/DissolveShader"); //아웃라인 쉐이더에서 기본 쉐이더로 변경
        }
        else
        {
            IsClickEnemy = true;

            if (!IsWeaponEquiped) EquipWeapon(true);


            if (enemy!=null) ChangeShader(enemy.GetComponentsInChildren<SkinnedMeshRenderer>(), "Custom/DissolveShader"); //이전 클릭한 몬스터가 있다면 기본 쉐이더로 변경

            

            if (enemy != null && !enemy.IsDead())
                ChangeShader(enemy.GetComponentsInChildren<SkinnedMeshRenderer>(), "Custom/OutlineShader"); //클릭한 몬스터르 외곽선 쉐이더 적용

            
        }
    }

    void OnSkillAction()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            SkillCommands[0].Execute(gameObject);
        }
        else if(Input.GetKeyUp(KeyCode.Q))
        {
            if (IsWhirlwind()) SkillCommands[1].Execute(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            SkillCommands[2].Execute(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SkillCommands[3].Execute(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SkillCommands[4].Execute(gameObject);
        }
        if (Input.GetMouseButtonDown(1))
        {
            SkillCommands[5].Execute(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SkillCommands[6].Execute(gameObject);
        }
    }

    void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!Health.IsFullHealth())
            {
                Inventory.Instance.UseItem(ITEM_TYPE.HP);
                MemoryPoolManager.Instance.CreateObject("RecoveryHP", GetFxCenter());
                Health.RecoveryHP(100);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            if (!Mana.IsFullMana())
            {
                Inventory.Instance.UseItem(ITEM_TYPE.MP);
                MemoryPoolManager.Instance.CreateObject("RecoveryMP", GetFxCenter());
                Mana.RecoveryMP(50);
            }
        }
    }

    public void EquipWeapon(bool b)
    {
        IsWeaponEquiped = b;

        if (IsWeaponEquiped)
        {
            BackWeapon.gameObject.SetActive(false);
            ArmWeapon.gameObject.SetActive(true);
        }
        else
        {
            BackWeapon.gameObject.SetActive(true);
            ArmWeapon.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float enemyAttack)
    {
        StartCoroutine(ReDoColor());

        ChangeColor(GetComponentsInChildren<SkinnedMeshRenderer>(), new Color(0.3455882f, 0, 0));

        if (CHState == CH_STATE.Idle || m_Anim.GetCurrentAnimatorStateInfo(0).IsName("InWeapon")) SetState(CH_STATE.Wait);

        Health.TakeDamage(enemyAttack);

        PlayerIO.SaveData();

        if (Health.IsDeath())
        {
            SetState(CH_STATE.Dead);
        }
    }

    public void OnAttack()
    {
        enemy.TakeDamage();
    }

    public void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!IsOpenInventory)
            {
                IsOpenInventory = true;
                Inventory.Instance.gameObject.SetActive(true);
            }
            else
            {
                IsOpenInventory = false;
                Inventory.Instance.gameObject.SetActive(false);
            }
        }
    }

    public bool IsWhirlwind() { return (CH_STATE.Whirlwind == CHState); }
    public bool IsBash() { return (CH_STATE.Bash == CHState); }
    public bool IsDash() { return (CH_STATE.Dash == CHState); }
    public bool IsThrowAxe() { return (CH_STATE.ThrowAxe == CHState); }
    public bool IsAvoid() { return (CH_STATE.Avoid == CHState); }
    public bool IsBuff() { return (CH_STATE.OnBuff == CHState); }
    public bool IsDead() { return (CH_STATE.Dead == CHState); }

    public int Layer { get { return layerMask; } }
    public Transform GetFxCenter() { return FxCenter; }
    public FSMEnemy Enemy { get { return enemy; } set { enemy = value; } }
    public bool IsInventory { get { return IsOpenInventory; } set { IsOpenInventory = value; } }
    public int Gold { get { return gold.Value; } set { gold.SetValue(value); } }
    public float MaxDamage { get { return MaxAttackDamage; } set { MaxAttackDamage = value; } }
    public float MinDamage { get { return MinAttackDamage; } set { MinAttackDamage = value; } }


    /////////////////////////////////////////////코루틴////////////////////////////////////////////////////////
    protected override IEnumerator Idle() 
    {
        do
        {
            yield return null;

        } while (!isNewState);
    }

    protected virtual IEnumerator Wait()
    {
        float WaitTime = 0.0f;

        do
        {
            yield return null;

            if (!IsWeaponEquiped) EquipWeapon(true);

            WaitTime += Time.deltaTime;

            //대기시간이 끝나면 다시 Idle 상태로 돌린다.
            if (WaitTime >= 2.0f)
            {
                WaitTime = 0.0f;
                SetState(CH_STATE.Idle);
            }

        } while (!isNewState);
    }


    protected virtual IEnumerator Run() 
    {
        do
        {
            yield return null;

            //거리가 1미만으로 줄어들면 달리기를 멈춤

            if (MoveUtil.MoveFrame(m_CC, Point, State.WalkSpeed, State.TurnSpeed) < 1.0f)
            {  
                SetState(CH_STATE.Wait);
                break;
            }
        } while (!isNewState);
    }

    protected virtual IEnumerator AttackRun()
    {
        do
        {
            yield return null;

            //거리가 공격범위 안에 들어오면 공격 상태로 전환

            if (MoveUtil.MoveFrame(m_CC, Point, State.WalkSpeed, State.TurnSpeed) <= State.AttackRange)
            { 
                SetState(CH_STATE.Attack);
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator Attack()
    {
        do
        {
            yield return null;

            if (enemy.IsDead()) //적이 죽었을 경우
            {
                SetState(CH_STATE.Wait);
            }

            MoveUtil.RotateToDirBurst(transform,Point); //적으로 바로 회전

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("AttackR") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.7f)
            {
                SetState(CH_STATE.Wait);
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator Whirlwind()
    {
        float angle = 0;
        do
        {
            yield return null;

            Mana.ConsumeMP(5.0f * Time.deltaTime);

            if (m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.9f && m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Whirlwind"))
            {
                WhirlwindFX.SetActive(false);
                WhirlwindFX.SetActive(true);

                SoundManager.Instance.PlaySFX("Whirlwind_Loop",1.0f,true);
            }
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("WhirlwindShot"))
            {
                MoveUtil.MoveFrame(m_CC, Point, State.WalkSpeed, State.TurnSpeed);

                angle -= Time.deltaTime * State.TurnSpeed;

                WhirlwindFX.transform.localPosition = new Vector3(0, 0, 0);
                WhirlwindFX.transform.localRotation = Quaternion.Euler(90, 0, angle);
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator Bash()
    {
        m_CC.skinWidth = 0.0f;

        do
        {
            yield return null;
              
            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Bash") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.4f)
            {
                m_CC.skinWidth = 0.08f;
                SetState(CH_STATE.Wait);
                break;
            }
            else if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Bash") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f < 0.33f)
            {
                MoveUtil.MoveFrame(m_CC, Point, State.RunSpeed, State.TurnSpeed);
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator ThrowAxe()
    {
        do
        {
            yield return null;

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("ThrowAxe") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.7f)
            {
                SetState(CH_STATE.Wait);
                break;
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator Dash()
    {
        do
        {
            yield return null;

            if (MoveUtil.MoveFrame(m_CC, Point, State.RunSpeed, State.TurnSpeed) < 1.0f)
            {
                DashTrail.SetActive(false);
                SetState(CH_STATE.Wait);
                break;
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator Avoid()
    {
        do
        {
            yield return null;

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Avoid") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.75f)
            { 
                SetState(CH_STATE.Wait);
                break;
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator OnBuff()
    {
        do
        {
            yield return null;

            if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("OnBuff") && m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.8f)
            {
                SetState(CH_STATE.Wait);
                break;
            }

        } while (!isNewState);
    }

    protected virtual IEnumerator Dead()
    {
         yield return new WaitForSeconds(5.0f);

        StartCoroutine(RealDead());
    }

    //////////////////////////////////////////////일반 코루틴///////////////////////////////////////////////////////
    public virtual IEnumerator EndBuff() //버프가 끝났을 때
    {
        yield return new WaitForSeconds(10.0f);

        State.AttackDamage /= 2;

        MinAttackDamage = State.AttackDamage - State.AttackDamage * 0.1f;
        MaxAttackDamage = State.AttackDamage + State.AttackDamage * 0.1f;
    }

    protected virtual IEnumerator RealDead() //죽고나서 5초뒤에 시체가 사라지면서 마을로 이동
    {
        float Color = 0;

        yield return null;

        do
        {
            Color += Time.smoothDeltaTime * 1.0f;

            SkinnedMeshRenderer[] Meshes = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < Meshes.Length; ++i)
            {
                Material[] materials = Meshes[i].materials;

                for (int j = 0; j < materials.Length; ++j)
                {
                    materials[j].SetFloat("_DissolveAmount", Color);
                }
            }
        } while (Color < 1);

        if (Color >= 1)
        {
            LoadingSceneManager.LoadingScene("TownScene");
        }
    }
}
