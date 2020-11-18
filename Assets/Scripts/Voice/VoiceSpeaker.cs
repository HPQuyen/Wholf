using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceSpeaker : MonoBehaviour
{
    //#region Private Methods
    //private int actorID;
    //#endregion
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
