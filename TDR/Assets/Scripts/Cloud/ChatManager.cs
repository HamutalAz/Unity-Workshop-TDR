using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    private FirebaseFirestore dbReference;
    private CollectionReference roomChatCollection;
    private ListenerRegistration listener;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("At ChatManager.start()");
        dbReference = FirebaseFirestore.DefaultInstance; 
    }

    async public void addMessageToFirestore(Message newMessage)
    {
        Debug.Log("At ChatHandler.addMessageToFirestore()");
        await roomChatCollection.Document().SetAsync(newMessage);
    }

    public void startListeningToMessages()
    {
        Debug.Log("At ChatManager.startListening()");
        roomChatCollection = dbReference.Collection("Rooms").Document(DataBaseManager.roomID).Collection("room_chat");
        listener = roomChatCollection.Listen(collection =>
        {
            //find the newest messages
            foreach (DocumentChange doc in collection.GetChanges())
            {
                if(doc.ChangeType == DocumentChange.Type.Added)
                {
                    string text = doc.Document.GetValue<string>("text");
                    string sentBy = doc.Document.GetValue<string>("sentBy");

                    if (!sentBy.Equals(DataBaseManager.userName))
                    {
                        DataBaseManager.instance.chatHandler.sendMessageToChat(text, sentBy);
                    }
                }
            }
        });
    }

    public void stopListeningToMessages()
    {
        Debug.Log("At ChatManager.stopListening()");
        listener.Stop();
    }
}
