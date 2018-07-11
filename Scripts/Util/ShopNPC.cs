using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopNPC : MonoBehaviour {

    Animator m_Anim;
    public UnityEvent OpenShopEvent;
    public IntVariable Gold;


    int currentSelect = -1;

    private void Awake()
    {
        m_Anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StopAnimation());
            m_Anim.SetBool("IsCommunity", true);
            OpenShop();
        }
    }

    void OpenShop()
    {
        currentSelect = -1;

        if(OpenShopEvent!=null)
            OpenShopEvent.Invoke();
    }

    public void Buy()
    {
        if (Gold.Value > 100)
        {
            if (currentSelect == 0)
            {
                Gold.ApplyChange(-100);

                Item item = new Item();
                item.Init("HP", 10);

                Inventory.Instance.AddItem(item);
            }
            else if (currentSelect == 1)
            {
                Gold.ApplyChange(-100);

                Item item = new Item();
                item.Init("MP", 10);

                Inventory.Instance.AddItem(item);
            }
        }
    }

    private IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(0.3f);

        m_Anim.SetBool("IsCommunity", false);
    }

    public int SelectNumber { set {currentSelect= value;}}
}
