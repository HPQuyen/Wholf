using Firebase.Auth;

public static class PlayerProfile
{
    private static FirebaseAuth firebaseAuth = null;

    public static void AddAuth(FirebaseAuth auth)
    {
        firebaseAuth = auth;
    }

    public static string GetPlayerName()
    {
        if (firebaseAuth == null)
            return null;
        return firebaseAuth.CurrentUser.DisplayName;
    }

    public static void SignOut()
    {
        if (firebaseAuth != null)
        {
            firebaseAuth.SignOut();
            firebaseAuth = null;
        }
    }
}
