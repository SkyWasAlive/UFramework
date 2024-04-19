using UnityEngine;
using UnityEngine.Events;

namespace UFramework.GameEvents
{
    [HideInInspector]
    public class GameEventListener<T> : MonoBehaviour
    {
        public GameEvent<T> Event;
        public UnityEvent<T?> Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke(null);
        }

        public void OnEventRaised(T eventParameter)
        {
            Response.Invoke(eventParameter);
        }
    }
}
