using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour, IRole
{
    #region Protected Fields
    protected Sect sect;
    protected RoleID roleID;
    protected bool isKill;
    protected int playerID;
    protected bool isSelectable;
    protected List<IRole> target;
    protected AnimationHandler animHandler;
    [SerializeField]
    protected RoleInformation roleInfo;
    #endregion

    #region Private Fields
    private int timesActivation;
    #endregion

    #region MonoFunctions
    protected virtual void Start()
    {
        isKill = false;
        isSelectable = false;
        timesActivation = 0;
        sect = Sect.wolves;
        roleID = RoleID.wolf;
        target = new List<IRole>();
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
        if (isSelectable)
        {
            ActionEventHandler.Invoke(this);
        }
    }
    #endregion

    #region Public Functions
    public virtual void BeKilled()
    {
        if (sect == Sect.cupid)
        {
            ListPlayerController.GetInstance().GetRole(Sect.cupid, playerID).SetIsKill(true);
        }
    }
    public virtual void MyDeath()
    {
        animHandler.Die();
    }
    public virtual bool IsMyRole(RoleID roleID)
    {
        return this.roleID == roleID;
    }
    public virtual void CastAbility(IRole opponent, PotionType type)
    {
        target.Add(opponent);
        
        // Raise event to moderator
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    public virtual void ReceiveCastAbility(object[] data)
    {
        timesActivation++;
        if(data[2] == null)
        {
            return;
        }
        int opponentID = (int) data[2];
        IRole target = ListPlayerController.GetInstance().GetRole(opponentID);
        this.target.Add(target);
        if (ListPlayerController.GetInstance().CountNumberOfRole(RoleID.wolf) % timesActivation == 0 && timesActivation > 0)
        {
            timesActivation = 0;
            if(this.target.Count > 0)
            {
                this.target[new System.Random().Next(0, this.target.Count)].SetIsKill(true);
                this.target.Clear();
            }
        }
        // call update UI affect
        IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
        if (myRole != null && myRole.IsMyRole(RoleID.wolf))
        {
            PlayerUIController.GetInstance().AddRoleAffection(RoleID.wolf, target.GetPlayerID());
        }
    }
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
    public bool GetIsKill()
    {
        return isKill;
    }
    public int GetPlayerID()
    {
        return playerID;
    }
    public virtual RoleID GetRoleID()
    {
        return RoleID.wolf;
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
    public virtual void CompleteMyTurn()
    {
        object[] data;
        if (target.Count == 0)
            data = new object[] { this.GetRoleID(), this.playerID, null };
        else
        {
            data = new object[] { this.GetRoleID(), this.playerID, target[target.Count - 1].GetPlayerID() };
            target.RemoveAt(target.Count - 1);
        }
        animHandler.CompleteMyTurn();
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public virtual void InMyTurn()
    {
        animHandler.InMyTurn();
    }
    #endregion


}
