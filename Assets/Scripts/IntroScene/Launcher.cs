using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Fields SerializeFields

    #endregion

    #region Private Fields
    private const byte MAX_PLAYER = 8; 
    private UIController UIcontroller = null;
    private string roomID = string.Empty;
    private byte maxPlayerOnRoom = 8;
    #endregion

    #region Public Fields

    #endregion

    #region Monobehavior Methods

    private void Start()
    {
        PhotonNetwork.GameVersion = "v1.0";
        PhotonNetwork.KeepAliveInBackground = 300000;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        UIcontroller = UIController.GetInstance();
    }
    #endregion

    #region Private Methods

    #endregion

    #region Public Methods
    public void OnClick_HostRoom()
    {
        if (UIcontroller.GetNamePlayer() == "")
        {
            UIcontroller.DisplayError("What is your name?");
            return;
        }
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            UIcontroller.DisplayError("Connecting to server failed");
            return;
        }
        UIcontroller.OnClick_CreateRoom();
    }
    public void OnClick_JoinRoom()
    {
        if (UIcontroller.GetNamePlayer() == "")
        {
            UIcontroller.DisplayError("What is your name?");
            return;
        }
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            UIcontroller.DisplayError("Connecting to server failed");
            return;
        }
        if (UIcontroller.GetRoomID().Equals(""))
        {
            UIcontroller.DisplayError("RoomID is empty!");
            return;
        }
        PhotonNetwork.NickName = UIcontroller.GetNamePlayer();
        PhotonNetwork.JoinRoom(UIcontroller.GetRoomID());
    }

    public void OnClick_Apply()
    {
        UIcontroller.OnApplyCreateRoom((maxPlayer) => {
            roomID = Random.Range(1000000000, int.MaxValue).ToString();
            maxPlayerOnRoom = (byte)maxPlayer;
            RoomOptions roomOption = new RoomOptions
            {
                MaxPlayers = maxPlayerOnRoom,
                IsOpen = true,
                IsVisible = true,
                PublishUserId = true
            };
            PhotonNetwork.NickName = UIcontroller.GetNamePlayer();
            PhotonNetwork.CreateRoom(roomID, roomOption);
        });
    }

    public void OnClick_Settings()
    {
        UIController.GetInstance().OnClick_Setting();
    }
    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnClick_StartGame()
    {
        if(PhotonNetwork.PlayerList.Length < maxPlayerOnRoom)
        {
            UIcontroller.DisplayError("Can't start a match, not enough players");
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("GameplayScene");
    }
    public void OnClick_LogOut()
    {
        PhotonNetwork.Disconnect();
        PlayerProfile.SignOut();
        SceneManager.LoadScene("LoginScene");
    }
    #endregion

    #region Public Photon Callback
    public override void OnCreatedRoom()
    {
        Debug.Log("Create room");
        UIcontroller.OnCreateRoom(roomID);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Join Room");
        if(!PhotonNetwork.IsMasterClient)
            UIcontroller.OnJoinRoom();
        
        for (int i = 0; i < MAX_PLAYER; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                UIcontroller.DisplayNamePlayer(i, PhotonNetwork.PlayerList[i].NickName, PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer ? Color.red : Color.white);
            }
            else
            {
                UIcontroller.DisplayNamePlayer(i, "", Color.white);
            }
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        UIcontroller.OnLeaveRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player join room: " + newPlayer.NickName);
        UIcontroller.DisplayNamePlayer(PhotonNetwork.PlayerList.Length-1, newPlayer.NickName, Color.white);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
        Debug.Log("Player Left Room: " + otherPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            UIcontroller.SetActiveStartButton(true);
        }
        for (int i = 0; i < MAX_PLAYER; i++)
        {
            if(i < PhotonNetwork.PlayerList.Length)
            {
                UIcontroller.DisplayNamePlayer(i, PhotonNetwork.PlayerList[i].NickName, PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer ? Color.red : Color.white);
            }
            else
            {
                UIcontroller.DisplayNamePlayer(i, "", Color.white);
            }
        }

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room: " + message);
        OnClick_HostRoom();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Return Code: " + returnCode);
        Debug.Log("Failed to join room: " + message);
        if(returnCode == 32758)
        {
            UIcontroller.DisplayError("The room does not exist anymore");
        }
        else
        {
            UIcontroller.DisplayError("The room is full");
        }
    }
    public override void OnConnected()
    {
        UIcontroller.DisplayError("Connect to server");
        Debug.Log(PhotonNetwork.GetPing());
        Debug.Log(PhotonNetwork.Server);
        Debug.Log(PhotonNetwork.ServerAddress);
        Debug.Log(PhotonNetwork.CloudRegion);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected: " + cause);
        PhotonNetwork.GameVersion = "v1.0";
        PhotonNetwork.KeepAliveInBackground = 300000;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        UIcontroller = UIController.GetInstance();
    }

    #endregion

}
