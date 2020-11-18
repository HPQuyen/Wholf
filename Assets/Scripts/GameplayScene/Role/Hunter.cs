using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Villager
{
    public override bool IsMyRole(RoleID roleID)
    {
        Debug.Log(roleID);
        Debug.Log(roleID == RoleID.hunter);
        return roleID == RoleID.hunter;
    }
}
