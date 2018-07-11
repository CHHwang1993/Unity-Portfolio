using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRespawn : MonoBehaviour {

    public string Name;

	// Use this for initialization
	void Start() {
        if (Name != null)
            MemoryPoolManager.Instance.CreateChildObject(Name, this.transform);
    }
}
