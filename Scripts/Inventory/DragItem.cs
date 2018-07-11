using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragItem : MonoBehaviour {

    public Transform Img;

    private Image EmptyImg;
    private Slot slot;


	// Use this for initialization
	void Start () {
        slot = GetComponent<Slot>();
        Img = GameObject.FindGameObjectWithTag("DragImg").transform;
        EmptyImg = Img.GetComponent<Image>();
	}

    public void Down()
    {
        if (!slot.GetIsSlot()) return;

        if(Input.GetMouseButtonDown(1))
        {
            switch (slot.GetItem().type)
            {
                case ITEM_TYPE.HP:
                    {
                        if (!GameSceneManager.Instance.Player.Health.IsFullHealth())
                        {
                            slot.UseItem();
                        }
                        break;

                    }
                case ITEM_TYPE.MP:
                    {
                        if (!GameSceneManager.Instance.Player.Mana.IsFullMana())
                        {
                            slot.UseItem();
                        }
                        break;

                    }
            }

            
            return;
        }

        Img.gameObject.SetActive(true);

        float Size = slot.transform.GetComponent<RectTransform>().sizeDelta.x;
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size);
        EmptyImg.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size);

        EmptyImg.sprite = slot.GetItem().DefaultImg;

        Img.transform.position = Input.mousePosition;

        slot.UpdateInfo(true, slot.DefaultImg);

        slot.text.text = "";
    }

    public void Drag()
    {
        if (!slot.GetIsSlot()) return;

        Img.transform.position = Input.mousePosition;
    }
    public void DragEnd()
    {
        if (!slot.GetIsSlot()) return;

        Inventory.Instance.Swap(slot, Img.transform.position);
    }

    public void Up()
    {
        if (!slot.GetIsSlot()) return;

        Img.gameObject.SetActive(false);

        slot.UpdateInfo(true, slot.StackSlot.Peek().DefaultImg);
    }
}
