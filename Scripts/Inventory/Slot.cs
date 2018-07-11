using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {

    public Stack<Item> StackSlot;
    public Text text;
    public Sprite DefaultImg;

    private Image ItemImg;
    private bool IsSlot;

    public Item GetItem() { return StackSlot.Peek(); } //슬롯에 존재하는 아이템 반환
    public bool ItemMax(Item item) { return item.MaxCount > StackSlot.Count; }
    public bool GetIsSlot() { return IsSlot; }
    public void SetIsSlot(bool b) { this.IsSlot = b;}

    // Use this for initialization
    private void Awake()
    {
        StackSlot = new Stack<Item>();

        ItemImg = transform.GetChild(0).GetComponent<Image>();

        IsSlot = false;
    }

    void Start () {

        float Size = text.gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        text.fontSize = (int)(Size * 0.5f);
	}

    public void AddItem(Item item)
    {
        StackSlot.Push(item);
        UpdateInfo(true, item.DefaultImg);
    }

    public void UseItem()
    {
        if (!IsSlot) return;

        if (StackSlot.Count == 1)
        {
            StackSlot.Clear();

            UpdateInfo(false, DefaultImg);

            return;
        }

        StackSlot.Pop();

        UpdateInfo(IsSlot, ItemImg.sprite);
    }

    public void UpdateInfo(bool isSlot, Sprite sprite)
    {
        SetIsSlot(isSlot);

        ItemImg.sprite = sprite;
        text.text = StackSlot.Count > 1 ? StackSlot.Count.ToString() : "";

        ItemIO.SaveData();
    }
}
