using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkHandler : MonoBehaviourPunCallbacks
{


    #region Monobehaviour Pun Callbacks
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("IntroScene");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
    }
    #endregion
}
