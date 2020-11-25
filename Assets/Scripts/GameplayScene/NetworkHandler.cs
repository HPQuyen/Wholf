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
        ActionEventHandler.RemoveAllAction();
        PunEventHandler.RemoveAllEvent();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("IntroScene");
    }
    #region Monobehaviour Pun Callbacks
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        ActionEventHandler.Invoke(ActionEventID.MasterClientDisconnect);
    }
    #endregion
}
