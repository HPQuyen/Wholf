using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class EviromentController : MonoBehaviour
{
    #region Private SerializeField
    [SerializeField]
    private Animator fireAnim = null;
    [SerializeField]
    private Animator globalLight = null;
    [SerializeField]
    private GameObject moon = null;
    //[SerializeField]
    //private Animator moon = null;
    #endregion
    #region Private Fields
    private bool isDay;
    #endregion

    #region Monobehaviour Methods
    private void Start()
    {
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.EndGame, EndGame);
    }
    private void Update()
    {
        
    }
    #endregion
    
    private void DaytimeTransition()
    {
        isDay = true;
        fireAnim.SetBool("IsDay", isDay);
        globalLight.SetBool("IsDay", isDay);
    }
    private void NighttimeTransition()
    {
        isDay = false;
        fireAnim.SetBool("IsDay", isDay);
        globalLight.SetBool("IsDay", isDay);
    }
    private void EndGame()
    {
        moon.SetActive(false);
        if (isDay)
            globalLight.Play("EndgameDay");
        else
            globalLight.Play("EndgameNight");
        fireAnim.SetBool("IsDay", true);
    }
}
