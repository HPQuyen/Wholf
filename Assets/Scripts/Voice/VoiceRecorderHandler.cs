using Photon.Voice.PUN;
using UnityEngine;

public class VoiceRecorderHandler : MonoBehaviour
{
    #region Private Fields

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

    #endregion

    #region Local Action Event Handler
    private void EnableVoice()
    {
        SetActiveRecorder(true);
    }
    private void DisableVoice()
    {
        SetActiveRecorder(false);
    }
    #endregion


}
