using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatHandler : MonoBehaviour
{
    public GameObject chatPanel, textObject;
    public TMP_InputField inputField;

    [SerializeField]
    public List<Message> messageList = new List<Message>();
    // Start is called before the first frame update
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
                sendMessageToChat(inputField.text);
                inputField.text = "";
            }
        }
        
    }

    public void sendMessageToChat(string text)
    {
        Message newMessage = new Message();
        //to do: add normal constructor to class Message. 
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<TMP_Text>();
        newMessage.textObject.text = newMessage.text;

        messageList.Add(newMessage);
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public string sentBy;
    public TMP_Text textObject;

}
