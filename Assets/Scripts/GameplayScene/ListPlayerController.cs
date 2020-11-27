using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
    public Transform[] characterSpawnPosition = null;
    #endregion

    #region Private Fields
    private static ListPlayerController instance = null;
    private Dictionary<int, GameObject> listPlayerObject = new Dictionary<int, GameObject>();
    private Dictionary<int, IRole> listPlayerRole = new Dictionary<int, IRole>();
    private Dictionary<int, int> listVoteBallot = new Dictionary<int, int>();
    private List<object> listPlayerInTurn = new List<object>();
    #endregion

    #region Monobehavior Methods
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }
    public static ListPlayerController GetInstance()
    {
        return instance;
    }
    #endregion

    #region Public Methods

    #region Turnbase Mechanism Methods
    public void InitAllPlayerRole(int[] playersID, byte[] roleID, Action onInitializeRole)
    {
        try
        {
            for(int i = 0; i < playersID.Length;i++)
            {
                for (int j = 0; j < PhotonNetwork.PlayerList.Length; j++)
                {
                    GameObject playerObject = null;
                    IRole playerRole = null;
                    if (playersID[i] == PhotonNetwork.PlayerList[j].ActorNumber)
                    {
                        InitOnePlayerRole(roleID[i], ref playerObject, ref playerRole,playersID[i], i);
                        PlayerUIController.GetInstance().LoadPlayerName(i, PhotonNetwork.PlayerList[j].NickName,playersID[i]);
                        listPlayerObject.Add(PhotonNetwork.PlayerList[j].ActorNumber, playerObject);
                        listPlayerRole.Add(PhotonNetwork.PlayerList[j].ActorNumber, playerRole);
                        // Load RoleUI for local player
                        if (playersID[i] == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            PlayerUIController.GetInstance().LoadPlayerUI(listPlayerRole[PhotonNetwork.LocalPlayer.ActorNumber], roleID[i]);
                        }
                        break;
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
    public void CallRoleWakeUp(RoleID roleID,Action onCallRoleWakeUp)
    {
        Debug.Log("Call role: " + roleID);
        foreach (var item in listPlayerRole)
        {
            Debug.Log(item.Key + " " + item.Value);
            if(item.Value.IsMyRole(roleID))
            {
                Debug.Log("My actor ID: " + PhotonNetwork.LocalPlayer.ActorNumber);
                Debug.Log("ActorID: " + item.Key);
                // It is turn of players have that role
                listPlayerInTurn.Add(item.Key);
            }
        }
        // Raise Event for other players
        if(listPlayerInTurn.Count > 0)
        {
            PunEventHandler.QuickRaiseEvent(PunEventID.RoleAwakeness, listPlayerInTurn.ToArray(), new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            onCallRoleWakeUp();
        }
    }
    public object[] GetDeathPlayer(bool byVote = false)
    {
        // Get death player by vote
        if(byVote)
        {
            if (listVoteBallot.Count == 0)
                return null;
            int playerID = listVoteBallot.Aggregate((x,y) => { return x.Value > y.Value ? x : y; }).Key;
            bool doubleMaxVote = false;
            foreach (var item in listVoteBallot)
            {
                if(playerID != item.Key && listVoteBallot[playerID] == item.Value)
                {
                    doubleMaxVote = true;
                }
            }
            if (!doubleMaxVote)
                listPlayerRole[playerID].SetIsKill(true);
            else
                return null;
        }
        // Get death player by role cast ability
        List<object> listDeathPlayer = new List<object>();
        foreach (var item in listPlayerRole)
        {
            if (item.Value.GetIsKill())
                item.Value.BeKilled();
        }
        foreach (var item in listPlayerRole)
        {
            if(item.Value.GetIsKill())
            {
                listDeathPlayer.Add(item.Key);
            }
        }
        if (listDeathPlayer.Count > 0)
            return listDeathPlayer.ToArray();
        return null;
    }
    public void RemoveDeathPlayer(object[] data,Action onRemoveDeathPlayer)
    {
        if (data != null)
        {
            try
            {
                int playerID;
                foreach (var item in data)
                {
                    playerID = (int)item;
                    if (playerID == PhotonNetwork.LocalPlayer.ActorNumber)
                        ActionEventHandler.Invoke(ActionEventID.AfterMyDeath);
                    listPlayerRole[playerID].MyDeath();
                    listPlayerRole.Remove(playerID);
                    listPlayerObject.Remove(playerID);
                }
            }
            catch (Exception)
            {
                //Debug.LogError("Error: " + exc.Message);
            }
        }
        listVoteBallot.Clear();
        onRemoveDeathPlayer();
    }
    public void RemoveDeathPlayer(int playerID)
    {
        try
        {
            listPlayerRole.Remove(playerID);
        }
        catch (Exception exc)
        {
            Debug.Log("Error: " + exc.Message);
        }
    }
    #endregion
    public void SetAllSelectable(List<int> exceptID,bool state)
    {
        foreach(KeyValuePair<int,IRole> item in listPlayerRole)
        {
            if(!exceptID.Contains(item.Value.GetPlayerID()))
            {
                item.Value.SetIsSelectable(state);
            }
        }
    }
    public void SetAllSelectable(bool state)
    {
        foreach (KeyValuePair<int, IRole> item in listPlayerRole)
        {
            listPlayerRole[item.Key].SetIsSelectable(state);
        }
    }
    public void SetAllSelectable(int exceptID, bool state)
    {
        foreach (KeyValuePair<int, IRole> item in listPlayerRole)
        {
            if (exceptID != item.Key)
            {
                item.Value.SetIsSelectable(state);
            }
        }
    }
    public List<int> GetListPlayerSurvivor()
    {
        List<int> listPlayerBeKilled = new List<int>();
        foreach (var item in listPlayerRole)
        {
            if (!item.Value.GetIsKill())
                listPlayerBeKilled.Add(item.Key);
        }
        return listPlayerBeKilled;
    }
    public IRole GetRole(int playerID)
    {
        try
        {
            return listPlayerRole[playerID];
        }
        catch (Exception exc)
        {
            Debug.Log(playerID);
            Debug.Log("Error: " + playerID + exc.Message);
        }
        return null;
    }
    public List<IRole> GetRole(RoleID roleID)
    {
        List<IRole> role = new List<IRole>();
        foreach (var item in listPlayerRole)
        {
            if (item.Value.IsMyRole(roleID))
                role.Add(item.Value);
        }
        return role.Count == 0 ? null : role;
    }
    public IRole GetRole(Sect sect,int exceptID)
    {
        IRole role = null;
        foreach (var item in listPlayerRole)
        {
            if(item.Value.GetSect() == sect && item.Value.GetPlayerID() != exceptID)
            {
                role = item.Value;
            }
        }
        return role;
    }
    public int CountNumberOfRole(RoleID roleID)
    {
        int sum = 0;
        foreach (var item in listPlayerRole)
        {
            if (item.Value.IsMyRole(roleID))
            {
                sum++;
            }
        }
        return sum;
    }
    public int CountNumberOfRole(Sect sect)
    {
        int sum = 0;
        foreach (var item in listPlayerRole)
        {
            if (item.Value.GetSect() == sect)
            {
                sum++;
            }
        }
        return sum;
    }
    #endregion

    #region Pun(Network) Event Methods
    public void ReceiveRoleWakeUp(int playerID,Action OnAwakeRole)
    {
        try
        {
            PlayerUIController.GetInstance().RoleActionNotification(listPlayerRole[playerID].GetNameRole() + "'s Turn");
        }
        catch (Exception exc)
        {
            Debug.Log("Error: " + exc.Message);
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            OnAwakeRole.Invoke();
        }
    }
    // Function callback when player done action
    public void ReceiveCompleteActionRole(object[] data, Action OnCompleteActionRole)
    {
        int playerID = (int) data[1];
        try
        {
            if (data[0] != null)
                listPlayerRole[playerID].ReceiveCastAbility(data);
            if (PhotonNetwork.IsMasterClient && listPlayerInTurn.Remove(playerID) && listPlayerInTurn.Count <= 0 && !listPlayerRole[playerID].IsMyRole(RoleID.wolf))
                OnCompleteActionRole.Invoke();
        }catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    // Special function callback when player done action (only for wolf) 
    public void ReceiveAfterWolfHunt(object[] data, Action OnCompleteActionRole)
    {
        if (data[0] != null)
        {
            IRole target = GetRole((int)data[0]);
            if(target != null)
                target.SetIsKill(true);
        }
        if(PhotonNetwork.IsMasterClient)
            OnCompleteActionRole.Invoke();
    }
    public void ReceiveVote(object[] data)
    {
        int playerID = (int)data[0];
        if(listVoteBallot.ContainsKey(playerID))
        {
            listVoteBallot[playerID]++;
        }
        else
        {
            listVoteBallot.Add(playerID, 1);
        }
        PlayerUIController.GetInstance().VoteCountDisplay(playerID, listVoteBallot[playerID]);
    }
    #endregion



    #region Private Methods
    private void InitOnePlayerRole(byte roleID, ref GameObject playerObjectRef, ref IRole playerRoleRef,int playerID,int spawnPosition)
    {
        string path = "CharacterObject/Character1/";
        try
        {
            switch (roleID)
            {
                case (byte)RoleID.villager:
                    {
                        playerObjectRef = Instantiate(Resources.Load<GameObject>(path + "Villager"), 
                            characterSpawnPosition[spawnPosition].position, 
                            characterSpawnPosition[spawnPosition].rotation);
                        playerRoleRef = playerObjectRef.GetComponent<Villager>();
                        break;
                    }
                case (byte)RoleID.wolf:
                    {
                        playerObjectRef = Instantiate(Resources.Load<GameObject>(path + "Wolf"),
                            characterSpawnPosition[spawnPosition].position,
                            characterSpawnPosition[spawnPosition].rotation);
                        playerRoleRef = playerObjectRef.GetComponent<Wolf>();
                        break;
                    }
                case (byte)RoleID.seer:
                    {
                        playerObjectRef = Instantiate(Resources.Load<GameObject>(path + "Seer"),
                            characterSpawnPosition[spawnPosition].position,
                            characterSpawnPosition[spawnPosition].rotation);
                        playerRoleRef = playerObjectRef.GetComponent<Seer>();
                        break;
                    }
                case (byte)RoleID.witch:
                    {
                        playerObjectRef = Instantiate(Resources.Load<GameObject>(path + "Witch"),
                            characterSpawnPosition[spawnPosition].position,
                            characterSpawnPosition[spawnPosition].rotation);
                        playerRoleRef = playerObjectRef.GetComponent<Witch>();
                        break;
                    }
                case (byte)RoleID.guardian:
                    {
                        playerObjectRef = Instantiate(Resources.Load<GameObject>(path + "Guardian"),
                            characterSpawnPosition[spawnPosition].position,
                            characterSpawnPosition[spawnPosition].rotation);
                        playerRoleRef = playerObjectRef.GetComponent<Guardian>();
                        break;
                    }
                case (byte)RoleID.cupid:
                    {
                        playerObjectRef = Instantiate(Resources.Load<GameObject>(path + "Cupid"),
                            characterSpawnPosition[spawnPosition].position,
                            characterSpawnPosition[spawnPosition].rotation);
                        playerRoleRef = playerObjectRef.GetComponent<Cupid>();
                        break;
                    }
                case (byte)RoleID.hunter:
                    {
                        playerObjectRef = Instantiate(Resources.Load<GameObject>(path + "Hunter"),
                            characterSpawnPosition[spawnPosition].position,
                            characterSpawnPosition[spawnPosition].rotation);
                        playerRoleRef = playerObjectRef.GetComponent<Hunter>();
                        break;
                    }
                default:
                    break;
            }
            // Set player ID
            playerRoleRef.SetPlayerID(playerID);
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    #endregion

}

