using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommandPattern;

public class SkillCommand : Command {

    private int _Index = -1;

    public SkillCommand(int index)
    {
        _Index = index;
    }

    public override bool Execute(GameObject target)
    {
        Skill skill = target.GetComponent<Skill>();

        if (skill != null)
        {
            //마나가 필요없는 경우
            if (_Index == 5)
            {
                skill.Avoid();
                return true;
            }

            if (base.Execute(target))
            {
                if (_Index == 0) skill.PlayWhirlwind();
                else if (_Index == 1) skill.StopWhirlwind();
                else if (_Index == 2) skill.Bash();
                else if (_Index == 3) skill.Dash();
                else if (_Index == 4) skill.ThrowAxe();
                else if (_Index == 6) skill.Buff();


                PlayerIO.SaveData();

                return true;
            }
        }
        return false;
    }
}
