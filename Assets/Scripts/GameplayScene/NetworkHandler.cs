using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkHandler : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        ActionEventHandler.AddNewActionEvent(ActionEventID.MasterClientDisconnect, MasterClientDisconnect);
    }
    public void MasterClientDisconnect()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("IntroScene");
        ActionEventHandler.RemoveAllAction();
        PunEventHandler.RemoveAllEvent();
    }
    #region Monobehaviour Pun Callbacks
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        ActionEventHandler.Invoke(ActionEventID.MasterClientDisconnect);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        IRole role = ListPlayerController.GetInstance().GetRole(otherPlayer.ActorNumber);
        if (role != null)
            ListPlayerController.GetInstance().RemoveDeathPlayer(role.GetPlayerID());

    }
    #endregion
}
