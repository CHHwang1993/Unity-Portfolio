using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    [System.Serializable]
    public class Command
    {

        public virtual bool Execute(GameObject target)
        {
            CharacterMana mana = target.GetComponent<CharacterMana>();

            if (mana != null && !mana.IsEmptyMana())
            {
                return true;
            }
            return false;
        }
    }
}

