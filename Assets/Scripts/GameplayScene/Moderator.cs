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
    private ListPlayerController listPlayerController = null;
    
    #endregion

    #region Private Fields
    private static Moderator instance = null;
    private bool isDayTime;
    private bool isSetUpComplete;
    private bool isPlayerOnAction;
    private RoleID roleOnAction = RoleID.wolf;
    private bool isSecondPassed = true;
    private float disscustionTime;
    

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
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleAwakeness, ReceiveAwakenRole);
        PunEventHandler.RegisterReceiveEvent(PunEventID.RoleActionComplete, ReceiveCompleteActionRole);
        PunEventHandler.RegisterReceiveEvent(PunEventID.DaytimeTransition, ReceiveDaytimeTransition);
        PunEventHandler.RegisterReceiveEvent(PunEventID.NighttimeTransition, ReceiveNighttimeTransition);
        PunEventHandler.RegisterReceiveEvent(PunEventID.DiscusstionTimeCountdown, ReceiveDiscussionTimeCountdown);

        // Local Register Event
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, StartGame);


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
                PunEventHandler.RaiseEvent(PunEventID.DaytimeTransition, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                break;
            default:
                break;
        }
    }
    private bool IsEndGame()
    {
        if (listPlayerController.CountNumberOfRole(RoleID.villager) <= listPlayerController.CountNumberOfRole(RoleID.wolf))
        {
            return true;
        }
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
        object[] content = new object[] { 0 };
        isSetUpComplete = false;
        return content;
    }
    private object[] NighttimeTransitionPun()
    {
        object[] content = new object[] { 0 };
        isSetUpComplete = false;
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
        });
    }
    private void ReceiveDiscussionTimeCountdown(EventData eventData)
    {
        Debug.Log("Receive Discussion Time");
        object[] data = (object[])eventData.CustomData;
        PlayerUIController.GetInstance().SetCooldownTime(((float) data[0]).ToString("0"));
    }
    private void ReceiveDaytimeTransition(EventData eventData)
    {
        Debug.Log("Receive DaytimeTransition");
        ActionEventHandler.Invoke(ActionEventID.DaytimeTransition);
    }
    public void ReceiveNighttimeTransition(EventData eventData)
    {
        Debug.Log("Receive NighttimeTransition");
        ActionEventHandler.Invoke(ActionEventID.NighttimeTransition);
    }
    #endregion

    #region Local Action Event Methods
    private void DaytimeTransition()
    {
        PlayerUIController.GetInstance().SetActiveCooldownTime(true);
        isDayTime = true;
        isSecondPassed = true;
        isSetUpComplete = true;
    }
    private void NighttimeTransition()
    {
        PlayerUIController.GetInstance().SetActiveCooldownTime(false);
        disscustionTime = discussionTimeDefault;
        isDayTime = false;
        isSetUpComplete = true;
    }

    #endregion
}
