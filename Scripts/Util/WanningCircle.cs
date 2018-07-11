using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanningCircle : MonoBehaviour
{
    float Speed = 1.5f;
    Material[] Materials;
    Color color;

    // Use this for initialization
    void Start()
    {
        Materials = GetComponent<MeshRenderer>().materials;
    }

    private void OnEnable()
    {
        color = new Color(1, 0, 0, 1);
        Invoke("DisableObject", 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        color.r -= Speed * Time.deltaTime;

        if (color.r <= 0) color.r = 1.0f;

        for (int i = 0; i < Materials.Length; ++i)
        {
            Materials[i].SetColor("_EmissionColor", color);
        }
    }

    void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}
