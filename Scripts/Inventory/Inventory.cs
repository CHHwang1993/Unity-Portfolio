using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : SingletonBase<Inventory> {

    public List<GameObject> AllSlot;
    public RectTransform InvenRect;
    public GameObject OriginSlot;
    public Sprite[] SpriteArray;
    public IntVariable Gold;
 
    public float  slotSize;
    public float  slotGap;
    public float  slotCountX;
    public float  slotCountY;

    private float InvenWidth;
    private float InvenHeight;

    private Vector2 DragPosition;
    private Text GoldText;

    //인벤토리 초기화
    private void Awake()
    { 
        InvenWidth = (slotCountX * slotSize) + (slotCountX * slotGap) + slotGap;
        InvenHeight = (slotCountY * slotSize) + (slotCountY * slotGap) + slotGap;

        InvenHeight += InvenHeight * 0.3f;

        InvenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, InvenWidth);
        InvenRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, InvenHeight);

        for(int y=0; y< slotCountY; ++y)
        {
            for(int x=0; x<slotCountX; ++x)
            {
                GameObject slot = Instantiate(OriginSlot) as GameObject;
                RectTransform slotRect = slot.GetComponent<RectTransform>();

                RectTransform Item = slot.transform.GetChild(0).GetComponent<RectTransform>();

                slot.name = "slot_" + y + "_" + x;
                slot.transform.SetParent(this.transform);

                slotRect.localPosition = new Vector3((slotSize * x) + (slotGap * (x + 1)), -InvenHeight * 0.10f- ((slotSize * y) + (slotGap * (y + 1))), 0);
                slotRect.localScale = Vector3.one;

                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize);

                Item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize - slotSize * 0.3f);
                Item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize - slotSize * 0.3f);

                AllSlot.Add(slot);
            }
        }

        GoldText = this.transform.GetChild(0).GetComponent<Text>();

        Invoke("Init", 0.01f);
    }

    //XML에서 아이템 데이터 로드
    void Init()
    {
        ItemIO.LoadData(AllSlot);
        Inventory.Instance.gameObject.SetActive(false);
    }

    private void Update()
    {
        //현재 골드 텍스트가 있으면 갱신해준다.
        if (GoldText) GoldText.text = Gold.Value + "Gold";
    }

    //아이템을 클릭했을 때의 드래그 좌표 얻기
    public void Down()
    {
        DragPosition.x = Input.mousePosition.x - this.transform.position.x;
        DragPosition.y = Input.mousePosition.y - this.transform.position.y;
    }

    //드래그중은 마우스 좌표와 드래그 좌표의 차를 이용해 이동
    public void Drag()
    {
        this.transform.position = Input.mousePosition;
        this.transform.position = new Vector3(this.transform.position.x - DragPosition.x, this.transform.position.y - DragPosition.y, this.transform.position.z);
    }

    //아이템을 더한다.
    public bool AddItem(Item item)
    {
        int slotCount = AllSlot.Count;

        //슬롯에 아이템이 있고 아이템의 맥스치까지 채워지지 않았다면 추가
        for(int i=0; i<slotCount; ++i)
        {
            Slot slot = AllSlot[i].GetComponent<Slot>();

            if (!slot.GetIsSlot()) continue;

            if(slot.GetItem().type == item.type && slot.ItemMax(item))
            {
                slot.AddItem(item);
                return true;
            }
        }

        //위의 경우를 제외하고 빈 슬롯이 존재하면 추가
        for(int i=0; i<slotCount; ++i)
        {
            Slot slot = AllSlot[i].GetComponent<Slot>();

            if (slot.GetIsSlot()) continue;

            slot.AddItem(item);
            return true;
        }

        return false;
    }

    //아이템을 가져온다.
    public Slot GetItem(ITEM_TYPE type)
    {
        int Count = AllSlot.Count;

        for (int i = 0; i < Count; ++i)
        {
            Slot slot = AllSlot[i].GetComponent<Slot>();

            if (!slot.GetIsSlot()) continue;

            if (slot.GetItem().type == type)
            {
                return slot;
            }
        }

        return null;
    }

    //아이템을 사용
    public void UseItem(ITEM_TYPE type)
    {
        Slot slot = GetItem(type);

        if(slot !=null)
        {
            slot.UseItem();
        }
    }

    //가까운 슬롯을 가져온다.
    public Slot NearSlot(Vector3 pos)
    {
        float Min = 10000.0f;
        int Index = -1;

        int Count = AllSlot.Count;

        for(int i=0; i<Count; ++i)
        {
            Vector3 vPos = AllSlot[i].transform.GetChild(0).position;
            float Dist = Vector2.Distance(vPos, pos);

            if(Dist <Min)
            {
                Min = Dist;
                Index = i;
            }
        }

        if (Min > slotSize) return null;

        return AllSlot[Index].GetComponent<Slot>();
    }

    //스왑
    public void Swap(Slot slot, Vector3 pos)
    {
        Slot FirstSlot = NearSlot(pos);

        if(slot == FirstSlot || FirstSlot ==null) //같은 슬롯이거나 슬롯이 존재하지 않는다면 끝냄
        {
            slot.UpdateInfo(true, slot.StackSlot.Peek().DefaultImg); 
            return;
        }

        //바꿀 슬롯이 비어있다면 그냥 스왑~
        if(!FirstSlot.GetIsSlot())
        {
            Swap(FirstSlot, slot);
        }
        else //슬롯이 안비어 있다면 temp Stack에 데이터를 담아서 슬롯끼리 스왑
        {
            int Count = slot.StackSlot.Count;
            Item item = slot.StackSlot.Peek();
            Stack<Item> temp = new Stack<Item>();

            for(int i=0; i<Count; ++i)
            {
                temp.Push(item);
            }

            slot.StackSlot.Clear();

            Swap(slot, FirstSlot);

            Count = temp.Count;

            item = temp.Peek();

            for(int i=0; i<Count; ++i)
            {
                FirstSlot.StackSlot.Push(item);
            }

            FirstSlot.UpdateInfo(true, temp.Peek().DefaultImg);
        }
    }

    //슬롯끼리 스왑
    void Swap(Slot xFirst, Slot oSecond)
    {
        int Count = oSecond.StackSlot.Count;
        Item item = oSecond.StackSlot.Peek();

        for(int i=0; i<Count; ++i)
        {
            if(xFirst !=null)
            {
                xFirst.StackSlot.Push(item);
            }
        }

        if(xFirst !=null)
        {
            xFirst.UpdateInfo(true, oSecond.StackSlot.Peek().DefaultImg);
        }

        oSecond.StackSlot.Clear();
        oSecond.UpdateInfo(false, oSecond.DefaultImg);
    }

}
