using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ChatboxManager : MonoBehaviour,IChatClientListener
{
    #region Private SerializeField
    [SerializeField]
    private Text contentMessage = null;
    [SerializeField]
    private InputField inputField = null;
    [SerializeField]
    private Toggle[] channelTab = null;
    #endregion
    #region Private Fields
    private ChatClient chatClient = null;
    private string[] channels = new string[] { "PublicChannel", "WolfChannel", "CoupleChannel" };
    private string currentChannel = "PublicChannel";
    private bool isDay;
    #endregion
    #region Monobehaviour Methods
    // Start is called before the first frame update
    private void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, StartGame);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.AfterMyDeath, AfterMyDeath);
        ActionEventHandler.AddNewActionEvent(ActionEventID.InCupidPairing, () => { SubscribeNewChannel(2); });
        ActionEventHandler.AddNewActionEvent(ActionEventID.TwoWolfInGame, () => { SubscribeNewChannel(1); });
    }

    // Update is called once per frame
    private void Update()
    {
        chatClient.Service();
    }
    private void OnDestroy()
    {
        if(chatClient != null)
            chatClient.Disconnect();
    }
    private void OnApplicationQuit()
    {
        if (chatClient != null)
            chatClient.Disconnect();
    }
    #endregion

    #region Public Methods
    public void OnClick_Send()
    {
        inputField.text = inputField.text.Trim('\n');
        if (!inputField.text.Equals("") && !inputField.text.Equals("\n") && inputField.interactable)
        {
            SendMessages(inputField.text);
        }
        inputField.text = "";
    }
    public void OnEnter_Send()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnClick_Send();
        }
    }
    public void ChangeChannel(string channel)
    {
        currentChannel = channel;
        if (!isDay)
            inputField.interactable = !currentChannel.Equals("PublicChannel");
        else
            inputField.interactable = true;
        ChatChannel chatChannel;
        if(chatClient != null && chatClient.TryGetChannel(currentChannel,out chatChannel))
        {
            contentMessage.text = chatChannel.ToStringMessages();
        }
    }
    private void SendMessages(string message)
    {
        chatClient.PublishMessage(currentChannel, message);
    }
    #endregion

    #region Local ActionEvent Methods
    private void StartGame()
    {
        isDay = false;
        inputField.interactable = false;
    }
    private void NighttimeTransition()
    {
        isDay = false;
        if(currentChannel.Equals("PublicChannel"))
            inputField.interactable = false;
    }
    private void DaytimeTransition()
    {
        isDay = true;
        inputField.interactable = true;
    }
    private void AfterMyDeath()
    {
        inputField.gameObject.SetActive(false);
    }
    private void SubscribeNewChannel(int index)
    {
        chatClient.Subscribe(channels[index]);
        if (channelTab != null)
        {
            channelTab[0].interactable = true;
            channelTab[index].gameObject.SetActive(true);
        }
    }
    #endregion

    #region ChatClient Callback Methods
    public void DebugReturn(DebugLevel level, string message)
    {
        // Do Nothing

    }

    public void OnChatStateChange(ChatState state)
    {
        // Do Nothing
        
    }

    public void OnConnected()
    {
        // Do Nothing
        chatClient.Subscribe(new string[] { channels[0],channels[1] });
    }

    public void OnDisconnected()
    {
        // Do Nothing
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        lock (contentMessage)
        {
            ChatChannel chatChannel;
            if (!chatClient.TryGetChannel(currentChannel, out chatChannel))
            {
                Debug.Log("ShowChannel failed to find channel: " + channelName);
                return;
            }
            contentMessage.text = chatChannel.ToStringMessages();
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // Do Nothing
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        // Do Nothing
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        // Do Nothing
        //foreach (string channel in channels)
        //{
        //    chatClient.PublishMessage(channel, "says 'hi'."); // you don't HAVE to send a msg on join but you could.
        //}

        Debug.Log("OnSubscribed: " + string.Join(", ", channels));
    }

    public void OnUnsubscribed(string[] channels)
    {
        // Do Nothing
    }

    public void OnUserSubscribed(string channel, string user)
    {
        // Do Nothing
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        // Do Nothing
    }
    #endregion
}
