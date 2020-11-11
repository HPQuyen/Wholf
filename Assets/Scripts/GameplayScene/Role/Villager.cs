using System;
using UnityEngine;

public class Villager : MonoBehaviour,IRole
{

    #region Protected Fields
    protected string nameOutfit;
    protected Sect sect;
    protected bool isKill;
    protected RoleInformation roleInfo;
    #endregion

    #region Public Functions
    public virtual void Die() { }
    public virtual void RoleAction(Action onRoleAction,IRole Target) { }
    public virtual IRole SetTargetKill() { return null; }
    #endregion

    #region MonoFunctions
    protected virtual void Start() { }
    #endregion
}
