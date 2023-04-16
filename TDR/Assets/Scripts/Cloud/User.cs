using Firebase.Firestore;
using System;
using System.Collections.Generic;

[FirestoreData]
public class User
{
    [FirestoreProperty]
    public string userName { get; set; }
    [FirestoreProperty]
    public string userId { get; set; }
    [FirestoreProperty]
    public string gameId { get; set; }
    [FirestoreProperty]
    public string roomId { get; set; }
    [FirestoreProperty]
    public string location { get; set; }
    public User()
    {
    }
    public User(string userName)
    {
        this.userName = userName;
    }

    public User(string userName, string userId)
    {
        this.userName = userName;
        this.userId = userId;
        this.gameId = null;
        this.roomId = null;
        this.location = null;
    }

}
