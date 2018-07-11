using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {

        private List<GameEventListener> eventListeners = new List<GameEventListener>();

        public void Raise()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnRegisterListener(GameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }

}