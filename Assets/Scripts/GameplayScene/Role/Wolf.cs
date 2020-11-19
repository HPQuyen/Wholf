using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class Wolf : MonoBehaviour, IRole
{
    #region Protected Fields
    protected Sect sect;
    protected bool isKill;
    protected int playerID;
    protected IRole target;
    [SerializeField]
    protected RoleInformation roleInfo;
    #endregion

    #region Private Fields

    #endregion

    #region MonoFunctions
    protected virtual void Start()
    {
        sect = Sect.wolves;
        isKill = false;

    }
    protected virtual void Update()
    {

    }

    public virtual void OnMouseDown()
    {
        ActionEventHandler.Invoke(this);
    }
    #endregion

    #region Public Functions
    public virtual void Die() { }
    public virtual void RoleAction(Action onRoleAction, IRole Target) { }
    public virtual IRole SetTargetKill() { return null; }
    public virtual bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.wolf;
    }
    public virtual Sprite GetSpriteRole()
    {
        return roleInfo.spriteRole;
    }
    public virtual string GetNameRole()
    {
        return roleInfo.nameRole;
    }
    public virtual int GetTimeRoleAction()
    {
        return roleInfo.timeRoleAction;
    }
    public virtual void SetKilledTarget()
    {
        isKill = true;
    }
    public virtual void CastAbility(IRole opponent, byte typeAbility)
    {
        // If need, call effect.
        target = opponent;
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    public virtual int GetPlayerID()
    {
        return playerID;
    }
    public virtual RoleID GetRoleID()
    {
        return RoleID.wolf;
    }

    #endregion

    #region Local Action Event Methods

    public virtual void CompleteMyTurn()
    {
        object[] data;
        // Debug.Log("Complete My Turn Call");
        if (target == null)
        {
            data = new object[] { null, this.playerID };
        }
        else
        {
            data = new object[] { this.GetRoleID(), this.playerID, target.GetPlayerID(), -1 };
        }
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    #endregion


}
