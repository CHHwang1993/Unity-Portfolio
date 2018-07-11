using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrhowAxeTrigger : MonoBehaviour {

    FSMPlayer player;
    Vector3 direction;
    Vector3 targetPosiiton;
    Vector3 originPosition;
    float Speed = 15.0f;

    private void Awake()
    {
        player = GameSceneManager.Instance.Player;
    }

    private void OnEnable()
    {
        if (player)
        {
            targetPosiiton = new Vector3(player.Point.position.x, player.transform.position.y, player.Point.position.z);

            direction = Vector3.Normalize(targetPosiiton - player.transform.position);

            originPosition = player.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0, 0, 1), 30);
        this.transform.Translate(direction * Time.deltaTime * Speed, Space.World);

        if (Vector3.Distance(originPosition, transform.position) > 15)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            FSMEnemy enemy = other.GetComponent<FSMEnemy>();

            enemy.TakeDamage();
        }
    }
}
