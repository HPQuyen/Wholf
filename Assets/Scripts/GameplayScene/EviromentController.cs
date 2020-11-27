using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EviromentController : MonoBehaviour
{
    [SerializeField]
    private Animator fireAnim = null;

    #region Monobehaviour Methods
    void Start()
    {
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
    }

    #endregion

    private void DaytimeTransition()
    {
        fireAnim.SetBool("IsDay", true);
    }
    private void NighttimeTransition()
    {
        fireAnim.SetBool("IsDay", false);
    }
}
