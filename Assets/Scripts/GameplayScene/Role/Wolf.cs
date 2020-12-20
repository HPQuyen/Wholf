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
    protected IRole target;
    protected bool isKill;
    protected int playerID;
    protected bool isSelectable;
    protected string playerName;
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
        sect = Sect.wolves;
        roleID = RoleID.wolf;
        target = null;
        animHandler = GetComponent<AnimationHandler>();
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
        this.enabled = false;
    }
    public virtual bool IsMyRole(RoleID roleID)
    {
        return this.roleID == roleID;
    }
    public virtual void CastAbility(IRole opponent, PotionType type)
    {
        target = opponent;
        
        // Raise event to moderator
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    public virtual void ReceiveCastAbility(object[] data)
    {
        // Log
        //LogController.DoneAction(roleID, false, playerID, new object[] { data[2] });

        if (PhotonNetwork.IsMasterClient)
        {
            List<int> targetOfWolf = ListPlayerController.GetInstance().targetOfWolf;
            ListPlayerController.GetInstance().wolfActivation++;
            if (data[2] != null)
            {
                targetOfWolf.Add(ListPlayerController.GetInstance().GetRole((int)data[2]).GetPlayerID());
            }
            if (ListPlayerController.GetInstance().wolfActivation == ListPlayerController.GetInstance().CountNumberOfRole((role) => role.GetRoleID() == RoleID.wolf))
            {
                ListPlayerController.GetInstance().wolfActivation = 0;
                object[] content;
                if (targetOfWolf.Count == 0)
                    content = new object[] { null };
                else
                    content = new object[] { targetOfWolf[new System.Random().Next(0, targetOfWolf.Count)] };
                targetOfWolf.Clear();
                PunEventHandler.QuickRaiseEvent(PunEventID.AfterWolfHunt, content , new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
        }
        if (data[2] == null)
            return;
        // call update UI effect
        int opponentID = (int)data[2];
        IRole target = ListPlayerController.GetInstance().GetRole(opponentID);
        IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
        if (ListPlayerController.GetInstance().IsGhostView() || myRole != null && myRole.IsMyRole(RoleID.wolf))
        {
            PlayerUIController.GetInstance().AddRoleEffect(RoleID.wolf, target.GetPlayerID());
        }
        Debug.Log("End cast ability WolfHunt");
    }
    #endregion

    #region Getter/Setter
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
    public Sect GetSect()
    {
        return sect;
    }
    public string GetPlayerName()
    {
        return playerName;
    }
    public void SetPlayerID(int playerID)
    {
        this.playerID = playerID;
    }

    public void SetIsSelectable(bool state)
    {
        isSelectable = state;
        animHandler.SetSelectable(state);
    }
    public void SetIsKill(bool state)
    {
        isKill = state;
    }
    public void SetSect(Sect sect)
    {
        this.sect = sect;
    }
    public void SetPlayerName(string name)
    {
        playerName = name;
    }
    #endregion

    #region Local Action Event Methods
    public virtual void CompleteMyTurn()
    {
        object[] data;
        if (target == null)
            data = new object[] { this.GetRoleID(), this.playerID, null };
        else
        {
            data = new object[] { this.GetRoleID(), this.playerID, target.GetPlayerID() };
            target = null;
        }
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public virtual void InMyTurnEffect()
    {
        animHandler.InMyTurn();
        PlayerUIController.GetInstance().SwitchLight(playerID, true);
    }
    public virtual void CompleteMyTurnEffect()
    {
        animHandler.CompleteMyTurn();
        PlayerUIController.GetInstance().SwitchLight(playerID, false);
    }
    #endregion


}
