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
    bool IsMyRole(RoleID roleID);
    void CompleteMyTurn();

    #region Getter/Setter
    Sprite GetSpriteRole();
    string GetNameRole();
    int GetTimeRoleAction();
    #endregion
}
