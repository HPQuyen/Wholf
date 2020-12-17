using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

#pragma warning disable 4096
public class Moderator : MonoBehaviour
{
    #region Private Fields SerializeField
    [SerializeField]
    private RoleDelivery roleDelivery = null;
    [SerializeField]
    private float discussionTimeDefault = 120f;
    #endregion

    #region Private Fields
    private static Moderator instance { get; set; } = null;
    private bool isDayTime;
    private bool isSetUpComplete;
    private bool isPlayerOnAction;
    private RoleID roleOnAction;
    private bool isSecondPassed = true;
    private float disscustionTime;
    private ListPlayerController listPlayerController = null;
    private int receiveRoleSuccessCount = 0;

    #endregion

    #region Public Fields

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
    public static Moderator GetInstance()
    {
        return instance;
    }
    // Start is called before the first frame update
    private void Start()
    {
        listPlayerController = ListPlayerController.GetInstance();
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
        ActionEventHandler.AddNewActionEvent(ActionEventID.EndGame,EndGame);


        PlayerUIController.GetInstance().NotificationTransition("Start Game In", () =>
        {
            ActionEventHandler.Invoke(ActionEventID.StartGame);
        });
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
            case RoleID.cupid:
                this.roleOnAction = RoleID.wolf;
                break;
            case RoleID.wolf:
                if (roleDelivery.seer == 0)
                    ChooseNextActiveRole(RoleID.seer);
                else
                    this.roleOnAction = RoleID.seer;
                break;
            case RoleID.seer:
                if (roleDelivery.guardian == 0)
                    ChooseNextActiveRole(RoleID.guardian);
                else
                    this.roleOnAction = RoleID.guardian;
                break;
            case RoleID.guardian:
                if (roleDelivery.witch == 0)
                    ChooseNextActiveRole(RoleID.witch);
                else
                    this.roleOnAction = RoleID.witch;
                break;
            case RoleID.witch:
                if (listPlayerController.CountNumberOfRole((role) => role.GetRoleID() == RoleID.hunter) == 0)
                    ChooseNextActiveRole(RoleID.hunter);
                else
                    this.roleOnAction = RoleID.hunter;
                break;
            case RoleID.hunter:
                this.roleOnAction = RoleID.wolf;
                PunEventHandler.RaiseEvent(PunEventID.DaytimeTransition, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                break;
            default:
                break;
        }
    }
    public bool IsEndGame(out Sect sectWin)
    {
        int villagerSect = listPlayerController.CountNumberOfRole((role) => role.GetSect() == Sect.villagers);
        int cupidSect = listPlayerController.CountNumberOfRole((role) => role.GetSect() == Sect.cupid);
        int wolfSect = listPlayerController.CountNumberOfRole((role) => role.GetSect() == Sect.wolves);
        int survivor = listPlayerController.CountNumberOfRole((role) => true);
        if (cupidSect == 2 && survivor == 2)
        {
            // Cupid win
            sectWin = Sect.cupid;
            return true;
        }
        if (wolfSect == 0 && cupidSect == 0)
        {
            // Villager win
            sectWin = Sect.villagers;
            return true;
        }
        if (wolfSect >= survivor - wolfSect)
        {
            // Wolf win
            sectWin = Sect.wolves;
            return true;
        }
        sectWin = Sect.villagers;
        return false;
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
        Debug.Log("DaytimeTransitionPun");
        return content;
    }
    private object[] NighttimeTransitionPun()
    {
        object[] content = listPlayerController.GetDeathPlayer(true);
        if (content == null)
            content = new object[] { null };
        isSecondPassed = true;
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
            // Invoke Event

        }
    }
    private void ReceiveAwakenRole(EventData eventData)
    {
        object[] data = (object[]) eventData.CustomData;
        RoleID roleID = (RoleID)(data[0]);
        for (int i = 1; i < data.Length; i++)
        {
            listPlayerController.ReceiveRoleWakeUp(roleID,data[i], () => {
                Debug.Log("In My Turn");
                ActionEventHandler.Invoke(ActionEventID.InMyTurn);
            });
        }
    }
    // Function callback when player done action
    private void ReceiveCompleteActionRole(EventData eventData)
    {
        object[] data = (object[])eventData.CustomData;

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
            if(IsEndGame(out Sect sectWin))
            {
                if (PhotonNetwork.IsMasterClient)
                    PunEventHandler.QuickRaiseEvent(PunEventID.ReceiveEndGame, new object[] { sectWin }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
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
        listPlayerController.ReceiveVote(data);
    }
    // Function change day to night
    private void ReceiveNighttimeTransition(EventData eventData)
    {
        Debug.Log("Receive NighttimeTransition");
        object[] data = (object[])eventData.CustomData;
        listPlayerController.RemoveDeathPlayer(data[0] == null ? null : data, () => {
            Sect sectWin;
            if (IsEndGame(out sectWin))
            {
                if (PhotonNetwork.IsMasterClient)
                    PunEventHandler.QuickRaiseEvent(PunEventID.ReceiveEndGame, new object[] { sectWin }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                return;
            }   
            ActionEventHandler.Invoke(ActionEventID.NighttimeTransition);
        });
    }
    // Function receive end game call
    public void ReceiveEndGame(EventData eventData)
    {
        object[] data = (object[])eventData.CustomData;
        RoleExposition.SetSectWin((Sect)data[0]);
        RoleExposition.SetSurvivor(listPlayerController.GetListPlayerRole());
        ActionEventHandler.Invoke(ActionEventID.EndGame);
    }
    #endregion

    #region Local Action Event Methods
    private void StartGame()
    {
        roleOnAction = roleDelivery.cupid == 0 ? RoleID.wolf : RoleID.cupid;
        disscustionTime = discussionTimeDefault;
        isSetUpComplete = true;
    }
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
    private void EndGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(EndGameInSeconds(3));
        }
    }
    IEnumerator EndGameInSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.LoadLevel("EndgameScene");
    }
    #endregion
}
