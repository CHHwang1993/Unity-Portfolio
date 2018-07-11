using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPoolManager : SingletonBase<MemoryPoolManager>
{ 
    ///////////////프리팹 및 매니저배열//////////////////////////
    public GameObject[] Prefab;
    public List<GameObject> Manager;

	public void SetObject(GameObject gameObject, int maxCount, string Name, Transform parent =null)
    {
        for(int i=0; i<maxCount; ++i)
        {
            GameObject obj = GameObject.Instantiate(gameObject) as GameObject;
            obj.transform.name = Name;
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(false);
            obj.transform.SetParent(parent);
            Manager.Add(obj);
        }
    }

    public void SetObject(string Name, int maxCount =5, Transform parent =null)
    {
        GameObject obj = null;

        int count = Prefab.Length;

        for(int i=0; i<count; ++i)
        {
            if(Prefab[i].name == Name)
            {
                obj = Prefab[i];
                break;
            }
        }

        SetObject(obj, maxCount, Name, parent);
    }

    public GameObject GetObject(string Name)
    {
        if (Manager == null) return null;

        int count = Manager.Count;

        for (int i = 0; i < count; ++i)
        {
            if (Name != Manager[i].name) continue;

            GameObject obj = Manager[i];

            if (obj.activeSelf)
            {
                if (i == count - 1)
                {
                    SetObject(obj, 1, Name);
                    return Manager[i + 1];
                }
                continue;
            }
            return Manager[i];
        }

        return null;
    }

    public void CreateObject(string Name, Transform transform)
    {
        GameObject obj = GetObject(Name);

        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;

        obj.SetActive(true);
    }

    public void CreateObject(string Name)
    {
        GameObject obj = GetObject(Name);

        obj.SetActive(true);
    }

    public void CreateObject(string Name, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetObject(Name);

        obj.transform.position = position;
        obj.transform.rotation = rotation;

        obj.SetActive(true);
    }

    public void CreateChildObject(string Name, Transform transform)
    {
        GameObject obj = GetObject(Name);

        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.transform.parent = transform.root;

        obj.SetActive(true);
    }

    public void CreateTextObject(string Name, Vector3 position, int Damage)
    {
        GameObject obj = GetObject(Name);

        obj.transform.position = position;

        obj.GetComponent<TMPro.TextMeshPro>().text = Damage.ToString();

        obj.SetActive(true);
    }

    public void MemoryDelete()
    {
        if (Manager == null)
            return;

        int count = Manager.Count;

        for(int i=0; i<count; ++i)
        {
            GameObject obj = Manager[i];
            GameObject.Destroy(obj);
        }

        Manager = null;
    }

    
}
