using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSpeaker : MonoBehaviour, IPunInstantiateMagicCallback
{
    //#region Private Methods
    //private int actorID;
    //#endregion
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate: " + info.photonView.Controller.ActorNumber);
    }
}
