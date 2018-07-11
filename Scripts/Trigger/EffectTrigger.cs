using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTrigger : MonoBehaviour {

    Vector3 Dir;
    Vector3 FirstPos;
    public float Speed = 10.0f;
    public float Distance = 20.0f;
    public string Name=null;
    public float AttackDamage;
    public bool isGroundCollision = false;

    private void OnEnable()
    {
        Dir = this.transform.forward;

        FirstPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Dir * Time.deltaTime * Speed, Space.World);

        if (Vector3.Distance(FirstPos, transform.position) > Distance)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FSMPlayer player = other.GetComponent<FSMPlayer>();

            if (player != null)
            {
                if (player.IsAvoid()) return;
                if (player.IsDead()) return;
                if (player.IsInvicibility) return;

                if (Name != null)
                    MemoryPoolManager.Instance.CreateObject(Name, transform);

                player.TakeDamage(AttackDamage);

                this.gameObject.SetActive(false);
            }
        }

        //For Plasma
        if(isGroundCollision)
        {
            if (other.gameObject.layer ==8)
            {
                MemoryPoolManager.Instance.CreateObject("Plasma", transform);
                this.gameObject.SetActive(false);
            }
        }
    }
}
