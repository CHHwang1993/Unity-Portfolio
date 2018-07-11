using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMemoryPool : MonoBehaviour {

    public int[] EffectCount;
    public string[] EffectName;

    // Use this for initialization
    private void Start()
    {
        //effect
        if (EffectCount == null) return;
        if (EffectName == null) return;

        if (EffectName.Length != EffectCount.Length) return;
        for (int i = 0; i < EffectCount.Length; ++i)
        {
            MemoryPoolManager.Instance.SetObject(EffectName[i], EffectCount[i]);
        }
    }
}
