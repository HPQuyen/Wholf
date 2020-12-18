﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class Cupid : Villager
{
    protected List<IRole> target;

    protected override void Start()
    {
        roleID = RoleID.cupid;
        sect = Sect.villagers;
        target = new List<IRole>();
        animHandler = GetComponent<AnimationHandler>();
        
    }

    public override void CastAbility(IRole opponent, PotionType type)
    {
        target.Add(opponent);
        opponent.SetIsSelectable(false);
        // Raise event to moderator
        if (target.Count == 2)
        {
            ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
        }
    }
    public override void ReceiveCastAbility(object[] data)
    {
        // Log
        //LogController.DoneAction(roleID, false, playerID, new object[] { data[2],data[3] });

        roleID = RoleID.villager;
        if (data[2] == null || data[3] == null)
            return;
        IRole target1 = ListPlayerController.GetInstance().GetRole((int)data[2]);
        IRole target2 = ListPlayerController.GetInstance().GetRole((int)data[3]);
        target.Add(target1);
        target.Add(target2);
        target1.SetSect(Sect.cupid);
        target2.SetSect(Sect.cupid);
        if(PhotonNetwork.LocalPlayer.ActorNumber == target1.GetPlayerID() || PhotonNetwork.LocalPlayer.ActorNumber == target2.GetPlayerID())
        {
            PlayerUIController.GetInstance().AddRoleEffect(RoleID.cupid, target1.GetPlayerID());
            PlayerUIController.GetInstance().AddRoleEffect(RoleID.cupid, target2.GetPlayerID());
            ActionEventHandler.Invoke(ActionEventID.InCupidPairing);
            return;
        }
        IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
        if (ListPlayerController.IsGhostView() || myRole != null && myRole.IsMyRole(RoleID.cupid))
        {
            PlayerUIController.GetInstance().AddRoleEffect(RoleID.cupid, target1.GetPlayerID());
            PlayerUIController.GetInstance().AddRoleEffect(RoleID.cupid, target2.GetPlayerID());
        }
    }
    public override void CompleteMyTurn()
    {
        object[] data;
        Debug.Log("Complete My Turn Call");
        if (target.Count != 2)
            data = new object[] { this.GetRoleID(), this.playerID, null , null};
        else
            data = new object[] { this.GetRoleID(), this.playerID, target[0].GetPlayerID(), target[1].GetPlayerID()};
        target.Clear();
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public override RoleID GetRoleID()
    {
        return RoleID.cupid;
    }
    public override IRole GetTarget()
    {
        if (target.Count == 0)
            return null;
        return target[0];
    }
}
