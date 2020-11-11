using System;

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

}
