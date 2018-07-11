using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public sealed class PlayerIO : MonoBehaviour {

	public static void SaveData()
    {
       FSMPlayer player = GameSceneManager.Instance.Player;
       
       XmlDocument XmlDoc = new XmlDocument();
       XmlElement XmlEl = XmlDoc.CreateElement("PlayerDB");
       XmlDoc.AppendChild(XmlEl);
       
       XmlElement ElementSetting = XmlDoc.CreateElement("Player");
       
       int HP = (int)player.Health.HP;
       int MP = (int)player.Mana.MP;
       
       ElementSetting.SetAttribute("CurrentHP", HP.ToString());
       ElementSetting.SetAttribute("CurrentMP", MP.ToString());
       ElementSetting.SetAttribute("Gold", player.Gold.ToString());
       
       XmlEl.AppendChild(ElementSetting);
       
       XmlDoc.Save(Application.dataPath + "/Resources/PlayerData.xml");

    }

    public static void LoadData()
    {
       if (!System.IO.File.Exists(Application.dataPath + "/Resources/PlayerData.xml")) return;
       
       XmlDocument XmlDoc = new XmlDocument();
       
       XmlDoc.Load(Application.dataPath + "/Resources/PlayerData.xml");
       XmlElement XmlEl = XmlDoc["PlayerDB"];
       
       FSMPlayer player = GameSceneManager.Instance.Player;

        foreach (XmlElement ItemElement in XmlEl.ChildNodes)
        { 
            player.Health.HP = System.Convert.ToSingle(ItemElement.GetAttribute("CurrentHP"));
            player.Mana.MP = System.Convert.ToSingle(ItemElement.GetAttribute("CurrentMP"));
            player.Gold = System.Convert.ToInt32(ItemElement.GetAttribute("Gold"));
        }
    }
}
