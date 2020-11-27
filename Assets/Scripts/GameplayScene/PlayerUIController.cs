using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{

    #region Public Fields
    public GameObject[] playerUIPrefab = null;
    #endregion
    #region Private SerializeField
    [SerializeField]
    private GameObject roleDescription = null;
    [SerializeField]
    private TextMeshProUGUI cooldownMyTurn_Text = null;
    [SerializeField]
    private Nameboard[] nameBoard = null;
    [SerializeField]
    private GameObject cancelPanel = null;
    [SerializeField]
    private Sprite[] iconAffection = null;
    [SerializeField]
    private Affection[] panelAffection = null;
    [SerializeField]
    private TextMeshProUGUI notification_Text = null;
    #endregion
    #region Private Fields
    private static PlayerUIController instance = null;
    private ListPlayerController listPlayerController = null;
    private GameObject playerUI = null;
    private IRole playerRole = null;
    private bool isMyAbilityTurn = false;
    private bool isVoteTurn = false;
    private float cooldownTime;
    private Dictionary<int, Affection> mapPanelAffection = new Dictionary<int, Affection>();
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
        ActionEventHandler.AddNewActionEvent(ActionEventID.InMyTurn, playerRole.InMyTurn);
        try
        {
            Debug.Log("Load UI");
            Debug.Log("My role: " + (RoleID)roleID);
            playerUI = playerUIPrefab[roleID];
            playerUI.SetActive(true);
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public void RoleActionNotification(string notice)
    {
        notification_Text.text = notice;
    }
    public void SetActiveCooldownTime(bool state)
    {
        cooldownMyTurn_Text.gameObject.SetActive(state);
    }
    public void SetCooldownTime(String time)
    {
        cooldownMyTurn_Text.text = time;
    }

    public void AddRoleAffection(RoleID roleID, int playerID,PotionType type = PotionType.kill)
    {
        try
        {
            if (!mapPanelAffection.ContainsKey(playerID))
            {
                foreach (var item in panelAffection)
                {
                    if (!item.gameObject.activeSelf)
                    {
                        item.gameObject.SetActive(true);
                        item.CreateAffection(PhotonNetwork.CurrentRoom.Players[playerID].NickName);
                        mapPanelAffection.Add(playerID, item);
                        break;
                    }
                }
            }
            if (roleID == RoleID.witch)
            {
                mapPanelAffection[playerID].AddAffection(iconAffection[type == PotionType.kill ? (int)RoleID.witch - 1 : (int)RoleID.witch]);
            }
            else
            {
                mapPanelAffection[playerID].AddAffection(iconAffection[(int)roleID]);
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
        catch (Exception)
        {

        }
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
        listPlayerController.SetAllSelectable(false);
        ActionEventHandler.RemoveAction();
    }
    public void OnClick_Skip()
    {
        if (isMyAbilityTurn)
            ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
        else if (isVoteTurn)
            ActionEventHandler.Invoke(ActionEventID.CompleteVoteTurn);
    }
    public void OnClick_Vote()
    {
        if (!isVoteTurn)
            return;
        RoleCastAction();
        listPlayerController.SetAllSelectable(true);
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
        listPlayerController.SetAllSelectable(playerRole.GetPlayerID(),true);
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
        listPlayerController.SetAllSelectable(playerRole.GetPlayerID(), true);
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

        listPlayerController.SetAllSelectable(playerRole.GetPlayerID(), true);
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
        listPlayerController.SetAllSelectable(true);
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
        listPlayerController.SetAllSelectable(true);
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
            listPlayerController.SetAllSelectable(playerRole.GetTarget().GetPlayerID(), true);
        else
            listPlayerController.SetAllSelectable(true);
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
        listPlayerController.SetAllSelectable(false);
        cancelPanel.SetActive(false);
        playerUI.SetActive(true);
        SetActiveCooldownTime(false);
        roleDescription.SetActive(false);
        ActionEventHandler.RemoveAction();
    }
    private void InVoteTurn()
    {
        if(!playerRole.GetIsKill())
            isVoteTurn = true;
    }
    private void CompleteVoteTurn()
    {
        isVoteTurn = false;
        cancelPanel.SetActive(false);
        playerUI.SetActive(true);
        listPlayerController.SetAllSelectable(false);
        ActionEventHandler.RemoveAction();
    }
    public void NighttimeTransition()
    {
        foreach (var item in mapPanelAffection)
        {
            item.Value.ResetAffection();
        }
        mapPanelAffection.Clear();
        foreach (var item in mapNameBoard)
        {
            nameBoard[item.Value].ResetVote();
        }
        SetActiveCooldownTime(false);
        ActionEventHandler.Invoke(ActionEventID.CompleteVoteTurn);
        RoleActionNotification("Night Time");
    }
    public void DaytimeTransition()
    {
        SetActiveCooldownTime(true);
        ActionEventHandler.Invoke(ActionEventID.InVoteTurn);
        RoleActionNotification("Morning Time"); 
    }
    #endregion
}
