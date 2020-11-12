using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : Villager
{
    public override bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.guardian;
    }
}
