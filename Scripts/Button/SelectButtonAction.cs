using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectButtonAction : MonoBehaviour {

    public Animator m_Anim;
    public UnityEvent SelectButtonEvent;

    public void Select()
    {
        m_Anim.SetInteger("state", 1);

        if(SelectButtonEvent!=null)
            SelectButtonEvent.Invoke();
    }
}
