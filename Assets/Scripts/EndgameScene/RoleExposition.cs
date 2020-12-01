using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleExposition : MonoBehaviour
{
    private static Dictionary<int, IRole> survivor = new Dictionary<int, IRole>();
    private static Dictionary<int, IRole> victim = new Dictionary<int, IRole>();
    private static Sect sectWin;
    public static void AddVictim(IRole player)
    {
        victim.Add(player.GetPlayerID(), player);
    }
    public static Dictionary<int, IRole> GetVictim()
    {
        return victim;
    }
    public static void SetSectWin(Sect sect)
    {
        sectWin = sect;
    }
    public static Sect GetSectWin()
    {
        return sectWin;
    }
    public static void SetSurvivor(Dictionary<int,IRole> sur)
    {
        survivor = sur;
    }
    public static Dictionary<int, IRole> GetSurvivor()
    {
        return survivor;
    }
    public static void ClearAll()
    {
        survivor.Clear();
        victim.Clear();
    }
}
