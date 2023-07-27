using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatHandler : ChatHandlerInterface
{
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("At ChatHandler.start()");
        DataBaseManager.instance.setChatHandler(this);
        //enable listener at ChatManager
        DataBaseManager.instance.chatManager.startListeningToMessages();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("user pressed enter, about to send message to chat.");
                Message newMessage = sendMessageToChat(inputField.text, DataBaseManager.userName);
                inputField.text = "";
                DataBaseManager.instance.chatManager.addMessageToFirestore(newMessage);
            }
        }
        
    }

    private void OnDestroy()
    {
        Debug.Log("At ChatHandler.OnDestroy()");
        //disable listener at chatManager
        DataBaseManager.instance.chatManager.stopListeningToMessages();
    }

    public override Message sendMessageToChat(string text, string sentBy)
    {
        // itay levy needs to change to => DataBaseManager.userName
        Message newMessage = new Message(text, sentBy, "white");

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        TMP_Text tmp_text = newText.GetComponent<TMP_Text>();
        tmp_text.text = newMessage.sentBy + ": " + newMessage.text;

        //needs to be changed later
        tmp_text.color = Color.white;

        messageList.Add(newMessage);

        if(newMessage.sentBy != DataBaseManager.userName)
            gameObject.GetComponent<AudioSource>().PlayOneShot(clip);

        return newMessage;
    }
}


