using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutFont : MonoBehaviour
{
    float PositionY;
    float ColorAlpha = 1.0f;
    public float Speed;
    TMPro.TextMeshPro MeshPro;

    // Use this for initialization
    void Start()
    { 
        MeshPro = GetComponent<TMPro.TextMeshPro>();
    }

    private void OnEnable()
    {
        StartCoroutine(DisableFont());

        PositionY = 0.0f;
        ColorAlpha = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Camera.main.transform.rotation;

        PositionY += Speed * Time.deltaTime;

        ColorAlpha -= 1.0f * Time.deltaTime;
        
        MeshPro.faceColor = new Color(MeshPro.faceColor.r, MeshPro.faceColor.g, MeshPro.faceColor.b, ColorAlpha);
        MeshPro.outlineColor = new Color(MeshPro.outlineColor.r, MeshPro.outlineColor.g, MeshPro.outlineColor.b, ColorAlpha);

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + PositionY, this.transform.position.z);
    }

    IEnumerator DisableFont()
    {
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
    }
}
