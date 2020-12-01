using System;
using System.Collections.Generic;
using UnityEngine;

public enum Sect
{
    villagers,
    wolves,
    cupid
}
public interface IRole
{

    //  Handle player be killed
    void BeKilled();
    void MyDeath();

    /*  Roles activate their abitity.
        This function will send a signal to game moderator. */
    void CastAbility(IRole roleID, PotionType type = PotionType.kill);
    //  This function will take a signal from moderator and handle.
    void ReceiveCastAbility(object[] data);
    bool IsMyRole(RoleID roleID);
    void InMyTurnEffect();
    void CompleteMyTurnEffect();
    void CompleteMyTurn();

    #region Getter/Setter
    Sprite GetSpriteRole();
    string GetNameRole();
    int GetTimeRoleAction();
    int GetPlayerID();
    bool GetIsKill();
    RoleID GetRoleID();
    IRole GetTarget();
    AnimationHandler GetAnimHandler();
    Sect GetSect();
    void SetPlayerID(int playerID);
    void SetIsSelectable(bool state);
    void SetIsKill(bool state);
    void SetSect(Sect sect);
    #endregion
}
