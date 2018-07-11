using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneManager : SingletonBase<GameSceneManager>
{  
    private FSMPlayer player;
    private Scene currentScene;
    

    private void Awake()
    { 
        if(GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<FSMPlayer>();

        currentScene = SceneManager.GetActiveScene();

        Invoke("Init", 0.02f);
    }
    void Init()
    {
        PlayerIO.LoadData();
    }

    public FSMPlayer Player { get { return player; } }
    public Scene Scene { get { return currentScene; } }
}
