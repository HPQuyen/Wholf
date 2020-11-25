using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour, IRole
{

    #region Protected Fields
    protected Sect sect;
    protected RoleID roleID;
    protected int playerID { get; set; }
    protected bool isKill { get; set; }
    protected bool isSelectable { get; set; }

    protected AnimationHandler animHandler;

    [SerializeField]
    protected RoleInformation roleInfo;
    #endregion

    #region Private Fields

    #endregion

    #region MonoFunctions
    protected virtual void Start() 
    {
        isKill = false;
        isSelectable = false;
        sect = Sect.villagers;
        roleID = RoleID.villager;
        animHandler = GetComponent<AnimationHandler>();
    }
    protected virtual void Update()
    {

    }
    public virtual void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    public virtual void OnMouseEnter()
    {
        if (isSelectable)
            GetComponent<SpriteRenderer>().color = Color.red;
        else
            GetComponent<SpriteRenderer>().color = Color.white;
    }
    public virtual void OnMouseDown()
    {
        if(isSelectable)
        {
            ActionEventHandler.Invoke(this);
        }
    }
    #endregion

    #region Public Functions
    public virtual bool IsMyRole(RoleID roleID)
    {
        return this.roleID == roleID;
    }
    public virtual void BeKilled() 
    {
        if(sect == Sect.cupid)
            ListPlayerController.GetInstance().GetRole(Sect.cupid, playerID).SetIsKill(true);
    }
    public virtual void MyDeath()
    {
        animHandler.Die();
    }
    public virtual void CastAbility(IRole opponent, PotionType type) { }
    public virtual void ReceiveCastAbility(object[] data) { }
    #endregion

    #region Getter/Setter
    public Sprite GetSpriteRole()
    {
        return roleInfo.spriteRole;
    }
    public string GetNameRole()
    {
        return roleInfo.nameRole;
    }
    public int GetTimeRoleAction()
    {
        return roleInfo.timeRoleAction;
    }
    public int GetPlayerID()
    {
        return playerID;
    }
    public bool GetIsKill()
    {
        return isKill;
    }
    public virtual RoleID GetRoleID()
    {
        return RoleID.villager;
    }
    public virtual IRole GetTarget()
    {
        return null;
    }
    public AnimationHandler GetAnimHandler()
    {
        return animHandler;
    }
    public Sect GetSect()
    {
        return sect;
    }
    public void SetPlayerID(int playerID)
    {
        this.playerID = playerID;
    }
    public void SetIsSelectable(bool state)
    {
        isSelectable = state;
    }
    public void SetIsKill(bool state)
    {
        isKill = state;
    }
    public void SetSect(Sect sect)
    {
        this.sect = sect;
    }
    #endregion

    #region Local Action Event Methods

    public virtual void CompleteMyTurn(){}
    public virtual void InMyTurn() {
        animHandler.InMyTurn();
    }
    #endregion
}
