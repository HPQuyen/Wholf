using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PunEventHandler : MonoBehaviour
{
    #region Private Fields
    private static Dictionary<byte, Func<object[]>> eventListener = new Dictionary<byte, Func<object[]>>();
    private static Dictionary<byte, Action<EventData>> eventReceiver = new Dictionary<byte, Action<EventData>>();
    private const byte MaxEventID = 200;
    #endregion

    #region Monobehavior Methods
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    #endregion
    #region Private Methods
    private void OnEvent(EventData photonEvent)
    {
        try
        {
            if (photonEvent.Code < MaxEventID)
            {
                eventReceiver[photonEvent.Code].Invoke(photonEvent);
            }
        }
        catch (Exception exc)
        {
            Debug.LogError("Error: " + exc.Message +", photonEvent Code: " + photonEvent.Code);
        }
    }
    #endregion
    #region Public Methods
    public static void RegisterEvent(byte eventID, Func<object[]> func)
    {
        eventListener.Add(eventID, func);
    }
    public static void RaiseEvent(byte eventID, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
    {
        try
        {
            PhotonNetwork.RaiseEvent(eventID, eventListener[eventID].Invoke(), raiseEventOptions, sendOptions);
        }
        catch (Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public static void QuickRaiseEvent(byte eventID,object[] data, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
    {
        PhotonNetwork.RaiseEvent(eventID, data, raiseEventOptions, sendOptions);
    }
    public static void RegisterReceiveEvent(byte eventID,Action<EventData> action)
    {
        eventReceiver.Add(eventID, action);
    }
    public static void RemoveAllEvent()
    {
        eventReceiver.Clear();
        eventListener.Clear();
    }
    #endregion
}
