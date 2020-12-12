using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Wholf.LoginScene 
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject panelLogin = null;
        [SerializeField]
        private GameObject panelSignUp = null;
        [SerializeField]
        private Authentication auth = null;

        //Login variables
        [Header("Login")]
        [SerializeField]
        private TMP_InputField emailLoginField;
        [SerializeField]
        private TMP_InputField passwordLoginField;

        //Register variables
        [Header("Register")]
        [SerializeField]
        private TMP_InputField emailRegisterField;
        [SerializeField]
        private TMP_InputField passwordRegisterField;
        [SerializeField]
        private TMP_InputField passwordConfirmedField;
        [SerializeField]
        private TMP_InputField usernameRegisterField;

        [SerializeField]
        private TMP_Text warning;

        //Function for the login button
        public void OnClick_Login()
        {
            //Call the login coroutine passing the email and password
            StartCoroutine(auth.Login(emailLoginField.text, passwordLoginField.text));
        }

        public void OnClick_Register()
        {
            StartCoroutine(auth.Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text, passwordConfirmedField.text));
        }

        public void OnClick_SwitchToRegisterPanel()
        {
            panelLogin.SetActive(false);
            panelSignUp.SetActive(true);
        }

        public void OnClick_SwitchToLoginPanel()
        {
            panelLogin.SetActive(true);
            panelSignUp.SetActive(false);
        }

        public void DisplayError(string ErrorMsg)
        {
            warning.text = ErrorMsg;
        }
    }
}