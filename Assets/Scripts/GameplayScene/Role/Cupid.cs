using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cupid : Villager
{
    protected List<IRole> target = new List<IRole>();
    public override bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.cupid;
    }
    public override void CastAbility(IRole opponent, byte typeAbility)
    {
        // opponent.SetKilledTarget();
        target.Add(opponent);
        if (target.Count == 2)
        {
            ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
        }
    }
    public override void CompleteMyTurn()
    {
        object[] data;
        Debug.Log("Complete My Turn Call");
        if (target.Count == 0)
        {
            data = new object[] { null, this.playerID };
        } else
        {
            data = new object[] { this.GetRoleID(), this.playerID, target[0].GetPlayerID(), target[1].GetPlayerID(), -1 };
        }
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public override RoleID GetRoleID()
    {
        return RoleID.cupid;
    }
}
