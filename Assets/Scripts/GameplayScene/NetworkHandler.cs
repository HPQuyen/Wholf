using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class NetworkHandler : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        ActionEventHandler.AddNewActionEvent(ActionEventID.MasterClientDisconnect,OnMasterClientDisconnect);
    }
    private void OnMasterClientDisconnect()
    {
        ActionEventHandler.RemoveAllAction();
        PunEventHandler.RemoveAllEvent();
        RoleExposition.ClearAll();
        PhotonNetwork.LeaveRoom();
    }
    private IEnumerator LoadIntroScene()
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel("IntroScene");
    }


    #region Monobehaviour Pun Callbacks
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(ActionEventHandler.EventExist(ActionEventID.MasterClientDisconnect))
            ActionEventHandler.Invoke(ActionEventID.MasterClientDisconnect);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        IRole role = ListPlayerController.GetInstance().GetRole(otherPlayer.ActorNumber);
        if (role != null)
        {
            RoleExposition.AddVictim(role);
            ListPlayerController.GetInstance().RemoveDeathPlayer(role.GetPlayerID());
        }
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    if (Moderator.GetInstance().IsEndGame(out Sect sectWin))
        //    {
        //        PunEventHandler.QuickRaiseEvent(PunEventID.ReceiveEndGame, new object[] { sectWin }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
        //    }
        //}

    }
    public override void OnLeftRoom()
    {
        StartCoroutine(LoadIntroScene());
    }

    #endregion
}
