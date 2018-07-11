using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneButtonAction : MonoBehaviour {

    public string sceneName;

    public void LoadingScene()
    {
        LoadingSceneManager.LoadingScene(sceneName);
    }
}
