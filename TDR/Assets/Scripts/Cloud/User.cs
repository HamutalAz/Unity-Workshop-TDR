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
    [FirestoreProperty]
    public bool toRotate { get; set; }
    [FirestoreProperty]
    public string avatar { get; set; }
    public User()
    {
    }
    public User(string userName)
    {
        this.userName = userName;
    }

    public User(string userName, string userId, string avatar)
    {
        this.userName = userName;
        this.userId = userId;
        this.avatar = avatar;
        this.gameId = null;
        this.roomId = null;
        this.location = null;
        this.toRotate = false;
    }

}
