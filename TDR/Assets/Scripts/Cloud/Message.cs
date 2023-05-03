using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class Message
{
    [FirestoreProperty]
    public string text { get; set; }
    [FirestoreProperty]
    public string sentBy { get; set; }
    [FirestoreProperty]
    public string userColor { get; set; }

    public Message(string text, string sentBy, string userColor)
    {
        this.text = text;
        this.sentBy = sentBy;
        this.userColor = userColor;
    }

    public Message()
    {

    }

}
