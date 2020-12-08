using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator anim = null;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        StartGame();
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
    }    

    private void StartGame()
    {
        anim.SetFloat("PositionX", this.gameObject.transform.position.x);
        anim.SetFloat("PositionY", this.gameObject.transform.position.y);
        anim.SetBool("IsDay",false);
    }
    public void DaytimeTransition()
    {
        anim.SetBool("IsDay", true);
    }
    public void InMyTurn()
    {
        anim.SetBool("IsDay", true);
    }
    public void CompleteMyTurn()
    {
        anim.SetBool("IsDay", false);
    }
    public void NighttimeTransition()
    {
        anim.SetBool("IsDay", false);
    }
    public void SetSelectable(bool state)
    {
        anim.SetBool("IsSelectable", state);
    }
    public void Die()
    {
        anim.Play("Die",-1,0f);
    }
}
