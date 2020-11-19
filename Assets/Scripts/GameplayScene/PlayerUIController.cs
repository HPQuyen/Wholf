using System;
using TMPro;
using UnityEngine;

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
    #endregion
    #region Private Fields
    private static PlayerUIController instance = null;
    private IRole playerRole = null;
    private GameObject playerUI = null;
    private bool isMyTurn = false;
    private float cooldownTime;
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
        ActionEventHandler.AddNewActionEvent(ActionEventID.InMyTurn, InMyTurn);
        ActionEventHandler.AddNewActionEvent(ActionEventID.CompleteMyTurn, CompleteMyTurn);
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
    public void LoadPlayerName(int position,String nickName)
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
    public void SetActiveCooldownTime(bool state)
    {
        cooldownMyTurn.gameObject.SetActive(state);
    }
    public void SetCooldownTime(String time)
    {
        cooldownMyTurn.text = time;
    }
    #endregion

    #region Button On_Click Methods
    public void OnClick_RoleDescription()
    {
        roleDescription.SetActive(!roleDescription.activeSelf);
    }
    public void OnClick_Skip()
    {
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
        #region Seer
    public void OnClick_SeerAbility()
    {
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj, 0);
        });
    }

        #endregion

        #region Hunter
    public void OnClick_HunterAbility()
    {
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj, 0);
        });
    }
        #endregion

        #region Witch
    public void OnClick_WitchAbilityKill()
    {
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj, 0);
        });
    }
    public void OnClick_WitchAbilityRescue()
    {
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj, 1);
        });
    }
        #endregion

        #region Cupid
    public void OnClick_CupidAbility()
    {
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj, 0);
        });
    }
        #endregion

        #region Wolf
    public void OnClick_WolfAbility()
    {
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj, 0);
        });
    }
        #endregion

        #region Guardian
    public void OnClick_GuardianAbility()
    {
        ActionEventHandler.AddNewActionEvent((IRole obj) => {
            playerRole.CastAbility(obj, 0);
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
        playerUI.SetActive(false);
        roleDescription.SetActive(false);
        SetActiveCooldownTime(false);
    }
    #endregion
}
