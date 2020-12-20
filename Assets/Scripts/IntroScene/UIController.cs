using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Private Fields SerializeFields
    [SerializeField]
    private TextMeshProUGUI errorMessage = null;
    [Space(10)]
    [Header("========== Panel Lobby ==========")]
    /* ##################################################
    *  #                Panel Lobby Fields              #
    *  ################################################## */
    [SerializeField]
    private GameObject panelLobby = null;
    [SerializeField]
    private TextMeshProUGUI roomID_Display = null;
    [SerializeField]
    private TextMeshProUGUI[] namePlayer_Display = null;
    [SerializeField]
    private GameObject startButton = null;
    [SerializeField]
    private RoleDelivery roleDelivery = null;
    [SerializeField]
    private SetRoleHandler setRoleHandler = null;
    /* ##################################################
     * #                Panel Menu Fields               #
     * ################################################## */
    [Space(10)]
    [Header("========== Panel Menu ==========")]
    [SerializeField]
    private GameObject panelMenu = null;
    [SerializeField]
    private TMP_InputField namePlayer_Input = null;
    [SerializeField]
    private TMP_InputField roomID_Input = null;
    /* ##################################################
     * #                Panel Settings Fields               #
     * ################################################## */
    [Space(10)]
    [Header("========== Panel Settings ==========")]
    [SerializeField]
    private GameObject panelSettings = null;
    /* ##################################################
     * #                Panel SetRole Fields               #
     * ################################################## */
    [Space(10)]
    [Header("========== Panel Set Role ==========")]
    [SerializeField]
    private GameObject panelSetRole = null;
    #endregion
    

    #region Private Fields
    private static UIController instance = null;
    #endregion

    #region Public Fields

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
        if(PlayerProfile.GetPlayerName() != null)
        {
            namePlayer_Input.text = PlayerProfile.GetPlayerName();
            namePlayer_Input.interactable = false;
        }
    }
    #endregion

    #region Public Methods
    public static UIController GetInstance()
    {
        return instance;
    }

    public void SetActivePanelMenu(bool state)
    {
        panelMenu.SetActive(state);
    }
    public void SetActivePanelLobby(bool state)
    {
        panelLobby.SetActive(state);
    }
    public void SetActiveStartButton(bool state)
    {
        startButton.SetActive(state);
    }
    public void SetActivePanelSetRole(bool state)
    {
        panelSetRole.SetActive(state);
    }
    public void SetActivePanelSettings(bool state)
    {
        panelSettings.SetActive(state);
    }
    public void DisplayNamePlayer(int index, string namePlayer,Color color)
    {
        try
        {
            namePlayer_Display[index].text = namePlayer;
            namePlayer_Display[index].color = color;
        }
        catch (IndexOutOfRangeException exc)
        {
            Debug.Log("Error: " + exc.Message);
        }
    }
    public void DisplayRoomID(string roomID)
    {
        roomID_Display.text = ("ID: " + roomID);
    }
    public void DisplayError(string errorMessage)
    {
        this.errorMessage.text = errorMessage;
        TimeManipulator.GetInstance().InvokeActionAfterSeconds(3f, () => { this.errorMessage.text = ""; });
    }
    public string GetNamePlayer()
    {
        return namePlayer_Input.text;
    }
    public string GetRoomID()
    {
        return roomID_Input.text;
    }

    public void OnCreateRoom(string roomID)
    {
        SetActivePanelMenu(false);
        SetActivePanelSetRole(false);
        SetActivePanelSettings(false);
        SetActivePanelLobby(true);
        SetActiveStartButton(true);
        DisplayRoomID(roomID);
    }
    public void OnJoinRoom()
    {
        SetActivePanelMenu(false);
        SetActivePanelLobby(true);
        SetActivePanelSettings(false);
        DisplayRoomID(GetRoomID());
    }
    public void OnLeaveRoom()
    {
        SetActivePanelLobby(false);
        SetActivePanelSetRole(false);
        SetActiveStartButton(false);
        SetActivePanelSettings(false);
        SetActivePanelMenu(true);
    }

    public void OnApplyCreateRoom(Action<int> OnCompleteApplyCreateRoom)
    {
        if (!setRoleHandler.CheckNumber())
        {
            DisplayError("Minimum players is 5");
            //return;
        }
        roleDelivery.cupid = (byte)setRoleHandler.GetValue(RoleID.cupid);
        roleDelivery.wolf = (byte)setRoleHandler.GetValue(RoleID.wolf);
        roleDelivery.witch = (byte)setRoleHandler.GetValue(RoleID.witch);
        roleDelivery.hunter = (byte)setRoleHandler.GetValue(RoleID.hunter);
        roleDelivery.villager = (byte)setRoleHandler.GetValue(RoleID.villager);
        roleDelivery.guardian = (byte)setRoleHandler.GetValue(RoleID.guardian);
        roleDelivery.seer = (byte)setRoleHandler.GetValue(RoleID.seer);

        OnCompleteApplyCreateRoom.Invoke(setRoleHandler.CountNumberOfRole());
        
    }

    public void OnClick_Setting()
    {
        SetActivePanelSettings(!panelSettings.activeSelf);
    }
    public void OnClick_CreateRoom()
    {
        SetActivePanelSetRole(true);
        SetActivePanelMenu(false);
    }
    #endregion
}
