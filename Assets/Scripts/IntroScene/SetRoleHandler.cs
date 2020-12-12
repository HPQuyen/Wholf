using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetRoleHandler : MonoBehaviour
{
    private Dictionary<RoleID, int> RoleLst = new Dictionary<RoleID, int>();
    [SerializeField]
    private TextMeshProUGUI[] RoleNumber = null;

    private void DisplayRole(RoleID roleID)
    {
        if (RoleLst.ContainsKey(roleID))
        {
            RoleNumber[(int)roleID].text = RoleLst[roleID].ToString();
        }
    }
    public void Plus(int ID)
    {
        RoleID roleID = (RoleID)ID;
        switch(roleID)
        {
            case RoleID.cupid:
                {
                    if (RoleLst.ContainsKey(RoleID.hunter) && RoleLst[RoleID.hunter] == 1)
                        return;

                    if (!RoleLst.ContainsKey(RoleID.cupid) || RoleLst[RoleID.cupid] == 0)
                        RoleLst.Add(RoleID.cupid, 1);

                    break;
                }
            case RoleID.seer:
                {
                    if (!RoleLst.ContainsKey(RoleID.seer) || RoleLst[RoleID.seer] == 0)
                        RoleLst.Add(RoleID.seer, 1);

                    break;
                }
            case RoleID.hunter:
                {
                    if (RoleLst.ContainsKey(RoleID.cupid) && RoleLst[RoleID.cupid] == 1)
                        return;

                    if (!RoleLst.ContainsKey(RoleID.hunter) || RoleLst[RoleID.hunter] == 0)
                        RoleLst.Add(RoleID.hunter, 1);

                    break;
                }
            case RoleID.witch:
                {
                    if (!RoleLst.ContainsKey(RoleID.witch) || RoleLst[RoleID.witch] == 0)
                        RoleLst.Add(RoleID.witch, 1);

                    break;
                }
            case RoleID.guardian:
                {
                    if (!RoleLst.ContainsKey(RoleID.guardian) || RoleLst[RoleID.guardian] == 0)
                        RoleLst.Add(RoleID.guardian, 1);

                    break;
                }
            case RoleID.wolf:
                {
                    if (!RoleLst.ContainsKey(RoleID.wolf))
                        RoleLst.Add(RoleID.wolf, 0);

                    if (RoleLst[RoleID.wolf] < 2)
                        RoleLst[RoleID.wolf]++;

                    break;
                }
            case RoleID.villager:
                {
                    if (!RoleLst.ContainsKey(RoleID.villager))
                        RoleLst.Add(RoleID.villager, 0);

                    if (RoleLst[RoleID.villager] < 2)
                        RoleLst[RoleID.villager]++;

                    break;
                }
            default:break;
        }

        DisplayRole(roleID);
    }

    public void Minus(int ID)
    {
        RoleID roleID = (RoleID)ID;
        switch (roleID)
        {
            case RoleID.cupid:
                {
                    if (!RoleLst.ContainsKey(RoleID.cupid))
                        return;

                    if (RoleLst[RoleID.cupid] == 1)
                        RoleLst[RoleID.cupid] = 0;

                    break;
                }
            case RoleID.seer:
                {
                    if (!RoleLst.ContainsKey(RoleID.seer))
                        return;

                    if (RoleLst[RoleID.seer] == 1)
                        RoleLst[RoleID.seer] = 0;

                    break;
                }
            case RoleID.hunter:
                {
                    if (!RoleLst.ContainsKey(RoleID.hunter))
                        return;

                    if (RoleLst[RoleID.hunter] == 1)
                        RoleLst[RoleID.hunter] = 0;

                    break;
                }
            case RoleID.witch:
                {
                    if (!RoleLst.ContainsKey(RoleID.witch))
                        return;

                    if (RoleLst[RoleID.witch] == 1)
                        RoleLst[RoleID.witch] = 0;

                    break;
                }
            case RoleID.guardian:
                {
                    if (!RoleLst.ContainsKey(RoleID.guardian))
                        return;

                    if (RoleLst[RoleID.guardian] == 1)
                        RoleLst[RoleID.guardian] = 0;

                    break;
                }
            case RoleID.wolf:
                {
                    if (!RoleLst.ContainsKey(RoleID.wolf))
                        return;

                    if (RoleLst[RoleID.wolf] < 3 && RoleLst[RoleID.wolf] > 0)
                        RoleLst[RoleID.wolf]--;

                    break;
                }
            case RoleID.villager:
                {
                    if (!RoleLst.ContainsKey(RoleID.villager))
                        return;

                    if (RoleLst[RoleID.villager] < 3 && RoleLst[RoleID.villager] > 0)
                        RoleLst[RoleID.villager]--;

                    break;
                }
            default: break;
        }

        DisplayRole(roleID);
    }
}
