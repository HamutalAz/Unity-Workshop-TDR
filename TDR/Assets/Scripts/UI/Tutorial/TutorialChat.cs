using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialChat : ChatHandlerInterface
{
    bool firstMessage = true;
    public AudioClip clip;

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
                Debug.Log("user pressed enter, about to send message to chat.");
                Message newMessage = sendMessageToChat(inputField.text, "You");
                inputField.text = "";

                if (firstMessage)
                {
                    Message AutoMessage = sendMessageToChat("Congruts! You just sent your first message!", "Tutorial Guide");
                    firstMessage = false;
                    gameObject.GetComponent<AudioSource>().PlayOneShot(clip);

                }
            }
        }

    }

    public void OnDestroy()
    {
        Debug.Log("At ChatHandler.OnDestroy()");
    }

}


