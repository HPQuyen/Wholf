using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public enum PotionType
{
    kill,
    rescue
}

public class Witch : Villager
{
    protected Queue<IRole> target;

    private Queue<PotionType> atype;
    private int timesActivation;
    protected override void Start()
    {
        timesActivation = 0;
        roleID = RoleID.witch;
        sect = Sect.villagers;
        target = new Queue<IRole>();
        atype = new Queue<PotionType>();
        animHandler = GetComponent<AnimationHandler>();
    }

    public override void CastAbility(IRole opponent, PotionType type)
    {
        target.Enqueue(opponent);
        atype.Enqueue(type);
        PlayerUIController.GetInstance().OnClick_Cancel();
        // Raise event to Moderator
        if (target.Count == 2 || timesActivation == 1)
            ActionEventHandler.Invoke(ActionEventID.CompleteMyTurn);
    }
    
    public override void ReceiveCastAbility(object[] data)
    {
        //Log
        //LogController.DoneAction(roleID, false, playerID, new object[] { data[2] } ,(byte)((PotionType)data[3]));
        
        timesActivation = data.Length == 4 ? timesActivation + 1 : 2;
        for (int i = 2; i < data.Length; i += 2)
        {
            IRole target = ListPlayerController.GetInstance().GetRole((int)data[i]);
            PotionType type = (PotionType)data[i+1];
            target.SetIsKill(type == PotionType.kill);
            // call update UI effect
            IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
            if (ListPlayerController.GetInstance().IsGhostView() || myRole != null && myRole.IsMyRole(RoleID.witch))
            {
                PlayerUIController.GetInstance().AddRoleEffect(RoleID.witch, target.GetPlayerID(), type);
            }
        }
    }
    public override void CompleteMyTurn()
    {
        List<object> data = new List<object>() { null,playerID };
        Debug.Log("Complete My Turn Call");
        if(target.Count != 0)
        {
            data.Clear();
            data.Add(GetRoleID());
            data.Add(playerID);
            while(target.Count != 0)
            {
                data.Add(target.Dequeue().GetPlayerID());
                data.Add(atype.Dequeue());
            }
        }
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data.ToArray(), new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public override RoleID GetRoleID()
    {
        return RoleID.witch;
    }
}
