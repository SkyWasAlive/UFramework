using System.Collections.Generic;
using UnityEngine;

namespace UFramework.GameEvents
{
    
    [CreateAssetMenu(menuName = "SkyFramework/Events/Game Event",fileName = "New Game Event",order = 0)]
    public class GameEvent<T> : ScriptableObject
    {
        private List<GameEventListener<T>> _listeners = new List<GameEventListener<T>>();
        
        public void Raise()
        {
            for(int i = _listeners.Count -1; i >= 0; i--)
                _listeners[i].OnEventRaised();
        }

        public void Raise(T eventParameter){
            for(int i = _listeners.Count -1; i >= 0; i--)
                _listeners[i].OnEventRaised(eventParameter);
        }

        public void RegisterListener(GameEventListener<T> listener)
        {
            _listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener<T> listener)
        {
            _listeners.Remove(listener);
        }
    }
}