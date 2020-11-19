using System;
using UnityEngine;

public enum Sect
{
    villagers,
    wolves,
    cupid
}
public interface IRole
{
    void RoleAction(Action onRoleAction, IRole Target);
    void Die();
    IRole SetTargetKill();

    void SetKilledTarget();
    /* Roles cast abitity.
    This function will send a signal to game moderator. 
    */
    void CastAbility(IRole roleID, byte typeAbility); 
    bool IsMyRole(RoleID roleID);
    void CompleteMyTurn();

    #region Getter/Setter
    Sprite GetSpriteRole();
    string GetNameRole();
    int GetTimeRoleAction();
    int GetPlayerID();
    RoleID GetRoleID();
    #endregion
}
