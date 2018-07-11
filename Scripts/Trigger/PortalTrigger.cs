using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour {


    public string SceneName;
    public float RotateSpeed=2;
    public float ScaleSpeed = 1.0f;

    // Use this for initialization
    void Start () {
      
    }
	
	// Update is called once per frame
	void Update () {

        float Speed = ScaleSpeed * Time.deltaTime;

        if (this.transform.localScale.x <3)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x + Speed, 
                this.transform.localScale.y + Speed,
                this.transform.localScale.z + Speed);
        }
        this.transform.Rotate(new Vector3(0, 0, 1), RotateSpeed);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (this.transform.localScale.x > 3)
        {
            if (other.CompareTag("Player"))
            {
                LoadingSceneManager.LoadingScene(SceneName);
            }
        }
    }
}
