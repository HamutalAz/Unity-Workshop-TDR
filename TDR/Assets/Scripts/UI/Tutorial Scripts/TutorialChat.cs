using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialChat : MonoBehaviour
{
    public GameObject chatPanel, textObject;
    public TMP_InputField inputField;

    [SerializeField]
    public List<Message> messageList = new List<Message>();
    // Start is called before the first frame update
    bool firstMessage = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("user pressed enter, about to send message to chat.");
                Message newMessage = sendMessageToChat(inputField.text, "You");
                inputField.text = "";

                if (firstMessage)
                {
                    Message AutoMessage = sendMessageToChat("Congruts! You just sent your first message!", "Tutorial Guide");
                    firstMessage = false;
                }
            }
        }

    }

    private void OnDestroy()
    {
        Debug.Log("At ChatHandler.OnDestroy()");

    }

    public Message sendMessageToChat(string text, string sentBy)
    {
        // itay levy needs to change to => DataBaseManager.userName
        Message newMessage = new Message(text, sentBy, "white");

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        TMP_Text tmp_text = newText.GetComponent<TMP_Text>();
        tmp_text.text = newMessage.sentBy + ": " + newMessage.text;

        //TODO: needs to be changed later
        if (sentBy == "Tutorial Guide")
            tmp_text.color = Color.magenta;
        else
            tmp_text.color = Color.white;

        messageList.Add(newMessage);
        
        return newMessage;

    }
}


