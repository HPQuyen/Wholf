using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Wholf.EndgameScene
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI[] playerName_Text = null;
        //[SerializeField]
        //private Image[] roleImage = null;
        [SerializeField]
        private TextMeshProUGUI[] roleImage_Text = null;
        [SerializeField]
        private GameObject[] roleExpositionDisplay = null;
        [SerializeField]
        private TextMeshProUGUI sectWinDisplay_Text = null;
        private void Start()
        {
            sectWinDisplay_Text.text = RoleExposition.GetSectWin().ToString().ToUpper() + " WIN";
            InitAllRoleExposition();
        }
        private void InitAllRoleExposition()
        {
            int i = 0;
            InitOneRoleExposition(RoleID.wolf, ref i);
            InitOneRoleExposition(RoleID.villager, ref i);
            InitOneRoleExposition(RoleID.cupid, ref i);
            InitOneRoleExposition(RoleID.witch, ref i);
            InitOneRoleExposition(RoleID.guardian, ref i);
            InitOneRoleExposition(RoleID.hunter, ref i);
            InitOneRoleExposition(RoleID.seer, ref i);
        }
        private void InitOneRoleExposition(RoleID roleID, ref int position)
        {
            bool isRoleExist = false;
            roleImage_Text[position].text = roleID.ToString() + " Image";
            playerName_Text[position].text = "";
            Dictionary<int, IRole> survivor = RoleExposition.GetSurvivor();
            foreach (var item in survivor)
            {
                if (item.Value.GetRoleID() == roleID)
                {
                    isRoleExist = true;
                    playerName_Text[position].text += PhotonNetwork.CurrentRoom.Players[item.Value.GetPlayerID()].NickName + "(survivor)";
                    if (item.Value.GetSect() == Sect.cupid)
                        playerName_Text[position].text += "(pair) ";
                    else
                        playerName_Text[position].text += " ";
                }
            }
            Dictionary<int, IRole> victim = RoleExposition.GetVictim();
            foreach (var item in victim)
            {
                if (item.Value.IsMyRole(roleID))
                {
                    isRoleExist = true;
                    playerName_Text[position].text += PhotonNetwork.CurrentRoom.Players[item.Value.GetPlayerID()].NickName;
                    if (item.Value.GetSect() == Sect.cupid)
                        playerName_Text[position].text += "(pair) ";
                    else
                        playerName_Text[position].text += " ";
                }
            }
            if (!isRoleExist)
                return;
            roleExpositionDisplay[position].SetActive(true);
            position++;
        }
        public void OnClick_Menu()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
            RoleExposition.ClearAll();
            ActionEventHandler.RemoveAllAction();
            PunEventHandler.RemoveAllEvent();
            SceneManager.LoadScene("IntroScene");
        }
        public void OnClick_Exit()
        {
            PhotonNetwork.Disconnect();
            Application.Quit();
        }
    }
}
