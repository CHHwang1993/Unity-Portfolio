using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class FSMBase : MonoBehaviour
{
    public CharacterController m_CC;
    public Animator m_Anim;

    public CH_STATE CHState;
    public bool isNewState;

    public CharacterState State;

    protected virtual void Awake()
    {
        m_CC = GetComponent<CharacterController>();
        m_Anim = GetComponent<Animator>();
        State = GetComponent<CharacterState>();
    }

    protected virtual void OnEnable()
    {
        CHState = CH_STATE.Idle;
        StartCoroutine(FSMMain());
    }

    protected virtual void ChangeColor(SkinnedMeshRenderer[] meshs, Color color)
    {
        for (int i = 0; i < meshs.Length; ++i)
        {
            Material[] materials = meshs[i].materials;

            for (int j = 0; j < materials.Length; ++j)
            {
                materials[j].SetColor("_EmissionColor", color);
            }
        }
    }

    public void ChangeShader(SkinnedMeshRenderer[] meshs, string ShaderName)
    {
        SkinnedMeshRenderer[] Meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        Meshes = meshs;

        for (int i = 0; i < Meshes.Length; ++i)
        {
            Material[] materials = Meshes[i].materials;

            for (int j = 0; j < materials.Length; ++j)
            {
                Shader outline = Shader.Find(ShaderName);

                materials[j].shader = outline;
            }
        }
    }

    public void SetState(CH_STATE newState)
    {
        isNewState = true;
        CHState = newState;

        m_Anim.SetInteger("state", (int)CHState);
    }

    protected virtual IEnumerator FSMMain()
    {
        while (true)
        {
            isNewState = false;
            yield return StartCoroutine(CHState.ToString());
        }
    }

    protected virtual IEnumerator Idle()
    {
        do
        {
            yield return null;
        }
        while (!isNewState);
    }

    protected virtual IEnumerator ReDoColor()
    {
        do
        {
            yield return new WaitForSeconds(0.5f);

            ChangeColor(GetComponentsInChildren<SkinnedMeshRenderer>(), new Color(0, 0, 0));

        } while (!isNewState);
    }
}
