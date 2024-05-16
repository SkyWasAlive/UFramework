using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameEventParameterized : UnityEvent< List<Argument>>{

}

[HideInInspector]
public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    public GameEventParameterized Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised( List<Argument> Arguments)
    {
        Response.Invoke(Arguments);
    }
}