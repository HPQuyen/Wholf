using UnityEngine;

public class EviromentController : MonoBehaviour
{
    #region Private SerializeField
    [SerializeField]
    private Animator fireAnim = null;
    [SerializeField]
    private Animator globalLight = null;
    [SerializeField]
    private Animator background_D = null;
    #endregion
    #region Private Fields
    private bool isDay = true;
    #endregion

    #region Monobehaviour Methods
    private void Start()
    {
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, StartGame);
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.EndGame, EndGame);
    }

    #endregion
    private void StartGame()
    {
        isDay = false;
        globalLight.SetTrigger("Start");
        fireAnim.SetTrigger("Start");
        fireAnim.SetBool("IsDay", isDay);
        globalLight.SetBool("IsDay", isDay);
        background_D.Play("NighttimeTransition");
    }
    private void DaytimeTransition()
    {
        isDay = true;
        fireAnim.SetBool("IsDay", isDay);
        globalLight.SetBool("IsDay", isDay);
        background_D.Play("DaytimeTransition");
    }
    private void NighttimeTransition()
    {
        isDay = false;
        fireAnim.SetBool("IsDay", isDay);
        globalLight.SetBool("IsDay", isDay);
        background_D.Play("NighttimeTransition");
    }
    private void EndGame()
    {
        if (isDay)
            globalLight.Play("EndgameDay");
        else
            globalLight.Play("EndgameNight");
        fireAnim.SetBool("IsDay", true);
    }
}
