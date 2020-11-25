﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Seer : Villager
{
    protected IRole target;
    protected override void Start()
    {
        target = null;
        roleID = RoleID.seer;
        animHandler = GetComponent<AnimationHandler>();
    }
    public override bool IsMyRole(RoleID roleID)
    {
        return this.roleID == roleID;
    }

    public override void CastAbility(IRole target, PotionType type)
    {
        this.target = target;

        // Raise event to moderator
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    public override void ReceiveCastAbility(object[] data)
    {
        target = ListPlayerController.GetInstance().GetRole((int)data[2]);

        // call update UI affect
        IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
        if (myRole != null && myRole.IsMyRole(RoleID.seer))
            target.GetAnimHandler().SeerDetection(target.IsMyRole(RoleID.wolf));

        target = null;

    }
    public override void CompleteMyTurn()
    {
        object[] data;
        Debug.Log("Complete My Turn Call");
        animHandler.CompleteMyTurn();
        if (target == null)
            data = new object[] { null, this.playerID };
        else
            data = new object[] { this.GetRoleID(), this.playerID, target.GetPlayerID() };
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public override RoleID GetRoleID()
    {
        return RoleID.seer;
    }
}
