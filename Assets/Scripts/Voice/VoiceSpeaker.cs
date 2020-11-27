using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSpeaker : MonoBehaviour, IPunInstantiateMagicCallback
{
    #region Private Methods
    private AudioSource audioSource = null;
    #endregion
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //VoiceSpeakerHandler.GetInstance().AddVoiceSpeaker(info.photonView.Controller.ActorNumber, this);
    }
}
