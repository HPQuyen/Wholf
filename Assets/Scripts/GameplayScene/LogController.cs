using Photon.Pun;
using UnityEngine;

public static class LogController
{
    public static void LogWakeUp(RoleID roleID, bool isFakeCall = false,int playerID = -1)
    {
        try
        {
            if(isFakeCall)
            {
                Debug.Log("*******FAKE CALL ROLE: " + roleID + "*******");
                return;
            }
            Debug.Log("********Log Wake Up********");
            Debug.Log("Call role: " + roleID);
            Debug.Log("Player ID: " + playerID);
            Debug.Log("Player Name: " + PhotonNetwork.CurrentRoom.Players[playerID].NickName);
            Debug.Log("***************************");
        }
        catch (System.Exception exc)
        {
            Debug.Log("Error: " + exc.Message);
        }

    }
    public static void DoneAction(RoleID roleID, bool isFakeCall = false, int playerID = -1,object[] target = null,byte type = 0)
    {
        try
        {
            if (isFakeCall)
            {
                Debug.Log("*******FAKE CALL DONE: " + roleID + "*******");
                return;
            }
            Debug.Log("********Log Done Action********");
            Debug.Log("Role call done action: " + roleID);
            Debug.Log("Player Name Action: " + PhotonNetwork.CurrentRoom.Players[playerID].NickName);
            if(target != null)
            {
                foreach (var item in target)
                {
                    if (item != null)
                        Debug.Log("Player Name Receive: " + PhotonNetwork.CurrentRoom.Players[(int)item].NickName);
                }
            }
            else
            {
                Debug.Log("Action: Skip");
            }
            if(roleID == RoleID.witch)
                Debug.Log("Type: " + (type == 0 ?"Kill":"Rescue"));
            Debug.Log("*******************************");
        }
        catch (System.Exception exc)
        {
            Debug.Log("Error: " + exc.Message);
        }
    }
}
