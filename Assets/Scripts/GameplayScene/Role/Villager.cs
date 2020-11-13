using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class Villager : MonoBehaviour, IRole
{

    #region Protected Fields
    protected Sect sect;
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
    #endregion

    #region Local Action Event Methods

    public virtual void CompleteMyTurn()
    {
        Debug.Log("Complete My Turn Call");
        object[] data = new object[] { PhotonNetwork.LocalPlayer.ActorNumber };
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    #endregion
}
