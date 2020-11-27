using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Guardian : Villager
{
    protected IRole target;
    protected IRole previousTarget;
    protected override void Start()
    {
        target = null;
        previousTarget = null;
        roleID = RoleID.guardian;
        sect = Sect.villagers;
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
        target.SetIsKill(false);
        // call update UI affect
        IRole myRole = ListPlayerController.GetInstance().GetRole(PhotonNetwork.LocalPlayer.ActorNumber);
        if (myRole != null && myRole.IsMyRole(RoleID.guardian))
            PlayerUIController.GetInstance().AddRoleAffection(RoleID.guardian, target.GetPlayerID());
        target = null;
    }
    public override void CompleteMyTurn()
    {
        object[] data;
        previousTarget = target;
        animHandler.CompleteMyTurn();
        if (target == null)
            data = new object[] { null, playerID };
        else
            data = new object[] { GetRoleID(), playerID, target.GetPlayerID() };
        PunEventHandler.QuickRaiseEvent(PunEventID.RoleActionComplete, data, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }
    public override RoleID GetRoleID()
    {
        return RoleID.guardian;
    }
    public override IRole GetTarget()
    {
        return previousTarget;
    }
}
