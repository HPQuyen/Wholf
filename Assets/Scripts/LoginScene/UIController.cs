
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
        private Authentication authentication = null;

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
        private TMP_Text warningText;

        //Function for the login button
        public void OnClick_Login()
        {
            //Call the login coroutine passing the email and password
            StartCoroutine(authentication.Login(emailLoginField.text, passwordLoginField.text));
        }

        public void OnClick_Register()
        {
            StartCoroutine(authentication.Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text, passwordConfirmedField.text));
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
        public void OnClick_Guest()
        {
            authentication.PlayAsGuest();
        }
        public void OnClick_Exit()
        {
            Application.Quit();
        }
        public void DisplayError(string ErrorMsg)
        {
            warningText.text = ErrorMsg;
            TimeManipulator.GetInstance().InvokeActionAfterSeconds(3f, () => { warningText.text = ""; });
        }



        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if(panelLogin.activeSelf)
                {
                    if (emailLoginField.isFocused)
                    {
                        passwordLoginField.Select();
                    }
                    else
                    {
                        emailLoginField.Select();
                    }
                }
                else
                {
                    if (emailRegisterField.isFocused)
                    {
                        usernameRegisterField.Select();
                    }
                    else if (usernameRegisterField.isFocused)
                    {
                        passwordRegisterField.Select();
                    }
                    else if (passwordRegisterField.isFocused)
                    {
                        passwordConfirmedField.Select();
                    }
                    else
                    {
                        emailRegisterField.Select();
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                if (panelLogin.activeSelf)
                {
                    OnClick_Login();
                }
                else
                {
                    OnClick_Register();
                }
            }
        }
    }
}