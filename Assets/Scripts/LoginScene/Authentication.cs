using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;

public class Authentication : MonoBehaviour
{
    [Header("Firebase")]
    private DependencyStatus dependencyStatus;
    private FirebaseAuth auth = null;
    private FirebaseUser user = null;

    [SerializeField]
    private Wholf.LoginScene.UIController UIcontroller = null;

    private void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log(auth);
    }

    
    

    public IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            UIcontroller.DisplayError(message);
        }
        else
        {
            // Check if account is verified yet or not
            user = LoginTask.Result;
            List<string> testAccount = new List<string>() {
                "tester1@gmail.com", "tester2@gmail.com", "tester3@gmail.com", "tester4@gmail.com",
                "tester5@gmail.com","tester6@gmail.com","tester7@gmail.com","tester8@gmail.com"};
            if (!user.IsEmailVerified && !testAccount.Contains(user.Email))
            {
                UIcontroller.DisplayError("Verify your account first");
            }
            else
            {
                //User is now logged in
                //Now get the result
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
                PlayerProfile.AddAuth(auth);
                SceneManager.LoadScene("IntroScene");
            }
        }
    }

    

    public IEnumerator Register(string _email, string _password, string _username, string _confirmpass)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            UIcontroller.DisplayError("Missing Username");
        }
        else if (_password != _confirmpass)
        {
            //If the password does not match show a warning
            UIcontroller.DisplayError("Password Does Not Match!");
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                UIcontroller.DisplayError(message);
            }
            else
            {
                //User has now been created
                //Now get the result
                user = RegisterTask.Result;

                if (user != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };
                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        UIcontroller.DisplayError("Username Set Failed!");
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        //instance.LoginScreen();
                        UIcontroller.DisplayError("Registered Successfully.Please check your mail to verify account");
                        var VerifiedTask = user.SendEmailVerificationAsync();
                    }
                }
            }
        }
    }
    public void PlayAsGuest()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
