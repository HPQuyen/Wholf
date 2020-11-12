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
    void InMyTurn();
    void CompleteMyTurn();

    Sprite GetSpriteRole();
    string GetNameRole();

}
