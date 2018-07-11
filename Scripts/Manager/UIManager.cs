using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonBase<UIManager>
{
    public Text FPS;
    public Image MonsterPanel;
    public Image MonsterHP;
    public Text MonsterName;
    public Image[] CoolTimes;

    private float timeleft;
    private float fps;
    private int frames; // Frames drawn over the interval

    private void Update()
    {
        //FPS
        timeleft -= Time.deltaTime;
        frames++;

        if (frames >= 60) frames = 60;

        if (timeleft <= 0.0f)
        {
            fps = frames;
            frames = 0;
            timeleft = 1;
        }

        FPS.text = "FPS : " + fps;

        if (GameSceneManager.Instance.Player.IsClickEnemy || (MonsterName.text=="Evil Orca" && !GameSceneManager.Instance.Player.Enemy.IsDead()))
        {
            if(!MonsterPanel.gameObject.activeSelf)
                MonsterPanel.gameObject.SetActive(true);

            MonsterHP.fillAmount = GameSceneManager.Instance.Player.Enemy.Health.RemainingAmount();

            MonsterName.text = GameSceneManager.Instance.Player.Enemy.State.Name;
        }
        else
        {
            MonsterPanel.gameObject.SetActive(false);
        }
    }



    ///////////////////////쿨타임 코루틴////////////////////////////////////////

    public IEnumerator SkillCoolTime(int index)
    {
        do
        {
            yield return null;

            CoolTimes[index].fillAmount -= 1 * Time.smoothDeltaTime / GameSceneManager.Instance.Player.CoolTimes[index];

        } while (CoolTimes[index].fillAmount > 0);
    }
}
