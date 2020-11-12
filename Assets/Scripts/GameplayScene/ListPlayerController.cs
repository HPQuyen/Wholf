﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

using UnityEngine;


public enum RoleID
{
    villager,
    wolf,
    seer,
    witch,
    guardian,
    cupid,
    hunter
}
public class ListPlayerController : MonoBehaviour
{
    #region Private SerializeField Fields

    #endregion

    #region Public Fields
    public Transform[] characterPosition = null;
    #endregion

    #region Private Fields
    private Dictionary<int, GameObject> listPlayerObject = new Dictionary<int, GameObject>();
    private Dictionary<int, IRole> listPlayerRole = new Dictionary<int, IRole>();
    List<object> listPlayerInTurn = new List<object>();
    #endregion

    #region Monobehavior Methods

    #endregion

    #region Public Methods
    public void InitAllPlayerRole(int[] playersID, byte[] roleID, Action onInitializeRole)
    {
        try
        {
            for(int i = 0; i < playersID.Length;i++)
            {
                for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
                {
                    GameObject playerObject = null;
                    IRole playerRole = null;
                    if (playersID[i] == PhotonNetwork.PlayerList[j].ActorNumber)
                    {
                        InitOnePlayerRole(roleID[i], ref playerObject, ref playerRole);
                        // Load player prefabs from Resources
                        playerObject = Instantiate(playerObject, characterPosition[j].position, characterPosition[j].rotation);
                        listPlayerObject.Add(PhotonNetwork.PlayerList[j].ActorNumber, playerObject);
                        listPlayerRole.Add(PhotonNetwork.PlayerList[j].ActorNumber, playerRole);
                        // Load RoleUI for local player
                        if (playersID[i] == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            PlayerUIController.GetInstance().LoadPlayerUI(listPlayerRole[PhotonNetwork.LocalPlayer.ActorNumber], roleID[j]);
                        }
                        break;
                    }
                }
            }
            // Load UI for local player
            onInitializeRole();
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public void CallRoleWakeUp(RoleID roleID)
    {
        Debug.Log(roleID);
        foreach (var item in listPlayerRole)
        {
            if(item.Value.IsMyRole(roleID))
            {
                // It is turn of player has that role
                listPlayerInTurn.Add(item.Key);
            }
        }
        // Raise Event for this player
        PunEventHandler.QuickRaiseEvent(EventID.RoleAwakeness, listPlayerInTurn.ToArray(), new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    
    public int CountNumberOfRole(RoleID roleID)
    {
        int sum = 0;
        foreach (var item in listPlayerRole)
        {
            if (item.Value.IsMyRole(roleID))
            {
                sum++;
            }
        }
        return sum;
    }
    public void ReceiveRoleWakeUp(int playerID,Action OnAwakeRole)
    {
        if(!listPlayerInTurn.Contains(playerID))
        {
            listPlayerInTurn.Add(playerID);
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            OnAwakeRole.Invoke();
        }
    }
    // Test function callback done role action
    public void ReceiveRoleActionComplete(int playerID,Action OnDoneInMyTurn)
    {
        try
        {
            listPlayerInTurn.Remove(playerID);
            if (listPlayerInTurn.Count <= 0)
                OnDoneInMyTurn.Invoke();
        }catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    #endregion

    #region Private Methods
    private void InitOnePlayerRole(byte roleID, ref GameObject playerObjectRef, ref IRole playerRoleRef)
    {
        string path = "CharacterObject/";
        try
        {
            switch (roleID)
            {
                case (byte)RoleID.villager:
                    {

                        playerObjectRef = Resources.Load<GameObject>(path + "Villager");
                        playerRoleRef = playerObjectRef.GetComponent<Villager>();
                        break;
                    }
                case (byte)RoleID.wolf:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Wolf");
                        playerRoleRef = playerObjectRef.GetComponent<Wolf>();
                        break;
                    }
                case (byte)RoleID.seer:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Seer");
                        playerRoleRef = playerObjectRef.GetComponent<Seer>();
                        break;
                    }
                case (byte)RoleID.witch:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Witch");
                        playerRoleRef = playerObjectRef.GetComponent<Witch>();
                        break;
                    }
                case (byte)RoleID.guardian:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Guardian");
                        playerRoleRef = playerObjectRef.GetComponent<Guardian>();
                        break;
                    }
                case (byte)RoleID.cupid:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Cupid");
                        playerRoleRef = playerObjectRef.GetComponent<Cupid>();
                        break;
                    }
                case (byte)RoleID.hunter:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Hunter");
                        playerRoleRef = playerObjectRef.GetComponent<Hunter>();
                        break;
                    }
                default:
                    break;
            }
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }

    #endregion

}

