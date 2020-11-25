using Photon.Pun;
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
    private TextMeshProUGUI cooldownMyTurn = null;
    [SerializeField]
    private TextMeshProUGUI[] displayPlayerName = null;
    [SerializeField]
    private GameObject cancelPanel = null;
    [SerializeField]
    private Sprite[] iconAffection = null;
    [SerializeField]
    private Affection[] panelAffection = null;
    [SerializeField]
    private TextMeshProUGUI notification = null;
    #endregion
    #region Private Fields
    private static PlayerUIController instance = null;
    private ListPlayerController listPlayerController = null;
    private GameObject playerUI = null;
    private IRole playerRole = null;
    private bool isMyTurn = false;
    private float cooldownTime;
    private Dictionary<int, Affection> mapPanelAffection = new Dictionary<int, Affection>();
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
    }
    private void Update()
    {
        if(isMyTurn)
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
    public void LoadPlayerName(int position,String nickName,int playerID)
    {
        try
        {
            displayPlayerName[position].text = nickName;
            displayPlayerName[position].gameObject.SetActive(true);
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
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public void RoleActionNotification(string notice)
    {
        notification.text = notice;
    }
    public void SetActiveCooldownTime(bool state)
    {
        cooldownMyTurn.gameObject.SetActive(state);
    }
    public void SetCooldownTime(String time)
    {
        cooldownMyTurn.text = time;
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
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
        #region Seer
    public void OnClick_SeerAbility()
    {
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
        isMyTurn = true;
        cooldownTime = playerRole.GetTimeRoleAction();
        playerUI.SetActive(true);
        roleDescription.SetActive(false);
        SetActiveCooldownTime(true);
    }
    private void CompleteMyTurn()
    {
        isMyTurn = false;
        listPlayerController.SetAllSelectable(false);
        cancelPanel.SetActive(false);
        playerUI.SetActive(false);
        SetActiveCooldownTime(false);
        roleDescription.SetActive(false);
        ActionEventHandler.RemoveAction();
    }
    public void NighttimeTransition()
    {
        foreach (var item in mapPanelAffection)
        {
            item.Value.ResetAffection();
        }
        mapPanelAffection.Clear();
        SetActiveCooldownTime(false);
        RoleActionNotification("Night Time");
    }
    public void DaytimeTransition()
    {
        SetActiveCooldownTime(true);
        RoleActionNotification("Morning Time");
    }
    #endregion
}
