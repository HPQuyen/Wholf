using Photon.Voice.PUN;
using UnityEngine;

public class VoiceRecorderHandler : MonoBehaviour
{
    #region Private Fields
    private bool isDaytime = true;
    #endregion

    #region Monobehaviour Methods
    private void Start()
    {
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, EnableVoice);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, DisableVoice);
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, DisableVoice);
    }
    #endregion

    #region Private Methods
    private void SetActiveRecorder(bool state)
    {
        PhotonVoiceNetwork.Instance.PrimaryRecorder.IsRecording = state;
    }
    private void SetActiveDayTime(bool state)
    {
        isDaytime = state;
    }
    #endregion

    #region Local Action Event Handler
    private void EnableVoice()
    {
        SetActiveDayTime(true);
        SetActiveRecorder(true);
    }
    private void DisableVoice()
    {
        SetActiveDayTime(false);
        SetActiveRecorder(false);
    }
    #endregion



    #region UI Methods
    public void OnClick_Mute()
    {
        if(isDaytime)
            SetActiveRecorder(!PhotonVoiceNetwork.Instance.PrimaryRecorder.IsRecording);
    }
    #endregion

}
