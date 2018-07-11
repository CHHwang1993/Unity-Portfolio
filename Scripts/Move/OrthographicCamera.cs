using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicCamera : MonoBehaviour {

    private FSMPlayer player;

    private void Start()
    {
        player = GameSceneManager.Instance.Player;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }
}
