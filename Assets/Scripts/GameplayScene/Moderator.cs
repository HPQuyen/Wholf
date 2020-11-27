using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.IO;

#pragma warning disable 4096
public class Moderator : MonoBehaviour
{
    #region Private Fields SerializeField
    [SerializeField]
    private RoleDelivery roleDelivery = null;
    
    #endregion

    #region Private Fields
    private static Moderator instance = null;
    private bool isDayTime;
    private bool isSetUpComplete;
    private bool isPlayerOnAction;
    private RoleID roleOnAction = RoleID.wolf;
    private bool isSecondPassed = true;
    private float disscustionTime;
    private ListPlayerController listPlayerController = null;
    private int receiveRoleSuccessCount = 0;

    #endregion

    #region Public Fields
    public float discussionTimeDefault = 20f;
    #endregion

    #region MonoFunction
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }
    private void StartGame()
    {
        listPlayerController = ListPlayerController.GetInstance();
        disscustionTime = discussionTimeDefault;
    }
    // Start is called before the first frame update
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PunEventHandler.QuickRaiseEvent(PunEventID.RoleDelivery, DeliverRole() , new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            PunEventHandler.RegisterEvent(PunEventID.DaytimeTransition, DaytimeTransitionPun);
            PunEventHandler.RegisterEvent(PunEventID.NighttimeTransition, NighttimeTransitionPun);
            
        }
        // Pun Register Event
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleDelivery, ReceiveRole);
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleReceiveSuccess, ReceiveRoleSuccess);
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleAwakeness, ReceiveAwakenRole);
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleActionComplete, ReceiveCompleteActionRole);
        PunEventHandler.RegisterReceiveEvent(PunEventID.AfterWolfHunt, ReceiveAfterWolfHunt);
        PunEventHandler.RegisterReceiveEvent(PunEventID.DaytimeTransition, ReceiveDaytimeTransition);
        PunEventHandler.RegisterReceiveEvent(PunEventID.ReceiveVote,ReceiveVote);
        PunEventHandler.RegisterReceiveEvent(PunEventID.NighttimeTransition, ReceiveNighttimeTransition);
        PunEventHandler.RegisterReceiveEvent(PunEventID.DiscusstionTimeCountdown, ReceiveDiscussionTimeCountdown);
        PunEventHandler.RegisterReceiveEvent(PunEventID.ReceiveEndGame, ReceiveEndGame);

        // Local Register Event
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, StartGame);
        ActionEventHandler.AddNewActionEvent(ActionEventID.EndGame,()=> { StartCoroutine(EndGame()); } );

        // Invoke Event
        ActionEventHandler.Invoke(ActionEventID.StartGame);
    }


    // Update is called once per frame
    private void Update()
    {
        if (isSetUpComplete && PhotonNetwork.IsMasterClient)
        {
            if (isDayTime)
                Morning();
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
    
    private void Morning()
    {
        if(isSecondPassed)
        {
            isSecondPassed = false;
            PunEventHandler.QuickRaiseEvent(PunEventID.DiscusstionTimeCountdown, new object[] { disscustionTime }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            StartCoroutine(DiscussionTimeCountdown(() =>
            {
                Debug.Log("onOneSecondPassed");
                disscustionTime -= 1f;
                if (disscustionTime <= -1f)
                {
                    PunEventHandler.RaiseEvent(PunEventID.NighttimeTransition, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
                isSecondPassed = true;
            }));
        }
    }
    IEnumerator DiscussionTimeCountdown(Action onOneSecondPassed)
    {
        yield return new WaitForSeconds(1f);
        onOneSecondPassed();
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
                if (listPlayerController.CountNumberOfRole(RoleID.witch) == 0)
                    ChooseNextActiveRole(RoleID.witch);
                else
                    this.roleOnAction = RoleID.witch;
                break;
            case RoleID.witch:
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
                if (listPlayerController.CountNumberOfRole(RoleID.guardian) == 0)
                    ChooseNextActiveRole(RoleID.guardian);
                else
                    this.roleOnAction = RoleID.guardian;
                break;
            case RoleID.guardian:
                this.roleOnAction = RoleID.wolf;
                PunEventHandler.RaiseEvent(PunEventID.DaytimeTransition, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                break;
            default:
                break;
        }
    }
    private bool IsEndGame()
    {
        int villagerSect = listPlayerController.CountNumberOfRole(Sect.villagers);
        int cupidSect = listPlayerController.CountNumberOfRole(Sect.cupid);
        int wolfSect = listPlayerController.CountNumberOfRole(Sect.wolves);
        if (cupidSect == 2 && villagerSect + wolfSect == 0)
        {
            // Cupid win
            return true;
        }
        if (wolfSect == 0)
        {
            // Villager win
            return true;
        }
        if (wolfSect >= villagerSect + cupidSect)
        {
            // Wolf win
            return true;
        }
        return false;
    }
    public IEnumerator EndGame()
    {
        PhotonNetwork.LeaveRoom();
        ActionEventHandler.RemoveAllAction();
        PunEventHandler.RemoveAllEvent();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("IntroScene");
    }
    #endregion

    #region Pun Event Handle Methods
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
        generateRole.Invoke(roleDelivery.villager, playerRolePair,(byte) RoleID.villager);
        generateRole.Invoke(roleDelivery.hunter, playerRolePair, (byte) RoleID.hunter);
        generateRole.Invoke(roleDelivery.witch, playerRolePair, (byte) RoleID.witch);
        generateRole.Invoke(roleDelivery.wolf, playerRolePair, (byte) RoleID.wolf);
        generateRole.Invoke(roleDelivery.seer, playerRolePair, (byte) RoleID.seer);
        generateRole.Invoke(roleDelivery.cupid, playerRolePair, (byte) RoleID.cupid);
        generateRole.Invoke(roleDelivery.guardian, playerRolePair, (byte) RoleID.guardian);
        int i = 0;
        foreach (var item in playerRolePair)
        {
            Debug.Log("Player ID: " + item.Key);
            Debug.Log("Role ID: " + item.Value);
        }
        object[] content = new object[PhotonNetwork.PlayerList.Length*2];
        foreach (var item in playerRolePair)
        {
            content[i] = item.Key;
            content[i+1] = item.Value;
            i += 2;
        }      
        return content;
    }
    private object[] DaytimeTransitionPun()
    {
        object[] content = listPlayerController.GetDeathPlayer();
        if (content == null)
            content = new object[] { null };
        isSetUpComplete = false;
        return content;
    }
    private object[] NighttimeTransitionPun()
    {
        object[] content = listPlayerController.GetDeathPlayer(true);
        if (content == null)
            content = new object[] { null };
        isSetUpComplete = false;
        return content;
    }
    private void ReceiveRole(EventData eventData)
    {
        object[] data = (object[]) eventData.CustomData;
        int[] playerID = new int[PhotonNetwork.PlayerList.Length];
        byte[] actorID = new byte[PhotonNetwork.PlayerList.Length];
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerID[i] = (int) data[i*2];
            actorID[i] = (byte) data[i*2 + 1];
        }
        listPlayerController.InitAllPlayerRole(playerID, actorID, () => {
            PunEventHandler.QuickRaiseEvent(PunEventID.RoleReceiveSuccess, new object[] { true }, new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendReliable);
        });
    }
    private void ReceiveRoleSuccess(EventData eventData)
    {
        receiveRoleSuccessCount++;
        if(receiveRoleSuccessCount == PhotonNetwork.PlayerList.Length)
        {
            isSetUpComplete = true;
        }
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
    // Function callback when player done action
    private void ReceiveCompleteActionRole(EventData eventData)
    {
        object[] data = (object[])eventData.CustomData;
        Debug.Log("PlayerID: " + (int)data[1] + " call done action");
        listPlayerController.ReceiveCompleteActionRole(data, () =>
        {
            // Call this action when the player complete their role action(only for master client)
            ChooseNextActiveRole(this.roleOnAction);
            isPlayerOnAction = false;
        });

    }
    // Special function callback when player done action (only call by master client)
    public void ReceiveAfterWolfHunt(EventData eventData)
    {
        object[] data = (object[])eventData.CustomData;
        listPlayerController.ReceiveAfterWolfHunt(data, () => {
            // Call this action when the player complete their role action(only call by master client)
            ChooseNextActiveRole(this.roleOnAction);
            isPlayerOnAction = false;
        });
    }
    // Function cooldown for discussion time  
    private void ReceiveDiscussionTimeCountdown(EventData eventData)
    {
        Debug.Log("Receive Discussion Time");
        object[] data = (object[])eventData.CustomData;
        PlayerUIController.GetInstance().SetCooldownTime(((float) data[0]).ToString("0"));
    }
    // Function change night to day
    private void ReceiveDaytimeTransition(EventData eventData)
    {
        Debug.Log("Receive DaytimeTransition");
        object[] data = (object[])eventData.CustomData;
        listPlayerController.RemoveDeathPlayer(data[0] == null ? null : data, () => {
            if(IsEndGame())
            {
                if (PhotonNetwork.IsMasterClient)
                    PunEventHandler.QuickRaiseEvent(PunEventID.ReceiveEndGame, new object[] { null }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                return;
            }

            ActionEventHandler.Invoke(ActionEventID.DaytimeTransition);
        });
    }
    // Function take the vote ballot from player
    private void ReceiveVote(EventData eventData)
    {
        // Call update UI vote ballot
        object[] data = (object[])eventData.CustomData;
        if (data[0] == null)
            return;
        listPlayerController.ReceiveVote(data);
    }
    // Function change day to night
    private void ReceiveNighttimeTransition(EventData eventData)
    {
        Debug.Log("Receive NighttimeTransition");
        object[] data = (object[])eventData.CustomData;
        listPlayerController.RemoveDeathPlayer(data[0] == null ? null : data, () => {
            if (IsEndGame())
            {
                if (PhotonNetwork.IsMasterClient)
                    PunEventHandler.QuickRaiseEvent(PunEventID.ReceiveEndGame, new object[] { null }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                return;
            }
            ActionEventHandler.Invoke(ActionEventID.NighttimeTransition);
        });
    }
    // Function receive end game call
    public void ReceiveEndGame(EventData eventData)
    {
        ActionEventHandler.Invoke(ActionEventID.EndGame);
    }
    #endregion

    #region Local Action Event Methods
    private void DaytimeTransition()
    {
        isDayTime = true;
        isSecondPassed = true;
        isSetUpComplete = true;
    }
    private void NighttimeTransition()
    {
        disscustionTime = discussionTimeDefault;
        isDayTime = false;
        isSetUpComplete = true;
    }

    #endregion
}
