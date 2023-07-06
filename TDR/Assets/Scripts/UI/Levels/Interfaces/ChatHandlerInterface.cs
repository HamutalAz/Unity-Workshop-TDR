using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ChatHandlerInterface : MonoBehaviour
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
        
    }

    public virtual Message sendMessageToChat(string text, string sentBy)
    {
        Message newMessage = new Message(text, sentBy, "white");
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        TMP_Text tmp_text = newText.GetComponent<TMP_Text>();
        tmp_text.text = newMessage.sentBy + ": " + newMessage.text;
        
        if (sentBy == "Tutorial Guide")
            tmp_text.color = Color.magenta;
        else
            tmp_text.color = Color.white;

        messageList.Add(newMessage);

        return newMessage;
    }
}
