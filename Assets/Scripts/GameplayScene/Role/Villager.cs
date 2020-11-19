using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class Villager : MonoBehaviour, IRole
{

    #region Protected Fields
    protected Sect sect;
    protected int playerID;
    protected bool isKill;
    [SerializeField]
    protected RoleInformation roleInfo;
    #endregion

    #region Private Fields

    #endregion

    #region MonoFunctions
    protected virtual void Start() 
    {
        sect = Sect.villagers;
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
    public virtual Sprite GetSpriteRole()
    {
        return roleInfo.spriteRole;
    }
    public virtual string GetNameRole()
    {
        return roleInfo.nameRole;
    }
    public virtual bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.villager;
    }
    public virtual int GetTimeRoleAction()
    {
        return roleInfo.timeRoleAction;
    }
    public virtual void CastAbility(IRole opponent, byte typeAbility) {}
    public virtual void SetKilledTarget()
    {
        isKill = true;
    }
    public virtual int GetPlayerID()
    {
        return playerID;
    }
    public virtual RoleID GetRoleID()
    {
        return RoleID.villager;
    }

    #endregion

    #region Local Action Event Methods

    public virtual void CompleteMyTurn(){}
    #endregion
}
