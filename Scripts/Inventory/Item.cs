using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ITEM_TYPE
{
    HP,
    MP,
    WEAPON
}

public class Item {

    public ITEM_TYPE type;
    public Sprite DefaultImg;
    public int MaxCount;
    public string Name;

    public void Init(string Name, int MaxCount)
    {
        switch (Name)
        {
            case "HP": type = ITEM_TYPE.HP; break;
            case "MP": type = ITEM_TYPE.MP; break;
        }
        this.Name = Name;
        this.MaxCount = MaxCount;

        Sprite[] SpriteArray = Inventory.Instance.SpriteArray;
        int Count = SpriteArray.Length;

        for(int i=0; i<Count; ++i)
        {
            if(SpriteArray[i].name == this.Name)
            {
                DefaultImg = SpriteArray[i];
                break;
            }
        }
    }

    void AddItem()
    {
        Inventory Iv = Inventory.Instance;

       if(!Iv.AddItem(this))
       {
           Debug.Log("아이템이 꽉참");
       }
    }
}
