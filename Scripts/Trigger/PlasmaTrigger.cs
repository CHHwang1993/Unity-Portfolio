using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaTrigger : MonoBehaviour {

    public float DestroyTime=1.0f;

    private void OnEnable()
    {
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        this.transform.position = new Vector3(this.transform.position.x, -4.5f, this.transform.position.z);
        Invoke("DisableObject", DestroyTime);
    }

    private void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}
