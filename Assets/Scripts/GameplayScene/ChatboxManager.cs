using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatboxManager : MonoBehaviour,IChatClientListener
{
    #region Private SerializeField
    [SerializeField]
    private Text contentMessage = null;
    [SerializeField]
    private InputField inputField = null;
    #endregion
    #region Private Fields
    private ChatClient chatClient = null;
    private string publicChannel = "publicChannel";
    #endregion
    #region Monobehaviour Methods
    // Start is called before the first frame update
    private void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
        ActionEventHandler.AddNewActionEvent(ActionEventID.StartGame, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.NighttimeTransition, NighttimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.DaytimeTransition, DaytimeTransition);
        ActionEventHandler.AddNewActionEvent(ActionEventID.AfterMyDeath, AfterMyDeath);
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
        if (!inputField.text.Equals("") && !inputField.text.Equals("\n"))
        {
            SendMessages(inputField.text);
            inputField.text = "";
        }
    }
    public void OnEnter_Send()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnClick_Send();
        }
    }
    private void SendMessages(string message)
    {
        chatClient.PublishMessage(publicChannel, message);
    }
    #endregion

    #region Local ActionEvent Methods
    private void NighttimeTransition()
    {
        inputField.interactable = false;
    }
    private void DaytimeTransition()
    {
        inputField.interactable = true;
    }
    private void AfterMyDeath()
    {
        inputField.gameObject.SetActive(false);
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
        chatClient.Subscribe(publicChannel);
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
            if (!chatClient.TryGetChannel(publicChannel, out chatChannel))
            {
                Debug.Log("ShowChannel failed to find channel: " + channelName);
                return;
            }
            //string msgs = "";
            //for (int i = 0; i < senders.Length; i++)
            //{
            //    msgs = string.Format("{0}: {1}, ", msgs, senders[i], messages[i]);
            //    contentMessage.text += msgs;
            //}
            //Debug.LogFormat("OnGetMessages: {0} ({1}) > {2}", channelName, senders.Length, msgs);
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
        foreach (string channel in channels)
        {
            this.chatClient.PublishMessage(channel, "says 'hi'."); // you don't HAVE to send a msg on join but you could.
        }

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
