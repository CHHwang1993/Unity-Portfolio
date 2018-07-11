using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public sealed class ItemIO : MonoBehaviour {

	public static void SaveData()
    {
        List<GameObject> item = Inventory.Instance.AllSlot;

        XmlDocument XmlDoc = new XmlDocument();
        XmlElement XmlEl = XmlDoc.CreateElement("ItemDB");
        XmlDoc.AppendChild(XmlEl);

        int Count = item.Count;

        for(int i=0; i<Count; ++i)
        {
            Slot ItemInfo = item[i].GetComponent<Slot>();

            if (!ItemInfo.GetIsSlot()) continue;

            XmlElement ElementSetting = XmlDoc.CreateElement("Item");

            ElementSetting.SetAttribute("SlotNumber", i.ToString());
            ElementSetting.SetAttribute("Name", ItemInfo.GetItem().type.ToString());
            ElementSetting.SetAttribute("Count", ItemInfo.StackSlot.Count.ToString());
            ElementSetting.SetAttribute("MaxCount", ItemInfo.GetItem().MaxCount.ToString());
            XmlEl.AppendChild(ElementSetting);
        }

        XmlDoc.Save(Application.dataPath + "/Resources/InventoryData.xml");
    }

    public static List<GameObject> LoadData(List<GameObject> SlotList)
    {
        if (!System.IO.File.Exists(Application.dataPath + "/Resources/InventoryData.xml"))
        return SlotList;

        XmlDocument XmlDoc = new XmlDocument();
        XmlDoc.Load(Application.dataPath + "/Resources/InventoryData.xml");
        XmlElement XmlEl = XmlDoc["ItemDB"];

        foreach (XmlElement ItemElement in XmlEl.ChildNodes)
        {
            Slot slot = SlotList[System.Convert.ToInt32(ItemElement.GetAttribute("SlotNumber"))].GetComponent<Slot>();

            Item item = new Item();

            string Name = ItemElement.GetAttribute("Name");
            int MaxCount = System.Convert.ToInt32(ItemElement.GetAttribute("MaxCount"));
            item.Init(Name, MaxCount);

            int Count = System.Convert.ToInt32(ItemElement.GetAttribute("Count"));

            for(int i=0; i<Count; ++i)
            {
                slot.AddItem(item);
            }
        }
        return SlotList;
    }
}
