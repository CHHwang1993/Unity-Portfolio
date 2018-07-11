using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPortal : MonoBehaviour {

    public GameObject Orca;
    public GameObject Portal;
	
	// Update is called once per frame
	void Update () {
		
        if(!Orca.activeSelf && !Portal.activeSelf)
        {
            Portal.SetActive(true);
        }
	}
}
