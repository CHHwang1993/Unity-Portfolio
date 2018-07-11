using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartActionCamera : MonoBehaviour {

    public UnityEvent StartActionCameraEvent;
    public UnityEvent StopActionCameraEvent;
    public UnityEvent ReturnMainCameraEvent;
    public float coroutineTime;

    private void OnTriggerEnter(Collider other)
    {
        SoundManager.Instance.PlayBGM("BossOpen", true, 0.7f);

        if(StartActionCameraEvent!=null)
            StartActionCameraEvent.Invoke();

        StartCoroutine(StopCameraAction());
    }

    private IEnumerator StopCameraAction()
    {
        yield return new WaitForSeconds(coroutineTime);

        if(StopActionCameraEvent != null)
            StopActionCameraEvent.Invoke();
        StartCoroutine(ReturnMainCamera());

        SoundManager.Instance.PlaySFX("Orca_Attack2");
    }

    private IEnumerator ReturnMainCamera()
    {
        yield return new WaitForSeconds(5.0f);

        if(ReturnMainCameraEvent != null)
            ReturnMainCameraEvent.Invoke();

        SoundManager.Instance.PlayBGM("BossBGM");
    }
}
