using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : Villager
{
    public override bool IsMyRole(RoleID roleID)
    {
        return roleID == RoleID.witch;
    }
}
