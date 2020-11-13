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
    private bool isDay = false;
    private bool isSetUpComplete = false;
    private bool isPlayerOnAction = false;
    private RoleID roleOnAction = RoleID.wolf;
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
            PunEventHandler.RegisterEvent(PunEventID.RoleDelivery, DeliverRole);
            PunEventHandler.RaiseEvent(PunEventID.RoleDelivery, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleDelivery, ReceiveRole);
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleAwakeness, ReceiveAwakenRole);
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleActionComplete, ReceiveCompleteActionRole);
    }
    // Update is called once per frame
    private void Update()
    {
        if (isSetUpComplete && PhotonNetwork.IsMasterClient)
        {
            if (isDay)
                Day();
            else
                Night();
        }
    }
    #endregion

    #region Turnbase Mechanism Methods
    private void Night()
    {
        if(!isPlayerOnAction)
        {
            listPlayerController.CallRoleWakeUp(this.roleOnAction,() => { isPlayerOnAction = true; } );
        }
    }
    private void Day()
    {
        
    }
    private void ChooseNextActiveRole(RoleID roleOnAction)
    {
        switch (roleOnAction)
        {
            case RoleID.wolf:
                if (listPlayerController.CountNumberOfRole(RoleID.seer) == 0)
                    ChooseNextActiveRole(RoleID.seer);
                else
                    this.roleOnAction = RoleID.seer;
                break;
            case RoleID.seer:
                if (listPlayerController.CountNumberOfRole(RoleID.guardian) == 0)
                    ChooseNextActiveRole(RoleID.guardian);
                else
                    this.roleOnAction = RoleID.guardian;
                break;
            case RoleID.guardian:
                if (listPlayerController.CountNumberOfRole(RoleID.cupid) == 0)
                    ChooseNextActiveRole(RoleID.cupid);
                else
                    this.roleOnAction = RoleID.cupid;
                break;
            case RoleID.cupid:
                if (listPlayerController.CountNumberOfRole(RoleID.hunter) == 0)
                    ChooseNextActiveRole(RoleID.hunter);
                else
                    this.roleOnAction = RoleID.hunter;
                break;
            case RoleID.hunter:
                if (listPlayerController.CountNumberOfRole(RoleID.witch) == 0)
                    ChooseNextActiveRole(RoleID.witch);
                else
                    this.roleOnAction = RoleID.witch;
                break;
            case RoleID.witch:
                this.roleOnAction = RoleID.wolf;
                isDay = true;
                break;
            default:
                break;
        }
    }
    private bool IsEndGame()
    {
        return false;
    }
    #endregion

    #region Event Handle Methods
    private object[] DeliverRole()
    {
        Action<byte, Dictionary<int, byte>,byte> generateRole = (byte length, Dictionary<int, byte> playerRole,byte roleID) =>
          {
              System.Random rnd = new System.Random();
              for (byte j = 0; j < length; j++)
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
        generateRole.Invoke(roleDelivery.villager, playerRolePair,(byte) RoleID.villager);
        generateRole.Invoke(roleDelivery.hunter, playerRolePair, (byte) RoleID.hunter);
        generateRole.Invoke(roleDelivery.witch, playerRolePair, (byte) RoleID.witch);
        generateRole.Invoke(roleDelivery.wolf, playerRolePair, (byte) RoleID.wolf);
        generateRole.Invoke(roleDelivery.seer, playerRolePair, (byte) RoleID.seer);
        generateRole.Invoke(roleDelivery.cupid, playerRolePair, (byte) RoleID.cupid);
        generateRole.Invoke(roleDelivery.guardian, playerRolePair, (byte) RoleID.guardian);
        int i = 0;
        object[] content = new object[PhotonNetwork.PlayerList.Length*2];
        foreach (var item in playerRolePair)
        {
            content[i] = item.Key;
            content[i+1] = item.Value;
            i += 2;
        }      
        return content;
    }
    private void ReceiveRole(EventData photonEvent)
    {
        object[] data = (object[]) photonEvent.CustomData;
        int[] playerID = new int[PhotonNetwork.PlayerList.Length];
        byte[] actorID = new byte[PhotonNetwork.PlayerList.Length];
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerID[i] = (int) data[i*2];
            actorID[i] = (byte) data[i*2 + 1];
        }
        listPlayerController.InitAllPlayerRole(playerID, actorID, () => { 
            isSetUpComplete = true;
            Debug.Log("SetUpComplete");
        });
    }

    private void ReceiveAwakenRole(EventData eventData)
    {
        object[] data = (object[]) eventData.CustomData;
        foreach (var item in data)
        {
            listPlayerController.ReceiveRoleWakeUp((int) item,() => {
                Debug.Log("In My Turn");
                ActionEventHandler.Invoke(ActionEventID.InMyTurn);
            });
        }
    }
    // Test function callback done role action
    private void ReceiveCompleteActionRole(EventData eventData)
    {
        object[] data = (object[])eventData.CustomData;
        Debug.Log("PlayerID: " + (int)data[0] + " call done action");
        listPlayerController.ReceiveCompleteActionRole((int)data[0], () => {
            // Call this action when all player complete their role action
            ChooseNextActiveRole(this.roleOnAction);
            isPlayerOnAction = false;
            Debug.Log("Done in my turn");
        });
    }
    #endregion
}
