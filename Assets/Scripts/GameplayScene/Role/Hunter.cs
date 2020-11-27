using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Villager
{
    protected IRole target;
    protected override void Start()
    {
        target = null;
        roleID = RoleID.hunter;
        sect = Sect.villagers;
        animHandler = GetComponent<AnimationHandler>();
    }
    public override bool IsMyRole(RoleID roleID)
    {
        return this.roleID == roleID;
    }
    public override void BeKilled()
    {
        if (target != null)
            target.SetIsKill(true);
        if (sect == Sect.cupid)
            ListPlayerController.GetInstance().GetRole(Sect.cupid, playerID).SetIsKill(true);
    }

    public override void CastAbility(IRole target, PotionType type)
    {
        this.target = target;
        
        // Raise event to moderator
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    public override void ReceiveCastAbility(object[] data)
    {
        roleID = RoleID.villager;
        target = ListPlayerController.GetInstance().GetRole((int)data[2]);
        // call update UI affection
        IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
        if (myRole != null && myRole.IsMyRole(RoleID.hunter))
        {
            PlayerUIController.GetInstance().AddRoleAffection(RoleID.hunter, target.GetPlayerID());
        }
    }
    public override void CompleteMyTurn()
    {
        object[] data;
        Debug.Log("Complete My Turn Call");
        if (target == null)
            data = new object[] { null, this.playerID };
        else
            data = new object[] { this.GetRoleID(), this.playerID, target.GetPlayerID() };
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public override RoleID GetRoleID()
    {
        return RoleID.hunter;
    }
    public override IRole GetTarget()
    {
        return target;
    }
}
