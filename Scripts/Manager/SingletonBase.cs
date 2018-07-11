using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    Debug.LogError("New Manager Create Failed -> " + typeof(T));
                }
            }

            return instance;
        }
    }
}
