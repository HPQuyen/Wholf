using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class Moderator : MonoBehaviour
{
    #region Private Fields SerializeField
    [SerializeField]
    private RoleDelivery roleDelivery = null;
    [SerializeField]
    private ListPlayerController listPlayerController = null;
    #endregion

    #region Private Fields
    private static Moderator instance = null;

    #endregion

    #region Public Fields

    #endregion

    #region MonoFunction
    private void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PunEventHandler.RegisterEvent(EventID.RoleDelivery, DeliverRole);
            PunEventHandler.RaiseEvent(EventID.RoleDelivery, new RaiseEventOptions() { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
        }
        PunEventHandler.RegisterReceiveEvent(EventID.RoleDelivery, ReceiveRole);

    }
    // Update is called once per frame
    private void Update()
    {
        
    }
    #endregion

    #region Turnbase Mechanism Methods

    #endregion

    #region Event Handle Methods
    private object[] DeliverRole()
    {
        Action<int, Dictionary<int, byte>,byte> generateRole = (int length, Dictionary<int, byte> playerRole,byte roleID) =>
          {
              System.Random rnd = new System.Random();
              for (int j = 0; j < length; j++)
              {
                  do
                  {
                      int tmp = rnd.Next(0, PhotonNetwork.PlayerList.Length);
                      if (!playerRole.ContainsKey(PhotonNetwork.PlayerList[tmp].ActorNumber))
                      {
                          playerRole.Add(PhotonNetwork.PlayerList[tmp].ActorNumber, roleID);
                          break;
                      }
                  } while (true);

              }
          };
        Dictionary<int, byte> playerRolePair = new Dictionary<int, byte>(); 
        //roleDelivery = Resources.Load<RoleDelivery>("RoleDelivery/Custom");
        generateRole.Invoke(roleDelivery.villager,playerRolePair,(byte) RoleID.villager);
        generateRole.Invoke(roleDelivery.hunter, playerRolePair, (byte)RoleID.hunter);
        generateRole.Invoke(roleDelivery.witch, playerRolePair, (byte)RoleID.witch);
        generateRole.Invoke(roleDelivery.wolf, playerRolePair, (byte)RoleID.wolf);
        generateRole.Invoke(roleDelivery.seer, playerRolePair, (byte)RoleID.seer);
        generateRole.Invoke(roleDelivery.cupid, playerRolePair, (byte)RoleID.cupid);
        generateRole.Invoke(roleDelivery.guardian, playerRolePair, (byte)RoleID.guardian);
        foreach (var item in playerRolePair)
        {
            Debug.Log(item.Key);
            Debug.Log(item.Value);
        }
        int i = 0;
        object[] content = new object[PhotonNetwork.PlayerList.Length*2];
        foreach (var item in playerRolePair)
        {
            content[i] = (byte) item.Key;
            content[i+1] = item.Value;
            i += 2;
        }

        
        return content;
    }
    private void ReceiveRole(EventData photonEvent)
    {
        object[] data = (object[])photonEvent.CustomData;
        int[] playerID = new int[PhotonNetwork.PlayerList.Length];
        byte[] actorID = new byte[PhotonNetwork.PlayerList.Length];
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i += 2)
        {
            playerID[i] = (int) data[i];
            actorID[i] = (byte) data[i + 1];
        }
        listPlayerController.InitAllPlayerRole(playerID, actorID, OnDeliverRole);
    }
    private void OnDeliverRole()
    {
        
    }
    #endregion
}
