using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PlayerUIController : MonoBehaviour
{

    #region Public Fields
    public GameObject[] playerUIPrefab = null;
    #endregion
    #region Private SerializeField
    [SerializeField]
    private GameObject[] roleDescriptionPrefab = null;
    [SerializeField]
    private TextMeshProUGUI timer = null;
    [SerializeField]
    private Nameboard[] nameBoard = null;
    [SerializeField]
    private GameObject cancelPanel = null;
    [SerializeField]
    private Sprite[] iconEffect = null;
    [SerializeField]
    private Effect[] panelEffect = null;
    [SerializeField]
    private TextMeshProUGUI notificationTurnState_Text = null;
    [SerializeField]
    private TextMeshProUGUI notificationTransition_Text = null;
    [SerializeField]
    private SpotlightEffect[] spotlightEffects = null;
    #endregion
    #region Private Fields
    private static PlayerUIController instance = null;
    private ListPlayerController listPlayerController = null;
    private GameObject playerUI = null;
    private GameObject roleDescription = null;
    private IRole playerRole = null;
    private bool isMyAbilityTurn = false;
    private bool isVoteTurn = false;
    private float cooldownTime;
    private Dictionary<int, Effect> mapPanelEffect = new Dictionary<int, Effect>();
    private Dictionary<int, int> mapNameBoard = new Dictionary<int, int>();
    #endregion

    #region Monobehavior Methods
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this);
    }
    private void Start()
    {
        listPlayerController = ListPlayerController.GetInstance();
        ActionEventHandler.AddNewActionEvent(ActionEventID.InMyTurn, InMyTurn);
        ActionEventHandler.AddNewActionEvent(ActionEventID.CompleteMyTurn, CompleteMyTurn);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.InVoteTurn, InVoteTurn);
        ActionEventHandler.AddNewActionEvent(ActionEventID.CompleteVoteTurn, CompleteVoteTurn);
    }
    private void Update()
    {
        if(isMyAbilityTurn)
        {
            cooldownTime -= Time.deltaTime;
            if(cooldownTime <= 0)
            {
                ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
            }
            SetCooldownTime(cooldownTime.ToString("0"));
        }
    }
    #endregion

    #region Public Methods
    public static PlayerUIController GetInstance()
    {
        return instance;
    }
    public void LoadPlayerName(int position,string nickName,int playerID)
    {
        try
        {
            mapNameBoard.Add(playerID, position);
            nameBoard[position].gameObject.SetActive(true);
            nameBoard[position].AddName(nickName, PhotonNetwork.LocalPlayer.ActorNumber == playerID);
        }
        catch (Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public void LoadPlayerUI(IRole playerRole,byte roleID)
    {
        this.playerRole = playerRole;
        ActionEventHandler.AddNewActionEvent(ActionEventID.CompleteMyTurn, playerRole.CompleteMyTurn);
        try
        {
            Debug.Log("Load UI");
            Debug.Log("My role: " + (RoleID)roleID);
            playerUI = playerUIPrefab[roleID];
            roleDescription = roleDescriptionPrefab[roleID];
            playerUI.SetActive(true);
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public void NotificationTurnState(string notice)
    {
        notificationTurnState_Text.text = notice;
    }
    public void NotificationTransition(string content, Action onNotificationTransition)
    {
        notificationTransition_Text.gameObject.SetActive(true);
        notificationTransition_Text.text = content;
        StartCoroutine(NotificationTransition(3f,()=> {
            onNotificationTransition();
            notificationTransition_Text.gameObject.SetActive(false);
        }));
    }
    IEnumerator NotificationTransition(float timeRepeat ,Action onNotificationTransition)
    {
        yield return new WaitForSeconds(1f);
        notificationTransition_Text.text = timeRepeat.ToString();
        if (timeRepeat <= 0f)
        {
            onNotificationTransition();
            yield return null;
        }
        else
            StartCoroutine(NotificationTransition(timeRepeat - 1,onNotificationTransition));
    }
    public void SetActiveCooldownTime(bool state)
    {
        timer.gameObject.SetActive(state);
    }
    public void SetCooldownTime(string time)
    {
        timer.text = time;
    }

    public void AddRoleEffect(RoleID roleID, int playerID,PotionType type = PotionType.kill,bool state = true)
    {
        try
        {
            if(roleID == RoleID.seer)
            {
                spotlightEffects[mapNameBoard[playerID]].SeerDetection(state);
                return;
            }
            if (!mapPanelEffect.ContainsKey(playerID))
            {
                foreach (var item in panelEffect)
                {
                    if (!item.gameObject.activeSelf)
                    {
                        item.gameObject.SetActive(true);
                        item.CreateEffect(PhotonNetwork.CurrentRoom.Players[playerID].NickName);
                        mapPanelEffect.Add(playerID, item);
                        break;
                    }
                }
            }
            if (roleID == RoleID.witch)
            {
                mapPanelEffect[playerID].AddEffect(iconEffect[type == PotionType.kill ? (int)RoleID.witch - 1 : (int)RoleID.witch]);
            }
            else
            {
                mapPanelEffect[playerID].AddEffect(iconEffect[(int)roleID]);
            }
        }
        catch (Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public void VoteCountDisplay(int playerID,int amount)
    {
        try
        {
            nameBoard[mapNameBoard[playerID]].Vote(amount);
        }
        catch (Exception) {}
    }
    public void SwitchLight(int playerID,bool state)
    {
        try
        {
            spotlightEffects[mapNameBoard[playerID]].SwitchLight(state);
        }
        catch (Exception) {}
    }
    public void RoleCastAction()
    {
        playerUI.SetActive(false);
        cancelPanel.SetActive(true);
    }
    
    #endregion

    #region Button On_Click Methods
    public void OnClick_RoleDescription()
    {
        roleDescription.SetActive(!roleDescription.activeSelf);
    }
    public void OnClick_Cancel()
    {
        playerUI.SetActive(true);
        cancelPanel.SetActive(false);
        listPlayerController.SetAllSelectable( role => true ,false);
        ActionEventHandler.RemoveAction();
    }
    public void OnClick_Skip()
    {
        if (isMyAbilityTurn)
            ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
        else if (isVoteTurn)
        {
            ActionEventHandler.Invoke(ActionEventID.CompleteVoteTurn);
            PunEventHandler.QuickRaiseEvent(PunEventID.ReceiveVote, new object[] { -1 }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        }
    }
    public void OnClick_Vote()
    {
        if (!isVoteTurn)
            return;
        RoleCastAction();
        listPlayerController.SetAllSelectable(role => true, true);
        ActionEventHandler.AddNewActionEvent((IRole player) => {
            ActionEventHandler.Invoke(ActionEventID.CompleteVoteTurn);
            PunEventHandler.QuickRaiseEvent(PunEventID.ReceiveVote, new object[] { player.GetPlayerID() },new RaiseEventOptions() { Receivers = ReceiverGroup.All},SendOptions.SendReliable);
        });
    }
        #region Seer
    public void OnClick_SeerAbility()
    {
        if (!isMyAbilityTurn)
            return;
        RoleCastAction();
        listPlayerController.SetAllSelectable(role => playerRole.GetPlayerID() != role.GetPlayerID(),true);
        ActionEventHandler.AddNewActionEvent((IRole obj) => {

            playerRole.CastAbility(obj);
        });
    }

        #endregion

        #region Hunter
    public void OnClick_HunterAbility()
    {
        if (!isMyAbilityTurn)
            return;
        RoleCastAction();
        listPlayerController.SetAllSelectable(role => playerRole.GetPlayerID() != role.GetPlayerID(), true);
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj);
        });
    }
        #endregion

        #region Witch
    public void OnClick_WitchAbilityKill()
    {
        if (!isMyAbilityTurn)
            return;
        RoleCastAction();

        listPlayerController.SetAllSelectable(role => playerRole.GetPlayerID() != role.GetPlayerID(), true);
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            Debug.Log(EventSystem.current.currentSelectedGameObject);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
            playerRole.CastAbility(obj, PotionType.kill);
        });
    }
    public void OnClick_WitchAbilityRescue()
    {
        if (!isMyAbilityTurn)
            return;
        RoleCastAction();
        listPlayerController.SetAllSelectable(listPlayerController.GetListPlayerSurvivor(),true);
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            Debug.Log(EventSystem.current.currentSelectedGameObject);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
            playerRole.CastAbility(obj, PotionType.rescue);
        });
    }
        #endregion

        #region Cupid
    public void OnClick_CupidAbility()
    {
        if (!isMyAbilityTurn)
            return;
        RoleCastAction();
        if (playerRole.GetTarget() != null)
            listPlayerController.SetAllSelectable(role => playerRole.GetTarget().GetPlayerID() != role.GetPlayerID(), true);
        else
            listPlayerController.SetAllSelectable(role => true, true);
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj);
        });
    }
        #endregion

        #region Wolf
    public void OnClick_WolfAbility()
    {
        if (!isMyAbilityTurn)
            return;
        RoleCastAction();
        listPlayerController.SetAllSelectable(role => true, true);
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj);
        });
    }
        #endregion

        #region Guardian
    public void OnClick_GuardianAbility()
    {
        if (!isMyAbilityTurn)
            return;
        RoleCastAction();
        if (playerRole.GetTarget() != null)
            listPlayerController.SetAllSelectable(role => playerRole.GetTarget().GetPlayerID() != role.GetPlayerID(), true);
        else
            listPlayerController.SetAllSelectable(role => true, true);
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj);
        });
    }
    #endregion

    #endregion

    #region Local Action Event Methods
    private void InMyTurn()
    {
        Debug.Log("In My Turn Load UI");
        cooldownTime = playerRole.GetTimeRoleAction();
        roleDescription.SetActive(false);
        SetActiveCooldownTime(true);
        isMyAbilityTurn = true;
    }
    private void CompleteMyTurn()
    {
        isMyAbilityTurn = false;
        listPlayerController.SetAllSelectable(role => true,false);
        cancelPanel.SetActive(false);
        playerUI.SetActive(true);
        SetActiveCooldownTime(false);
        roleDescription.SetActive(false);
        ActionEventHandler.RemoveAction();
    }
    private void InVoteTurn()
    {
        if(!ListPlayerController.IsGhostView())
            isVoteTurn = true;
    }
    private void CompleteVoteTurn()
    {
        isVoteTurn = false;
        cancelPanel.SetActive(false);
        playerUI.SetActive(true);
        listPlayerController.SetAllSelectable(role => true, false);
        ActionEventHandler.RemoveAction();
    }
    public void NighttimeTransition()
    {
        foreach (var item in mapPanelEffect)
        {
            item.Value.ResetEffect();
        }
        mapPanelEffect.Clear();
        foreach (var item in mapNameBoard)
        {
            nameBoard[item.Value].ResetVote();
        }
        SetActiveCooldownTime(false);
        ActionEventHandler.Invoke(ActionEventID.CompleteVoteTurn);
        NotificationTurnState("Night");
    }
    public void DaytimeTransition()
    {
        SetActiveCooldownTime(true);
        ActionEventHandler.Invoke(ActionEventID.InVoteTurn);
        NotificationTurnState("Morning"); 
    }
    #endregion
}
