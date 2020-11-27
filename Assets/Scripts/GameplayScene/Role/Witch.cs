using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PotionType
{
    kill,
    rescue
}

public class Witch : Villager
{
    protected Stack<IRole> target;

    private PotionType atype;
    private int timesActivation;
    protected override void Start()
    {
        timesActivation = 0;
        roleID = RoleID.witch;
        sect = Sect.villagers;
        target = new Stack<IRole>();
        animHandler = GetComponent<AnimationHandler>();
    }
    public override bool IsMyRole(RoleID roleID)
    {
        return this.roleID == roleID;
    }
    public override void CastAbility(IRole opponent, PotionType type)
    {
        target.Push(opponent);
        atype = type;
        // Raise event to moderator
        ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    
    public override void ReceiveCastAbility(object[] data)
    {
        timesActivation++;
        if(timesActivation == 2)
        {
            roleID = RoleID.villager;
        }
        IRole target = ListPlayerController.GetInstance().GetRole((int)data[2]);
        PotionType type = (PotionType)((byte)data[3]);
        if (type == PotionType.kill)
            target.SetIsKill(true);
        else
            target.SetIsKill(false);
        // call update UI affection
        IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
        if (myRole != null && myRole.IsMyRole(RoleID.witch))
        {
            PlayerUIController.GetInstance().AddRoleAffection(RoleID.witch, target.GetPlayerID(), type);
        }
    }
    public override void CompleteMyTurn()
    {
        object[] data;
        Debug.Log("Complete My Turn Call");
        if (target.Count == 0)
            data = new object[] { null, playerID };
        else
            data = new object[] { GetRoleID(), playerID, target.Pop().GetPlayerID(), (byte)atype };
        animHandler.CompleteMyTurn();
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
}
