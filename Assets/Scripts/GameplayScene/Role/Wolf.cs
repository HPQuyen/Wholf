using System;
using UnityEngine;

public class Wolf : MonoBehaviour, IRole
{
    #region Protected Fields
    protected Sect sect;
    protected bool isKill;
    [SerializeField]
    protected RoleInformation roleInfo;
    #endregion

    #region Public Functions
    public virtual void Die() { }
    public virtual void RoleAction(Action onRoleAction, IRole Target) { }
    public virtual IRole SetTargetKill() { return null; }
    public virtual bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.wolf;
    }
    public virtual void InMyTurn()
    {
        
    }
    public virtual void CompleteMyTurn()
    {

    }
    public virtual Sprite GetSpriteRole()
    {
        return roleInfo.spriteRole;
    }
    public virtual string GetNameRole()
    {
        return roleInfo.nameRole;
    }
    #endregion

    #region MonoFunctions
    protected virtual void Start() {
        sect = Sect.wolves;
        isKill = false;
    }
    #endregion
}
