using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{

    #region Public Fields
    public GameObject[] playerUIPrefab = null;
    #endregion
    #region
    [SerializeField]
    private GameObject roleDescription;
    #endregion
    #region Private Fields
    private static PlayerUIController instance = null;
    private IRole playerRole = null;
    private GameObject playerUI;
    #endregion

    #region Monobehavior Methods
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this);
    }
    #endregion

    #region Public Methods
    public static PlayerUIController GetInstance()
    {
        return instance;
    }
    public void LoadPlayerUI(IRole playerRole,byte roleID)
    {
        this.playerRole = playerRole;
        try
        {
            Debug.Log("Load UI");
            playerUI = playerUIPrefab[roleID];
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
    public void InMyTurn()
    {
        Debug.Log("Load ability UI");
        playerUI.SetActive(true);
        roleDescription.SetActive(false);
        
    }
    public void CompleteMyTurn()
    {
        playerUI.SetActive(false);
        roleDescription.SetActive(false);
        playerRole.CompleteMyTurn();
    }
    public void OnClick_RoleDescription()
    {
        roleDescription.SetActive(!roleDescription.activeSelf);
    }
        #region Seer
    public void OnClick_SeerAbility()
    {
        
    }
        #endregion

        #region Hunter
    public void OnClick_HunterAbility()
    {

    }
        #endregion

        #region Witch
    public void OnClick_WitchAbilityKill()
    {

    }
    public void OnClick_WitchAbilitySave()
    {

    }
        #endregion

        #region Cupid
    public void OnClick_CupidAbility()
    {

    }
        #endregion

        #region Wolf
    public void OnClick_WolfAbility()
    {

    }
        #endregion

        #region Guardian
    public void OnClick_GuardianAbility()
    {

    }
        #endregion

    #endregion

    #region Private Methods

    #endregion
}
