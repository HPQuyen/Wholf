using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ActionEventID
{
    InMyTurn,
    CompleteMyTurn
}
public class MyEntityEvent : UnityEvent<IRole> { }
public static class ActionEventHandler
{
    private static Dictionary<ActionEventID, UnityEvent> listActionEvent = new Dictionary<ActionEventID, UnityEvent>();
    public static void AddNewActionEvent(ActionEventID eventID,UnityAction callback)
    {
        UnityEvent actionEvent;
        if(listActionEvent.TryGetValue(eventID,out actionEvent))
        {
            actionEvent.AddListener(callback);
        }
        else
        {
            actionEvent = new UnityEvent();
            actionEvent.AddListener(callback);
            listActionEvent.Add(eventID, actionEvent);
        }
    }
    public static void Invoke(ActionEventID eventID)
    {
        try
        {
            listActionEvent[eventID].Invoke();
        }
        catch (Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }

    }
    public static void RemoveAction(ActionEventID eventID)
    {
        try
        {
            listActionEvent[eventID].RemoveAllListeners();
        }
        catch (Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
}
