
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class VoiceSpeakerHandler : MonoBehaviourPunCallbacks
{
    #region Private Fields
    private static VoiceSpeakerHandler instance = null;
    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }
    #endregion
    public static VoiceSpeakerHandler GetInstance()
    {
        return instance;
    }
    private void InitVoiceSpeaker()
    {
        PhotonNetwork.Instantiate("VoiceSpeaker", Vector3.zero, new Quaternion());
    }

    #region Monobehavir Pun Callback
    public override void OnJoinedRoom()
    {
        InitVoiceSpeaker();
    }
    

    #endregion
}
