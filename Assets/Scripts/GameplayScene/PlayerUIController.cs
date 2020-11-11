using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    #region Public Fields
    public GameObject[] playerUIPrefab = null;
    
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
            playerUI = Instantiate(playerUIPrefab[roleID]);
            playerUI.transform.SetParent(this.gameObject.transform);
            playerUI.SetActive(false);
        }
        catch(Exception exc)
        {
            Debug.LogError("Error: " + exc.Message);
        }
    }
        #region Seer

        #endregion

        #region Hunter

        #endregion

        #region Witch

        #endregion

        #region Cupid

        #endregion

        #region Wolf

        #endregion

        #region Guardian

        #endregion

    #endregion

    #region Private Methods

    #endregion
}
