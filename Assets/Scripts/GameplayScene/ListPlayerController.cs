using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.PlayerLoop;

public enum RoleID
{
    villager,
    wolf,
    seer,
    witch,
    guardian,
    cupid,
    hunter
}
public class ListPlayerController : MonoBehaviour
{
    #region Private SerializeField Fields

    #endregion

    #region Public Fields

    #endregion

    #region Private Fields
    private Dictionary<int, GameObject> listPlayerObject = new Dictionary<int, GameObject>();
    private Dictionary<int, IRole> listPlayerRole = new Dictionary<int, IRole>();
    #endregion

    #region Monobehavior Methods

    #endregion

    #region Public Methods
    public void InitAllPlayerRole(int[] playersID, byte[] roleID, Action onInitializeRole)
    {
        try
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                GameObject playerObject = null;
                IRole playerRole = null;
                if(playersID[i] == PhotonNetwork.PlayerList[i].ActorNumber)
                {
                    InitOnePlayerRole(roleID[i], ref playerObject, ref playerRole);
                    listPlayerObject.Add(PhotonNetwork.PlayerList[i].ActorNumber, playerObject);
                    listPlayerRole.Add(PhotonNetwork.PlayerList[i].ActorNumber, playerRole);
                    if(playersID[i] == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        
                        PlayerUIController.GetInstance().LoadPlayerUI(listPlayerRole[PhotonNetwork.LocalPlayer.ActorNumber], roleID[i]);
                    }
                }
            }
            // Load UI for local player
            onInitializeRole();
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }

    }

    #endregion

    #region Private Methods
    private void InitOnePlayerRole(byte roleID, ref GameObject playerObjectRef, ref IRole playerRoleRef)
    {
        string path = "CharacterObject/";
        try
        {
            switch (roleID)
            {
                case (byte)RoleID.villager:
                    {

                        playerObjectRef = Resources.Load<GameObject>(path + "Villager");
                        playerRoleRef = playerObjectRef.GetComponent<Villager>();
                        break;
                    }
                case (byte)RoleID.wolf:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Wolf");
                        playerRoleRef = playerObjectRef.GetComponent<Wolf>();
                        break;
                    }
                case (byte)RoleID.seer:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Seer");
                        playerRoleRef = playerObjectRef.GetComponent<Seer>();
                        break;
                    }
                case (byte)RoleID.witch:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Witch");
                        playerRoleRef = playerObjectRef.GetComponent<Witch>();
                        break;
                    }
                case (byte)RoleID.guardian:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Guardian");
                        playerRoleRef = playerObjectRef.GetComponent<Guardian>();
                        break;
                    }
                case (byte)RoleID.cupid:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Cupid");
                        playerRoleRef = playerObjectRef.GetComponent<Cupid>();
                        break;
                    }
                case (byte)RoleID.hunter:
                    {
                        playerObjectRef = Resources.Load<GameObject>(path + "Hunter");
                        playerRoleRef = playerObjectRef.GetComponent<Hunter>();
                        break;
                    }
                default:
                    break;
            }
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }

    }

    #endregion

}

