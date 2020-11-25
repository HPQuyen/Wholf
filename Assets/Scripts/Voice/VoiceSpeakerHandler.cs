
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSpeakerHandler : MonoBehaviourPunCallbacks
{
    #region Private Fields
    private static VoiceSpeakerHandler instance = null;
    private Dictionary<int, GameObject> speakerPlayer = new Dictionary<int, GameObject>();
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
        speakerPlayer.Add(PhotonNetwork.LocalPlayer.ActorNumber, 
            PhotonNetwork.Instantiate("VoiceSpeaker", Vector3.zero, new Quaternion(), data: new object[] { PhotonNetwork.LocalPlayer.ActorNumber }));
    }

    #region Monobehavir Pun Callback
    public override void OnJoinedRoom()
    {
        InitVoiceSpeaker();
    }
    

    #endregion
}
