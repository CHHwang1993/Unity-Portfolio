using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopEffect : MonoBehaviour {

    public float effectTime = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(Stop());
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(effectTime);
        gameObject.SetActive(false);
    }
}
