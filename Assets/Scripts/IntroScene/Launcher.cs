using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class Launcher : MonoBehaviourPunCallbacks
{
    #region Private Fields SerializeFields

    #endregion

    #region Private Fields
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
            return;

        if (!PhotonNetwork.IsConnectedAndReady)
            return;
        roomID = Random.Range(1000000000, int.MaxValue).ToString();
        RoomOptions roomOption = new RoomOptions
        {
            MaxPlayers = maxPlayerOnRoom,
            IsOpen = true,
            IsVisible = true,
            PublishUserId = true
        };
        PhotonNetwork.NickName = UIcontroller.GetNamePlayer();
        PhotonNetwork.CreateRoom(roomID, roomOption);
    }
    public void OnClick_JoinRoom()
    {
        if (UIcontroller.GetNamePlayer() == "")
            return;
        if (!PhotonNetwork.IsConnectedAndReady)
            return;
        PhotonNetwork.NickName = UIcontroller.GetNamePlayer();
        PhotonNetwork.JoinRoom(UIcontroller.GetRoomID());
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
        PhotonNetwork.CurrentRoom.IsOpen = false;
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("GameplayScene");
    }

    public void OnClick_Exit()
    {
        Application.Quit();
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
        
        for (int i = 0; i < maxPlayerOnRoom; i++)
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
        for (int i = 0; i < maxPlayerOnRoom; i++)
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
        Debug.Log("Failed to join room: " + message);
    }
    public override void OnConnected()
    {
        Debug.Log("Connected to Server");
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
