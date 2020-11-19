using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Private Fields SerializeFields
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
    [SerializeField]
    private GameObject panelSettings = null;
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
        SetActivePanelLobby(true);
        SetActiveStartButton(true);
        DisplayRoomID(roomID);
    }
    public void OnJoinRoom()
    {
        SetActivePanelMenu(false);
        SetActivePanelLobby(true);
        DisplayRoomID(GetRoomID());
    }
    public void OnLeaveRoom()
    {
        SetActivePanelLobby(false);
        SetActivePanelMenu(true);
        SetActiveStartButton(false);
    }


    public void OnClick_Setting()
    {
        panelSettings.SetActive(!panelSettings.activeSelf);
    }

    #endregion
}
