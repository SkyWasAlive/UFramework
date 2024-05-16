using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Argument
{
    public string name;
    public string Type;
    public object Parameter;

    public Argument(string name, string type,object parameter)
    {
        this.name = name;
        this.Type = type;
        this.Parameter = parameter;
    }
}





[CreateAssetMenu(menuName = "SkyFramework/Events/Game Event", fileName = "New Game Event", order = 0)]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> _listeners = new List<GameEventListener>();
    public List<Argument> Arguments;

    public void Raise()
    {
        foreach (var listener in _listeners)
        {
            listener.OnEventRaised(Arguments);
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        _listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        _listeners.Remove(listener);
    }

    public void SetArgument(string argumentName, Type type,object value)
    {
        // Check if an argument with the same name already exists in the list
        Argument existingArgument = Arguments.Find(argument => argument.name == argumentName);
        if (existingArgument != null)
        {
            // If it exists, update its value
            existingArgument.Parameter = value;
        }
        else
        {
            Debug.Log("attempted to set Argument that doesnt exists");
        }
    }
}