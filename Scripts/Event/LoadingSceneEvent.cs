using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneEvent : MonoBehaviour {

    public string sceneName;

	public void LoadingScene()
    {
        LoadingSceneManager.LoadingScene(sceneName);
    }
}
