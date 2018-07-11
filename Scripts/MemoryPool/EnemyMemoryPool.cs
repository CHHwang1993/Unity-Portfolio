using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour {

    public int[] EnemyCount;
    public string[] EnemyName;

    // Use this for initialization
    private void Start()
    {
        //effect
        if (EnemyCount == null) return;
        if (EnemyName == null) return;

        if (EnemyName.Length != EnemyCount.Length) return;
        for (int i = 0; i < EnemyCount.Length; ++i)
        {
            MemoryPoolManager.Instance.SetObject(EnemyName[i], EnemyCount[i]);
        }
    }
}
