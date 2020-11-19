using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : Villager
{
    protected IRole target;
    public override bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.guardian;
    }
    public override void CastAbility(IRole opponent, byte typeAbility)
    {
        // opponent.SetKilledTarget();
        target = opponent;
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    public override void CompleteMyTurn()
    {
        object[] data;
        Debug.Log("Complete My Turn Call");
        if (target == null)
        {
            data = new object[] { null, this.playerID };
        }
        else
        {
            data = new object[] { this.GetRoleID(), this.playerID, target.GetPlayerID(), -1};
        }
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public override RoleID GetRoleID()
    {
        return RoleID.guardian;
    }
}
