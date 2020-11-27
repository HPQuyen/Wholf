using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;

public class VoiceRecorderHandler : MonoBehaviourPunCallbacks
{
    #region Private Fields
    private bool isDie = false;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Private Methods
    private void SetActiveRecorder(bool state)
    {
        if(!isDie)
            PhotonVoiceNetwork.Instance.PrimaryRecorder.IsRecording = state;
    }
    private void InitEvent()
    {
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, EnableVoice);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, DisableVoice);
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, DisableVoice);
        ActionEventHandler.AddNewActionEvent(ActionEventID.AfterMyDeath, AfterMyDeath);
    }
    #endregion

    #region Local Action Event Handler
    
    private void EnableVoice()
    {
        Debug.Log("Open voice");
        SetActiveRecorder(true);
    }
    private void DisableVoice()
    {
        SetActiveRecorder(false);
    }
    private void AfterMyDeath()
    {
        Debug.Log("After my death");
        SetActiveRecorder(false);
        isDie = true;
    }
    #endregion

    #region Pun callbacks
    public override void OnJoinedRoom()
    {
        isDie = false;
        SetActiveRecorder(true);
        InitEvent();
    }
    #endregion

}
