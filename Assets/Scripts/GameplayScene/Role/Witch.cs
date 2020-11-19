using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    kill,
    rescue
}

public class Witch : Villager
{
    protected IRole target;

    private AbilityType atype;
    public override bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.witch;
    }
    public override void CastAbility(IRole opponent, byte typeAbility)
    {
        target = opponent;
        atype = (AbilityType) typeAbility;
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    public override void CompleteMyTurn()
    {
        object[] data;
        Debug.Log("Complete My Turn Call");
        if (target == null)
        {
            data = new object[] { null, this.playerID };
        } else
        {
            data = new object[] { this.GetRoleID(), this.playerID, target.GetPlayerID(), (byte)this.atype };
        }
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
}
